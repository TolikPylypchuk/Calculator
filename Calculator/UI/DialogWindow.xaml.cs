using System.Windows;

namespace Calculator.UI
{
	/// <summary>
	/// Represents a type of a dialog window.
	/// </summary>
	public enum DialogType
	{
		/// <summary>
		/// The dialog window contains information about the program.
		/// </summary>
		About,

		/// <summary>
		/// The dialog window contains error informantion.
		/// </summary>
		Error
	}

	/// <summary>
	/// Interaction logic for DialogWindow.xaml.
	/// </summary>
	public partial class DialogWindow : Window
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the DialogWindow class.
		/// </summary>
		public DialogWindow()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Initializes a new instance of the DialogWindow class.
		/// </summary>
		/// <param name="owner">The owner window.</param>
		/// <param name="text">The text in this window.</param>
		/// <param name="type">The type of this window.</param>
		public DialogWindow(
			Window owner,
			string text,
			DialogType type)
			: this()
		{
			this.Owner = owner;
			this.textBlock.Text = text;

			switch (type)
			{
				case DialogType.About:
					this.Title = "About";
					this.textBlock.FontSize = 16;
					this.Width = 200;
					this.textBlock.Width = 150;
					break;
				case DialogType.Error:
					this.textBlock.FontSize = 20;
					break;
			}
		}

		#endregion

		#region Event handlers

		private void OK_Click(
			object sender,
			RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
		
		#endregion
	}
}
