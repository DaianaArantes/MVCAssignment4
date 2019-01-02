/*
* KGSail MVC Application Assignment 4
*
* KGValidations has methos to validate and transform inputs form metadataClass
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;


namespace KGClassLibrary
{
    public class KGValidations
    {
        /// <summary>
        /// Capitalizes input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string KGCapitalize(string input)
        {
            if (input == null)
            {
                return input = "";
            }
            input = input.ToLower();
            input = input.Trim();

            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            return myTI.ToTitleCase(input);

        }

        /// <summary>
        /// Extract and return only digits from input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string KGExtractDigits (string input )
        {
            return new String(input.Where(Char.IsDigit).ToArray());
        }

        /// <summary>
        /// Validades postalCode input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool KGPostalCodeValidation(string input)
        {
            if (input == null || input == "")
            {
                return true;
            }

            input = input.ToUpper();
            // if ismatch is true, return true, if is false return false
            return Regex.IsMatch(input, "[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy][0-9][ABCEGHJKLMNPRSTVWXYZabceghjklmnprstvwxyz] ?[0-9][ABCEGHJKLMNPRSTVWXYZabceghjklmnprstvwxyz][0-9]");
        }

        /// <summary>
        /// validates Zip code input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool KGZipCodeValidation(ref string input)
        {
            if (input == null || input == "")
            {
                input = "";
                return true;
            }

            string numberDigits = KGExtractDigits(input);
            

            if (numberDigits.Length == 5)
            {
                input = numberDigits;
                return true;
            }
            else if (numberDigits.Length == 9)
            {
                input = string.Format("{0:#####-####}", Convert.ToSingle(numberDigits));
                return true;
            }

            return false;

        }

        /// <summary>
        /// Formats postalcode, insert space if there is none
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string KGPostalCodeFormat(string input)
        {
            if (KGPostalCodeValidation(input) && (input != null|| input != ""))
            {
                if (!input.Contains(" "))
                {
                    input = input.Insert(3, " ").ToUpper();
                }
            }
            return input;
        }

        /// <summary>
        /// Trim input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string KGTrimFields(string input)
        {
            if (input != null)
            {
                return input.Trim();
            }
            else
            {
                return "";
            }
        }
    }
}
