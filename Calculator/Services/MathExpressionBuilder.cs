using System;
using System.Globalization;
using System.Text;

namespace Calculator.Services
{
	/// <summary>
	/// Represents an object that builds an expression in a format
	/// suitable for the MathExpression and MathFunction classes.
	/// </summary>
	public sealed class MathExpressionBuilder
	{
		/// <summary>
		/// The decimal separator user by the ExpressionBuilder objects.
		/// </summary>
		public static readonly string DecimalSeparator =
			CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

		#region Private fields

		private StringBuilder builder = new StringBuilder("0");
		private Token lastAddedToken = Token.None;

		private bool supportsVariables;

		#endregion

		/// <summary>
		/// An event that is raised when the expression changes.
		/// </summary>
		public event EventHandler ExpressionChanged;

		/// <summary>
		/// Represents a token in an expression.
		/// </summary>
		public enum Token
		{
			/// <summary>
			/// The default value.
			/// </summary>
			None,

			/// <summary>
			/// A digit of a number.
			/// </summary>
			Digit,

			/// <summary>
			/// A constant or a variable.
			/// </summary>
			NamedNumber,

			/// <summary>
			/// A decimal separator.
			/// </summary>
			DecimalSeparator,

			/// <summary>
			/// A binary or unary operator.
			/// </summary>
			Operator,

			/// <summary>
			/// A function.
			/// </summary>
			Function,

			/// <summary>
			/// An opening parenthesis.
			/// </summary>
			OpeningParenthesis,

			/// <summary>
			/// A closing parenthesis.
			/// </summary>
			ClosingParenthesis
		}

		#region Properties

		/// <summary>
		/// Gets or sets the value that indicates whether this builder
		/// supports variables in the expression.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The expression is not empty.
		/// </exception>
		public bool SupportsVariables
		{
			get => this.supportsVariables;

			set
			{
				if (!this.IsEmpty)
				{
					throw new InvalidOperationException(
						"Cannot set this value if " +
						"the expression is not empty.");
				}

				this.supportsVariables = value;
			}
		}

		/// <summary>
		/// Gets the value indicating whether this expression is empty.
		/// </summary>
		public bool IsEmpty => this.LastAddedToken == Token.None;

		/// <summary>
		/// Gets the number of opened parentheses in the expression.
		/// </summary>
		public int OpenedParenthesesCount { get; private set; }

		/// <summary>
		/// Gets the value that indicates whether closing parentheses
		/// can be added to the expression.
		/// </summary>
		public bool CanAddClosingParentheses =>
			this.OpenedParenthesesCount != 0 &&
			this.LastAddedToken != Token.OpeningParenthesis;

		/// <summary>
		/// Gets the value that indicates whether a decimal separator
		/// can be added to the expression.
		/// </summary>
		public bool CanAddDecimalSeparator { get; private set; } = true;

