using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace PassengerLib
{
    public static class AES
    {
        private static readonly Encoding encoding = Encoding.UTF8;
        private static Aes Create()
        {
            Aes aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            return aes;
        }

        public static string Encrypt(string plainText, string password)
        {
            try
            {
                Aes aes = Create();
                aes.Key = Argon2.Argon2HashPassword(password);
                aes.GenerateIV();

                var AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                var buffer = encoding.GetBytes(plainText);
                var encryptedText = Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                var mac = "";
                mac = BitConverter.ToString(HmacSHA256(Convert.ToBase64String(aes.IV) + encryptedText, password)).Replace("-", "").ToLower();
                var keyValues = new Dictionary<string, object>
                {
                    { "iv", Convert.ToBase64String(aes.IV) },
                    { "value", encryptedText },
                    { "mac", mac },
                };

                return Convert.ToBase64String(encoding.GetBytes(JsonSerializer.Serialize(keyValues)));
            }
            catch (Exception e)
            {
                return "Error encrypting: " + e.Message;
            }
        }

        public static string Decrypt(string plainText, string password)
        {
            try
            {
                Aes aes = Create();
                aes.Key = Argon2.Argon2HashPassword(password);

                var base64Decoded = Convert.FromBase64String(plainText);
                var base64DecodedStr = encoding.GetString(base64Decoded);
                var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(base64DecodedStr);
                aes.IV = Convert.FromBase64String(payload!["iv"]);
                var AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
                var buffer = Convert.FromBase64String(payload["value"]);

                Argon2.s_argon2!.Reset();
                Argon2.s_argon2.Dispose();
                return encoding.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch (Exception e)
            {
                return "Error decrypting: " + e.Message;
            }
        }

        private static byte[] HmacSHA256(string data, string key)
        {
            using (var hmac = new HMACSHA256(encoding.GetBytes(key)))
            {
                return hmac.ComputeHash(encoding.GetBytes(data));
            }
        }
    }
}
