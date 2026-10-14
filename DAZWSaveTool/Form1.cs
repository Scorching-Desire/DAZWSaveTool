using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;

namespace DAZWSaveTool
{
    public partial class Form1 : Form
    {
        private const int AesBlockSize = 16;
        private const int RsaSignatureSize = 256;

        // Values correspond to the order of the ToolMode combo box items.
        private enum FileType
        {
            SaveFile = 0, // *.sav files.
            SaveGameData = 1, // Save game data (*.txt).
            LegacyConfig = 2, // Pre-3.9.3 *.cfg files (custom encryption).
            AesConfig = 3, // 3.9.3-4.3.5 *.cfg files (AES encryption).
            SignedConfig = 4, // 4.3.7-4.4.2 *.cfg files (AES encryption + RSA sign).
            AdjustedConfig = 5, // 4.4.3+ *.cfg files (AES encryption + X Secret).
            Localization = 6, // 4.1.0-4.4.2 *.l10n files (AES encryption + RSA sign).
            AdjustedLocalization = 7 // 4.4.3+ *.l10n files (AES encryption + X Secret).
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void DecodeStart_Click(object sender, EventArgs e)
        {
            if (!TryGetFileType(out FileType fileType))
                return;

            using OpenFileDialog dialog = new() { Filter = GetDecodeFilter(fileType) };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string inputPath = dialog.FileName;
                switch (fileType)
                {
                    case FileType.SaveFile:
                    case FileType.SaveGameData:
                    case FileType.LegacyConfig:
                        DecodeLegacyFile(inputPath, fileType);
                        break;
                    case FileType.AesConfig:
                    case FileType.Localization:
                    case FileType.AdjustedLocalization:
                        byte[] key = ReadAesKey(fileType == FileType.AdjustedLocalization);
                        byte[] decoded = TransformAes(File.ReadAllBytes(inputPath), key, new byte[AesBlockSize], false);
                        string extension = fileType == FileType.AesConfig ? ".zip" : ".gz";
                        File.WriteAllBytes(inputPath + extension, decoded);
                        break;
                    case FileType.SignedConfig:
                    case FileType.AdjustedConfig:
                        DecodeSignedConfig(inputPath);
                        break;
                }

                MessageBox.Show("File saved.");
            }
            catch (Exception exception) when (IsFileOperationError(exception))
            {
                MessageBox.Show(exception.Message, "Decoding failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EncodeStart_Click(object sender, EventArgs e)
        {
            if (!TryGetFileType(out FileType fileType))
                return;

            using OpenFileDialog dialog = new() { Filter = GetEncodeFilter(fileType) };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string inputPath = dialog.FileName;
                switch (fileType)
                {
                    case FileType.SaveFile:
                    case FileType.SaveGameData:
                    case FileType.LegacyConfig:
                        EncodeLegacyFile(inputPath, fileType);
                        break;
                    case FileType.AesConfig:
                    case FileType.Localization:
                    case FileType.AdjustedLocalization:
                        byte[] key = ReadAesKey(fileType == FileType.AdjustedLocalization);
                        byte[] encoded = TransformAes(File.ReadAllBytes(inputPath), key, new byte[AesBlockSize], true);
                        // Remove only the decoded archive suffix (.zip or .gz).
                        string outputPath = Path.Combine(Path.GetDirectoryName(inputPath)!, Path.GetFileNameWithoutExtension(inputPath));
                        File.WriteAllBytes(outputPath, encoded);
                        break;
                    case FileType.SignedConfig:
                    case FileType.AdjustedConfig:
                        EncodeSignedConfig(inputPath);
                        break;
                }

                MessageBox.Show("File saved.");
            }
            catch (Exception exception) when (IsFileOperationError(exception))
            {
                MessageBox.Show(exception.Message, "Encoding failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryGetFileType(out FileType fileType)
        {
            fileType = (FileType)ToolMode.SelectedIndex;
            return Enum.IsDefined(fileType);
        }

        private static string GetDecodeFilter(FileType fileType) => fileType switch
        {
            FileType.SaveFile => "Encoded .sav files (*.sav)|*.sav",
            FileType.SaveGameData => "Encoded save data (*.txt)|*.txt",
            FileType.Localization or FileType.AdjustedLocalization => "Encoded localization files (*.l10n)|*.l10n",
            _ => "Encoded config files (*.cfg)|*.cfg"
        };

        private static string GetEncodeFilter(FileType fileType) => fileType switch
        {
            FileType.SaveFile => "Decoded .sav files (*.json)|*.json",
            FileType.SaveGameData => "Decoded save data (*.json)|*.json",
            FileType.LegacyConfig => "Decoded config files (*.json)|*.json",
            FileType.Localization or FileType.AdjustedLocalization => "Decoded localization data (*.gz)|*.gz",
            FileType.AdjustedConfig => "Decoded config data (*.gz;*.zip)|*.gz;*.zip",
            _ => "Decoded config data (*.zip;*.gz)|*.zip;*.gz"
        };

        private static ulong GetXorKey(FileType fileType) => fileType switch
        {
            FileType.SaveFile => unchecked((ulong)-1024797686),
            FileType.SaveGameData => unchecked((ulong)-1335824931),
            FileType.LegacyConfig => 60689136UL,
            _ => throw new ArgumentException("The selected file type does not use custom encryption.")
        };

        private static void DecodeLegacyFile(string inputPath, FileType fileType)
        {
            byte[] encoded = fileType == FileType.SaveGameData
                ? Convert.FromBase64String(File.ReadAllText(inputPath))
                : File.ReadAllBytes(inputPath);

            using MemoryStream input = new(encoded);
            using CryptStream decrypted = new(input, CryptMode.Decrypt, GetXorKey(fileType));
            using GZipInputStream gzip = new(decrypted);
            using StreamReader reader = new(gzip);
            using JsonTextReader jsonReader = new(reader);
            JObject data = JObject.Load(jsonReader);
            File.WriteAllText(Path.ChangeExtension(inputPath, ".json"), data.ToString());
        }

        private static void EncodeLegacyFile(string inputPath, FileType fileType)
        {
            JObject data = JObject.Parse(File.ReadAllText(inputPath));
            using MemoryStream output = new();
            using (CryptStream encrypted = new(output, CryptMode.Crypt, GetXorKey(fileType)))
            using (GZipOutputStream gzip = new(encrypted) { IsStreamOwner = false })
            {
                gzip.SetLevel(9);
                using StreamWriter writer = new(gzip);
                using JsonTextWriter jsonWriter = new(writer);
                data.WriteTo(jsonWriter);
            }

            byte[] encoded = output.ToArray();
            if (fileType == FileType.SaveGameData)
            {
                File.WriteAllText(Path.ChangeExtension(inputPath, ".txt"), Convert.ToBase64String(encoded));
                return;
            }

            string extension = fileType == FileType.SaveFile ? ".sav" : ".cfg";
            File.WriteAllBytes(Path.ChangeExtension(inputPath, extension), encoded);
        }

        private byte[] ReadAesKey(bool reverseBase64 = false)
        {
            string keyText = aesKey.Text.Trim();
            if (reverseBase64)
                keyText = ReverseBase64(keyText);

            return Convert.FromBase64String(keyText);
        }

        private static string ReverseBase64(string text)
        {
            // Reverse the key text while keeping its trailing Base64 padding in place.
            string unpadded = text.TrimEnd('=');
            return new string(unpadded.Reverse().ToArray()) + text[unpadded.Length..];
        }

        private static byte[] ParseSecretX(string text)
        {
            string[] values = text.Split(',');
            if (values.Length != AesBlockSize)
                throw new FormatException("xArray must contain exactly 16 comma-separated decimal bytes (0-255).");

            byte[] secretX = new byte[AesBlockSize];
            for (int i = 0; i < values.Length; i++)
            {
                if (!byte.TryParse(values[i].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out secretX[i]))
                    throw new FormatException($"xArray value {i + 1} must be a decimal byte between 0 and 255.");
            }

            return secretX;
        }

        private void DecodeSignedConfig(string inputPath)
        {
            byte[] secretX = ParseSecretX(xArray.Text);
            byte[] baseKey = ReadAesKey(reverseBase64: true);
            if (baseKey.Length != AesBlockSize)
                throw new FormatException("The reversed AES key must decode to exactly 16 bytes.");

            byte[] adjustedKey = new byte[AesBlockSize];
            for (int i = 0; i < adjustedKey.Length; i++)
                adjustedKey[i] = (byte)(baseKey[i] ^ secretX[i]);

            byte[] rawData = File.ReadAllBytes(inputPath);
            int payloadLength = rawData.Length - AesBlockSize - RsaSignatureSize;
            if (payloadLength <= 0 || payloadLength % AesBlockSize != 0)
                throw new FormatException("The config must contain a 16-byte IV, an AES payload, and a 256-byte RSA signature.");

            // The file starts with its IV and ends with an RSA signature, excluded from decryption.
            byte[] iv = rawData[..AesBlockSize];
            byte[] payload = rawData[AesBlockSize..^RsaSignatureSize];
            byte[] decoded;
            try
            {
                decoded = TransformAes(payload, adjustedKey, iv, false);
            }
            catch (CryptographicException)
            {
                // A memory dump captured after the XOR adjustment already contains the final key.
                decoded = TransformAes(payload, secretX, iv, false);
            }

            File.WriteAllBytes(inputPath + ".gz", decoded);
        }

        private void EncodeSignedConfig(string inputPath)
        {
            byte[] key = ReadAesKey();
            byte[] rawData = File.ReadAllBytes(inputPath);
            using MemoryStream compressed = new();
            using (GZipStream gzip = new(compressed, CompressionMode.Compress, leaveOpen: true))
                gzip.Write(rawData);

            byte[] iv = RandomNumberGenerator.GetBytes(AesBlockSize);
            byte[] encoded = TransformAes(compressed.ToArray(), key, iv, true);
            string outputPath = Path.Combine(Path.GetDirectoryName(inputPath)!, Path.GetFileNameWithoutExtension(inputPath) + "_encoded.cfg");
            using FileStream output = File.Create(outputPath);
            output.Write(iv);
            output.Write(encoded);
            // Preserve the existing placeholder signature; this does not produce a valid RSA signature.
            output.Write(new byte[RsaSignatureSize]);
        }

        private static byte[] TransformAes(byte[] data, byte[] key, byte[] iv, bool encrypt)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using ICryptoTransform transform = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
            return transform.TransformFinalBlock(data, 0, data.Length);
        }

        private static bool IsFileOperationError(Exception exception) =>
            exception is IOException or UnauthorizedAccessException or FormatException
                or CryptographicException or JsonException or ICSharpCode.SharpZipLib.SharpZipBaseException;

        private void signButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(originalValue.Text, out int value))
            {
                MessageBox.Show("Enter a valid 32-bit integer.", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            signedValue.Text = Sec.EncryptSecureInt(value).ToString();
        }
    }
}
