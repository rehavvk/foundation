using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rehawk.Foundation.Misc
{
    public static class NameUtility
    {
        public static string GetUniqueName(string formerName, string[] allNames)
        {
            Match match = Regex.Match(formerName, @"^(.+) \d+$");
            string firstPart = formerName;

            if (match.Success)
            {
                firstPart = match.Groups[1].Value;
            }

            string regex = @"^" + firstPart + @" (\d+)$";
            var numbers = new HashSet<int>(allNames.Select(name => GetNameNumber(name, regex)));

            return $"{firstPart} {GetFirstAvailableNumber(numbers)}";
        }

        private static int GetNameNumber(string completeName, string regex)
        {
            Match match = Regex.Match(completeName, regex);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            return 0;
        }

        private static int GetFirstAvailableNumber(ICollection<int> existingNumbers)
        {
            int result = 1;

            while (existingNumbers.Contains(result))
            {
                result++;
            }

            return result;
        }
    }
}