using System.Windows.Input;

namespace BatchExecute
{
	public static class CustomCommands
	{
		public static readonly RoutedUICommand RunAll = new RoutedUICommand
	(
		"RunAll",
		"RunAll",
		typeof(CustomCommands),
		new InputGestureCollection() { new KeyGesture(Key.R, ModifierKeys.Control) }
	);
	}
}
