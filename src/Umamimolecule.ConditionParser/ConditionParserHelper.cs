using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Umamimolecule
{
    internal class ConditionParserHelper
    {
        private readonly ConditionParserOptions options;

        public ConditionParserHelper(ConditionParserOptions options = null)
        {
            this.options = options ?? ConditionParserOptions.Default;
        }

        private static readonly Dictionary<Type, Func<object, object>> Converters =
            new Dictionary<Type, Func<object, object>>()
            {
                {typeof(int), (x) => Convert.ToDouble(x)},
                {typeof(DateOnly), (x) => ((DateOnly) x).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)},
            };

        private static readonly Type[] AllTypes = new[]
            {typeof(string), typeof(int), typeof(double), typeof(DateOnly), typeof(DateTime), typeof(bool)};

        private static readonly Type[] AllExceptBool = new[]
            {typeof(string), typeof(int), typeof(double), typeof(DateOnly), typeof(DateTime)};

        private static readonly Type[] StringOnly = new[] {typeof(string)};

        private static readonly Type[] BoolOnly = new[] {typeof(bool)};

        private static readonly Operator[] UnaryOperators = new[]
        {
            Operator.IsEmpty,
            Operator.IsFalse,
            Operator.IsNull,
            Operator.IsNullOrWhitespace,
            Operator.IsTrue,
            Operator.IsNotEmpty,
            Operator.IsNotNull,
            Operator.IsNotNullOrWhitespace,
        };

        private static readonly Dictionary<string, Operator> OperatorAliases =
            new Dictionary<string, Operator>(StringComparer.OrdinalIgnoreCase)
            {
                {"ne", Operator.DoesNotEqual},
                {"!=", Operator.DoesNotEqual},
                {"eq", Operator.Equals},
                {"==", Operator.Equals},
                {"gt", Operator.GreaterThan},
                {">", Operator.GreaterThan},
                {"ge", Operator.GreaterThanOrEqual},
                {">=", Operator.GreaterThanOrEqual},
                {"lt", Operator.LessThan},
                {"<", Operator.LessThan},
                {"le", Operator.LessThanOrEqual},
                {"<=", Operator.LessThanOrEqual},
            };

        private static readonly Dictionary<Operator, Type[]> ApplicableTypes = new Dictionary<Operator, Type[]>()
        {
            {Operator.Contains, StringOnly},
            {Operator.DoesNotContain, StringOnly},
            {Operator.DoesNotEndWith, StringOnly},
            {Operator.DoesNotEqual, AllTypes},
            {Operator.DoesNotMatchRegex, StringOnly},
            {Operator.DoesNotStartWith, StringOnly},
            {Operator.EndsWith, StringOnly},
            {Operator.Equals, AllTypes},
            {Operator.GreaterThan, AllExceptBool},
            {Operator.GreaterThanOrEqual, AllExceptBool},
            {Operator.IsEmpty, StringOnly},
            {Operator.IsFalse, BoolOnly},
            {Operator.IsNotEmpty, StringOnly},
            {Operator.IsNotNull, AllTypes},
            {Operator.IsNotNullOrWhitespace, StringOnly},
            {Operator.IsNull, AllTypes},
            {Operator.IsNullOrWhitespace, StringOnly},
            {Operator.IsTrue, BoolOnly},
            {Operator.LessThan, AllExceptBool},
            {Operator.LessThanOrEqual, AllExceptBool},
            {Operator.MatchesRegex, StringOnly},
            {Operator.StartsWith, StringOnly},
        };

        private static object ConvertValue(object value)
        {
            if (value != null && Converters.TryGetValue(value.GetType(), out var converter))
            {
                return converter(value);
            }

            return value;
        }

        private static void CheckType(Operator op, object value)
        {
            if (value != null)
            {
                var types = ApplicableTypes[op];
                if (!types.Contains(value.GetType()))
                {
                    throw new InvalidOperationException(
                        $"Object of type {value.GetType()} cannot be used with operator {op}");
                }
            }
        }

        private static object CheckTypeAndConvertValue(Operator op, object left)
        {
            CheckType(op, left);
            var l = ConvertValue(left);
            return l;
        }

        private static (object l, object r) CheckTypeAndConvertValue(Operator op, object left, object right)
        {
            CheckType(op, left);
            CheckType(op, right);

            var l = ConvertValue(left);
            var r = ConvertValue(right);

            if (l != null && r != null && l.GetType() != r.GetType())
            {
                throw new InvalidOperationException(
                    $"Cannot compare value of type {l.GetType().Name} with value of type {r.GetType().Name}");
            }

            return (l, r);
        }

        internal Operator ParseOperator(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Enum.TryParse<Operator>(value, true, out var parsed))
            {
                return parsed;
            }

            if (OperatorAliases.TryGetValue(value, out parsed))
            {
                return parsed;
            }

            throw new ArgumentException($"Value '{value}' is not a valid operator", nameof(value));
        }

        internal bool Evaluate(Operator op, object left)
        {
            if (!UnaryOperators.Contains(op))
            {
                throw new ArgumentException($"Operator '{op}' is not unary and requires a second value", nameof(op));
            }

            var l = CheckTypeAndConvertValue(op, left);

            switch (op)
            {
                case Operator.IsNull:
                    return l == null;

                case Operator.IsNotNull:
                    return l != null;

                case Operator.IsEmpty:
                    return l != null && l.ToString() == string.Empty;

                case Operator.IsNotEmpty:
                    return !(l != null && l.ToString() == string.Empty);

                case Operator.IsNullOrWhitespace:
                    return string.IsNullOrWhiteSpace(l?.ToString());

                case Operator.IsNotNullOrWhitespace:
                    return !string.IsNullOrWhiteSpace(l?.ToString());

                case Operator.IsFalse:
                    return !Convert.ToBoolean(l);

                case Operator.IsTrue:
                    return Convert.ToBoolean(l);
            }

            throw new InvalidOperationException(
                $"Object of type {left.GetType()} cannot be used with operator {op}");
        }

        internal bool Evaluate(Operator op, object left, object right)
        {
            if (UnaryOperators.Contains(op))
            {
                throw new ArgumentException($"Operator '{op}' is unary and does not accept a second value", nameof(op));
            }

            var (l, r) = CheckTypeAndConvertValue(op, left, right);

            switch (op)
            {
                case Operator.Contains:
                    return l != null &&
                           r != null &&
                           l.ToString().Contains(r.ToString(), this.options.StringComparison);

                case Operator.DoesNotContain:
                    return !(l != null &&
                             r != null &&
                             l.ToString().Contains(r.ToString(), this.options.StringComparison));

                case Operator.DoesNotEndWith:
                    return !(l != null &&
                             r != null &&
                             l.ToString().EndsWith(r.ToString(), this.options.StringComparison));

                case Operator.DoesNotEqual:
                    if (l is string || r is string)
                    {
                        return !string.Equals(l?.ToString(), r?.ToString(), this.options.StringComparison);
                    }

                    if (l == null)
                    {
                        return r != null;
                    }

                    return !l.Equals(r);

                case Operator.DoesNotMatchRegex:
                    return !(l != null &&
                             r != null &&
                             Regex.IsMatch(l.ToString(), r.ToString(), this.options.RegexOptions));

                case Operator.DoesNotStartWith:
                    return !(l != null && r != null &&
                             l.ToString().StartsWith(r.ToString(), this.options.StringComparison));

                case Operator.EndsWith:
                    return l != null &&
                           r != null &&
                           l.ToString().EndsWith(r.ToString(), this.options.StringComparison);

                case Operator.Equals:
                    if (l is string || r is string)
                    {
                        return string.Equals(l?.ToString(), r?.ToString(), this.options.StringComparison);
                    }

                    if (l == null)
                    {
                        return r == null;
                    }

                    return l.Equals(r);

                case Operator.GreaterThan:
                    if (l == null || r == null)
                    {
                        return false;
                    }

                    if (l is double)
                    {
                        return (double) l > (double) r;
                    }
                    else if (l is DateTime)
                    {
                        return (DateTime) l > (DateTime) r;
                    }
                    else if (l is string)
                    {
                        return string.Compare(l.ToString(), r.ToString(), this.options.StringComparison) > 0;
                    }

                    break;

                case Operator.GreaterThanOrEqual:
                    if (l == null || r == null)
                    {
                        return false;
                    }

                    if (l is double)
                    {
                        return (double) l >= (double) r;
                    }
                    else if (l is DateTime)
                    {
                        return (DateTime) l >= (DateTime) r;
                    }
                    else if (l is string)
                    {
                        return string.Compare(l.ToString(), r.ToString(), this.options.StringComparison) >= 0;
                    }

                    break;

                case Operator.LessThan:
                    if (l == null || r == null)
                    {
                        return false;
                    }

                    if (l is double)
                    {
                        return (double) l < (double) r;
                    }
                    else if (l is DateTime)
                    {
                        return (DateTime) l < (DateTime) r;
                    }
                    else if (l is string)
                    {
                        return string.Compare(l.ToString(), r.ToString(), this.options.StringComparison) < 0;
                    }

                    break;

                case Operator.LessThanOrEqual:
                    if (l == null || r == null)
                    {
                        return false;
                    }

                    if (l is double)
                    {
                        return (double) l <= (double) r;
                    }
                    else if (l is DateTime)
                    {
                        return (DateTime) l <= (DateTime) r;
                    }
                    else if (l is string)
                    {
                        return string.Compare(l.ToString(), r.ToString(), this.options.StringComparison) <= 0;
                    }

                    break;

                case Operator.MatchesRegex:
                    return l != null &&
                           r != null &&
                           Regex.IsMatch(l.ToString(), r.ToString(), this.options.RegexOptions);

                case Operator.StartsWith:
                    return l != null &&
                           r != null &&
                           l.ToString().StartsWith(r.ToString(), this.options.StringComparison);

                default:
                    throw new ArgumentException($"Unknown operator '{op}'", nameof(op));
            }

            throw new ArgumentException(
                $"Object of type {left.GetType()} cannot be used with operator {op}", nameof(left));
        }
    }
}