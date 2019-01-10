using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace BatchExecute
{
	public static class ListBoxTools
	{
		public static void DeleteSelected(this ListBox listBox)
		{
			if (listBox is null) return;
			var source = listBox.ItemsSource as IList<string>;
			if (source is null) return;
			do
			{
				var i = listBox.SelectedIndex;
				if (-1 == i) break;
				source.RemoveAt(i);
			} while (true);
		}

		public static void InvertSelection(this ListBox listBox)
		{
			if (listBox is null) return;
			var selected = new HashSet<object>(listBox.SelectedItems.Cast<string>());
			listBox.SelectedItems.Clear();
			foreach(var item in listBox.Items)
			{
				if(!selected.Contains(item))
				{
					listBox.SelectedItems.Add(item);
				}
			}
		}

		public static void Select(this ListBox listBox, string sSelect)
		{
			if (listBox is null) return;
			listBox.SelectedItems.Clear();
			foreach(var item in listBox.Items)
			{
				if (item.ToString().Contains(sSelect))
				{
					listBox.SelectedItems.Add(item);
				}
			}
		}
	}
}