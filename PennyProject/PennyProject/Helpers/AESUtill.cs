using System.Security.Cryptography;
using System.Text;

namespace PennyProject.Helpers
{
    public class AESUtill
    {
        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key">param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string EncryptAES(string text, string aesKey, string aesIv = "0000000000000000")
        {
            var sourceBytes = Encoding.UTF8.GetBytes(text);
            Aes aes = null;
            try
            {
                aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                aes.IV = Encoding.UTF8.GetBytes(aesIv);
                var transform = aes.CreateEncryptor();

                return Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
            }
            finally { aes.Clear(); }
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptAES(string text, string aesKey, string aesIv = "0000000000000000")
        {
            Aes aes = null;
            try
            {
                var encryptBytes = Convert.FromBase64String(text);
                aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                aes.IV = Encoding.UTF8.GetBytes(aesIv);
                var transform = aes.CreateDecryptor();
                return Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (aes != null)
                    aes.Clear();
            }
        }
    }
}
