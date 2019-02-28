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
		private NotifyIconWrapper	notifyIcon;

		private Mutex               mutex;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			mutex = new Mutex(false, "MacroMachineMutex");

			// 一つ目の起動時
			if (mutex.WaitOne(0, false))
			{
				this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
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
	}
}
