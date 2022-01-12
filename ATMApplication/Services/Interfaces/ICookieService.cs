using System.Collections.Generic;

namespace ATMApplication.Services
{
    public interface ICookieService
    {
        public string GenerateCookie(List<(string Key, string Value)> cookieClaims, string encryptKey = "atm_ecnrypt_key");
        public string GetCookie(string key, string cookies, string encryptKey = "atm_ecnrypt_key");
    }
}