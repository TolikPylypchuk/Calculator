using System.Reactive;

using ReactiveUI;

namespace Calculator
{
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
