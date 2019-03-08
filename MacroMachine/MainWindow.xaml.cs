using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// add
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Threading;

namespace MacroMachine
{
	using WindowsDef;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// キーごとのマクロデータ
		/// </summary>
		public static SortedDictionary<Key, Model>	Macros;
		public static Model                         SelectedMacro;

		//----------------------------------------------------------
		// WPF イベント
		//----------------------------------------------------------

		public MainWindow()
		{
			InitializeComponent();

			// 初期化
			Macros = new SortedDictionary<Key, Model>();
			SelectedMacro = null;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var rslt = MessageBox.Show("終了しますか？", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
			if(rslt == MessageBoxResult.Yes)
			{
				// 終了
				Application.Current.Shutdown();
			}
			else if(rslt == MessageBoxResult.No)
			{
				// 何もいせずウィンドウだけ閉じる。（タスクバーに残る）
			}
			else
			{
				// ウィンドウも閉じない
				e.Cancel = true;
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			App.FinalizeWindow();
		}

		//----------------------------------------------------------
		// Public
		//----------------------------------------------------------

		public static void SelectMacro(Key k)
		{
			// キーがなければ追加
			if (!Macros.ContainsKey(k))
			{
				Macros.Add(k, new Model());
			}
			SelectedMacro = Macros[k];
		}
	}
}
