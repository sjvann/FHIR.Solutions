using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Extension
{
    public static class StringExtension
    {
        public static string ToUpperFirstLetter(this string source)
        {
            if(string.IsNullOrEmpty(source)) { return string.Empty; }
            var letters = source.ToCharArray();

            letters[0] = char.ToUpper(letters[0]);
            return new string(letters);
        }
        public static bool IsUpperFirstLetter(this string source)
        {
            if(string.IsNullOrEmpty(source)) { return false; }
            var letters = source.ToCharArray();
            return Char.IsUpper(letters[0]);
        }
        public static string RemoveSomeString(this string source, char startChar, char? endChar)
        {
            if (string.IsNullOrEmpty(source)) { return string.Empty; }
            string removeTarget = startChar + source.GetStringBetweenTwoChars(startChar, endChar) + endChar;
            return string.IsNullOrEmpty(removeTarget)?source: source.Replace(removeTarget, "").Trim();
            
        }
        public static string GetStringBetweenTwoChars(this string source, char startChar, char? endChar)
        {
            if(endChar != null)
            {
                if (source.Contains(startChar) )
                {
                    int startPoint = source.IndexOf(startChar) + 1;
                    int endPoint = source.IndexOf(endChar.Value) < 0? source.Length-1:source.IndexOf(endChar.Value);
                    return source.Substring(startPoint, endPoint - startPoint);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                if (source.Contains(startChar))
                {
                    int startPoint = source.IndexOf(startChar) + 1;
                    int endPoint = source.Length-1;
                    return source.Substring(startPoint, endPoint - startPoint);
                }
                else
                {
                    return string.Empty;
                }
            }
           

        }
    }
}
