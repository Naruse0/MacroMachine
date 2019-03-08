using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMachine
{
	using System.ComponentModel;

	class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged(string propertyName)
		{
			var h = PropertyChanged;
			if (h != null)
			{
				h(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
