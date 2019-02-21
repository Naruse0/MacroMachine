﻿using System;
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
		}

		private void Window_Closed(object sender, EventArgs e)
		{
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if(MouseHook.IsHooking)
			{
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
		}

	}
}
