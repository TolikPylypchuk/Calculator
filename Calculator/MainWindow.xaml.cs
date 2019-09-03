using ReactiveUI;

namespace Calculator
{
    public class ReactiveMainWindow : ReactiveWindow<MainViewModel> { }

    public partial class MainWindow : ReactiveMainWindow
    {
        public MainWindow()
            => this.InitializeComponent();
    }
}
