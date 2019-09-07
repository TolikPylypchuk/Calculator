using System;

using Calculator.Services;

using Xunit;

using Token = Calculator.Services.MathExpressionBuilder.Token;

namespace Calculator.Tests.Services
{
    public class MathExpressionBuilderTests
    {
        [Fact]
        public void Initialize()
        {
            var testBuilder = new MathExpressionBuilder();

            Assert.Equal(MathExpressionBuilder.DefaultExpression, testBuilder.ToString());
        }

        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('4')]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void AddDigitToEmpty(char digit)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit);

            Assert.Equal(digit.ToString(), testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', '5', "5")]
        [InlineData('1', '6', "16")]
        [InlineData('2', '7', "27")]
        [InlineData('3', '8', "38")]
        [InlineData('4', '9', "49")]
        public void AddDigits(char digit1, char digit2, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit1)
                       .AddDigit(digit2);

            Assert.Equal(result, testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', '+', '5', "0 + 5")]
        [InlineData('1', '-', '6', "1 - 6")]
        [InlineData('2', '*', '7', "2 * 7")]
        [InlineData('3', '/', '8', "3 / 8")]
        [InlineData('4', '^', '9', "4 ^ 9")]
        public void AddDigitsAfterOperator(char digit1, char op, char digit2, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit1)
                       .AddOperator(op)
                       .AddDigit(digit2);

            Assert.Equal(result, testBuilder.ToString());
        }

        [Fact]
        public void AddIncorrectDigit()
        {
            var testBuilder = new MathExpressionBuilder();
            Assert.Throws<ArgumentException>(() => testBuilder.AddDigit('a'));
        }

        [Fact]
        public void AddConstantEmpty()
        {
            var testBuilder = new MathExpressionBuilder();
            testBuilder.AddPi();
            Assert.Equal("p", testBuilder.ToString());
        }

        [Theory]
        [InlineData('+', "p + e")]
        [InlineData('*', "p * e")]
        public void AddConstantAfterOperator(char op, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddPi()
                       .AddOperator(op)
                       .AddE();

            Assert.Equal(result, testBuilder.ToString());
        }

        [Fact]
        public void AddConstantAfterConstant()
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddPi()
                       .AddE();

            Assert.Equal("p * e", testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', "0 * e")]
        [InlineData('1', "1 * e")]
        [InlineData('2', "2 * e")]
        [InlineData('3', "3 * e")]
        public void AddConstantAfterDecimalSeparator(char digit, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddDecimalSeparator()
                       .AddE();

            Assert.Equal(result, testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', '5')]
        [InlineData('1', '6')]
        [InlineData('2', '7')]
        [InlineData('3', '8')]
        [InlineData('4', '9')]
        public void AddDecimalSeparator(char digit1, char digit2)
        {
            var testBuilder = new MathExpressionBuilder();
            string s = MathExpressionBuilder.DecimalSeparator;

            testBuilder.AddDigit(digit1)
                       .AddDecimalSeparator()
                       .AddDigit(digit2);

            Assert.Equal($"{digit1}{s}{digit2}", testBuilder.ToString());

            testBuilder.AddDecimalSeparator();

            Assert.Equal($"{digit1}{s}{digit2}", testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', '5')]
        [InlineData('1', '6')]
        [InlineData('2', '7')]
        [InlineData('3', '8')]
        [InlineData('4', '9')]
        public void CanAddDecimalSeparator(char digit1, char digit2)
        {
            var testBuilder = new MathExpressionBuilder();

            Assert.True(testBuilder.CanAddDecimalSeparator);

            testBuilder.AddDigit(digit1);

            Assert.True(testBuilder.CanAddDecimalSeparator);

            testBuilder.AddDecimalSeparator();

            Assert.False(testBuilder.CanAddDecimalSeparator);

            testBuilder.AddDigit(digit2);

            Assert.False(testBuilder.CanAddDecimalSeparator);

            testBuilder.AddDecimalSeparator();

            Assert.False(testBuilder.CanAddDecimalSeparator);

            testBuilder.AddPi();

            Assert.True(testBuilder.CanAddDecimalSeparator);
        }


        [Theory]
        [InlineData('0', '+', '5', "0 + 5")]
        [InlineData('1', '-', '6', "1 - 6")]
        [InlineData('2', '*', '7', "2 * 7")]
        [InlineData('3', '/', '8', "3 / 8")]
        [InlineData('4', '^', '9', "4 ^ 9")]
        public void AddOperatorSimple(char digit1, char op, char digit2, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit1)
                       .AddOperator(op)
                       .AddDigit(digit2);

            Assert.Equal(result, testBuilder.ToString());
        }

        [Fact]
        public void AddMinusToEmpty()
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddOperator('-');

            Assert.Equal("0 -", testBuilder.ToString());
        }

        [Fact]
        public void AddMinusAfterOpeningParenthesis()
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddOpeningParenthesis()
                       .AddOperator('-');

            Assert.Equal("( 0 -", testBuilder.ToString());
        }

        [Fact]
        public void AddIncorrectOperator()
        {
            var testBuilder = new MathExpressionBuilder();
            Assert.Throws<ArgumentException>(() => testBuilder.AddOperator('a'));
        }

        [Theory]
        [InlineData("sin")]
        [InlineData("cos")]
        [InlineData("tg")]
        [InlineData("ctg")]
        [InlineData("sqrt")]
        public void AddFunctionToEmpty(string function)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddFunction(function);

            Assert.Equal(function, testBuilder.ToString());
        }

        [Fact]
        public void AddIncorrectFunction()
        {
            var testBuilder = new MathExpressionBuilder();
            Assert.Throws<ArgumentException>(() => testBuilder.AddFunction("incorrect"));
        }

        [Theory]
        [InlineData('1', "sin", "1 * sin")]
        [InlineData('2', "cos", "2 * cos")]
        [InlineData('3', "tg", "3 * tg")]
        [InlineData('4', "ctg", "4 * ctg")]
        [InlineData('5', "sqrt", "5 * sqrt")]
        public void AddFunctionAfterDecimalSeparator(char digit, string function, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddDecimalSeparator()
                       .AddFunction(function);

            Assert.Equal(result, testBuilder.ToString());
        }

        [Fact]
        public void AddOpeningParenthesisToEmpty()
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddOpeningParenthesis();

            Assert.Equal("(", testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', "0 * (")]
        [InlineData('1', "1 * (")]
        [InlineData('2', "2 * (")]
        [InlineData('3', "3 * (")]
        [InlineData('4', "4 * (")]
        [InlineData('5', "5 * (")]
        public void AddOpeningParenthesisAfterDecimalSeparator(char digit, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddDecimalSeparator()
                       .AddOpeningParenthesis();

            Assert.Equal(result, testBuilder.ToString());
        }

        [Theory]
        [InlineData('0', "( 0 )")]
        [InlineData('1', "( 1 )")]
        [InlineData('2', "( 2 )")]
        [InlineData('3', "( 3 )")]
        [InlineData('4', "( 4 )")]
        public void AddClosingParenthesis(char digit, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddOpeningParenthesis()
                       .AddDigit(digit)
                       .AddClosingParenthesis();

            Assert.Equal(result, testBuilder.ToString());
        }

        [Theory]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void CanAddClosingParenthesis(char digit)
        {
            var testBuilder = new MathExpressionBuilder();

            Assert.False(testBuilder.CanAddClosingParentheses);

            testBuilder.AddOpeningParenthesis();

            Assert.False(testBuilder.CanAddClosingParentheses);

            testBuilder.AddDigit(digit);

            Assert.True(testBuilder.CanAddClosingParentheses);

            testBuilder.AddClosingParenthesis();

            Assert.False(testBuilder.CanAddClosingParentheses);
        }

        [Theory]
        [InlineData('0', '5')]
        [InlineData('1', '6')]
        [InlineData('2', '7')]
        [InlineData('3', '8')]
        [InlineData('4', '9')]
        public void OpenParenthesesCount(char digit1, char digit2)
        {
            var testBuilder = new MathExpressionBuilder();

            Assert.Equal(0, testBuilder.OpenedParenthesesCount);

            testBuilder.AddDigit(digit1);

            Assert.Equal(0, testBuilder.OpenedParenthesesCount);

            testBuilder.AddOpeningParenthesis();

            Assert.Equal(1, testBuilder.OpenedParenthesesCount);

            testBuilder.AddDigit(digit2)
                       .AddClosingParenthesis();

            Assert.Equal(0, testBuilder.OpenedParenthesesCount);
        }

        [Theory]
        [InlineData('7', '5', '+', "sin")]
        public void LastAddedSymbol(char digit1, char digit2, char op, string function)
        {
            var testBuilder = new MathExpressionBuilder();

            Assert.Equal(Token.None, testBuilder.LastAddedToken);

            testBuilder.AddDigit(digit1);

            Assert.Equal(Token.Digit, testBuilder.LastAddedToken);

            testBuilder.AddDecimalSeparator();

            Assert.Equal(Token.DecimalSeparator, testBuilder.LastAddedToken);

            testBuilder.AddDigit(digit2)
                       .AddOperator(op);

            Assert.Equal(Token.Operator, testBuilder.LastAddedToken);

            testBuilder.AddOpeningParenthesis();

            Assert.Equal(Token.OpeningParenthesis, testBuilder.LastAddedToken);

            testBuilder.AddFunction(function);

            Assert.Equal(Token.Function, testBuilder.LastAddedToken);

            testBuilder.AddPi();

            Assert.Equal(Token.NamedNumber, testBuilder.LastAddedToken);

            testBuilder.AddClosingParenthesis();

            Assert.Equal(Token.ClosingParenthesis, testBuilder.LastAddedToken);

            testBuilder.Clear();

            Assert.Equal(Token.None, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('5', '+', '6', "5 +")]
        public void RemoveDigit(char digit1, char op, char digit2, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit1)
                       .AddOperator(op)
                       .AddDigit(digit2);

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal(digit2.ToString(), token);
            Assert.Equal(result, testBuilder.ToString());
            Assert.Equal(Token.Operator, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('1')]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('4')]
        [InlineData('5')]
        public void RemoveLastDigit(char digit)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit);

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal(digit.ToString(), token);
            Assert.Equal(MathExpressionBuilder.DefaultExpression, testBuilder.ToString());
            Assert.Equal(Token.None, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('5', '+', "5 +")]
        public void RemoveConstant(char digit, char op, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddOperator(op)
                       .AddPi();

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal("p", token);
            Assert.Equal(result, testBuilder.ToString());
            Assert.Equal(Token.Operator, testBuilder.LastAddedToken);
        }

        [Fact]
        public void RemoveLastConstant()
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddPi();

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal("p", token);
            Assert.Equal(MathExpressionBuilder.DefaultExpression, testBuilder.ToString());
            Assert.Equal(Token.None, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('4')]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void RemoveDecimalSeparator(char digit)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddDecimalSeparator();

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal(MathExpressionBuilder.DecimalSeparator, token);
            Assert.Equal(Token.Digit, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('5', '+')]
        [InlineData('6', '*')]
        public void RemoveOperator(char digit, char op)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddOperator(op);

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal(op.ToString(), token);
            Assert.Equal(digit.ToString(), testBuilder.ToString());
            Assert.Equal(Token.Digit, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('5', '+', "5 +")]
        public void RemoveOpeningParenthesis(char digit, char op, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddOperator(op)
                       .AddOpeningParenthesis();

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal("(", token);
            Assert.Equal(result, testBuilder.ToString());
            Assert.Equal(Token.Operator, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('5', '+', "5 + ( p")]
        public void RemoveClosingParenthesis(char digit, char op, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddOperator(op)
                       .AddOpeningParenthesis()
                       .AddPi()
                       .AddClosingParenthesis();

            var token = testBuilder.RemoveLastToken();

            Assert.Equal(")", token);
            Assert.Equal(result, testBuilder.ToString());
            Assert.Equal(Token.NamedNumber, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('5', '+', "tg", "5 +")]
        public void RemoveFunction(char digit, char op, string function, string result)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit)
                       .AddOperator(op)
                       .AddFunction(function);

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal(function, token);
            Assert.Equal(result, testBuilder.ToString());
            Assert.Equal(Token.Operator, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData("sin")]
        [InlineData("cos")]
        [InlineData("tg")]
        [InlineData("ctg")]
        public void RemoveLastFunction(string function)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddFunction(function);

            string? token = testBuilder.RemoveLastToken();

            Assert.Equal(function, token);
            Assert.Equal(MathExpressionBuilder.DefaultExpression, testBuilder.ToString());
            Assert.Equal(Token.None, testBuilder.LastAddedToken);
        }

        [Fact]
        public void RemoveFromEmpty()
        {
            var testBuilder = new MathExpressionBuilder();

            string? token = testBuilder.RemoveLastToken();

            Assert.Null(token);
            Assert.Equal(Token.None, testBuilder.LastAddedToken);
        }

        [Theory]
        [InlineData('1', '+', '2')]
        public void Clear(char digit1, char op, char digit2)
        {
            var testBuilder = new MathExpressionBuilder();

            testBuilder.AddDigit(digit1)
                       .AddOperator(op)
                       .AddDigit(digit2);

            testBuilder.Clear();

            Assert.Equal(MathExpressionBuilder.DefaultExpression, testBuilder.ToString());
        }

        [Theory]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void IsEmpty(char digit)
        {
            var testBuilder = new MathExpressionBuilder();

            Assert.True(testBuilder.IsEmpty);

            testBuilder.AddDigit(digit);

            Assert.False(testBuilder.IsEmpty);

            testBuilder.Clear();

            Assert.True(testBuilder.IsEmpty);
        }
    }
}
