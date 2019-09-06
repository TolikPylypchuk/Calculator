using System;
using System.Collections.Generic;
using System.Globalization;

using Calculator.Properties;

namespace Calculator.Services
{
    /// <summary>
    /// Contains helper methods for working with mathematical expressions.
    /// </summary>
    /// <remarks>
    /// The expressions used as arguments in the methods in this class
    /// should be built by the ExpressionBuilder class.
    /// </remarks>
    public static class MathExpression
    {
        /// <summary>
        /// The delimiter of the expression tokens.
        /// </summary>
        public const char Delimiter = ' ';

        /// <summary>
        /// Parses an expression and returns its result as a string.
        /// </summary>
        /// <param name="expr">The expression to parse.</param>
        /// <returns>
        /// The result of the expression.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The expression is invalid.
        /// </exception>
        /// <exception cref="ArithmeticException">
        /// The expression contains one of the following:
        /// <list type="bullet">
        /// <item>Division by zero.</item>
        /// <item>Zero to the power of zero.</item>
        /// <item>A root of a negative number.</item>
        /// <item>A logarithm of a negative number.</item>
        /// </list>
        /// </exception>
        public static string Parse(string expr)
        {
            var stack = new Stack<double>();

            foreach (string token in expr.ToPostfixForm().Split(Delimiter))
            {
                if (Double.TryParse(token, out double value))
                {
                    stack.Push(value);
                } else
                {
                    double first, second;
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
                            } else if (Math.Abs(first) > Double.Epsilon && Math.Abs(second) < Double.Epsilon)
                            {
                                throw new ArithmeticException(Messages.DivisionByZero);
                            } else
                            {
                                throw new ArithmeticException(Messages.ZeroByZero);
                            }
                            break;
                        case "^":
                            second = stack.Pop();
                            first = stack.Pop();
                            if (Math.Abs(first) > Double.Epsilon || Math.Abs(second) > Double.Epsilon)
                            {
                                stack.Push(Math.Pow(first, second));
                            } else
                            {
                                throw new ArithmeticException(Messages.ZeroToThePowerOfZero);
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
                                throw new ArithmeticException(Messages.RootOfNegativeNumber);
                            }
                            stack.Push(Math.Sqrt(first));
                            break;
                        case "ln":
                            first = stack.Pop();
                            if (first <= 0.0)
                            {
                                throw new ArithmeticException(Messages.LogarithmOfNegativeNumber);
                            }
                            stack.Push(Math.Log(first));
                            break;
                        case "lg":
                            first = stack.Pop();
                            if (first <= 0.0)
                            {
                                throw new ArithmeticException(Messages.LogarithmOfNegativeNumber);
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
                            throw new ArgumentException(Messages.InvalidExpression, nameof(expr));
                    }
                }
            }

            if (stack.Count != 1)
            {
                throw new ArgumentException(Messages.InvalidExpression, nameof(expr));
            }

            return stack.Pop().ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Transforms an expression from an infix form to a postfix form.
        /// </summary>
        /// <param name="infixForm">The expression to transform</param>
        /// <returns>
        /// The postfix form of the expression.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The expression is invalid.
        /// </exception>
        public static string ToPostfixForm(this string infixForm)
        {
            string result = String.Empty;
            var stack = new Stack<string>();

            foreach (string token in infixForm.Trim().Split(Delimiter))
            {
                if (Double.TryParse(token, out double value))
                {
                    result += value;
                    result += Delimiter;
                } else
                {
                    switch (token)
                    {
                        case "+":
                        case "-":
                            while (stack.Count != 0 && stack.Peek() != "(")
                            {
                                result += stack.Pop();
                                result += Delimiter;
                            }
                            stack.Push(token);
                            break;
                        case "×":
                        case "*":
                        case "/":
                            while (stack.Count != 0 &&
                                "×/^sincosctg√".IndexOf(stack.Peek(), StringComparison.Ordinal) != -1)
                            {
                                result += stack.Pop();
                                result += Delimiter;
                            }
                            stack.Push(token);
                            break;
                        case "^":
                            while (stack.Count != 0 &&
                                "sincosctg√".IndexOf(stack.Peek(), StringComparison.Ordinal) != -1)
                            {
                                result += stack.Pop();
                                result += Delimiter;
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
                                result += Delimiter;
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
                            result += token + Delimiter;
                            break;
                        default:
                            throw new ArgumentException(Messages.InvalidExpression);
                    }
                }
            }

            while (stack.Count != 0)
            {
                result += stack.Pop() + Delimiter;
            }

            return result.Trim();
        }
    }
}
