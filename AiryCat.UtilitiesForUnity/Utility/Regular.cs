using System.Text.RegularExpressions;
using UnityEngine;

namespace AiryCat.UtilitiesForUnity.Utility
{
    public class Regular : MonoBehaviour
    {
        private static string _patternEmail =
            "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

        // Use this for initialization
        public static bool IsValidEmail(string email)
        {
            var check = new Regex(_patternEmail, RegexOptions.IgnorePatternWhitespace);
            var result = !string.IsNullOrEmpty(email) && check.IsMatch(email);
            return result;
        }
    }
}
