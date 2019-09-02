using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Calculator.Services;

using Token = Calculator.Services.MathExpressionBuilder.Token;

namespace Calculator.Tests.Services
{
	[TestClass]
	public class MathExpressionBuilderTests
	{
		[TestMethod]
		public void Initialize()
		{
			var testBuilder = new MathExpressionBuilder();

			Assert.AreEqual("0", testBuilder.ToString());
		}

		[TestMethod]
		public void AddDigitToEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('1');

			Assert.AreEqual("1", testBuilder.ToString());
		}

		[TestMethod]
		public void AddDigits()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('1')
					   .AddDigit('2');

			Assert.AreEqual("12", testBuilder.ToString());
		}

		[TestMethod]
		public void AddDigitsAfterOperator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('1')
					   .AddOperator('+')
					   .AddDigit('2');

			Assert.AreEqual("1 + 2", testBuilder.ToString());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddIncorrectDigit()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('a');
		}

		[TestMethod]
		public void AddConstantEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddPi();

			Assert.AreEqual("p", testBuilder.ToString());
		}

		[TestMethod]
		public void AddConstantAfterOperator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddPi()
					   .AddOperator('*')
					   .AddE();

			Assert.AreEqual("p * e", testBuilder.ToString());
		}

		[TestMethod]
		public void AddConstantAfterConstant()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddPi()
					   .AddE();

			Assert.AreEqual("p * e", testBuilder.ToString());
		}

		[TestMethod]
		public void AddConstantAfterDecimalSeparator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('8')
					   .AddDecimalSeparator()
					   .AddE();

			Assert.AreEqual("8 * e", testBuilder.ToString());
		}

		[TestMethod]
		public void AddVariable()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddVariable();

			Assert.AreEqual("0", testBuilder.ToString());

			testBuilder.SupportsVariables = true;

			testBuilder.AddVariable();

			Assert.AreEqual("x", testBuilder.ToString());
		}

		[TestMethod]
		public void AddDecimalSeparator()
		{
			var testBuilder = new MathExpressionBuilder();
			string s = MathExpressionBuilder.DecimalSeparator;

			testBuilder.AddDigit('9')
					   .AddDecimalSeparator()
					   .AddDigit('0');

			Assert.AreEqual($"9{s}0", testBuilder.ToString());

			testBuilder.AddDecimalSeparator();

			Assert.AreEqual($"9{s}0", testBuilder.ToString());
		}

		[TestMethod]
		public void CanAddDecimalSeparator()
		{
			var testBuilder = new MathExpressionBuilder();

			Assert.IsTrue(testBuilder.CanAddDecimalSeparator);

			testBuilder.AddDigit('9');

			Assert.IsTrue(testBuilder.CanAddDecimalSeparator);

			testBuilder.AddDecimalSeparator();

			Assert.IsFalse(testBuilder.CanAddDecimalSeparator);

			testBuilder.AddDigit('0');

			Assert.IsFalse(testBuilder.CanAddDecimalSeparator);

			testBuilder.AddDecimalSeparator();

			Assert.IsFalse(testBuilder.CanAddDecimalSeparator);

			testBuilder.AddPi();

			Assert.IsTrue(testBuilder.CanAddDecimalSeparator);
		}

		[TestMethod]
		public void AddOperatorSimple()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('6')
					   .AddOperator('+')
					   .AddDigit('7');

			Assert.AreEqual("6 + 7", testBuilder.ToString());
		}

		[TestMethod]
		public void AddMinusToEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddOperator('-');

			Assert.AreEqual("0 -", testBuilder.ToString());
		}

		[TestMethod]
		public void AddMinusAfterOpeningParenthesis()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddOpeningParenthesis()
					   .AddOperator('-');

			Assert.AreEqual("( 0 -", testBuilder.ToString());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddIncorrectOperator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddOperator('a');
		}

		[TestMethod]
		public void AddFunctionToEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddFunction("sin");

			Assert.AreEqual("sin", testBuilder.ToString());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void AddIncorrectFunction()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddFunction("incorrect");
		}

		[TestMethod]
		public void AddFunctionAfterDecimalSeparator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('6')
					   .AddDecimalSeparator()
					   .AddFunction("sin");

			Assert.AreEqual("6 * sin", testBuilder.ToString());
		}

		[TestMethod]
		public void AddOpeningParenthesisToEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddOpeningParenthesis();

			Assert.AreEqual("(", testBuilder.ToString());
		}

		[TestMethod]
		public void AddOpeningParenthesisAfterDecimalSeparator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('4')
					   .AddDecimalSeparator()
					   .AddOpeningParenthesis();

			Assert.AreEqual("4 * (", testBuilder.ToString());
		}

		[TestMethod]
		public void AddClosingParenthesis()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddOpeningParenthesis()
					   .AddDigit('6')
					   .AddClosingParenthesis();

			Assert.AreEqual("( 6 )", testBuilder.ToString());
		}

		[TestMethod]
		public void CanAddClosingParenthesis()
		{
			var testBuilder = new MathExpressionBuilder();

			Assert.IsFalse(testBuilder.CanAddClosingParentheses);

			testBuilder.AddOpeningParenthesis();

			Assert.IsFalse(testBuilder.CanAddClosingParentheses);

			testBuilder.AddDigit('6');

			Assert.IsTrue(testBuilder.CanAddClosingParentheses);

			testBuilder.AddClosingParenthesis();

			Assert.IsFalse(testBuilder.CanAddClosingParentheses);
		}

		[TestMethod]
		public void OpenParenthesesCount()
		{
			var testBuilder = new MathExpressionBuilder();

			Assert.AreEqual(0, testBuilder.OpenedParenthesesCount);

			testBuilder.AddDigit('6');

			Assert.AreEqual(0, testBuilder.OpenedParenthesesCount);

			testBuilder.AddOpeningParenthesis();

			Assert.AreEqual(1, testBuilder.OpenedParenthesesCount);

			testBuilder.AddDigit('5');
			testBuilder.AddClosingParenthesis();

			Assert.AreEqual(0, testBuilder.OpenedParenthesesCount);
		}

		[TestMethod]
		public void LastAddedSymbol()
		{
			var testBuilder = new MathExpressionBuilder();

			Assert.AreEqual(Token.None, testBuilder.LastAddedToken);

			testBuilder.AddDigit('7');

			Assert.AreEqual(Token.Digit, testBuilder.LastAddedToken);

			testBuilder.AddDecimalSeparator();

			Assert.AreEqual(
				Token.DecimalSeparator,
				testBuilder.LastAddedToken);

			testBuilder.AddDigit('5');

			testBuilder.AddOperator('+');

			Assert.AreEqual(Token.Operator, testBuilder.LastAddedToken);

			testBuilder.AddOpeningParenthesis();

			Assert.AreEqual(
				Token.OpeningParenthesis,
				testBuilder.LastAddedToken);

			testBuilder.AddFunction("cos");

			Assert.AreEqual(Token.Function, testBuilder.LastAddedToken);

			testBuilder.AddPi();

			Assert.AreEqual(Token.NamedNumber, testBuilder.LastAddedToken);

			testBuilder.AddClosingParenthesis();

			Assert.AreEqual(
				Token.ClosingParenthesis,
				testBuilder.LastAddedToken);

			testBuilder.Clear();

			Assert.AreEqual(Token.None, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveDigit()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5')
					   .AddOperator('+')
					   .AddDigit('5');

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("5", token);
			Assert.AreEqual("5 +", testBuilder.ToString());
			Assert.AreEqual(Token.Operator, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveLastDigit()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5');

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("5", token);
			Assert.AreEqual("0", testBuilder.ToString());
			Assert.AreEqual(Token.None, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveConstant()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5')
					   .AddOperator('+')
					   .AddPi();

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("p", token);
			Assert.AreEqual("5 +", testBuilder.ToString());
			Assert.AreEqual(Token.Operator, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveLastConstant()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddPi();

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("p", token);
			Assert.AreEqual("0", testBuilder.ToString());
			Assert.AreEqual(Token.None, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveDecimalSeparator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('6')
					   .AddDecimalSeparator();

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual(MathExpressionBuilder.DecimalSeparator, token);
			Assert.AreEqual(Token.Digit, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveOperator()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5')
					   .AddOperator('+');

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("+", token);
			Assert.AreEqual("5", testBuilder.ToString());
			Assert.AreEqual(Token.Digit, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveOpeningParenthesis()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5')
					   .AddOperator('+')
					   .AddOpeningParenthesis();

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("(", token);
			Assert.AreEqual("5 +", testBuilder.ToString());
			Assert.AreEqual(Token.Operator, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveClosingParenthesis()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5')
					   .AddOperator('+')
					   .AddOpeningParenthesis()
					   .AddPi()
					   .AddClosingParenthesis();

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual(")", token);
			Assert.AreEqual("5 + ( p", testBuilder.ToString());
			Assert.AreEqual(Token.NamedNumber, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveFunction()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('5')
					   .AddOperator('+')
					   .AddFunction("tg");

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("tg", token);
			Assert.AreEqual("5 +", testBuilder.ToString());
			Assert.AreEqual(Token.Operator, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveLastFunction()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddFunction("ctg");

			var token = testBuilder.RemoveLastToken();

			Assert.AreEqual("ctg", token);
			Assert.AreEqual("0", testBuilder.ToString());
			Assert.AreEqual(Token.None, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void RemoveFromEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			var token = testBuilder.RemoveLastToken();

			Assert.IsNull(token);
			Assert.AreEqual(Token.None, testBuilder.LastAddedToken);
		}

		[TestMethod]
		public void Clear()
		{
			var testBuilder = new MathExpressionBuilder();

			testBuilder.AddDigit('1')
					   .AddOperator('+')
					   .AddDigit('2');

			testBuilder.Clear();

			Assert.AreEqual("0", testBuilder.ToString());
		}

		[TestMethod]
		public void IsEmpty()
		{
			var testBuilder = new MathExpressionBuilder();

			Assert.IsTrue(testBuilder.IsEmpty);

			testBuilder.AddDigit('9');

			Assert.IsFalse(testBuilder.IsEmpty);

			testBuilder.Clear();

			Assert.IsTrue(testBuilder.IsEmpty);
		}
	}
}
