using System;
using System.Collections.Generic;
using System.Globalization;

namespace Calculator.Services
{
	/// <summary>
	/// Contains helper methods for working
	/// with mathematical expressions.
	/// </summary>
	/// <remarks>
	/// The expressions used as arguments in the methods in this class
	/// should be built by the ExpressionBuilder class.
	/// </remarks>
	public static class MathExpression
	{
		/// <summary>
		/// Parses an expression and returns its result as a string.
		/// </summary>
		/// <param name="expr">The expression to parse.</param>
		/// <returns>
		/// The result of the expression.
		/// </returns>
		public static string Parse(string expr)
		{
			string postfixForm = ToPostfixForm(expr);
			double result = 0.0;
			var stack = new Stack<double>();
			
			foreach (string token in postfixForm.Split(' '))
			{
				if (Double.TryParse(token, out double value))
				{
					stack.Push(value);
				} else
				{
					double first = 0, second = 0;
					switch (token)
					{
						case "п":
						case "p":
							stack.Push(Math.PI);
							break;
						case "e":
							stack.Push(Math.E);
							break;
						case "+":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(first + second);
							break;
						case "-":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(first - second);
							break;
						case "×":
						case "*":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(first * second);
							break;
						case "/":
							second = stack.Pop();
							first = stack.Pop();
							if (Math.Abs(second) > Double.Epsilon)
							{
								stack.Push(first / second);
							} else if (Math.Abs(first) > Double.Epsilon &&
									   Math.Abs(second) < Double.Epsilon)
							{
								throw new ArithmeticException(
									"Division by zero equals infinity.");
							} else
							{
								throw new ArithmeticException(
									"0/0 is undefined.");
							}
							break;
						case "^":
							second = stack.Pop();
							first = stack.Pop();
							if (Math.Abs(first) > Double.Epsilon ||
								Math.Abs(second) > Double.Epsilon)
							{
								stack.Push(Math.Pow(first, second));
							} else
							{
								throw new ArithmeticException(
									"0^0 is undefined.");
							}
							break;
						case "sin":
							first = stack.Pop();
							stack.Push(Math.Sin(first));
							break;
						case "cos":
							first = stack.Pop();
							stack.Push(Math.Cos(first));
							break;
						case "tg":
							first = stack.Pop();
							stack.Push(Math.Tan(first));
							break;
						case "ctg":
							first = stack.Pop();
							stack.Push(1 / Math.Tan(first));
							break;
						case "√":
						case "sqrt":
							first = stack.Pop();
							if (first < 0.0)
							{
								throw new ArithmeticException(
									"A root of a negative number " +
									"is not a real number.");
							}
							stack.Push(Math.Sqrt(first));
							break;
						case "ln":
							first = stack.Pop();
							if (first <= 0.0)
							{
								throw new ArithmeticException(
									"A logarithm of a negative number " +
									"is not a real number.");
							}
							stack.Push(Math.Log(first));
							break;
						case "lg":
							first = stack.Pop();
							if (first <= 0.0)
							{
								throw new ArithmeticException(
									"A logarithm of a negative number " +
									"is not a real number.");
							}
							stack.Push(Math.Log10(first));
							break;
						case "abs":
							first = stack.Pop();
							stack.Push(Math.Abs(first));
							break;
						case "floor":
							first = stack.Pop();
							stack.Push(Math.Floor(first));
							break;
						case "ceil":
							first = stack.Pop();
							stack.Push(Math.Ceiling(first));
							break;
						case "sign":
							first = stack.Pop();
							stack.Push(Math.Sign(first));
							break;
						default:
							throw new ArgumentException(
								"Invalid expression.",
								nameof(expr));
					}
				}
			}

			if (stack.Count != 1)
			{
				throw new ArgumentException(
					"Invalid expression.",
					nameof(expr));
			}

			result = stack.Pop();

			return result.ToString(CultureInfo.InvariantCulture);
		}
		
		/// <summary>
		/// Transforms an expression from an infix form to a postfix form.
		/// </summary>
		/// <param name="infixForm">The expression to transform</param>
		/// <returns>
		/// The postfix form of the expression.
		/// </returns>
		public static string ToPostfixForm(string infixForm)
		{
			string result = String.Empty;
			var stack = new Stack<string>();
			
			foreach (string token in infixForm.Trim().Split(' '))
			{
				if (Double.TryParse(token, out double value))
				{
					result += value;
					result += ' ';
				} else
				{
					switch (token)
					{
						case "+":
						case "-":
							while (stack.Count != 0 && stack.Peek() != "(")
							{
								result += stack.Pop();
								result += ' ';
							}
							stack.Push(token);
							break;
						case "×":
						case "*":
						case "/":
							while (stack.Count != 0 &&
								"×/^sincosctg√".IndexOf(
									stack.Peek(),
									StringComparison.Ordinal) != -1)
							{
								result += stack.Pop();
								result += ' ';
							}
							stack.Push(token);
							break;
						case "^":
							while (stack.Count != 0 &&
								"sincosctg√".IndexOf(
									stack.Peek(),
									StringComparison.Ordinal) != -1)
							{
								result += stack.Pop();
								result += ' ';
							}
							stack.Push(token);
							break;
						case "sin":
						case "cos":
						case "tg":
						case "ctg":
						case "√":
						case "sqrt":
						case "ln":
						case "lg":
						case "abs":
						case "floor":
						case "ceil":
						case "sign":
						case "(":
							stack.Push(token);
							break;
						case ")":
							while (stack.Count != 0 && stack.Peek() != "(")
							{
								result += stack.Pop();
								result += ' ';
							}

							if (stack.Count != 0)
							{
								stack.Pop();
							}
							break;
						case "п":
						case "p":
						case "e":
						case "x":
							result += token + ' ';
							break;
						default:
							throw new ArgumentException(
								"An unknown error occured.");
					}
				}
			}

			while (stack.Count != 0)
			{
				result += stack.Pop() + ' ';
			}

			return result.Trim();
		}
	}
}
