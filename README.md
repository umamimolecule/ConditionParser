# ConditionParser

A rules engine to evaluate multiple combinations of conditions.

# Installation

Install the [ConditionParser NuGet package](http://nuget.org/packages/Umamimolecule.ConditionParser) in Visual Studio.

    PM> Install-Package Umamimolecule.ConditionParser

# Quick start

```c#
using System;
using Umamimolecule;

namespace ConditionParserExample
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Enter a number: ");
            string number = Console.ReadLine();
            if (double.TryParse(number, out var _))
            {
                // A simple example to check if a number is > 10.5 and <= 100
                var input = $"({number} GreaterThan 10.5) and ({number} LessThanOrEqual 100)";
                var result = new ConditionParser().Parse(input);
                Console.WriteLine($"{input} = {result}");
            }
            else
            {
                Console.WriteLine($"'{number}' is not a valid number.");
            }
        }
    }
}
```

# Documentation

