using System.Text.RegularExpressions;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class StringExtensions
    {
        public static int GetWordCount(this string str)
        {
            return Regex.Matches(str, "(\\w+)").Count;
        }
        
        public static float GetTimeToRead(this string str, float wordsPerMinute = 200)
        {
            float x = GetWordCount(str) / wordsPerMinute;
            
            int minutes = (int) x;
            float seconds = Mathf.Ceil((x * 60) + ((x - minutes) * 0.6f));

            return seconds;
        }
    }
}