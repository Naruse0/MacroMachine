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

namespace MacroMachine.Views
{
	using Commons.WindowsDef;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		//----------------------------------------------------------
		// WPF イベント
		//----------------------------------------------------------

		public MainWindow()
		{
			InitializeComponent();

			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string Ret = "\r\n";
			string str = "";
			str += "終了しますか？" + Ret;
			str += "" + Ret;
			str += "はい\t：アプリケーションを完全に終了します。" + Ret;
			str += "いいえ\t：ウィンドウのみ閉じて、タスクバーに残ります。" + Ret;

			var rslt = MessageBox.Show(str, "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);
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

	}
}
