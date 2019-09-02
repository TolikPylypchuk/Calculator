using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Calculator.Services;

namespace Calculator.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml.
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Fields

		private MathExpressionBuilder builder = new MathExpressionBuilder();

		private string result;

		#endregion

		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow()
		{
			this.InitializeComponent();

			this.builder.ExpressionChanged += (sender, e) =>
			{
				this.expressionTextBox.Text = this.Expression;
				expressionScrollViewer.ScrollToRightEnd();
			};
		}

		#region Properties

		private string Expression =>
			this.builder.ToString()
						.Replace(" ", String.Empty)
						.Replace("*", "×")
						.Replace("sqrt", "√")
						.Replace("p", "п");

		#endregion

		#region Event handlers
		
		private void Digit_Click(object sender, RoutedEventArgs e)
		{
			string content = (sender as Button)?.Content.ToString();
			if (content == null)
			{
				return;
			}

			this.ClearIfCalculated();

			this.builder.AddDigit(content[0]);
		}

		private void Operator_Click(object sender, RoutedEventArgs e)
		{
			string content = (sender as Button)?.Content.ToString();
			if (content == null)
			{
				return;
			}

			this.ClearIfCalculated();

			this.builder.AddOperator(content[0]);
		}

		private void Function_Click(object sender, RoutedEventArgs e)
		{
			string content = (sender as Button)?.Content.ToString();
			if (content == null)
			{
				return;
			}

			this.ClearIfCalculated();

			this.builder.AddFunction(content);
			this.builder.AddOpeningParenthesis();
		}

		private void Pi_Click(object sender, RoutedEventArgs e)
		{
			this.ClearIfCalculated();
			this.builder.AddPi();
		}
		
		private void E_Click(object sender, RoutedEventArgs e)
		{
			this.ClearIfCalculated();
			this.builder.AddE();
		}
		
		private void DecimalSeparator_Click(object sender, RoutedEventArgs e)
		{
			this.ClearIfCalculated();
			this.builder.AddDecimalSeparator();
		}
		
		private void OpenParenthesis_Click(object sender, RoutedEventArgs e)
		{
			this.ClearIfCalculated();
			this.builder.AddOpeningParenthesis();
		}
		
		private void CloseParenthesis_Click(object sender, RoutedEventArgs e)
		{
			this.ClearIfCalculated();
			this.builder.AddClosingParenthesis();
		}
		
		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			this.builder.Clear();
			this.result = null;
		}
		
		private void Backspace_Click(object sender, RoutedEventArgs e)
		{
			this.ClearIfCalculated();
			this.builder.RemoveLastToken();
		}
		
		private void Calculate_Click(object sender, RoutedEventArgs e)
		{
			if (result != null)
			{
				return;
			}
			
			string expression = this.builder.GetExpression();

			if (Regex.IsMatch(this.Expression, @"^-*[0-9,\.]+$"))
			{
				return;
			}

			switch (this.Expression)
			{
				case "п":
					this.result = Math.PI.ToString(
						CultureInfo.CurrentCulture);
					this.expressionTextBox.Text += "=" + result;
					return;
				case "e":
					this.result = Math.E.ToString(
						CultureInfo.CurrentCulture);
					this.expressionTextBox.Text += "=" + result;
					return;
			}

			expressionTextBox.Text += "=";
			result = string.Empty;

			try
			{
				result = MathExpression.Parse(expression);
				expressionTextBox.Text += result.Replace(
					".", MathExpressionBuilder.DecimalSeparator);
			} catch (ArithmeticException exp)
			{
				expressionTextBox.Text = expressionTextBox.Text
					.Substring(0, expressionTextBox.Text.Length - 1);
				new DialogWindow(this, exp.Message, DialogType.Error)
					.ShowDialog();
				expressionTextBox.Text = "0";
			} catch
			{
				expressionTextBox.Text = expressionTextBox.Text
					.Substring(0, expressionTextBox.Text.Length - 1);
				new DialogWindow(
					this,
					"An unknown error occured.",
					DialogType.Error)
					.ShowDialog();
				expressionTextBox.Text = "0";
			}
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			new DialogWindow(
				this,
				"Calculator\n" +
				$"Version {version.Major}.{version.Minor}\n" +
				"Created by\nTolik Pylypchuk",
				DialogType.About)
				.ShowDialog();
		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (this.result != null)
			{
				this.builder.Clear();
				this.result = null;
			} else if (e.KeyboardDevice.Modifiers == ModifierKeys.None ||
					   e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
			{
				switch (e.Key)
				{
					case Key.D0:
					case Key.NumPad0:
						this.builder.AddDigit('0');
						break;
					case Key.D1:
					case Key.NumPad1:
						this.builder.AddDigit('1');
						break;
					case Key.D2:
					case Key.NumPad2:
						this.builder.AddDigit('2');
						break;
					case Key.D3:
					case Key.NumPad3:
						this.builder.AddDigit('3');
						break;
					case Key.D4:
					case Key.NumPad4:
						this.builder.AddDigit('4');
						break;
					case Key.D5:
					case Key.NumPad5:
						this.builder.AddDigit('5');
						break;
					case Key.D6:
					case Key.NumPad6:
						this.builder.AddDigit('6');
						break;
					case Key.D7:
					case Key.NumPad7:
						this.builder.AddDigit('7');
						break;
					case Key.D8:
					case Key.NumPad8:
						this.builder.AddDigit('8');
						break;
					case Key.D9:
					case Key.NumPad9:
						this.builder.AddDigit('9');
						break;
					case Key.Add:
					case Key.OemPlus:
						this.builder.AddOperator('+');
						break;
					case Key.Subtract:
					case Key.OemMinus:
						this.builder.AddOperator('-');
						break;
					case Key.Multiply:
						this.builder.AddOperator('*');
						break;
					case Key.Divide:
						this.builder.AddOperator('/');
						break;
					case Key.Decimal:
					case Key.OemPeriod:
						this.builder.AddDecimalSeparator();
						break;
					case Key.OemOpenBrackets:
						this.builder.AddOpeningParenthesis();
						break;
					case Key.OemCloseBrackets:
						this.builder.AddClosingParenthesis();
						break;
					case Key.P:
						this.builder.AddPi();
						break;
					case Key.E:
						this.builder.AddE();
						break;
					case Key.Back:
						this.builder.RemoveLastToken();
						break;
					case Key.Delete:
					case Key.Escape:
					case Key.Clear:
					case Key.Cancel:
					case Key.OemClear:
						this.builder.Clear();
						break;
				}
			}
		}

		#endregion

		#region Other methods

		private void ClearIfCalculated()
		{
			if (result != null)
			{
				this.result = null;
				this.builder.Clear();
			}
		}

		#endregion
	}
}
