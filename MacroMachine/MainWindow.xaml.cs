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
		//----------------------------------------------------------
		// WPF イベント
		//----------------------------------------------------------

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			App.FinalizeWindow();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if(MouseHook.IsHooking)
			{
				MouseHook.RemoveEvent(ExecMouseHook);
				MouseHook.Stop();
				textBox1.Text = textBox2.Text = "-- Stop --";

			}
			else
			{
				MouseHook.AddEvent(ExecMouseHook);
				MouseHook.Start();
			}
		}


		private void ButtonKey_Click(object sender, RoutedEventArgs e)
		{
			if (KeyboardHook.IsHooking)
			{
				KeyboardHook.RemoveEvent(ExecKeyboardHook);
				KeyboardHook.Stop();
				textBox2.Text = textBox2.Text = "-- Stop --";

			}
			else
			{
				KeyboardHook.AddEvent(ExecKeyboardHook);
				KeyboardHook.Start();
			}
		}

		//----------------------------------------------------------
		// フックするメソッド
		//----------------------------------------------------------

		/// <summary>
		/// マウスフック時に実行するメソッド
		/// </summary>
		public void ExecMouseHook(ref MouseHook.MouseState s)
		{
			string Ret = "\r\n";
			textBox1.Text = "";
			textBox1.Text += "Stroke   : " + s.Stroke.ToString() + Ret;
			textBox1.Text += "Position : " + s.X + ", " + s.Y + Ret;

			if(s.Stroke == MouseHook.Stroke.LeftDown)
			{
				var inputs = new List<InputSimulator.INPUT>();
				InputSimulator.AddKeyboardInput(ref inputs, KEYEVENTF.KEYDOWN, Key.A);
				InputSimulator.SendInput(inputs);
			}
			else if(s.Stroke == MouseHook.Stroke.LeftUp)
			{
				var inputs = new List<InputSimulator.INPUT>();
				InputSimulator.AddKeyboardInput(ref inputs, KEYEVENTF.KEYUP, Key.A);
				InputSimulator.SendInput(inputs);
			}

			if (s.Stroke == MouseHook.Stroke.RightDown)
			{
				var inputs = new List<InputSimulator.INPUT>();
				InputSimulator.AddKeyboardInput(ref inputs, "AIEUO aiueo 0123456789");
				InputSimulator.SendInput(inputs);
			}
		}

		/// <summary>
		/// キーボードフック時に実行するメソッド
		/// </summary>
		public void ExecKeyboardHook(ref KeyboardHook.KeyboardState s)
		{
			string Ret = "\r\n";
			textBox2.Text = "";
			textBox2.Text += "Stroke   : " + s.Stroke.ToString() + Ret;
			textBox2.Text += "RawKey   : " + s.RawKey + Ret;
			textBox2.Text += "Key      : " + s.Key.ToString() + Ret;
			textBox2.Text += "Key(int) : " + (int)s.Key + Ret;
			textBox2.Text += Ret;

			// 全キーの表示
			textBox2.Text += "Keys     : ";
			foreach(var key in s.Keys)
			{
				textBox2.Text += key.ToString() + " + ";
			}

			// 最後の+が表示されないように整形
			int index = textBox2.Text.LastIndexOf('+');
			if (index > 0)
			{
				textBox2.Text = textBox2.Text.Substring(0, index);
			}
			textBox2.Text += Ret;
		}
	}
}
