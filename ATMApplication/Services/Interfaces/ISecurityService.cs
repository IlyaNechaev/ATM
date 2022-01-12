using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    // Работа с паролями и шифрованием
    public interface ISecurityService
    {
        // Получить хэш-сумму пароля
        public string GetPasswordHash(string password);

        // Соответствие пароля хэш-сумме
        public bool ValidatePassword(string password, string hash);

        // Шифрование текста
        public string Encrypt(string textToEncrypt, string key);

        // дешифрование текста
        public string Decrypt(string textToDecrypt, string key);

        // Генерация случайного кода длиной length
        public string GenerateCode(int length = 10);

        public string GenerateToken(string seed);

        public string GetSeedFromToken(string token);
    }
}
