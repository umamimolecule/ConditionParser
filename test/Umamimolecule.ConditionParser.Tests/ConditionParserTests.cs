using Shouldly;
using Xunit;

namespace Umamimolecule.ConditionParserTests;

public class ConditionParserTests
{
    private readonly ConditionParser parser = new ConditionParser();

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("(true)", true)]
    [InlineData("(false)", false)]
    [InlineData("((true))", true)]
    [InlineData("((false))", false)]
    public void Booleans(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("true or true", true)]
    [InlineData("true or false", true)]
    [InlineData("false or true", true)]
    [InlineData("false or false", false)]
    public void Or(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("true and true", true)]
    [InlineData("true and false", false)]
    [InlineData("false and true", false)]
    [InlineData("false and false", false)]
    [InlineData("(true and true)", true)]
    [InlineData("(true and (true))", true)]
    [InlineData("(true and ((true)))", true)]
    public void And(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("true Equals true", true)]
    [InlineData("true Equals false", false)]
    [InlineData("false Equals true", false)]
    [InlineData("false Equals false", true)]
    [InlineData("(true Equals true)", true)]
    [InlineData("false Equals null", false)]
    [InlineData("null Equals true", false)]
    [InlineData("(true Equals (true))", true)]
    [InlineData("(true Equals ((true)))", true)]
    [InlineData("((true) Equals true)", true)]
    [InlineData("(((true)) Equals true)", true)]
    [InlineData("((true) Equals (true))", true)]
    [InlineData("(((true)) Equals ((true)))", true)]
    public void Equals_Boolean(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("0.0 Equals 0.0", true)]
    [InlineData("0.0 eq 0.0", true)]
    [InlineData("0.0 == 0.0", true)]
    [InlineData("1.0 Equals 1.1", false)]
    [InlineData("-1.2 Equals -1.2", true)]
    [InlineData("-1.2 Equals 1.2", false)]
    [InlineData("-1.2 Equals -1.1", false)]
    [InlineData("-1.0 Equals -1", true)]
    [InlineData("1.0 Equals 1", true)]
    [InlineData("0.0 Equals 0", true)]
    [InlineData("1.0 Equals 2", false)]
    public void Equals_Double(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("0 Equals 0", true)]
    [InlineData("1 Equals 0", false)]
    [InlineData("-1 Equals -1", true)]
    [InlineData("-1 Equals 1", false)]
    [InlineData("-1 Equals -2", false)]
    [InlineData("0 Equals -0", true)]
    public void Equals_Integer(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("date\"2022-12-31\" Equals date\"2022-12-31\"", true)]
    [InlineData("date\"2022-12-31\" Equals date\"2022-12-30\"", false)]
    [InlineData("null Equals date\"2022-12-31\"", false)]
    [InlineData("date\"2022-12-31\" Equals null", false)]
    public void Equals_Date(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("datetime\"2022-12-31T01:02:03Z\" Equals datetime\"2022-12-31T01:02:03Z\"", true)]
    [InlineData("datetime\"2022-12-31T01:02:03Z\" Equals datetime\"2022-12-30T01:02:03Z\"", false)]
    [InlineData("null Equals datetime\"2022-12-31T01:02:03Z\"", false)]
    [InlineData("datetime\"2022-12-31T01:02:03Z\" Equals null", false)]
    public void Equals_DateTime(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("\"0\" Equals \"0\"", true)]
    [InlineData("\"1\" Equals \"0\"", false)]
    [InlineData("\"\" Equals \"\"", true)]
    [InlineData("\"\" Equals \"1\"", false)]
    [InlineData("\"\" Equals null", false)]
    public void Equals_String(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("null Equals null", true)]
    public void Equals_Null(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("true DoesNotEqual true", false)]
    [InlineData("true ne true", false)]
    [InlineData("true != true", false)]
    [InlineData("true DoesNotEqual false", true)]
    [InlineData("true ne false", true)]
    [InlineData("true != false", true)]
    [InlineData("false DoesNotEqual true", true)]
    [InlineData("false DoesNotEqual false", false)]
    [InlineData("(true DoesNotEqual true)", false)]
    [InlineData("false DoesNotEqual null", true)]
    [InlineData("null DoesNotEqual true", true)]
    public void DoesNotEqual_Boolean(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("0.0 DoesNotEqual 0.0", false)]
    [InlineData("1.0 DoesNotEqual 1.1", true)]
    [InlineData("-1.2 DoesNotEqual -1.2", false)]
    [InlineData("-1.2 DoesNotEqual 1.2", true)]
    [InlineData("-1.2 DoesNotEqual -1.1", true)]
    [InlineData("-1.0 DoesNotEqual -1", false)]
    [InlineData("1.0 DoesNotEqual 1", false)]
    [InlineData("0.0 DoesNotEqual 0", false)]
    [InlineData("1.0 DoesNotEqual 2", true)]
    public void DoesNotEqual_Double(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("0 DoesNotEqual 0", false)]
    [InlineData("1 DoesNotEqual 0", true)]
    [InlineData("-1 DoesNotEqual -1", false)]
    [InlineData("-1 DoesNotEqual 1", true)]
    [InlineData("-1 DoesNotEqual -2", true)]
    [InlineData("0 DoesNotEqual -0", false)]
    public void DoesNotEqual_Integer(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("date\"2022-12-31\" DoesNotEqual date\"2022-12-31\"", false)]
    [InlineData("date\"2022-12-31\" DoesNotEqual date\"2022-12-30\"", true)]
    [InlineData("null DoesNotEqual date\"2022-12-31\"", true)]
    [InlineData("date\"2022-12-31\" DoesNotEqual null", true)]
    public void DoesNotEqual_Date(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("datetime\"2022-12-31T01:02:03Z\" DoesNotEqual datetime\"2022-12-31T01:02:03Z\"", false)]
    [InlineData("datetime\"2022-12-31T01:02:03Z\" DoesNotEqual datetime\"2022-12-30T01:02:03Z\"", true)]
    [InlineData("null DoesNotEqual datetime\"2022-12-31T01:02:03Z\"", true)]
    [InlineData("datetime\"2022-12-31T01:02:03Z\" DoesNotEqual null", true)]
    public void DoesNotEqual_DateTime(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("\"0\" DoesNotEqual \"0\"", false)]
    [InlineData("\"1\" DoesNotEqual \"0\"", true)]
    [InlineData("\"\" DoesNotEqual \"\"", false)]
    [InlineData("\"\" DoesNotEqual \"1\"", true)]
    [InlineData("\"\" DoesNotEqual null", true)]
    public void DoesNotEqual_String(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("null DoesNotEqual null", false)]
    public void DoesNotEqual_Null(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("(100.1) GreaterThan 10.5", true)]
    [InlineData("(100.1) GreaterThan (10.5)", true)]
    [InlineData("(100.1 LessThanOrEqual 100)", false)]
    [InlineData("(100.1 GreaterThan 10.5) and (100.1 LessThanOrEqual 100)", false)]
    [InlineData("(100.1 GreaterThan 10.5 and 100.1 LessThanOrEqual 200)", true)]
    [InlineData("(100.1 GreaterThan 10.5 and 100.1 LessThanOrEqual 200) and (100 IsNull)", false)]
    public void CompoundExpressions(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData("100.1 DoesNotEqual 210.5", true)]
    [InlineData("100.1 DOESNOTEQUAL 210.5", true)]
    [InlineData("100.1 ne 210.5", true)]
    [InlineData("100.1 NE 210.5", true)]
    [InlineData("100.1 != 210.5", true)]
    [InlineData("100.1 Equals 100.1", true)]
    [InlineData("100.1 eq 100.1", true)]
    [InlineData("100.1 == 100.1", true)]
    [InlineData("100.1 LessThan 210.5", true)]
    [InlineData("100.1 lt 210.5", true)]
    [InlineData("100.1 < 210.5", true)]
    [InlineData("100.1 LessThanOrEqual 210.5", true)]
    [InlineData("100.1 le 210.5", true)]
    [InlineData("100.1 <= 210.5", true)]
    [InlineData("100.1 GreaterThan 10.5", true)]
    [InlineData("100.1 gt 10.5", true)]
    [InlineData("100.1 > 10.5", true)]
    [InlineData("100.1 GreaterThanOrEqual 10.5", true)]
    [InlineData("100.1 ge 10.5", true)]
    [InlineData("100.1 >= 10.5", true)]
    public void OperatorAliases(string input, bool expected)
    {
        var result = this.parser.Parse(input);
        result.ShouldBe(expected);
    }
}