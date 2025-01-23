using System.Collections.Generic;

namespace Rehawk.Foundation.CommandTerminal
{
    public class CommandAutocomplete
    {
        private List<string> knownWords = new List<string>();
        private List<string> buffer = new List<string>();

        public void Register(string word)
        {
            knownWords.Add(word.ToLower());
        }

        public string[] Complete(ref string text, ref int formatWidth)
        {
            string partialWord = EatLastWord(ref text).ToLower();
            string known;

            for (int i = 0; i < knownWords.Count; i++)
            {
                known = knownWords[i];

                if (known.StartsWith(partialWord))
                {
                    buffer.Add(known);

                    if (known.Length > formatWidth)
                    {
                        formatWidth = known.Length;
                    }
                }
            }

            string[] completions = buffer.ToArray();
            buffer.Clear();

            text += PartialWord(completions);
            return completions;
        }

        private string EatLastWord(ref string text)
        {
            int lastSpace = text.LastIndexOf(' ');
            string result = text.Substring(lastSpace + 1);

            text = text.Substring(0, lastSpace + 1); // Remaining (keep space)
            return result;
        }

        private string PartialWord(string[] words)
        {
            if (words.Length == 0)
            {
                return "";
            }

            string firstMatch = words[0];
            int partialLength = firstMatch.Length;

            if (words.Length == 1)
            {
                return firstMatch;
            }

            foreach (string word in words)
            {
                if (partialLength > word.Length)
                {
                    partialLength = word.Length;
                }

                for (int i = 0; i < partialLength; i++)
                {
                    if (word[i] != firstMatch[i])
                    {
                        partialLength = i;
                    }
                }
            }

            return firstMatch.Substring(0, partialLength);
        }
    }
}