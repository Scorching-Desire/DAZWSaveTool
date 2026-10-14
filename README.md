# DAZWSaveTool

DAZWSaveTool decodes and encodes DAZW save, configuration, and localization files, and generates Sec.xx values.

## DISCLAIMER!

I am not responsible for any banned accounts.

## How to get SAV, CFG and save files (pre-4.4.3)

1. Open a file explorer on your phone.
2. Go to `Android/data/com.mobirate.deadaheadtactics/files/` (make sure you have access to the folder on newer devices).
3. Transfer `aws_cognito_sync.db`, `43700.sav`, and `primarydata.4.3.7.cfg` to your PC. Version numbers in filenames may differ.
4. To edit save game data, open `aws_cognito_sync.db` with a SQLite database viewer and locate the `records` table. Select a record whose `identity_id` is a known identity such as `us-east-1...`, rather than `unknown`. Copy the string from its `value` column into a `.txt` file.

## How to get SAV, CFG and save files (4.4.3+, rooted device/emulator required)

1. Open a file explorer with root access on your phone or emulator.
2. Go to `/data/data/com.mobirate.deadaheadtactics/files/dazwint/` or `/data/user/0/com.mobirate.deadaheadtactics/files/dazwint/`.
3. Transfer `srv.sav`, `44300.sav`, and `primarydata.4.4.3.cfg` to your PC. Version numbers in filenames may differ.
4. To edit save game data, open `srv.sav` with a SQLite database viewer: it is a renamed database file. In the `records` table, select a record whose `identity_id` is a known identity such as `us-east-1...`, rather than `unknown`. Copy the string from its `value` column into a `.txt` file. Select file type **1** for this extracted text, rather than file type **0** for the database itself.

## File types and encryption systems

The type numbers match the order of the file-type selector in the application.

| Type | File/data | Game version | Encryption system | Decoded output |
| --- | --- | --- | --- | --- |
| 0 | `*.sav` save files | Versions using the custom save format | Custom byte transformation and XOR, with GZip compression | `.json` |
| 1 | Save game data extracted into `*.txt` | Before and after 4.4.3 | Base64 wrapping around custom byte transformation and XOR, with GZip compression | `.json` |
| 2 | `*.cfg` configuration files | Before 3.9.3 | Custom byte transformation and XOR, with GZip compression | `.json` |
| 3 | `*.cfg` configuration files | 3.9.3–4.3.5 | AES encryption | `.cfg.zip` |
| 4 | `*.cfg` configuration files | 4.3.7–4.4.2 | AES encryption + RSA signature | `.cfg.gz` |
| 5 | `*.cfg` configuration files | 4.4.3+ | AES encryption + X Secret + key adjustments | `.cfg.gz` |
| 6 | `*.l10n` localization files | 4.1.0–4.4.2 | AES encryption + RSA signature | `.l10n.gz` |
| 7 | `*.l10n` localization files | 4.4.3+ | AES encryption + X Secret + key adjustments | `.l10n.gz` |

- **Custom encryption:** the compressed bytes are transformed using their position and XORed with a repeating four-byte key. Types 0, 1, and 2 each use a different key. Type 1 also stores the encrypted bytes as Base64 text.
- **AES:** the current implementation uses CBC mode with PKCS7 padding. Enter the Base64 key in the AES key text box. Types 3, 6, and 7 use a zero IV; the configuration handler for types 4 and 5 reads a 16-byte IV from the start of the file.
- **RSA signature:** a signature authenticates data separately from AES encryption. A valid RSA signature cannot be spoofed without the private key, which belongs to Mobirate. The current configuration decoder excludes the trailing 256-byte signature without verifying it. The encoder writes a zero-filled placeholder instead of a valid RSA signature. The localization handler currently decrypts the complete file and does not separately process an RSA signature.
- **X Secret/key adjustments:** the current configuration decoder for both types 4 and 5 reverses the Base64 key text while keeping its trailing `=` padding, decodes it, and XORs its 16 bytes with `xArray`. Enter exactly 16 comma-separated decimal bytes from 0 to 255, for example `53, 255, 182, 254, 224, 5, 27, 12, 248, 193, 43, 193, 235, 169, 145, 174`. If decryption fails, it retries using `xArray` directly as the key. The configuration encoder uses the entered Base64 key directly. Type 7 currently reverses the Base64 key text for both decoding and encoding; it does not use `xArray`.

The table identifies the file types; the notes describe the current implementation, including its signature and key-handling limitations.

## How to properly apply edits for 4.3.7 .cfg files

1. Select file type **4** and decode the config file.
2. Open the generated compressed file with 7-Zip and extract the decoded `.cfg` file into a folder. The current decoder appends `.gz`; other versions of the tool may produce `.zip`.
3. Format the JSON and apply your changes.
4. Archive the edited `.cfg` file into a `.zip` file using 7-Zip.
5. Encode the archive with file type **4**. The output filename ends in `_encoded.cfg`.
6. Make sure you use a DAZW APK with disabled Verifier and PairIP checks. The generated file contains a placeholder RSA signature.

## Compiling

Build on Windows with the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). For Visual Studio, use Visual Studio 2026 with the .NET desktop development workload.

Running a framework-dependent build requires the .NET 10 Desktop Runtime for Windows.

# Перевод на русский

## DISCLAIMER!

Я не несу ответственности за заблокированные аккаунты.

## Как получить файлы SAV, CFG и save (до версии 4.4.3)

