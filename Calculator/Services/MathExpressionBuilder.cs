namespace Calculator.Services;

/// <summary>
/// Represents an object that builds an expression in a format suitable for the MathExpression class.
/// </summary>
public sealed class MathExpressionBuilder
{
    /// <summary>
    /// The decimal separator user by the ExpressionBuilder objects.
    /// </summary>
    public static readonly string DecimalSeparator =
        CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

    /// <summary>
    /// The expression of a cleared builder.
    /// </summary>
    public const string DefaultExpression = "0";

    private readonly StringBuilder builder = new(DefaultExpression);
    private Token lastAddedToken = Token.None;

    private readonly BehaviorSubject<string> expression = new(DefaultExpression);

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

    /// <summary>
    /// Gets the value indicating whether this expression is empty.
    /// </summary>
    public bool IsEmpty =>
        this.LastAddedToken == Token.None;

    /// <summary>
    /// Gets the number of opened parentheses in the expression.
    /// </summary>
    public int OpenedParenthesesCount { get; private set; }

    /// <summary>
    /// Gets the value that indicates whether closing parentheses can be added to the expression.
    /// </summary>
    public bool CanAddClosingParentheses =>
        this.OpenedParenthesesCount != 0 && this.LastAddedToken != Token.OpeningParenthesis;

    /// <summary>
    /// Gets the value that indicates whether a decimal separator can be added to the expression.
    /// </summary>
    public bool CanAddDecimalSeparator { get; private set; } = true;

    /// <summary>
    /// Gets the last added token of the expression.
    /// </summary>
    public Token LastAddedToken
    {
        get => this.lastAddedToken;

        private set
        {
            this.CanAddDecimalSeparator = value switch
            {
                Token.DecimalSeparator => false,
                Token.Digit => this.CanAddDecimalSeparator,
                _ => true
            };

            this.lastAddedToken = value;
        }
    }

    /// <summary>
    /// Gets the observable of the current expression.
    /// </summary>
    public IObservable<string> Expression =>
        this.expression.AsObservable();

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
            throw new ArgumentException(Messages.ArgumentNotDigit, nameof(digit));
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

            this.builder.Append(MathExpression.Delimiter);
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
    public MathExpressionBuilder AddPi() =>
        this.AddNamedNumber("p");

    /// <summary>
    /// Adds Euler's number (≈2.7) to the expression.
    /// </summary>
    /// <returns>The current instance of the builder.</returns>
    public MathExpressionBuilder AddE() =>
        this.AddNamedNumber("e");

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

            this.builder.Append(MathExpression.Delimiter).Append(0);
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
    /// The allowed operators are: +, -, ×, *, ÷, /, ^
    /// </remarks>
    /// <exception cref="System.ArgumentException">
    /// The argument is not a valid operator.
    /// </exception>
    public MathExpressionBuilder AddOperator(char op)
    {
        if (!"+-×*÷/^".Contains(op))
        {
            throw new ArgumentException(Messages.ArgumentNotValidOperator, nameof(op));
        }

        if (op == '-' && this.LastAddedToken == Token.OpeningParenthesis)
        {
            this.AddDigit('0');
        } else if (op != '-' && this.LastAddedToken == Token.None)
        {
            return this;
        }

        op = op switch
        {
            '×' => '*',
            '÷' => '/',
            _ => op
        };

        if (this.LastAddedToken == Token.Operator ||
            this.LastAddedToken == Token.DecimalSeparator)
        {
            this.RemoveLastToken();
        }

        if (this.LastAddedToken == Token.NamedNumber ||
            this.LastAddedToken == Token.Digit ||
            this.LastAddedToken == Token.ClosingParenthesis ||
            op == '-' && this.LastAddedToken == Token.None)
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
    /// The allowed functions are: sin, cos, tg, ctg, √, sqrt, ln, lg, abs, floor, ceil and sign.
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
            throw new ArgumentException(Messages.ArgumentNotValidFunction, nameof(function));
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
            this.builder.Append(MathExpression.Delimiter);
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
            this.builder.Append(MathExpression.Delimiter);
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

        this.builder.Append(MathExpression.Delimiter).Append(')');
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
        this.CleanUp();
        return this.builder.ToString();
    }

    /// <summary>
    /// Clears the expression.
    /// </summary>
    /// <returns>The current instance of the builder.</returns>
    public MathExpressionBuilder Clear()
    {
        this.builder.Clear();
        this.builder.Append(DefaultExpression);

        this.OpenedParenthesesCount = 0;
        this.LastAddedToken = Token.None;
        this.OnExpressionChanged();

        return this;
    }

    /// <summary>
    /// Removes the last token from the expression.
    /// </summary>
    /// <returns>
    /// The removed token or <see langword="null" /> if the expression was empty.
    /// </returns>
    public string? RemoveLastToken()
    {
        if (this.IsEmpty)
        {
            return null;
        }

        string result;

        if (this.LastAddedToken == Token.Function)
        {
            int delimiterIndex = this.builder.ToString().LastIndexOf(MathExpression.Delimiter);

            if (delimiterIndex < 0)
            {
                result = this.builder.ToString();
                this.builder.Clear();
            } else
            {
                delimiterIndex++;
                result = this.builder.ToString()[delimiterIndex..this.builder.Length];

                this.builder.Remove(delimiterIndex, this.builder.Length - delimiterIndex);
            }
        } else
        {
            this.OpenedParenthesesCount = this.LastAddedToken switch
            {
                Token.OpeningParenthesis => this.OpenedParenthesesCount - 1,
                Token.ClosingParenthesis => this.OpenedParenthesesCount + 1,
                _ => this.OpenedParenthesesCount
            };

            result = this.builder[^1].ToString();
            this.builder.Remove(this.builder.Length - 1, 1);
        }

        if (this.builder.Length == 0)
        {
            this.builder.Append(DefaultExpression);
            this.LastAddedToken = Token.None;
            this.OnExpressionChanged();
            return result;
        }

        if (this.builder[^1] == MathExpression.Delimiter)
        {
            this.builder.Remove(this.builder.Length - 1, 1);
        }

        char lastChar = this.builder[^1];

        if (Char.IsDigit(lastChar))
        {
            this.LastAddedToken = Token.Digit;
        } else if (DecimalSeparator[^1] == lastChar)
        {
            this.LastAddedToken = Token.DecimalSeparator;
        } else if ("xepп".Contains(lastChar))
        {
            this.LastAddedToken = Token.NamedNumber;
        } else if ("+-×*÷/^".Contains(lastChar))
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
    /// This method should be used to get the expression in an intermediate state and does not guarantee that
    /// the expression is valid. To get the finished expression, use <see cref="GetExpression" />.
    /// </remarks>
    /// <seealso cref="GetExpression" />
    public override string ToString() =>
        this.builder.ToString();

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

            this.builder.Append(MathExpression.Delimiter);
        }

        this.builder.Append(name);
        this.LastAddedToken = Token.NamedNumber;
        this.OnExpressionChanged();

        return this;
    }

    private void ClearIfDefault()
    {
        if (this.ToString() == DefaultExpression)
        {
            this.builder.Clear();
        }
    }

    private void CleanUp()
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
        if (this.expression.Value != this.ToString())
        {
            this.expression.OnNext(this.ToString());
        }
    }
}
