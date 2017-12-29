namespace EncryptWithPassword
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public class EncryptionManager
    {
        //  Call this function to remove the key from memory after use for security
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);

        /// <summary>
        ///     Creates a random salt that will be used to encrypt your file. This method is required on FileEncrypt.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandomSalt()
        {
            var data = new byte[32];

            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }

        /// <summary>
        ///     Encrypts a file from its path and a plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        public void FileEncrypt(string inputFile, string password)
        {
            //http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

            //generate random salt
            var salt = GenerateRandomSalt();

            var aesFile = inputFile + ".aes";

            var fsCrypt = new FileStream(aesFile, FileMode.Create);

            var AES = CreateKey(password, salt);

            // write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);

            var cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            var fsIn = new FileStream(inputFile, FileMode.Open);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            var buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cs.Write(buffer, 0, read);
                }

                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }

            File.WriteAllText(aesFile + ".base64", Convert.ToBase64String(File.ReadAllBytes(aesFile)));
            File.Delete(aesFile);
        }

        /// <summary>
        ///     Decrypts an encrypted file with the FileEncrypt method through its path and the plain password.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="password"></param>
        public void FileDecrypt(string inputFile, string password)
        {
            var content = Convert.FromBase64String(File.ReadAllText(inputFile));
            var aesFile = inputFile + ".aes";
            File.WriteAllBytes(aesFile, content);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var salt = new byte[32];

            ////byte[] encryptedBytes = Convert.FromBase64String(File.ReadAllText(inputFile, Encoding.ASCII));

            var fsCrypt = new FileStream(aesFile, FileMode.Open);
            fsCrypt.Read(salt, 0, salt.Length);

            var AES = CreateKey(password, salt);
            var cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

            var decryptedFile = Path.GetDirectoryName(inputFile) + Path.DirectorySeparatorChar +
                                Path.GetFileNameWithoutExtension(inputFile) + ".decrypted_change_to_properext";
            var fsOut = new FileStream(decryptedFile, FileMode.Create);

            int read;
            var buffer = new byte[1048576];

            try
            {
                while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fsOut.Write(buffer, 0, read);
                }
            }
            catch (CryptographicException ex_CryptographicException)
            {
                Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            try
            {
                cs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
            }
            finally
            {
                fsOut.Close();
                fsCrypt.Close();
            }

            File.Delete(aesFile);
        }

        private RijndaelManaged CreateKey(string password, byte[] salt)
        {
            //convert password string to byte arrray
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            var AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;

            //http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            //Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
            AES.Mode = CipherMode.CFB;
            return AES;
        }
    }
}