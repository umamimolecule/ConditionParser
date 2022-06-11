using System;
using System.Text.RegularExpressions;

namespace Umamimolecule
{
    public class ConditionParserOptions
    {
        public static ConditionParserOptions Default = new ConditionParserOptions()
        {
            StringComparison = StringComparison.OrdinalIgnoreCase,
            RegexOptions = RegexOptions.IgnoreCase,
        };

        public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

        public RegexOptions RegexOptions { get; set; } = RegexOptions.IgnoreCase;
    }
}