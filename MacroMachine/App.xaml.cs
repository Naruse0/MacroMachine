using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using System.Threading;

namespace MacroMachine
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// アプリケーション全体でウィンドウを管理する変数
		/// </summary>
		private static MainWindow	window;

		private NotifyIconWrapper	notifyIcon;
		private Mutex               mutex;

		public static MainWindow Window
		{
			get { return window; }
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			mutex = new Mutex(false, "MacroMachineMutex");

			// 一つ目の起動時
			if (mutex.WaitOne(0, false))
			{
				this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

				// ウィンドウを生成
				ShowWindow();

				this.notifyIcon = new NotifyIconWrapper();
			}
			// 二重起動時
			else
			{
				MessageBox.Show("既に起動しています。", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);

				mutex.Close();
				mutex = null;
				this.Shutdown();
			}
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			this.notifyIcon.Dispose();

			// Mutexの解放処理
			if(mutex != null)
			{
				mutex.ReleaseMutex(); 
				mutex.Close();
			}

		}


		/// <summary>
		/// ウィンドウの表示（生成）を行う
		/// </summary>
		public static void ShowWindow()
		{
			if(window == null)
			{
				window = new MainWindow();
				window.Show();
			}
			else
			{
				window.Focus();
			}
		}

		/// <summary>
		/// ウィンドウの終了処理
		/// </summary>
		public static void FinalizeWindow()
		{
			window = null;
		}
	}
}
