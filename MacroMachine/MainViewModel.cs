using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace MacroMachine
{
	using MacroMachine;

	/// <summary>
	/// メインビューのビューモデル
	/// </summary>
	class MainViewModel : ViewModelBase
	{
		private DelegateCommand showDetailCommand;

		public DelegateCommand RecordingCommand
		{
			get
			{
				if (showDetailCommand == null) { showDetailCommand = new DelegateCommand(showDetail); }
				return showDetailCommand;
 			}
		}

		//----------------------------------------------------------
		// Private
		//----------------------------------------------------------

		private void showDetail(object obj)
		{
			if (obj == null) { return; }

			string	keyName = (string)obj;
			Key     key;
			
			// キーを取得し詳細ウィンドウを表示
			if(Enum.TryParse<Key>(keyName, out key))
			{
				MainWindow.SelectMacro(key);

				var wnd = new DetailWindow();
				wnd.Owner = App.Window;
				wnd.ShowDialog();
			}
			// keyNameが不正な場合
			else
			{
				System.Windows.MessageBox.Show("渡されたKeyの名前が正しくありません。(Parameter = " + keyName + ")");
			}
		}

		private bool isShowedDetail(object obj)
		{
			bool isNull = MainWindow.SelectedMacro == null;
			bool isShowed = false;

			if (!isNull)
			{
				isShowed = MainWindow.SelectedMacro.isShowedDetail;
			}

			return isNull && !isShowed;
		}
	}
}
