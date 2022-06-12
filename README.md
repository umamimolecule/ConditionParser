<p align="center">
<img src="https://raw.githubusercontent.com/umamimolecule/ConditionParser/main/.assets/logo-light.png#gh-light-mode-only" />
<img src="https://raw.githubusercontent.com/umamimolecule/ConditionParser/main/.assets/logo-dark.png#gh-dark-mode-only" />
</p>

# Condition Parser

A rules engine to evaluate multiple combinations of conditions.

```c#
new ConditionParser().Parse("'Quick brown fox' Contains 'fox'");    // true
new ConditionParser().Parse("123.456 > 200 and 123.456 <= 300");    // false
new ConditionParser().Parse("'Hot coffee' MatchesRegex '^Hot'");    // true
```

## Installation

Install the [ConditionParser NuGet package](http://nuget.org/packages/Umamimolecule.ConditionParser) in Visual Studio.

    PM> Install-Package Umamimolecule.ConditionParser

## Quick start

```c#
using System;
using Umamimolecule;

namespace ConditionParserExample
{
    public static class Program
    {
        public static void Main()
        {
            // A simple example to check if a number is > 10.5 and <= 100
            Console.WriteLine("Enter a number: ");
            string number = Console.ReadLine();
            var input = $"{number} > 10.5 and {number} <= 100";
            var result = new ConditionParser().Parse(input);
            Console.WriteLine($"{input} = {result}");
        }
    }
}
```

## Options

The parser behaviour can be controlled by passing in options in the constructor.

```c#
using System;
using System.Text.RegularExpressions;

var parser = new ConditionParser(
    new ConditionParserOptions()
    {
        // Behaviour for regex operations
        // Default = RegexOptions.IgnoreCase
        RegexOptions = RegexOptions.IgnoreCase | RegexOptions.RightToLeft,
        
        // Behaviour for string comparison operations
        // Default = StringComparison.OrdinalIgnoreCase
        StringComparison = StringComparison.CurrentCulture
    });
```

## Operators

The following operators are available:

| Operator                | Aliases        | Description                                                            | Examples                                                                  |
|-------------------------|----------------|------------------------------------------------------------------------|---------------------------------------------------------------------------|
| `Contains`              |                | Checks whether a string contains a substring                           | `'Been' Contains 'bee'`                                                   |
| `DoesNotContain`        |                | Checks whether a string does not contain a substring                   | `'Been' DoesNotContain 'fee'`                                             |
| `DoesNotEndWith`        |                | Checks whether a string does not end with a string                     | `'Been' DoesNotEndWith 'ien'`                                             |
| `DoesNotEqual`          | `!=`<br />`ne` | Checks whether a string does not equal another string                  | `'Been' != "bee"`<br />`'Been' ne 'bee'`<br />`'Been' DoesNotEqual 'bee'` |
| `DoesNotMatchRegex`     |                | Checks whether a string does not match a regex pattern                 | `'Been' DoesNotMatchRegex '^Bo*n'`                                        |
| `DoesNotStartWith`      |                | Checks whether a string does not start with a string                   | `'Been' DoesNotStartWith 'Fee'`                                           |
| `EndsWith`              |                | Checks whether a string ends with a string                             | `'Been' EndsWith 'en'`                                                    |
| `Equals`                | `==`<br />`eq` | Checks whether a value equals another                                  | `'Been' == 'been'`<br />`'Been" eq 'been'`<br />`'Been' Equals 'been'`    |
| `GreaterThan`           | `>`<br />`gt`  | Checks whether a value is greater than another<sup>1</sup>             | `123.45 > 200`<br />`123.45 gt 200`<br />`123.45 GreaterThan`             |
| `GreaterThanOrEqual`    | `>=`<br />`ge` | Checks whether a value is greater than or equal to another<sup>1</sup> | `123.45 >= 200`<br />`123.45 ge 200`<br />`123.45 GreaterThanOrEqualTo`   |
| `IsNull`                |                | Checks whether a value is null                                         | `'Been' IsNull`                                                           |
| `IsNotNull`             |                | Checks whether a value is not null                                     | `'Been' IsNotNull`                                                        |
| `IsEmpty`               |                | Checks whether a string is empty                                       | `'Been' IsEmpty`                                                          |
| `IsFalse`               |                | Checks whether a boolean value is false<sup>2</sup>                    | `false IsFalse`                                                           |
| `IsNotEmpty`            |                | Checks whether a string is not empty                                   | `'Been' IsNotEmpty`                                                       |
| `IsNotNullOrWhitespace` |                | Checks whether a string is not null or whitespace                      | `'Been' IsNotNullOrWhitespace`                                            |
| `IsNullOrWhitespace`    |                | Checks whether a string is null or whitespace                          | `'Been' IsNullOrWhitespace`                                               |
| `IsTrue`                |                | Checks whether a boolean value is true<sup>2</sup>                     | `true IsTrue`                                                             |
| `LessThan`              | `<`<br />`lt`  | Checks whether a value is less than another<sup>1</sup>                | `123.45 < 200`<br />`123.45 lt 200`<br />`123.45 LessThan 200`            |
| `LessThanOrEqual`       | `<=`<br />`le` | Checks whether a value is less than or equal to another<sup>1</sup>    | `123.45 <= 200`<br />`123.45 le 200`<br />`123.45 LessThanOrEqualTo 200`  |
| `MatchesRegex`          |                | Checks whether a string matches a regex pattern                        | `'Been' MatchesRegex '^B.*n'`                                             |
| `StartsWith`            |                | Checks whether a string starts with a string                           | `'Been' StartsWith 'Be'`                                                  |

<sup>1.</sup>Applies to all data types except for boolean.

<sup>2.</sup>Applies to boolean types only.

## Data types

The following data types are supported:
- [Null](#null)
- [Boolean](#boolean)
- [Date and date-time](#date-and-date-time)
- [Integer](#integer)
- [Double](#double)
- [String](#string)

### Null

Null values are represented by the keyword `null`.

Examples:
```c#
new ConditionParser().Parse("null == null");    // true
new ConditionParser().Parse("null IsNull");     // true
new ConditionParser().Parse("'abc' == null");   // false
```

### Boolean

Represented by the keywords `true` and `false`.

Examples:
```c#
new ConditionParser().Parse("true != false");   // true
new ConditionParser().Parse("true IsFalse");    // false
```

### Date and date-time

Date and date-time values are represented by a quoted date value, prefixed with either `date` or `datetime`.  Single or double-quotes can be used.

Date values are represented in UTC values at midnight.  Date and date-time values can be compared against each other.

Examples:
```c#
new ConditionParser().Parse("date'2022-12-31' > date'2022-12-01'"); // true
new ConditionParser().Parse("datetime'2022-12-31T01:23:45.678Z' > datetime'2022-12-31T00:00:00.000Z"); // true
new ConditionParser().Parse("date'2022-12-31' == datetime'2022-12-31T00:00:00Z'"); // true
```

### Integer

Integers are represented a sequence of digits, optionally with a leading `-` to indicate a negative value.

Examples:
```c#
new ConditionParser().Parse("1 > 0");       // true
new ConditionParser().Parse("-1 < 0");      // true
new ConditionParser().Parse("1234 == 1);    // false
```

### Double

Doubles are represented by a number with a decimal point, optionally with a leading `-` to indicate a negative value.

Doubles and integers can be compared against each other.

Examples:
```c#
new ConditionParser().Parse("1.23 > 1.1");      // true
new ConditionParser().Parse("-1.23 < 0.0");     // true
new ConditionParser().Parse("1.23 > 1");        // true
new ConditionParser().Parse("1.0 == 1");        // true
```

### String

String values in an expression can be surrounded by either single or double quotes.  If the string value contains quotes they need to be escaped as follows:

| Quote type   | Notes                                                                                                        | Examples                                                                                                                    |
|--------------|--------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------|
| Single (`'`) | Escape single quotes within the value as either `''` or `\'`<br />String may contain unescaped double quotes | `'o''brien' EndsWith 'rien'`<br />`'o\'brien' EndsWith 'rien'`<br />`'These "interesting" times' Contains '"int'`           |
| Double (`"`) | Escape double quotes within the value as either `""` or `\"`<br />String may contain unescaped single quotes | `"Bob ""Animal"" Smith" IsNotNull`<br />`"Bob \"Animal\" Smith" IsNotNull`<br />`"These 'interesting' times" Contains "'int"` |

## Order of precedence

Like many other languages like C#, C and Java, `and` operators are evaluated before `or`.

| Expression   | Is the same as   |
|--------------|------------------|
| `A or B and C` | `A or (B and C)` |
| `A and B or C` | `(A and B) or C` |

Expressions can be surrounded by parentheses to control order of evaluation.

```c#
new ConditionParser().Parse("('ABC' Contains 'B' or 'ABC' Contains 'C') and (202 >= 200 and 202 < 400)");    // true
```