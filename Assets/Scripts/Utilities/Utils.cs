using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ERPK.Utils
{
    public static class Utils
    {
        public static string FormatTimestamp(long timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp);

            string stringFormat = "d MMMM - HH:mm";

            return dateTime.ToString(stringFormat);
        }

        /// <summary>
        /// Generates a unique code based on an MD5 has from UtcNow Ticks and SystemInfo.deviceUniqueIdentifier.
        /// </summary>
        /// <returns>The unique code.</returns>
        public static string GenerateUniqueCode()
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#";
            string ticks = DateTime.UtcNow.Ticks.ToString();

            var code = "";

            for (var i = 0; i < characters.Length; i += 2)
            {
                if ((i + 2) <= ticks.Length)
                {
                    var number = int.Parse(ticks.Substring(i, 2));
                    if (number > characters.Length - 1)
                    {
                        var one = double.Parse(number.ToString().Substring(0, 1));
                        var two = double.Parse(number.ToString().Substring(1, 1));
                        code += characters[Convert.ToInt32(one)];
                        code += characters[Convert.ToInt32(two)];
                    }
                    else
                        code += characters[number];
                }
            }

            return GenerateMD5(code + SystemInfo.deviceUniqueIdentifier);
        }

        public static string GenerateMD5(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string FormatNumberKilo(ulong num)
        {
            return FormatNumberKilo(Convert.ToInt64(num));
        }

        public static string FormatNumberKilo(long num)
        {
            if (num >= 100000000000)
            {
                return Math.Round(num / 1000000000D, 1).ToString("0.#B");
            }
            if (num >= 1000000000)
            {
                return Math.Round(num / 1000000000D, 2).ToString("0.##B");
            }
            if (num >= 100000000)
            {
                return Math.Round(num / 1000000D, 1).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return Math.Round(num / 1000000D, 2).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return Math.Round(num / 1000D, 1).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return Math.Round(num / 1000D, 2).ToString("0.##k");
            }
            return num.ToString("#,0");
        }

        public static bool IsEmailValid(string email)
        {
            Regex regex = new Regex(@"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            return false;
        }

        public static string FormatNumberDecimal(float num, uint decimals)
        {
            return num.ToString("n" + decimals);
        }

        public static string FormatNumberIndicator(long num)
        {
            if (num >= 100000000)
            {
                return Math.Round(num / 1000000D, 0).ToString("0");
            }
            if (num >= 1000000)
            {
                return Math.Round(num / 1000000D, 0).ToString("0");
            }
            if (num >= 100000)
            {
                return Math.Round(num / 10000D, 0).ToString("0");
            }
            if (num >= 1000)
            {
                return Math.Round(num / 1000D, 0).ToString("0");
            }
            return num.ToString("#,0");
        }

        public static string RemoveSpecialCharacters(string _str, bool _allowDotsAndUnderscores = false)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in _str)
            {
                if (_allowDotsAndUnderscores)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' ||
                        c == '_')
                        sb.Append(c);
                }
                else
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                        sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string FormatCamelCaseStrings(string str, bool firstLetterUpperCase = true)
        {
            str = Regex.Replace(str, @"\B[A-Z]", m => " " + m.ToString().ToLower());
            if (firstLetterUpperCase)
            {
                str = Regex.Replace(str, @"^[a-z]", m => " " + m.ToString().ToUpper());
            }

            return str;
        }

        // Convert the string to camel case.
        public static string ToCamelCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return the_string;

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string FormatNumber(long num)
        {
            return num.ToString("n0");
        }

        public static int UnixTimestamp()
        {
            return (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }

    public static class Encryptor
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}