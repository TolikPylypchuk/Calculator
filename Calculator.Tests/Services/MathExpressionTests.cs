namespace Calculator.Tests.Services;

public class MathExpressionTests
{
    [Theory]
    [InlineData("1 + 2", "1 2 +")]
    [InlineData("1 - 2 * 4", "1 2 4 * -")]
    [InlineData("( 1 - 2 ) * 4", "1 2 - 4 *")]
    public void ToPostfixForm(string expr, string result) =>
        Assert.Equal(result, expr.ToPostfixForm());

    [Theory]
    [InlineData("1 + 2", "3")]
    [InlineData("1 - 2 * 4", "-7")]
    [InlineData("( 1 - 2 ) * 4", "-4")]
    public void Parse(string expr, string result) =>
        Assert.Equal(result, MathExpression.Parse(expr));
}
