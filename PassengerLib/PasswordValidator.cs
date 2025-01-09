using System.Buffers;
using System.Security;

namespace PassengerLib
{
    public static class PasswordValidator
    {
        private static readonly SearchValues<char> s_SpecialChars = 
            SearchValues.Create("`~!@#$%^&*()-_=+[]{}\\|;:',.<>/?");

        private static readonly SearchValues<char> s_CapitalLetters =
            SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZ"); 

        private static readonly SearchValues<char> s_LowercaseLetters =
            SearchValues.Create("abcdefghijklmnopqrstuvwxyz");

        private static readonly SearchValues<char> s_Digits =
            SearchValues.Create("0123456789");

        public static bool ValidatePassword(string password)
        {
            return CheckLength(password) && !CheckSpaceChar(password) && CheckSpecialChars(password)
                && CheckCapitalLetters(password) && CheckLowercaseLetters(password) && CheckDigits(password);
        }


        private static bool CheckLength(string input)
        {
            return input.Length > 8;
        }

        private static bool CheckSpaceChar(string input)
        {
            return input.Contains(' ');
        }

        private static bool CheckSpecialChars(string input)
        {
            return input.AsSpan().IndexOfAny(s_SpecialChars) > -1;
        }

        private static bool CheckCapitalLetters(string input)
        {
            return input.AsSpan().IndexOfAny(s_CapitalLetters) > -1;
        }

        private static bool CheckLowercaseLetters(string input)
        {
            return input.AsSpan().IndexOfAny(s_LowercaseLetters) > -1;
        }

        private static bool CheckDigits(string input)
        {
            return input.AsSpan().IndexOfAny(s_Digits) > -1;
        }

        public static string ConvertSecureStringToString(SecureString data)
        {
            return new System.Net.NetworkCredential(string.Empty, data).Password;
        }

        public static SecureString StringToSecureString(string data)
        {
            var secureString = new SecureString();
            foreach (var c in data)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
    }
}
