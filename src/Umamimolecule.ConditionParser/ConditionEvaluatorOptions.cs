using System;
using System.Text.RegularExpressions;

namespace Umamimolecule.ConditionParser;

internal class ConditionEvaluatorOptions
{
    public static ConditionEvaluatorOptions Default = new ConditionEvaluatorOptions()
    {
        StringComparison = StringComparison.OrdinalIgnoreCase,
        RegexOptions = RegexOptions.IgnoreCase,
    };

    public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    public RegexOptions RegexOptions { get; set; } = RegexOptions.IgnoreCase;
}