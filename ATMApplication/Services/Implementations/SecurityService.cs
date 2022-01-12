using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public class SecurityService : ISecurityService
    {
        public string GetPasswordHash(string password)
        {
            string hash = null;
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }

            return hash;
        }

        public bool ValidatePassword(string password, string hash)
        {
            return hash.Equals(GetPasswordHash(password));
        }

        public string GenerateCode(int length = 10)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string Encrypt(string textToEncrypt, string key)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(textToEncrypt);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    textToEncrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            return textToEncrypt;
        }

        public string Decrypt(string textToDecrypt, string key)
        {
            textToDecrypt = textToDecrypt.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(textToDecrypt);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    textToDecrypt = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return textToDecrypt;
        }

        public string GenerateToken(string seed)
        {
            byte[] code = Encoding.ASCII.GetBytes(seed);
            byte[] separator = Encoding.ASCII.GetBytes("&");
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Encoding.ASCII.GetString(code.Concat(separator).Concat(key).ToArray());

            return token;
        }

        public string GetSeedFromToken(string token)
        {
            byte[] data = Convert.FromBase64String(token);
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));

            return null;
        }
    }
}
