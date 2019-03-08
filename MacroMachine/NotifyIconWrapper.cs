using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMachine
{
	using System;
	using System.ComponentModel;
	using System.Windows;

	public partial class NotifyIconWrapper : Component
	{
		public NotifyIconWrapper()
		{
			InitializeComponent();

			this.toolStripMenuItem_Open.Click += this.toolStripMenuItem_Open_Click;
			this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;
			this.toolStripMenuItem_TogglePause.Click += this.toolStripMenuItem_TogglePause_Click;

			// 初期値を設定
			if (KeyboardHook.IsPause) { this.toolStripMenuItem_TogglePause.Text = "再開"; }
			else { this.toolStripMenuItem_TogglePause.Text = "一時停止"; }
		}

		public NotifyIconWrapper(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
		{
			App.ShowWindow();
		}

		private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void toolStripMenuItem_TogglePause_Click(object sender, EventArgs e)
		{
			if (KeyboardHook.IsPause)
			{
				this.toolStripMenuItem_TogglePause.Text = "一時停止";
				KeyboardHook.Unpause();
				MouseHook.Unpause();
			}
			else
			{
				this.toolStripMenuItem_TogglePause.Text = "再開";
				KeyboardHook.Pause();
				MouseHook.Pause();
			}
		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{

		}
	}
}
