using System;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Calculator.Properties;
using Calculator.Services;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Calculator.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly MathExpressionBuilder builder = new MathExpressionBuilder();

        public MainViewModel()
        {
            this.builder.Expression
                .Select(expr => expr.Replace(" ", String.Empty)
                    .Replace("*", "×")
                    .Replace("sqrt", "√")
                    .Replace("p", "п"))
                .ToPropertyEx(this, vm => vm.Expression);

            this.AddDigit = ReactiveCommand.Create<char>(this.OnAddDigit);
            this.AddOperator = ReactiveCommand.Create<char>(this.OnAddOperator);
            this.AddDecimalSeparator = ReactiveCommand.Create(this.OnAddDecimalSeparator);
            this.AddPi = ReactiveCommand.Create(this.OnAddPi);
            this.AddE = ReactiveCommand.Create(this.OnAddE);
            this.AddFunction = ReactiveCommand.Create<string>(this.OnAddFunction);
            this.AddOpeningParenthesis = ReactiveCommand.Create(this.OnAddOpeningParenthesis);
            this.AddClosingParenthesis = ReactiveCommand.Create(this.OnAddClosingParenthesis);
            this.RemoveLastToken = ReactiveCommand.Create(this.OnRemoveLastToken);

            this.Calculate = ReactiveCommand.Create(this.OnCalculate);
            this.Clear = ReactiveCommand.Create(this.OnClear);
            this.ShowAbout = ReactiveCommand.Create(this.OnShowAbout);

            this.ShowDialog = new Interaction<DialogViewModel, Unit>();
        }

        public string Expression { [ObservableAsProperty] get; }

        [Reactive]
        public string? Result { get; private set; }

        public ReactiveCommand<char, Unit> AddDigit { get; }
        public ReactiveCommand<char, Unit> AddOperator { get; }
        public ReactiveCommand<Unit, Unit> AddDecimalSeparator { get; }
        public ReactiveCommand<Unit, Unit> AddPi { get; }
        public ReactiveCommand<Unit, Unit> AddE { get; }
        public ReactiveCommand<string, Unit> AddFunction { get; }
        public ReactiveCommand<Unit, Unit> AddOpeningParenthesis { get; }
        public ReactiveCommand<Unit, Unit> AddClosingParenthesis { get; }
        public ReactiveCommand<Unit, Unit> RemoveLastToken { get; }

        public ReactiveCommand<Unit, Unit> Calculate { get; }
        public ReactiveCommand<Unit, Unit> Clear { get; }
        public ReactiveCommand<Unit, Unit> ShowAbout { get; }

        public Interaction<DialogViewModel, Unit> ShowDialog { get; }

        private void OnAddDigit(char digit)
        {
            this.ClearIfCalculated();
            this.builder.AddDigit(digit);
        }

        private void OnAddOperator(char op)
        {
            this.ClearIfCalculated();
            this.builder.AddOperator(op);
        }

        private void OnAddDecimalSeparator()
        {
            this.ClearIfCalculated();
            this.builder.AddDecimalSeparator();
        }

        private void OnAddPi()
        {
            this.ClearIfCalculated();
            this.builder.AddPi();
        }

        private void OnAddE()
        {
            this.ClearIfCalculated();
            this.builder.AddE();
        }

        private void OnAddFunction(string function)
        {
            this.ClearIfCalculated();
            this.builder.AddFunction(function);
        }

        private void OnAddOpeningParenthesis()
        {
            this.ClearIfCalculated();
            this.builder.AddOpeningParenthesis();
        }

        private void OnAddClosingParenthesis()
        {
            this.ClearIfCalculated();
            this.builder.AddClosingParenthesis();
        }

        private void OnRemoveLastToken()
        {
            this.ClearIfCalculated();
            this.builder.RemoveLastToken();
        }

        private async void OnCalculate()
        {
            if (this.Result != null)
            {
                return;
            }

            string expression = this.builder.GetExpression();

            if (Regex.IsMatch(this.Expression, @"^[0-9,\.]+$"))
            {
                return;
            }

            try
            {
                this.Result = this.Expression switch
                {
                    "п" => Math.PI.ToString(CultureInfo.CurrentCulture),
                    "e" => Math.E.ToString(CultureInfo.CurrentCulture),
                    _ => MathExpression.Parse(expression).Replace(".", MathExpressionBuilder.DecimalSeparator)
                };
            } catch (Exception exp)
            {
                await this.ShowDialog.Handle(new DialogViewModel(exp.Message, DialogType.Error));
            }
        }

        private void OnClear()
        {
            this.Result = null;
            this.builder.Clear();
        }

        private async void OnShowAbout()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version != null)
            {
                await this.ShowDialog.Handle(new DialogViewModel(
                    new[]
                    {
                        String.Format(Messages.VersionFormat, version.Major, version.Minor),
                        Messages.CreatedBy,
                        Messages.Author
                    }.Aggregate((acc, item) => acc + Environment.NewLine + item),
                    DialogType.About));
            }
        }

        private void ClearIfCalculated()
        {
            if (this.Result != null)
            {
                this.Result = null;
                this.builder.Clear();
            }
        }
    }
}
