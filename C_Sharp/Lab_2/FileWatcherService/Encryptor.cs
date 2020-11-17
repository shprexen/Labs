using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileWatcherService
{
    class Encryptor
    {
        private static readonly DESCryptoServiceProvider crypt = new DESCryptoServiceProvider
        {
            Key = Encoding.ASCII.GetBytes("qwertyui"),
            IV = Encoding.ASCII.GetBytes("qwertyui")
        };

        public static void Encrypt(Stream sourceStr, Stream targetStream)
        {
            using (var cryptoStream = new CryptoStream(targetStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
            {
                sourceStr.CopyTo(cryptoStream);
            }
        }

        public static void Decrypt(Stream sourceStr, Stream targetStream)
        {
            using(var deCryptoStream = new CryptoStream(sourceStr, crypt.CreateDecryptor(), CryptoStreamMode.Read))
            {
                deCryptoStream.CopyTo(targetStream);
            }
        }

        public static string GetTargetEncryptedFilePath(string filename, string targetPath)
        {
            filename = filename.Replace(Path.GetDirectoryName(filename), targetPath);
            return filename.Replace(Path.GetFileName(filename), Path.GetFileNameWithoutExtension(filename) + "_encrypted" + Path.GetExtension(filename));
        }

        public static string GetTargetDecryptedFilePath(string filename, string targetPath)
        {
            filename = Path.Combine(targetPath, filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            name = name.Replace("_encrypted", "_decrypted");
            return filename.Replace(Path.GetFileNameWithoutExtension(filename), name);
        }
    }
}
