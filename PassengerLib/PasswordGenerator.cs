﻿using System.Security.Cryptography;

namespace PassengerLib
{
    public class PasswordGenerator
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        private const string Numbers = "0123456789";
        private const string Symbols = "`~!@#$%^&*()-_=+[]{}\\|;:',<.>/?";

        public static string GeneratePassword(int length = 16, bool useUpper = true, bool useLower = true,
            bool useSymbols = true, bool useNumbers = true)
        {
            if (length < 1)
            {
                throw new ArgumentException($"Can not make a string of {length} length");
            }

            if (!new[] { useLower, useUpper, useSymbols, useNumbers }.Any(e => e))
            {
                throw new ArgumentException($"Can not make a string of {length} length while not using any chars");
            }

            var collection = useLower ? Alphabet.ToLower() : "";
            collection += useNumbers ? Numbers : "";
            collection += useUpper ? Alphabet.ToUpper() : "";
            collection += useSymbols ? Symbols : "";

            return GeneratePassword(collection.OrderBy(r => Guid.NewGuid().GetHashCode()).ToArray(), 0, length, useUpper, useLower, useSymbols, useNumbers);
        }

        private static string GeneratePassword(char[] chars, int attempt, int length = 16, bool useUpper = true, bool useLower = true,
             bool useSymbols = true, bool useNumbers = true)
        {

            var bytes = new byte[length * 8];
            RandomNumberGenerator.Create().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = chars[value % (uint)chars.Length];
            }
            var password = string.Join("", result);
            if (length > 7 && attempt < 5 && (useLower && !password.Any(e => Alphabet.ToLower().Contains(e))) ||
                (useSymbols && !password.Any(e => Symbols.Contains(e))) ||
                    (useNumbers && !password.Any(e => Numbers.Contains(e))) ||
                (useUpper && !password.Any(e => Alphabet.ToUpper().Contains(e))))
            {
                return GeneratePassword(chars, attempt + 1, length, useUpper, useUpper, useSymbols, useNumbers);
            }
            return password;
        }
    }
}
