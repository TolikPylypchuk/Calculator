using Calculator.ViewModels;

using ReactiveUI;

namespace Calculator.Views
{
    public class ReactiveMainWindow : ReactiveWindow<MainViewModel> { }

    public partial class MainWindow : ReactiveMainWindow
    {
        public MainWindow()
            => this.InitializeComponent();
    }
}
