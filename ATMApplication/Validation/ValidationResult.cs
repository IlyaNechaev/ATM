using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Validation
{
    public class ValidationResult
    {
        public bool HasErrors
        {
            get
            {
                return (ErrorMessages?.Keys.Count ?? 0) + (CommonMessages?.Count ?? 0) > 0;
            }
        }

        public Dictionary<string, List<string>> ErrorMessages { get; set; }

        public List<string> CommonMessages { get; set; }

        public void AddMessage(string key, string message)
        {
            if (ErrorMessages is null)
                ErrorMessages = new(5);

            if (!ErrorMessages.ContainsKey(key))
            {
                ErrorMessages.Add(key, new());
            }

            ErrorMessages[key].Add(message);
        }

        public void AddCommonMessage(string message)
        {
            if (CommonMessages is null)
                CommonMessages = new();

            if (CommonMessages.Contains(message))
                return;

            CommonMessages.Add(message);
        }

        public void RemoveMessages(string key)
        {
            var message = ErrorMessages.Where(msg => msg.Key.Equals(key));

            if (!ErrorMessages.ContainsKey(key))
            {
                return;
            }

            ErrorMessages.Remove(key);
        }

        public void RemoveCommonMessage(string message)
        {
            CommonMessages.Remove(message);
        }
    }
}
