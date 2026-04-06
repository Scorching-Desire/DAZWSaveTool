# DAZWSaveTool

This is a tool that allows you to fully decode/encode all DAZW files, as well as generating Sec.xx signs.

## DISCLAIMER!
I am not responsible for any banned accounts. Sec.xx is still WIP

## How to get SAV, CFG and save files:
1. Open any file explorer in your phone
2. Go to `Android/data/com.mobirate.deadaheadtactics/files/` (make sure you have access to the folder on newer devices)
3. Transfer `aws_cognito_sync.db`, `43500.sav` and `primarydata.4.3.5.cfg` to your PC (file version seen on their filename might change)
4. (In case you want to edit save files) get the string from `aws_cognito_sync.db` file (it is located in records page, make sure you extract right value by checking if `identity_id` is not unknown. Copy the value column string from record with known `identity_id (us-east-1...))` and create a TXT file with that string.

## How to properly apply edits for 4.3.5 .cfg files
1. Decode the config file
2. Open .zip file and extract decoded .cfg file into any folder
3. Format JSON, apply changes and compress it again
4. Put edited .cfg file into .zip file via 7-Zip
5. Encode the config file

Program itself is pretty straightforward, it doesn't need any explanations.

## Compiling
All it requires is just having Visual Studio Community 2022 and .NET Framework 6 installed in your Visual Studio environment.

# Перевод на русский:

## DISCLAIMER!
Я не несу ответственности за заблокированные аккаунты.

## Как получить файлы SAV, CFG и save:
1. Откройте любой файловый менеджер на телефоне.
2. Перейдите в папку `Android/data/com.mobirate.deadaheadtactics/files/` (на новых устройствах убедитесь, что у вас есть доступ к этой папке).
3. Перенесите файлы `aws_cognito_sync.db`, `43500.sav` и `primarydata.4.3.5.cfg` на ваш ПК (версия файла в названии может отличаться).
4. (Если вы хотите редактировать save-файлы) получите строку из файла `aws_cognito_sync.db` (она находится на странице records, убедитесь, что вы извлекаете правильное значение, проверив, что `identity_id` не равен unknown. Скопируйте строку из столбца value записи с известным `identity_id (us-east-1...)`) и создайте TXT-файл с этой строкой.

## Как правильно применять изменения для файлов .cfg версии 4.3.5
1. Декодируйте .cfg файл.
2. Откройте .zip файл и извлеките декодированный .cfg файл в любую папку.
3. Отформатируйте JSON, внесите изменения и снова сожмите файл.
4. Поместите отредактированный .cfg файл обратно в .zip архив с помощью 7-Zip.
5. Закодируйте .cfg файл.

Сама программа довольно простая и не требует дополнительных объяснений.

## Сборка
Всё, что требуется — это установленная Visual Studio Community 2022 и .NET Framework 6 в вашей среде Visual Studio.
