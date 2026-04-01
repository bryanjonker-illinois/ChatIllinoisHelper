using System.Text.RegularExpressions;

namespace ChatIllinoisHelper.Data.Logic {
    public static class ThinkTextRemover {
        private static readonly Regex ThinkTagPattern = new Regex(
            @"<think>.*?</think>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static string RemoveThinkTags(this string input) {
            if (string.IsNullOrEmpty(input)) {
                return input;
            }

            return ThinkTagPattern.Replace(input, string.Empty);
        }
    }
}
