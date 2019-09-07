using System.Reactive;

using ReactiveUI;

namespace Calculator.ViewModels
{
    public enum DialogType { About, Error }

    public class DialogViewModel : ReactiveObject
    {
        public DialogViewModel(string text, DialogType type)
        {
            this.Text = text;
            this.Type = type;

            this.Close = ReactiveCommand.Create(() => { });
        }

        public string Text { get; }
        public DialogType Type { get; }

        public ReactiveCommand<Unit, Unit> Close { get; }
    }
}
