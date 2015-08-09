using System.Collections.Generic;

namespace SimpleGrind.Extensions
{
    public static class StringExtensions
    {
        public static Dictionary<string, string> ToDictionary(this string arg,char pairSeparator,char fieldSeparator)
        {
            var dictionary = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(arg)) return dictionary;
            foreach (var keyValuePair in arg.Split(pairSeparator))
            {
                var keyValue = keyValuePair.Split(fieldSeparator);
                dictionary.Add(keyValue[0], keyValue[1]);
            }
            return dictionary;
        }
    }
}
