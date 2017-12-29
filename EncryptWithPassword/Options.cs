namespace EncryptWithPassword
{
    using CommandLine;

    public enum OperationEnum
    {
        Encrypt,
        Decrypt
    }

    internal class Options
    {
        [Option('f', "file", Required = true, HelpText = "File to be encrypted or decrypted.")]
        public string File { get; set; }

        [Option('p', "password", Required = true)]
        public string Password { get; set; }

        [Option('o', "operation", Required = true, HelpText = "Encrypt to encrypt and decrypt to decrypt")]
        public OperationEnum Operation { get; set; }
    }
}