		/// <summary>
		/// Gets the last added symbol of the expression.
		/// </summary>
		public Token LastAddedToken
		{
			get => this.lastAddedToken;

			private set
			{
				switch (value)
				{
					case Token.DecimalSeparator:
						this.CanAddDecimalSeparator = false;
						break;
					case Token.Digit:
						break;
					default:
						this.CanAddDecimalSeparator = true;
						break;
				}

				this.lastAddedToken = value;
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Adds a digit to the expression.
		/// </summary>
		/// <param name="digit">The digit to add.</param>
		/// <returns>The current instance of the builder.</returns>
		/// <exception cref="System.ArgumentException">
		/// The argument is not a digit.
		/// </exception>
		public MathExpressionBuilder AddDigit(char digit)
		{
			if (!Char.IsDigit(digit))
			{
				throw new ArgumentException(
					"The argument is not a digit.",
					nameof(digit));
			}

			this.ClearIfDefault();

			if (this.LastAddedToken != Token.None &&
				this.LastAddedToken != Token.Digit &&
				this.LastAddedToken != Token.DecimalSeparator)
			{
				if (this.LastAddedToken == Token.NamedNumber ||
					this.LastAddedToken == Token.ClosingParenthesis)
				{
					this.AddOperator('×');
				}
				
				this.builder.Append(' ');
			}

			this.builder.Append(digit);
			this.LastAddedToken = Token.Digit;
			this.OnExpressionChanged();

			return this;
		}
		
		/// <summary>
		/// Adds pi (≈3.14) to the expression.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		public MathExpressionBuilder AddPi()
		{
			return this.AddNamedNumber("p");
		}

		/// <summary>
		/// Adds Euler's number (≈2.7) to the expression.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		public MathExpressionBuilder AddE()
		{
			return this.AddNamedNumber("e");
		}

		/// <summary>
		/// Adds an unknown variable to the expression.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		public MathExpressionBuilder AddVariable(string variable = "x")
		{
			return this.SupportsVariables
				? this.AddNamedNumber(variable)
				: this;
		}

		/// <summary>
		/// Adds the decimal separator to the expression.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		/// <remarks>
		/// The decimal separator is specified by the current culture.
		/// </remarks>
		public MathExpressionBuilder AddDecimalSeparator()
		{
			if (!this.CanAddDecimalSeparator)
			{
				return this;
			}

			if (this.LastAddedToken != Token.None &&
				this.LastAddedToken != Token.Digit)
			{
				if (this.LastAddedToken == Token.NamedNumber ||
					this.LastAddedToken == Token.Digit ||
					this.LastAddedToken == Token.ClosingParenthesis)
				{
					this.AddOperator('×');
				}

				this.builder.Append(" 0");
			}

			this.builder.Append(DecimalSeparator);
			this.LastAddedToken = Token.DecimalSeparator;
			this.OnExpressionChanged();

			return this;
		}

		/// <summary>
		/// Adds an operator to the expression.
		/// </summary>
		/// <param name="op">The operator to add.</param>
		/// <returns>The current instance of the builder.</returns>
		/// <remarks>
		/// The allowed operators are:
		/// +, -, ×, *, ÷, /, ^
		/// </remarks>
		/// <exception cref="System.ArgumentException">
		/// The argument is not a valid operator.
		/// </exception>
		public MathExpressionBuilder AddOperator(char op)
		{
			if ("+-×*÷/^".IndexOf(op) == -1)
			{
				throw new ArgumentException(
					"The argument is not a valid operator.",
					nameof(op));
			}
			
			if (op == '-' && this.LastAddedToken == Token.OpeningParenthesis)
			{
				this.AddDigit('0');
			} else if (op != '-' && this.LastAddedToken == Token.None)
			{
				return this;
			}

			switch (op)
			{
				case '×':
					op = '*';
					break;
				case '÷':
					op = '/';
					break;
			}

			if (this.LastAddedToken == Token.Operator ||
				this.LastAddedToken == Token.DecimalSeparator)
			{
				this.RemoveLastToken();
			}

			if (this.LastAddedToken == Token.NamedNumber ||
				this.LastAddedToken == Token.Digit ||
				this.LastAddedToken == Token.ClosingParenthesis ||
				(op == '-' && this.LastAddedToken == Token.None))
			{
				this.builder.Append(" " + op);
			}

			this.LastAddedToken = Token.Operator;
			this.OnExpressionChanged();

			return this;
		}

		/// <summary>
		/// Adds a function to the expression.
		/// </summary>
		/// <param name="function">The function to add.</param>
		/// <returns>The current instance of the builder.</returns>
		/// <remarks>
		/// The allowed functions are:
		/// sin, cos, tg, ctg, √, sqrt, ln, lg, abs, floor, ceil and sign.
		/// </remarks>
		public MathExpressionBuilder AddFunction(string function)
		{
			function = function.ToLowerInvariant();

			if (function != "floor" &&
				function != "ceil" &&
				function != "sign" &&
				function != "sqrt" &&
				function != "abs" &&
				function != "sin" &&
				function != "cos" &&
				function != "ctg" &&
				function != "tg" &&
				function != "ln" &&
				function != "lg" &&
				function != "√")
			{
				throw new ArgumentException(
					"The argument is not a valid function.",
					nameof(function));
			}

			this.ClearIfDefault();

			if (this.LastAddedToken == Token.DecimalSeparator)
			{
				this.RemoveLastToken();
			}

			if (this.LastAddedToken == Token.Digit ||
				this.LastAddedToken == Token.NamedNumber ||
				this.LastAddedToken == Token.ClosingParenthesis)
			{
				this.AddOperator('×');
			}

			if (this.LastAddedToken != Token.None)
			{
				this.builder.Append(' ');
			}

			this.builder.Append(function);
			this.LastAddedToken = Token.Function;
			this.OnExpressionChanged();

			return this;
		}

		/// <summary>
		/// Adds an opening parenthesis to the expression.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		public MathExpressionBuilder AddOpeningParenthesis()
		{
			this.ClearIfDefault();

			if (this.LastAddedToken == Token.DecimalSeparator)
			{
				this.RemoveLastToken();
			}

			if (this.LastAddedToken == Token.Digit ||
				this.LastAddedToken == Token.NamedNumber ||
				this.LastAddedToken == Token.ClosingParenthesis)
			{
				this.AddOperator('×');
			}

			if (this.LastAddedToken != Token.None)
			{
				this.builder.Append(' ');
			}

			this.builder.Append('(');
			this.OpenedParenthesesCount++;
			this.LastAddedToken = Token.OpeningParenthesis;
			this.OnExpressionChanged();

			return this;
		}

		/// <summary>
		/// Adds a closing parenthesis the the expression if it is allowed.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		public MathExpressionBuilder AddClosingParenthesis()
		{
			if (!this.CanAddClosingParentheses)
			{
				return this;
			}

			this.builder.Append(" )");
			this.OpenedParenthesesCount--;
			this.LastAddedToken = Token.ClosingParenthesis;
			this.OnExpressionChanged();

			return this;
		}
		
		/// <summary>
		/// Returns the built expression.
		/// </summary>
		/// <returns>The built expression.</returns>
		public string GetExpression()
		{
			this.Cleanup();
			return this.builder.ToString();
		}
		
		/// <summary>
		/// Clears the expression.
		/// </summary>
		/// <returns>The current instance of the builder.</returns>
		public MathExpressionBuilder Clear()
		{
			this.builder.Clear();
			this.builder.Append('0');

			this.OpenedParenthesesCount = 0;
			this.LastAddedToken = Token.None;
			this.OnExpressionChanged();

			return this;
		}

		/// <summary>
		/// Removes the last token from the expression.
		/// </summary>
		/// <returns>
		/// The removed token or <c>null</c> if the expression was empty.
		/// </returns>
		public string RemoveLastToken()
		{
			if (this.IsEmpty)
			{
				return null;
			}

			string result = null;

			if (this.LastAddedToken == Token.Function)
			{
				int spaceIndex = this.builder.ToString()
						.LastIndexOf(' ');
				if (spaceIndex == -1)
				{
					result = this.builder.ToString();
					this.builder.Clear();
				} else
				{
					spaceIndex++;
					result = this.builder
						.ToString()
						.Substring(
							spaceIndex,
							this.builder.Length - spaceIndex);

					this.builder.Remove(
						spaceIndex,
						this.builder.Length - spaceIndex);
				}
			} else
			{
				switch (this.LastAddedToken)
				{
					case Token.OpeningParenthesis:
						this.OpenedParenthesesCount--;
						break;
					case Token.ClosingParenthesis:
						this.OpenedParenthesesCount++;
						break;
				}

				result = this.builder[this.builder.Length - 1]
						.ToString();
				this.builder.Remove(this.builder.Length - 1, 1);
			}

			if (this.builder.Length == 0)
			{
				this.builder.Append('0');
				this.LastAddedToken = Token.None;
				this.OnExpressionChanged();
				return result;
			}

			if (this.builder[this.builder.Length - 1] == ' ')
			{
				this.builder.Remove(this.builder.Length - 1, 1);
			}
			
			char lastChar = this.builder[this.builder.Length - 1];
			
			if (Char.IsDigit(lastChar))
			{
				this.LastAddedToken = Token.Digit;
			} else if (DecimalSeparator[DecimalSeparator.Length - 1] ==
				lastChar)
			{
				this.LastAddedToken = Token.DecimalSeparator;
			} else if ("xepп".IndexOf(lastChar) != -1)
			{
				this.LastAddedToken = Token.NamedNumber;
			} else if ("+-×*÷/^".IndexOf(lastChar) != -1)
			{
				this.LastAddedToken = Token.Operator;
			} else if (lastChar == '(')
			{
				this.LastAddedToken = Token.OpeningParenthesis;
			} else if (lastChar == ')')
			{
				this.LastAddedToken = Token.ClosingParenthesis;
			} else
			{
				this.LastAddedToken = Token.Function;
			}

			this.OnExpressionChanged();

			return result;
		}

		/// <summary>
		/// Returns the built expression without the necessary cleanup.
		/// </summary>
		/// <returns>
		/// The built expression without the necessary cleanup.
		/// </returns>
		/// <remarks>
		/// This method should be used to get the expression
		/// in an intermediate state and does not guarantee that
		/// the expression is valid.
		/// To get the finished expression, use the GetExpression method.
		/// </remarks>
		/// <seealso cref="GetExpression" />
		public override string ToString() => this.builder.ToString();

		#endregion

		#region Private methods

		private MathExpressionBuilder AddNamedNumber(string name)
		{
			this.ClearIfDefault();

			if (this.LastAddedToken != Token.None)
			{
				if (this.LastAddedToken == Token.DecimalSeparator)
				{
					this.RemoveLastToken();
				}
				
				if (this.LastAddedToken == Token.NamedNumber ||
					this.LastAddedToken == Token.Digit ||
					this.LastAddedToken == Token.ClosingParenthesis)
				{
					this.AddOperator('×');
				}

				this.builder.Append(' ');
			}

			this.builder.Append(name);
			this.LastAddedToken = Token.NamedNumber;
			this.OnExpressionChanged();

			return this;
		}

		private void ClearIfDefault()
		{
			if (this.ToString() == "0")
			{
				this.builder.Clear();
			}
		}

		private void Cleanup()
		{
			while (this.LastAddedToken == Token.Function ||
				   this.LastAddedToken == Token.Operator ||
				   this.LastAddedToken == Token.DecimalSeparator ||
				   this.LastAddedToken == Token.OpeningParenthesis)
			{
				this.RemoveLastToken();
			}

			while (this.OpenedParenthesesCount > 0)
			{
				this.AddClosingParenthesis();
			}

			this.OnExpressionChanged();
		}

		private void OnExpressionChanged()
		{
			this.ExpressionChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}
}
