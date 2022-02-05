using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Validation
{
    public interface IUserValidationModel
    {
        public string GetLogin() => string.Empty;

        public string GetPassword() => string.Empty;

        public string GetPhoneNumber() => string.Empty;

        public string GetEmail() => string.Empty;
    }
}
