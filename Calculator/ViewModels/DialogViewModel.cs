namespace Calculator.ViewModels;

public enum DialogType { About, Error }

public class DialogViewModel(string text, DialogType type) : ReactiveObject
{
    public string Text { get; } = text;
    public DialogType Type { get; } = type;

    public ReactiveCommand<Unit, Unit> Close { get; } = ReactiveCommand.Create(() => { });
}
