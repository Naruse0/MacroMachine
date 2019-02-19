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
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			MouseHook.AddEvent(HookFunc);
		}

		public void HookFunc(ref MouseHook.StateMouse s)
		{
			string RET = "\r\n";
			textBox1.Text = "";
			textBox1.Text += "BUTTON   : " + s.Stroke.ToString() + RET;
			textBox1.Text += "POSITION : " + s.X + "," + s.Y + RET;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (MouseHook.IsHooking)
			{
				if (MouseHook.IsPaused)
				{
					MouseHook.UnPause();
				}
				else
				{
					MouseHook.Pause();
				}
			}
			else
			{
				MouseHook.Start();
			}
		}

	}
}
