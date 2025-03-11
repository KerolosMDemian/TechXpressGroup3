
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TechXpress.Data.ValueObjects
{
    public class EmailUser
    {
        public string Value { get;}
        private EmailUser()
        {
            Value = string.Empty;
        }
        public EmailUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$"))
                throw new ArgumentException("Invalid Email Format");
            Value = email;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not EmailUser other) return false;
            return Value == other.Value;
        }
        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString()
        {
            return Value;
        }
    }
}
