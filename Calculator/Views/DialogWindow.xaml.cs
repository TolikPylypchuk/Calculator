using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Windows;

using Calculator.Properties;
using Calculator.ViewModels;

using ReactiveUI;

namespace Calculator.Views
{
    public class ReactiveDialogWindow : ReactiveWindow<DialogViewModel> { }

    public partial class DialogWindow : ReactiveDialogWindow
    {
        public DialogWindow()
            => this.InitializeComponent();

        public DialogWindow(Window owner, DialogViewModel viewModel)
            : this()
        {
            this.Owner = owner;
            this.ViewModel = viewModel;

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(this.ViewModel, vm => vm.Text, v => v.TextBlock.Text)
                    .DisposeWith(disposables);

                this.BindType(v => v.Title, Messages.About, Messages.Error, disposables);

                this.BindType(v => v.Width, 200, 260, disposables);

                this.BindType(v => v.TextBlock.FontSize, 16, 20, disposables);

                this.BindType(v => v.TextBlock.Width, 150, 200, disposables);

                this.BindCommand(this.ViewModel, vm => vm.Close, v => v.OkButton)
                    .DisposeWith(disposables);

                this.ViewModel.Close
                    .Subscribe(_ => this.DialogResult = true)
                    .DisposeWith(disposables);
            });
        }

        private void BindType<T>(
            Expression<Func<DialogWindow, T>> property,
            T aboutValue,
            T errorValue,
            CompositeDisposable disposables)
            => this.OneWayBind(
                this.ViewModel,
                vm => vm.Type,
                property,
                type => type == DialogType.About ? aboutValue : errorValue)
                .DisposeWith(disposables);
    }
}
