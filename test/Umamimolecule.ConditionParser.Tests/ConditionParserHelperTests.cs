using System;
using System.Globalization;
using Shouldly;
using Xunit;

namespace Umamimolecule.ConditionParserHelperTests;

public class ConditionParserHelperTests
{
    private readonly ConditionParserHelper instance = new ConditionParserHelper();
    
    [Theory]
    [InlineData(Operator.DoesNotEqual, "DoesNotEqual", "DOESNOTEQUAL", "ne", "NE", "!=")]
    [InlineData(Operator.Equals, "Equals", "eq", "==")]
    [InlineData(Operator.GreaterThan, "GreaterThan", "gt", ">")]
    [InlineData(Operator.GreaterThanOrEqual, "GreaterThanOrEqual", "ge", ">=")]
    [InlineData(Operator.LessThan, "LessThan", "lt", "<")]
    [InlineData(Operator.LessThanOrEqual, "LessThanOrEqual", "le", "<=")]
    internal void ParseOperator(Operator expected, params string[] values)
    {
        foreach (var value in values)
        {
            var result = this.instance.ParseOperator(value);
            result.ShouldBe(expected);
        }
    }
    
    [Theory]
    [InlineData("abc", "b", true)]
    [InlineData("ABC", "ab", true)]
    [InlineData("abc", "d", false)]
    [InlineData("abc", null, false)]
    [InlineData(null, "a", false)]
    public void Evaluate_Contains(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.Contains, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "b", false)]
    [InlineData("ABC", "ab", false)]
    [InlineData("abc", "d", true)]
    [InlineData("abc", null, true)]
    [InlineData(null, "a", true)]
    public void Evaluate_DoesNotContain(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.DoesNotContain, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "b", true)]
    [InlineData("ABC", "c", false)]
    [InlineData("abc", "abc", false)]
    [InlineData("abc", null, true)]
    [InlineData(null, "a", true)]
    public void Evaluate_DoesNotEndWith(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.DoesNotEndWith, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "b", true)]
    [InlineData("abc", "", true)]
    [InlineData("", "a", true)]
    [InlineData("ABC", "abc", false)]
    [InlineData("abc", null, true)]
    [InlineData(null, "a", true)]
    [InlineData(null, null, false)]
    [InlineData(1, 1.0, false)]
    [InlineData(-1, -1.0, false)]
    [InlineData(1, 1.1, true)]
    [InlineData(false, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(false, true, true)]
    public void Evaluate_DoesNotEqual(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.DoesNotEqual, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31", false)]
    [InlineData("2022-12-31", "2022-12-30", true)]
    [InlineData("2022-12-31", null, true)]
    [InlineData(null, "2022-12-30", true)]
    public void Evaluate_DoesNotEqual_DateOnly(string left, string right, bool expected)
    {
        object leftDateOnly = left == null ? null : DateOnly.Parse(left);
        object rightDateOnly = right == null ? null : DateOnly.Parse(right);
        
        var result = this.instance.Evaluate(Operator.DoesNotEqual, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-31T01:02:03Z", false)]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-30T01:02:03Z", true)]
    [InlineData("2022-12-31T01:02:03Z", null, true)]
    [InlineData(null, "2022-12-30T01:02:03Z", true)]
    public void Evaluate_DoesNotEqual_DateTime(string left, string right, bool expected)
    {
        object leftDateTime = left == null ? null : DateTime.Parse(left);
        object rightDateTime = right == null ? null : DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.DoesNotEqual, leftDateTime, rightDateTime);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31T00:00:00Z", false)]
    [InlineData("2022-12-30", "2022-12-31T01:02:03Z", true)]
    public void Evaluate_DoesNotEqual_DateOnlyDateTime(string left, string right, bool expected)
    {
        object leftDateTime = DateOnly.Parse(left);
        object rightDateTime = DateTime.Parse(right, styles: DateTimeStyles.AdjustToUniversal);
        
        var result = this.instance.Evaluate(Operator.DoesNotEqual, leftDateTime, rightDateTime);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.DoesNotEqual, rightDateTime, leftDateTime);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", ".*", false)]
    [InlineData("abc", "abc", false)]
    [InlineData("ABC", "abc", false)]
    [InlineData("abc", "d", true)]
    [InlineData("", "a", true)]
    [InlineData("abc", null, true)]
    [InlineData(null, "a", true)]
    public void Evaluate_DoesNotMatchRegex(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.DoesNotMatchRegex, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "a", false)]
    [InlineData("ABC", "abc", false)]
    [InlineData("abc", "d", true)]
    [InlineData("", "a", true)]
    [InlineData("abc", null, true)]
    [InlineData(null, "a", true)]
    [InlineData(null, null, true)]
    public void Evaluate_DoesNotStartWith(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.DoesNotStartWith, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "b", false)]
    [InlineData("ABC", "c", true)]
    [InlineData("abc", "abc", true)]
    [InlineData("abc", null, false)]
    [InlineData(null, "a", false)]
    public void Evaluate_EndsWith(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.EndsWith, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "b", false)]
    [InlineData("abc", "", false)]
    [InlineData("", "a", false)]
    [InlineData("ABC", "abc", true)]
    [InlineData("abc", null, false)]
    [InlineData(null, "a", false)]
    [InlineData(null, null, true)]
    [InlineData(1, 1.0, true)]
    [InlineData(-1, -1.0, true)]
    [InlineData(1, 1.1, false)]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    [InlineData(false, true, false)]
    public void Evaluate_Equals(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.Equals, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31", true)]
    [InlineData("2022-12-31", "2022-12-30", false)]
    [InlineData("2022-12-31", null, false)]
    [InlineData(null, "2022-12-30", false)]
    public void Evaluate_Equals_DateOnly(string left, string right, bool expected)
    {
        object leftDateOnly = left == null ? null : DateOnly.Parse(left);
        object rightDateOnly = right == null ? null : DateOnly.Parse(right);
        
        var result = this.instance.Evaluate(Operator.Equals, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-31T01:02:03Z", true)]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-30T01:02:03Z", false)]
    [InlineData("2022-12-31T01:02:03Z", null, false)]
    [InlineData(null, "2022-12-30T01:02:03Z", false)]
    public void Evaluate_Equals_DateTime(string left, string right, bool expected)
    {
        object leftDateTime = left == null ? null : DateTime.Parse(left);
        object rightDateTime = right == null ? null : DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.Equals, leftDateTime, rightDateTime);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31T00:00:00Z", true)]
    [InlineData("2022-12-30", "2022-12-31T01:02:03Z", false)]
    public void Evaluate_Equals_DateOnlyDateTime(string left, string right, bool expected)
    {
        object leftDateTime = DateOnly.Parse(left);
        object rightDateTime = DateTime.Parse(right, styles: DateTimeStyles.AdjustToUniversal);
        
        var result = this.instance.Evaluate(Operator.Equals, leftDateTime, rightDateTime);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.Equals, rightDateTime, leftDateTime);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(0, 0, false, false)]
    [InlineData(1, 0, true, false)]
    [InlineData(1.1, 1.1, false, false)]
    [InlineData(1.2, 1.1, true, false)]
    [InlineData(1, 1.0, false, false)]
    [InlineData(2, 1.1, true, false)]
    [InlineData("a", "a", false, false)]
    [InlineData("abc", "ABC", false, false)]
    [InlineData("b", "a", true, false)]
    [InlineData("a", "", true, false)]
    [InlineData("a", null, false, false)]
    [InlineData(null, null, false, false)]
    public void Evaluate_GreaterThan(object left, object right, bool expected, bool expectedInverse)
    {
        var result = this.instance.Evaluate(Operator.GreaterThan, left, right);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThan, right, left);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-31T01:02:03Z", false, false)]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-30T01:02:03Z", true, false)]
    public void Evaluate_GreaterThan_DateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateTime.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.GreaterThan, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThan, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31", false, false)]
    [InlineData("2022-12-31", "2022-12-30", true, false)]
    public void Evaluate_GreaterThan_DateOnly(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateOnly.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.GreaterThan, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThan, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31T00:00:00Z", false, false)]
    [InlineData("2022-12-31", "2022-12-30T01:00:00Z", true, false)]
    [InlineData("2022-12-30", "2022-12-31T01:02:03Z", false, true)]
    public void Evaluate_GreaterThan_DateOnlyDateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateTime = DateOnly.Parse(left);
        object rightDateTime = DateTime.Parse(right, styles: DateTimeStyles.AdjustToUniversal);
        
        var result = this.instance.Evaluate(Operator.GreaterThan, leftDateTime, rightDateTime);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThan, rightDateTime, leftDateTime);
        result.ShouldBe(expectedInverse);
    }
    
    [Theory]
    [InlineData(0, 0, true, true)]
    [InlineData(1, 0, true, false)]
    [InlineData(1.1, 1.1, true, true)]
    [InlineData(1.2, 1.1, true, false)]
    [InlineData(1, 1.0, true, true)]
    [InlineData(2, 1.1, true, false)]
    [InlineData("a", "a", true, true)]
    [InlineData("abc", "ABC", true, true)]
    [InlineData("b", "a", true, false)]
    [InlineData("a", "", true, false)]
    [InlineData("a", null, false, false)]
    [InlineData(null, null, false, false)]
    public void Evaluate_GreaterThanOrEqual(object left, object right, bool expected, bool expectedInverse)
    {
        var result = this.instance.Evaluate(Operator.GreaterThanOrEqual, left, right);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThanOrEqual, right, left);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-31T01:02:03Z", true, true)]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-30T01:02:03Z", true, false)]
    public void Evaluate_GreaterThanOrEqual_DateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateTime.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.GreaterThanOrEqual, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThanOrEqual, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31", true, true)]
    [InlineData("2022-12-31", "2022-12-30", true, false)]
    public void Evaluate_GreaterThanOrEqual_DateOnly(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateOnly.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.GreaterThanOrEqual, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThanOrEqual, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31T00:00:00Z", true, true)]
    [InlineData("2022-12-31", "2022-12-30T01:00:00Z", true, false)]
    [InlineData("2022-12-30", "2022-12-31T01:02:03Z", false, true)]
    public void Evaluate_GreaterThanOrEqual_DateOnlyDateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateTime = DateOnly.Parse(left);
        object rightDateTime = DateTime.Parse(right, styles: DateTimeStyles.AdjustToUniversal);
        
        var result = this.instance.Evaluate(Operator.GreaterThanOrEqual, leftDateTime, rightDateTime);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.GreaterThanOrEqual, rightDateTime, leftDateTime);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData(null, true)]
    [InlineData("a", false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(1.1, false)]
    [InlineData(-1.1, false)]
    public void Evaluate_IsNull(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsNull, value);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(null, false)]
    [InlineData("a", true)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    [InlineData(-1, true)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(1.1, true)]
    [InlineData(-1.1, true)]
    public void Evaluate_IsNotNull(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsNotNull, value);
        result.ShouldBe(expected);
    } 
    
    [Theory]
    [InlineData(null, false)]
    [InlineData("", true)]
    [InlineData(" ", false)]
    [InlineData("a", false)]
    public void Evaluate_IsEmpty(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsEmpty, value);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void Evaluate_IsFalse(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsFalse, value);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("a", true)]
    public void Evaluate_IsNotEmpty(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsNotEmpty, value);
        result.ShouldBe(expected);
    } 
    
    [Theory]
    [InlineData(null, false)]
    [InlineData("a", true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    public void Evaluate_IsNotNullOrWhitespace(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsNotNullOrWhitespace, value);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(null, true)]
    [InlineData("a", false)]
    [InlineData("", true)]
    [InlineData(" ", true)]
    public void Evaluate_IsNullOrWhitespace(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsNullOrWhitespace, value);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void Evaluate_IsTrue(object value, bool expected)
    {
        var result = this.instance.Evaluate(Operator.IsTrue, value);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(0, 0, false, false)]
    [InlineData(1, 0, false, true)]
    [InlineData(1.1, 1.1, false, false)]
    [InlineData(1.2, 1.1, false, true)]
    [InlineData(1, 1.0, false, false)]
    [InlineData(2, 1.1, false, true)]
    [InlineData("a", "a", false, false)]
    [InlineData("abc", "ABC", false, false)]
    [InlineData("b", "a", false, true)]
    [InlineData("a", "", false, true)]
    [InlineData("a", null, false, false)]
    [InlineData(null, null, false, false)]
    public void Evaluate_LessThan(object left, object right, bool expected, bool expectedInverse)
    {
        var result = this.instance.Evaluate(Operator.LessThan, left, right);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThan, right, left);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-31T01:02:03Z", false, false)]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-30T01:02:03Z", false, true)]
    public void Evaluate_LessThan_DateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateTime.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.LessThan, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThan, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31", false, false)]
    [InlineData("2022-12-31", "2022-12-30", false, true)]
    public void Evaluate_LessThan_DateOnly(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateOnly.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.LessThan, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThan, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31T00:00:00Z", false, false)]
    [InlineData("2022-12-31", "2022-12-30T01:00:00Z", false, true)]
    [InlineData("2022-12-30", "2022-12-31T01:02:03Z", true, false)]
    public void Evaluate_LessThan_DateOnlyDateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateTime = DateOnly.Parse(left);
        object rightDateTime = DateTime.Parse(right, styles: DateTimeStyles.AdjustToUniversal);
        
        var result = this.instance.Evaluate(Operator.LessThan, leftDateTime, rightDateTime);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThan, rightDateTime, leftDateTime);
        result.ShouldBe(expectedInverse);
    }
    
    [Theory]
    [InlineData(0, 0, true, true)]
    [InlineData(1, 0, false, true)]
    [InlineData(1.1, 1.1, true, true)]
    [InlineData(1.2, 1.1, false, true)]
    [InlineData(1, 1.0, true, true)]
    [InlineData(2, 1.1, false, true)]
    [InlineData("a", "a", true, true)]
    [InlineData("abc", "ABC", true, true)]
    [InlineData("b", "a", false, true)]
    [InlineData("a", "", false, true)]
    [InlineData("a", null, false, false)]
    [InlineData(null, null, false, false)]
    public void Evaluate_LessThanOrEqual(object left, object right, bool expected, bool expectedInverse)
    {
        var result = this.instance.Evaluate(Operator.LessThanOrEqual, left, right);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThanOrEqual, right, left);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-31T01:02:03Z", true, true)]
    [InlineData("2022-12-31T01:02:03Z", "2022-12-30T01:02:03Z", false, true)]
    public void Evaluate_LessThanOrEqual_DateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateTime.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.LessThanOrEqual, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThanOrEqual, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31", true, true)]
    [InlineData("2022-12-31", "2022-12-30", false, true)]
    public void Evaluate_LessThanOrEqual_DateOnly(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateOnly = DateOnly.Parse(left);
        object rightDateOnly = DateTime.Parse(right);
        
        var result = this.instance.Evaluate(Operator.LessThanOrEqual, leftDateOnly, rightDateOnly);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThanOrEqual, rightDateOnly, leftDateOnly);
        result.ShouldBe(expectedInverse);
    } 
    
    [Theory]
    [InlineData("2022-12-31", "2022-12-31T00:00:00Z", true, true)]
    [InlineData("2022-12-31", "2022-12-30T01:00:00Z", false, true)]
    [InlineData("2022-12-30", "2022-12-31T01:02:03Z", true, false)]
    public void Evaluate_LessThanOrEqual_DateOnlyDateTime(string left, string right, bool expected, bool expectedInverse)
    {
        object leftDateTime = DateOnly.Parse(left);
        object rightDateTime = DateTime.Parse(right, styles: DateTimeStyles.AdjustToUniversal);
        
        var result = this.instance.Evaluate(Operator.LessThanOrEqual, leftDateTime, rightDateTime);
        result.ShouldBe(expected);

        result = this.instance.Evaluate(Operator.LessThanOrEqual, rightDateTime, leftDateTime);
        result.ShouldBe(expectedInverse);
    }
    
    [Theory]
    [InlineData("abc", ".*", true)]
    [InlineData("abc", "abc", true)]
    [InlineData("ABC", "abc", true)]
    [InlineData("abc", "d", false)]
    [InlineData("", "a", false)]
    [InlineData("abc", null, false)]
    [InlineData(null, "a", false)]
    public void Evaluate_MatchesRegex(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.MatchesRegex, left, right);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("abc", "a", true)]
    [InlineData("ABC", "abc", true)]
    [InlineData("abc", "d", false)]
    [InlineData("", "a", false)]
    [InlineData("abc", null, false)]
    [InlineData(null, "a", false)]
    [InlineData(null, null, false)]
    public void Evaluate_StartsWith(object left, object right, bool expected)
    {
        var result = this.instance.Evaluate(Operator.StartsWith, left, right);
        result.ShouldBe(expected);
    }
}