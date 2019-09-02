using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Calculator.Services
{
	/// <summary>
	/// Contains helper methods for working with
	/// mathematical functions.
	/// </summary>
	public static class MathFunction
	{
		#region Fields

		private static readonly MethodInfo Sin =
			typeof(Math).GetMethod("Sin", new[] { typeof(double) });
		private static readonly MethodInfo Cos =
			typeof(Math).GetMethod("Cos", new[] { typeof(double) });
		private static readonly MethodInfo Tan =
			typeof(Math).GetMethod("Tan", new[] { typeof(double) });
		private static readonly MethodInfo Ln =
			typeof(Math).GetMethod("Log", new[] { typeof(double) });
		private static readonly MethodInfo Lg =
			typeof(Math).GetMethod("Log10", new[] { typeof(double) });
		private static readonly MethodInfo Sqrt =
			typeof(Math).GetMethod("Sqrt", new[] { typeof(double) });
		private static readonly MethodInfo Abs =
			typeof(Math).GetMethod("Abs", new[] { typeof(double) });
		private static readonly MethodInfo Floor =
			typeof(Math).GetMethod("Floor", new[] { typeof(double) });
		private static readonly MethodInfo Ceil =
			typeof(Math).GetMethod("Ceiling", new[] { typeof(double) });
		private static readonly MethodInfo Sign =
			typeof(Math).GetMethod("Sign", new[] { typeof(double) });

		#endregion
		
		/// <summary>
		/// Compiles an expression into an executable function.
		/// </summary>
		/// <param name="expr">The expression to compile.</param>
		/// <returns>
		/// An executable function that represents the expression.
		/// </returns>
		/// <remarks>
		/// The expressions used as an argument of this method
		/// should be built by the ExpressionBuilder class.
		/// </remarks>
		public static Func<double, double> Compile(string expr)
		{
			var x = Expression.Parameter(typeof(double), "x");

			string postfixExpr = MathExpression.ToPostfixForm(expr);

			var stack = new Stack<Expression>();

			var tokens = postfixExpr.Split(' ');

			foreach (string token in tokens)
			{
				if (Double.TryParse(token, out double value))
				{
					stack.Push(Expression.Constant(value));
				} else
				{
					Expression first = null, second = null;

					switch (token)
					{
						case "x":
							stack.Push(x);
							break;
						case "п":
						case "p":
							stack.Push(Expression.Constant(Math.PI));
							break;
						case "e":
							stack.Push(Expression.Constant(Math.E));
							break;
						case "+":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(Expression.Add(first, second));
							break;
						case "-":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(Expression.Subtract(first, second));
							break;
						case "*":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(Expression.Multiply(first, second));
							break;
						case "/":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(Expression.Divide(first, second));
							break;
						case "^":
							second = stack.Pop();
							first = stack.Pop();
							stack.Push(Expression.Power(first, second));
							break;
						case "sin":
							first = stack.Pop();
							stack.Push(Expression.Call(Sin, first));
							break;
						case "cos":
							first = stack.Pop();
							stack.Push(Expression.Call(Cos, first));
							break;
						case "tg":
							first = stack.Pop();
							stack.Push(Expression.Call(Tan, first));
							break;
						case "ctg":
							first = stack.Pop();
							stack.Push(Expression.Divide(
								Expression.Constant(1.0),
								Expression.Call(Tan, first)));
							break;
						case "sqrt":
						case "√":
							first = stack.Pop();
							stack.Push(Expression.Call(Sqrt, first));
							break;
						case "ln":
							first = stack.Pop();
							stack.Push(Expression.Call(Ln, first));
							break;
						case "lg":
							first = stack.Pop();
							stack.Push(Expression.Call(Lg, first));
							break;
						case "abs":
							first = stack.Pop();
							stack.Push(Expression.Call(Abs, first));
							break;
						case "floor":
							first = stack.Pop();
							stack.Push(Expression.Call(Floor, first));
							break;
						case "ceil":
							first = stack.Pop();
							stack.Push(Expression.Call(Ceil, first));
							break;
						case "sign":
							first = stack.Pop();
							stack.Push(Expression.Call(Sign, first));
							break;
						default:
							throw new Exception(
								"An unknown error occured.");
					}
				}
			}

			var lambda = Expression.Lambda<Func<double, double>>(
				stack.Pop(),
				x);

			return lambda.Compile();
		}
	}
}
