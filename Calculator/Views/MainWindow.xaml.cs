using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Calculator.Properties;
using Calculator.ViewModels;

using ReactiveUI;

namespace Calculator.Views
{
    public class ReactiveMainWindow : ReactiveWindow<MainViewModel> { }

    public partial class MainWindow : ReactiveMainWindow
    {
        private readonly Dictionary<Key, char> digitKeys = new Dictionary<Key, char>
        {
            [Key.D0] = '0',
            [Key.D1] = '1',
            [Key.D2] = '2',
            [Key.D3] = '3',
            [Key.D4] = '4',
            [Key.D5] = '5',
            [Key.D6] = '6',
            [Key.D7] = '7',
            [Key.D8] = '8',
            [Key.D9] = '9',
            [Key.NumPad0] = '0',
            [Key.NumPad1] = '1',
            [Key.NumPad2] = '2',
            [Key.NumPad3] = '3',
            [Key.NumPad4] = '4',
            [Key.NumPad5] = '5',
            [Key.NumPad6] = '6',
            [Key.NumPad7] = '7',
            [Key.NumPad8] = '8',
            [Key.NumPad9] = '9'
        };

        private readonly Dictionary<Key, char> operatorKeys = new Dictionary<Key, char>
        {
            [Key.Add] = '+',
            [Key.OemPlus] = '+',
            [Key.Subtract] = '-',
            [Key.OemMinus] = '-',
            [Key.Multiply] = '*',
            [Key.Divide] = '/'
        };

        public MainWindow()
        {
            this.InitializeComponent();
            this.ViewModel = new MainViewModel();

            this.WhenActivated(disposables =>
            {
                this.CreateBindings(disposables);
                this.CreateKeyBindings(disposables);

                this.ViewModel.ShowDialog.RegisterHandler(ctx =>
                    Observable.Start(() =>
                    {
                        MessageBox.Show(
                            ctx.Input.Text,
                            Messages.Calculator,
                            MessageBoxButton.OK,
                            ctx.Input.Type == DialogType.Error ? MessageBoxImage.Error : MessageBoxImage.Information);

                        ctx.SetOutput(Unit.Default);
                    }, RxApp.MainThreadScheduler));
            });
        }

        private void CreateBindings(CompositeDisposable disposables)
        {
            var expression = this.WhenAnyValue(
                    v => v.ViewModel!.Expression,
                    v => v.ViewModel!.Result,
                    (expr, res) => res != null ? $"{expr}={res}" : expr);

            expression.BindTo(this, v => v.ExpressionTextBox.Text)
                .DisposeWith(disposables);

            expression
                .Subscribe(expr => this.ExpressionScrollViewer.ScrollToRightEnd())
                .DisposeWith(disposables);

            this.BindDigit(v => v.D0Button, '0', disposables);
            this.BindDigit(v => v.D1Button, '1', disposables);
            this.BindDigit(v => v.D2Button, '2', disposables);
            this.BindDigit(v => v.D3Button, '3', disposables);
            this.BindDigit(v => v.D4Button, '4', disposables);
            this.BindDigit(v => v.D5Button, '5', disposables);
            this.BindDigit(v => v.D6Button, '6', disposables);
            this.BindDigit(v => v.D7Button, '7', disposables);
            this.BindDigit(v => v.D8Button, '8', disposables);
            this.BindDigit(v => v.D9Button, '9', disposables);

            this.BindOperator(v => v.AddButton, '+', disposables);
            this.BindOperator(v => v.SubtractButton, '-', disposables);
            this.BindOperator(v => v.MultiplyButton, '*', disposables);
            this.BindOperator(v => v.DivideButton, '/', disposables);
            this.BindOperator(v => v.PowerButton, '^', disposables);

            this.BindFunction(v => v.SinButton, "sin", disposables);
            this.BindFunction(v => v.CosButton, "cos", disposables);
            this.BindFunction(v => v.TgButton, "tg", disposables);
            this.BindFunction(v => v.CtgButton, "ctg", disposables);
            this.BindFunction(v => v.SqrtButton, "sqrt", disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddPi, v => v.PiButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddE, v => v.EButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddOpeningParenthesis, v => v.OpenParenthesisButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddClosingParenthesis, v => v.CloseParenthesisButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.AddDecimalSeparator, v => v.DecimalSeparatorButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Calculate, v => v.CalculateButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.RemoveLastToken, v => v.BackspaceButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.Clear, v => v.ClearButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.ShowAbout, v => v.AboutButton)
                .DisposeWith(disposables);
        }

        private void BindDigit(
            Expression<Func<MainWindow, Button>> property,
            char digit,
            CompositeDisposable disposables)
            => this.BindCommand(this.ViewModel, vm => vm.AddDigit, property, Observable.Return(digit))
                .DisposeWith(disposables);

        private void BindOperator(
            Expression<Func<MainWindow, Button>> property,
            char op,
            CompositeDisposable disposables)
            => this.BindCommand(this.ViewModel, vm => vm.AddOperator, property, Observable.Return(op))
                .DisposeWith(disposables);

        private void BindFunction(
            Expression<Func<MainWindow, Button>> property,
            string function,
            CompositeDisposable disposables)
            => this.BindCommand(this.ViewModel, vm => vm.AddFunction, property, Observable.Return(function))
                .DisposeWith(disposables);

        private void CreateKeyBindings(CompositeDisposable disposables)
        {
            var keyUp = this.Events().KeyUp
                .Where(e => e.KeyboardDevice.Modifiers == ModifierKeys.None ||
                            e.KeyboardDevice.Modifiers == ModifierKeys.Shift);

            keyUp
                .Where(e => this.digitKeys.ContainsKey(e.Key))
                .Select(e => this.digitKeys[e.Key])
                .InvokeCommand(this.ViewModel, vm => vm.AddDigit)
                .DisposeWith(disposables);

            keyUp
                .Where(e => this.operatorKeys.ContainsKey(e.Key))
                .Select(e => this.operatorKeys[e.Key])
                .InvokeCommand(this.ViewModel, vm => vm.AddOperator)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.P)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.AddPi)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.E)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.AddE)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.OemPeriod || e.Key == Key.Decimal)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.AddDecimalSeparator)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.OemOpenBrackets)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.AddOpeningParenthesis)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.OemCloseBrackets)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.AddClosingParenthesis)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.Back)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.RemoveLastToken)
                .DisposeWith(disposables);

            keyUp
                .Where(e => e.Key == Key.Delete || e.Key == Key.Escape || e.Key == Key.Clear ||
                            e.Key == Key.Cancel || e.Key == Key.OemClear)
                .Select(_ => Unit.Default)
                .InvokeCommand(this.ViewModel, vm => vm.Clear)
                .DisposeWith(disposables);
        }
    }
}
