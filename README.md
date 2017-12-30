# EncryptWithPassword
This code was created to allow encrypting and decrypting sensitive information using master password.
Author does not take responsibility for it.

Most of the code was taken from this article:
https://ourcodeworld.com/articles/read/471/how-to-encrypt-and-decrypt-files-using-the-aes-encryption-algorithm-in-c-sharp

To encrypt file use:
EncryptWithPassword -p somepassword -f c:\temp\content\file.png -o Encrypt
Above command will create file c:\temp\content\file.png.aes.base64.
So you can open it with notepad or notepad++ and copy this string content to application like KeePass.

To decrypt file use:
EncryptWithPassword -p somepassword -f c:\temp\content\file.png.aes.base64 -o Decrypt


Important:
<p>1. If you are securing list of some sensitive data beware of viruses/malware and keyloggers.</p>
<p>2. Always use antivirus that is up to date.</p>
<p>3. If you want to store list of recovery passphrases (pdf, png, jpg, etc) inside KeePass or other password DB</p>
<p>  a. Protect your DB with certificate and password that unlocks DB. Password for unlocking DB should not be same as the one used to encrypt sensitive data.</p>
<p>  b. DB and certificate should be backed up to cloud (e.g. onedrive or google drive). Always verify if synchronization cloud client shows that data were successfully synchronized.</p>
<p>  c. Base64 string that is created for recovery passphrases should be generated on computer that is not internet connected (this step increases security and prevents from eaves-dropping).
  Copy file that you want to encrypt to secure machine using USB stick. Generate encrypted base64 file. Copy encrypted base64 file on USB stick back to machine that is using KeePass.
  Copy content of base64 file into Notes field.</p>
<p>  d. In case of laptop loss or data loss, recovery first recover KeePass DB or other password provider from internet.</p>
  If you placed base64 content into Notes field of KeePass you should first create empty file e.g. file.png.aes.base64 and open it with notepad or notepad++.
Then you have to copy base64 string into newly created file.
Once you run decrypt operation new file will be created and it will be called something like this
c:\temp\content\file.png.aes.base64.decrypted_change_to_properext.
You have to remove extension ".aes.base64.decrypted_change_to_properext" so file is called file.png.
Important - To increase security execute Decrypt operation on machine that is not internet connected. 
If it is a file with passphrases for your wallet don't copy this file back to machine that is internet connected.
