﻿using System.Windows.Input;

namespace BatchExecute
{
	public static class CustomCommands
	{
		public static readonly RoutedUICommand RunAll = new RoutedUICommand
		(
			nameof(RunAll),
			nameof(RunAll),
			typeof(CustomCommands),
			new InputGestureCollection() { new KeyGesture(Key.R, ModifierKeys.Control) }
		);

		public static readonly RoutedUICommand DeleteSelected = new RoutedUICommand
		(
			nameof(DeleteSelected),
			nameof(DeleteSelected),
			typeof(CustomCommands),
			new InputGestureCollection() { new KeyGesture(Key.Delete) }
		);
	}
}
