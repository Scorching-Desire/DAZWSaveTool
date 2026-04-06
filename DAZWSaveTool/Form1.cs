using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace DAZWSaveTool
{
    public partial class Form1 : Form
    {
        private JObject loadedDataRoot;
        private dynamic loadedData;
        private byte[] decryptedData;
        private byte[] encryptedData;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DecodeStart_Click(object sender, EventArgs e)
        {
            ulong xorkey;
            OpenFileDialog openFileDialog1 = new();
            if (ToolMode.SelectedIndex == 0)
            {
                xorkey = unchecked((ulong)(-1024797686));
                openFileDialog1.Filter = "Encoded .sav files(*.sav)|*.sav";
            }
            else if (ToolMode.SelectedIndex == 1 || ToolMode.SelectedIndex == 3)
            {
                xorkey = 60689136UL;
                openFileDialog1.Filter = "Encoded config files(*.cfg)|*.cfg";
            }
            else if (ToolMode.SelectedIndex == 2)
            {
                xorkey = unchecked((ulong)(-1335824931));
                openFileDialog1.Filter = "Encoded save data(*.txt)|*.txt";
            }
            else { return; }
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string temp = openFileDialog1.FileName;
            if (ToolMode.SelectedIndex != 3)
            {
                if (ToolMode.SelectedIndex == 2)
                {
                    byte[] Base64StrBytes = Convert.FromBase64String(File.ReadAllText(openFileDialog1.FileName));
                    File.WriteAllBytes(openFileDialog1.FileName + ".temp", Base64StrBytes);
                    openFileDialog1.FileName += ".temp";
                }
                using (Stream stream2 = File.OpenRead(openFileDialog1.FileName))
                {
                    using (Stream stream3 = new CryptStream(stream2, CryptMode.Decrypt, xorkey))
                    {
                        using (GZipInputStream gzipInputStream = new GZipInputStream(stream3))
                        {
                            using (TextReader textReader2 = new StreamReader(gzipInputStream))
                            {
                                using (JsonReader jsonReader2 = new JsonTextReader(textReader2))
                                {
                                    gzipInputStream.IsStreamOwner = false;
                                    this.loadedDataRoot = (JObject)JToken.ReadFrom(jsonReader2);
                                    this.loadedData = this.loadedDataRoot;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                byte[] key = Convert.FromBase64String("VjR2BNKI0CT+uMOiUGmORA=="); // Leaving the key open is truly a genius idea
                byte[] IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // ARE YA CODING SON? You are supposed to use it so you won't get hacked even harder

                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;

                byte[] encryptedData = File.ReadAllBytes(openFileDialog1.FileName);

                using MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedData, 0, encryptedData.Length);
                    cs.FlushFinalBlock();
                }

                decryptedData = ms.ToArray();
                File.WriteAllBytes(openFileDialog1.FileName + ".zip", decryptedData);
            }

            if (ToolMode.SelectedIndex != 3)
            {
                string filename2;
                if (ToolMode.SelectedIndex == 2)
                {
                    File.Delete(openFileDialog1.FileName);
                    filename2 = temp.Replace(".txt", "") + ".json";
                }
                else
                {
                    if (ToolMode.SelectedIndex == 0) { filename2 = openFileDialog1.FileName.Replace(".sav", "") + ".json"; }
                    else { filename2 = openFileDialog1.FileName.Replace(".cfg", "") + ".json"; }
                }
                File.WriteAllText(filename2, loadedData.ToString());
            }

            MessageBox.Show("File saved.");
        }

        private void EncodeStart_Click(object sender, EventArgs e)
        {
            ulong xorkey;
            string type;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (ToolMode.SelectedIndex == 0)
            {
                xorkey = unchecked((ulong)(-1024797686));
                openFileDialog1.Filter = "Decoded .sav files(*.json)|*.json";
                type = ".sav";
            }
            else if (ToolMode.SelectedIndex == 1)
            {
                xorkey = 60689136UL;
                openFileDialog1.Filter = "Decoded config files(*.json)|*.json";
                type = ".cfg";
            }
            else if (ToolMode.SelectedIndex == 2)
            {
                xorkey = unchecked((ulong)(-1335824931));
                openFileDialog1.Filter = "Decoded save data(*.json)|*.json";
                type = ".temp";
            }
            else if (ToolMode.SelectedIndex == 3)
            {
                xorkey = unchecked((ulong)(-1335824931));
                openFileDialog1.Filter = "Decoded config data(*.zip)|*.zip";
                type = ".temp";
            }
            else { return; }
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            if (ToolMode.SelectedIndex != 3)
            {
                using (Stream stream2 = File.OpenRead(openFileDialog1.FileName))
                {
                    using (TextReader textReader2 = new StreamReader(stream2))
                    {
                        using (JsonReader jsonReader2 = new JsonTextReader(textReader2))
                        {
                            this.loadedDataRoot = (JObject)JToken.ReadFrom(jsonReader2);
                            this.loadedData = this.loadedDataRoot;
                        }
                    }
                }
                using (Stream stream4 = File.Open(openFileDialog1.FileName.Replace(".json", "") + type, FileMode.Create))
                {
                    using (Stream stream5 = new CryptStream(stream4, CryptMode.Crypt, xorkey))
                    {
                        using (GZipOutputStream gzipOutputStream = new GZipOutputStream(stream5))
                        {
                            using (TextWriter textWriter2 = new StreamWriter(gzipOutputStream))
                            {
                                using (JsonWriter jsonWriter2 = new JsonTextWriter(textWriter2))
                                {
                                    gzipOutputStream.IsStreamOwner = false;
                                    gzipOutputStream.SetLevel(9);
                                    this.loadedDataRoot.WriteTo(jsonWriter2, new JsonConverter[0]);
                                }
                            }
                        }
                    }
                }
                if (ToolMode.SelectedIndex == 2)
                {
                    byte[] RawBytes = File.ReadAllBytes(openFileDialog1.FileName.Replace(".json", "") + type);
                    string Base64String = Convert.ToBase64String(RawBytes);
                    File.Delete(openFileDialog1.FileName.Replace(".json", ".temp"));
                    File.WriteAllText(openFileDialog1.FileName.Replace(".json", "") + ".txt", Base64String);
                }
            }
            else
            {
                byte[] key = Convert.FromBase64String("VjR2BNKI0CT+uMOiUGmORA=="); // Leaving the key open is truly a genius idea
                byte[] IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // ARE YA CODING SON? You are supposed to use it so you won't get hacked even harder

                using Aes aes = Aes.Create();
                aes.Key = key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;

                byte[] decryptedData = File.ReadAllBytes(openFileDialog1.FileName);

                using MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(decryptedData, 0, decryptedData.Length);
                    cs.FlushFinalBlock();
                }

                encryptedData = ms.ToArray();
                File.WriteAllBytes(openFileDialog1.FileName.Remove(openFileDialog1.FileName.Length - 4), encryptedData);
            }
            MessageBox.Show("File saved.");
        }

        private void signButton_Click(object sender, EventArgs e)
        {
            signedValue.Text = Sec.EncryptSecureInt(Int32.Parse(originalValue.Text)).ToString();
        }
    }
}