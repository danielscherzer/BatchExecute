using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BatchExecute
{
	public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void SetNotify<TYPE>(ref TYPE valueBackend, TYPE value, Action<TYPE> action = null, [CallerMemberName] string memberName = "")
		{
			valueBackend = value;
			action?.Invoke(value);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
	}
}
