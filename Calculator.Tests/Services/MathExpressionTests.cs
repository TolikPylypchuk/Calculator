using Microsoft.VisualStudio.TestTools.UnitTesting;

using Calculator.Services;

namespace Calculator.Tests.Services
{
	[TestClass]
	public class MathExpressionTests
	{
		#region ToPostfixForm tests

		[TestMethod]
		public void ToPostfixFormSimpleTest1()
		{
			const string expr = "1 + 2";
			Assert.AreEqual("1 2 +", MathExpression.ToPostfixForm(expr));
		}

		[TestMethod]
		public void ToPostfixFormSimpleTest2()
		{
			const string expr = "1 - 2 * 4";
			Assert.AreEqual("1 2 4 * -", MathExpression.ToPostfixForm(expr));
		}

		[TestMethod]
		public void ToPostfixFormParenthesesTest()
		{
			const string expr = "( 1 - 2 ) * 4";
			Assert.AreEqual("1 2 - 4 *", MathExpression.ToPostfixForm(expr));
		}

		#endregion

		#region Parse tests

		[TestMethod]
		public void ParseSimpleTest1()
		{
			const string expr = "1 + 2";
			Assert.AreEqual("3", MathExpression.Parse(expr));
		}

		[TestMethod]
		public void ParseSimpleTest2()
		{
			const string expr = "1 - 2 * 4";
			Assert.AreEqual("-7", MathExpression.Parse(expr));
		}

		[TestMethod]
		public void ParseParenthesesTest()
		{
			const string expr = "( 1 - 2 ) * 4";
			Assert.AreEqual("-4", MathExpression.Parse(expr));
		}

		#endregion
	}
}
