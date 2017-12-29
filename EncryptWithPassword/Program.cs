namespace EncryptWithPassword
{
    using System;
    using System.Runtime.InteropServices;
    using CommandLine;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();
            var isValid = Parser.Default.ParseArguments(args, options);
            if (isValid == false)
            {
                Console.WriteLine($"Unable to parse {string.Join(",", args)}");
            }
            else
            {
                var encryptionManager = new EncryptionManager();
                // For additional security Pin the password of your files
                var gch = GCHandle.Alloc(options.Password, GCHandleType.Pinned);
                if (options.Operation == OperationEnum.Encrypt)
                {
                    encryptionManager.FileEncrypt(options.File, options.Password);
                    Console.WriteLine($"{options.File} was encrypted and can be found {options.File}.aes.base64");
                }
                else
                {
                    encryptionManager.FileDecrypt(options.File, options.Password);
                    Console.WriteLine(
                        $"File {options.File} was decrypted and can be found {options.File}.decrypted_change_to_properext");
                }

                // To increase the security of the encryption, delete the given password from the memory !
                EncryptionManager.ZeroMemory(gch.AddrOfPinnedObject(), options.Password.Length * 2);
                gch.Free();
                // You can verify it by displaying its value later on the console (the password won't appear)
                // Console.WriteLine("The given password is surely nothing: " + options.Password);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}