using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace BatchExecute
{
	public static class ListBoxTools
	{
		//public static void DeleteSelected(this ListBox listBox)
		//{
		//	if (listBox is null) return;
		//	var list = listBox.DataSource as IList<string>;
		//	if (list is null) return;
		//	//Create reverse copy
		//	var indexList = new List<int>(listBox.SelectedIndices.Count);
		//	foreach (int selected in listBox.SelectedIndices)
		//	{
		//		indexList.Insert(0, selected);
		//	}
		//	foreach (int selected in indexList)
		//	{
		//		list.RemoveAt(selected);
		//	}
		//}

		//public static void InvertSelection(this ListBox listBox)
		//{
		//	if (listBox is null) return;
		//	for (int i = 0; i < listBox.Items.Count; ++i)
		//	{
		//		bool selected = listBox.GetSelected(i);
		//		listBox.SetSelected(i, !selected);
		//	}
		//}

		public static void Select(this ListBox listBox, string sSelect)
		{
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