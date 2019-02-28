using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MacroMachine
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private NotifyIconWrapper notifyIcon;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
			this.notifyIcon = new NotifyIconWrapper();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			this.notifyIcon.Dispose();
		}
	}
}
