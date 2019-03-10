using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace MacroMachine.ViewModels
{
	using Commons;
	using Views;

	/// <summary>
	/// メインビューのビューモデル
	/// </summary>
	class MainViewModel : ViewModelBase
	{
		private DelegateCommand showDetailCommand;

		public	DelegateCommand ShowDetailCommand
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

		/// <summary>
		/// 押されたキーのマクロ情報の詳細ウィンドウを表示する
		/// </summary>
		/// <param name="obj">Input.Keyの文字列</param>
		private void showDetail(object obj)
		{
			if (obj == null) { return; }

			string	keyName = (string)obj;
			Key     key;
			
			// キーを取得し詳細ウィンドウを表示
			if(Enum.TryParse<Key>(keyName, out key))
			{
				App.SelectMacro(key);
				App.SelectedMacro.isShowedDetail = true;

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

		/// <summary>
		/// 詳細ウィンドウの表示状態を取得
		/// </summary>
		private bool isShowedDetail(object obj)
		{
			if (obj == null) { return true; }

			string  keyName = (string)obj;
			Key     key;
			bool	isShowed = false;

			// キーを取得し詳細ウィンドウを表示
			if (Enum.TryParse<Key>(keyName, out key))
			{
				if (App.Macros.ContainsKey(key))
				{
					isShowed = App.Macros[key].isShowedDetail;
				}
			}
			// keyNameが不正な場合
			else
			{
				System.Windows.MessageBox.Show("渡されたKeyの名前が正しくありません。(Parameter = " + keyName + ")");
			}

			return !isShowed;
		}
	}
}
