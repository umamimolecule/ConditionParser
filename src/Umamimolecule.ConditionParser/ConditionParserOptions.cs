using System;
using System.Text.RegularExpressions;

namespace Umamimolecule
{
    /// <summary>
    /// Options for a <see cref="ConditionParser"/> instance.
    /// </summary>
    public class ConditionParserOptions
    {
        /// <summary>
        /// The default options.
        /// </summary>
        public static ConditionParserOptions Default = new ConditionParserOptions()
        {
            StringComparison = StringComparison.OrdinalIgnoreCase,
            RegexOptions = RegexOptions.IgnoreCase,
        };

        /// <summary>
        /// Get or set the behaviour for string comparison operations.
        /// </summary>
        public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Gets or sets the behaviour for regex operations.
        /// </summary>
        public RegexOptions RegexOptions { get; set; } = RegexOptions.IgnoreCase;
    }
}