1. Откройте файловый менеджер на телефоне.
2. Перейдите в папку `Android/data/com.mobirate.deadaheadtactics/files/` (на новых устройствах убедитесь, что у вас есть доступ к этой папке).
3. Перенесите файлы `aws_cognito_sync.db`, `43700.sav` и `primarydata.4.3.7.cfg` на ПК. Номера версий в названиях файлов могут отличаться.
4. Чтобы редактировать данные сохранения, откройте `aws_cognito_sync.db` в программе для просмотра баз SQLite и найдите таблицу `records`. Выберите запись с известным `identity_id`, например `us-east-1...`, вместо `unknown`. Скопируйте строку из столбца `value` в файл `.txt`.

## Как получить файлы SAV, CFG и save (4.4.3+, требуется root на устройстве/эмуляторе)

1. Откройте файловый менеджер с root-доступом на телефоне или эмуляторе.
2. Перейдите в `/data/data/com.mobirate.deadaheadtactics/files/dazwint/` или `/data/user/0/com.mobirate.deadaheadtactics/files/dazwint/`.
3. Перенесите файлы `srv.sav`, `44300.sav` и `primarydata.4.4.3.cfg` на ПК. Номера версий в названиях файлов могут отличаться.
4. Чтобы редактировать данные сохранения, откройте `srv.sav` в программе для просмотра баз SQLite: это переименованный файл базы данных. В таблице `records` выберите запись с известным `identity_id`, например `us-east-1...`, вместо `unknown`. Скопируйте строку из столбца `value` в файл `.txt`. Для извлечённого текста выберите тип файла **1**; не выбирайте тип **0** для самой базы данных.

## Типы файлов и системы шифрования

Номера типов соответствуют порядку элементов в списке выбора типа файла в программе.

| Тип | Файл/данные | Версия игры | Система шифрования | Результат декодирования |
| --- | --- | --- | --- | --- |
| 0 | Файлы сохранения `*.sav` | Версии с собственным форматом сохранения | Собственное преобразование байтов и XOR, сжатие GZip | `.json` |
| 1 | Данные сохранения, извлечённые в `*.txt` | До и после 4.4.3 | Base64 поверх собственного преобразования байтов и XOR, сжатие GZip | `.json` |
| 2 | Файлы конфигурации `*.cfg` | До 3.9.3 | Собственное преобразование байтов и XOR, сжатие GZip | `.json` |
| 3 | Файлы конфигурации `*.cfg` | 3.9.3–4.3.5 | Шифрование AES | `.cfg.zip` |
| 4 | Файлы конфигурации `*.cfg` | 4.3.7–4.4.2 | Шифрование AES + подпись RSA | `.cfg.gz` |
| 5 | Файлы конфигурации `*.cfg` | 4.4.3+ | Шифрование AES + преобразования ключа + X Secret | `.cfg.gz` |
| 6 | Файлы локализации `*.l10n` | 4.1.0–4.4.2 | Шифрование AES + подпись RSA | `.l10n.gz` |
| 7 | Файлы локализации `*.l10n` | 4.4.3+ | Шифрование AES + преобразования ключа + X Secret | `.l10n.gz` |

- **Собственное шифрование:** сжатые байты преобразуются с учётом их позиции, затем применяется XOR с повторяющимся четырёхбайтовым ключом. Типы 0, 1 и 2 используют разные ключи. Для типа 1 зашифрованные байты дополнительно представлены текстом Base64.
- **AES:** текущая реализация использует режим CBC с заполнением PKCS7. Введите ключ Base64 в поле AES key. Типы 3, 6 и 7 используют нулевой IV; обработчик конфигураций типов 4 и 5 читает 16-байтовый IV из начала файла.
- **Подпись RSA:** подпись подтверждает подлинность данных отдельно от шифрования AES. Действительную подпись RSA нельзя подделать без закрытого ключа, который принадлежит Mobirate. Текущий декодер конфигураций исключает последние 256 байтов подписи без её проверки. Кодер записывает заполненную нулями заглушку вместо действительной подписи RSA. Обработчик локализации сейчас расшифровывает весь файл и не обрабатывает подпись RSA отдельно.
- **X Secret/преобразования ключа:** текущий декодер конфигураций для обоих типов 4 и 5 переворачивает текст ключа Base64, сохраняя завершающие символы `=`, декодирует его и применяет XOR к его 16 байтам с `xArray`. Введите ровно 16 десятичных байтов от 0 до 255 через запятую, например `53, 255, 182, 254, 224, 5, 27, 12, 248, 193, 43, 193, 235, 169, 145, 174`. Если расшифровка не удаётся, декодер повторяет попытку, используя `xArray` напрямую как ключ. Кодер конфигураций использует введённый ключ Base64 напрямую. Тип 7 сейчас переворачивает текст ключа Base64 при декодировании и кодировании; `xArray` для него не используется.

Таблица описывает типы файлов, а примечания — текущую реализацию, включая ограничения обработки подписей и ключей.

## Как правильно применять изменения для файлов .cfg версии 4.3.7

1. Выберите тип файла **4** и декодируйте конфигурацию.
2. Откройте полученный сжатый файл в 7-Zip и извлеките декодированный `.cfg` в папку. Текущий декодер добавляет расширение `.gz`; другие версии программы могут создавать `.zip`.
3. Отформатируйте JSON и внесите изменения.
4. Поместите изменённый `.cfg` в архив `.zip` с помощью 7-Zip.
5. Закодируйте архив, выбрав тип файла **4**. Имя результата заканчивается на `_encoded.cfg`.
6. Используйте APK DAZW с отключёнными проверками Verifier и PairIP. Созданный файл содержит заглушку подписи RSA.

## Сборка

Для сборки в Windows установите [SDK .NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Если используете Visual Studio, установите Visual Studio 2026 с рабочей нагрузкой «Разработка классических приложений .NET».

Для запуска сборки, зависящей от установленной среды выполнения, требуется .NET 10 Desktop Runtime для Windows.
