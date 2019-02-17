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
		// DLL
		//----------------------------------------------------------

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		//----------------------------------------------------------
		// キーシミュレート関係
		//----------------------------------------------------------

		public enum VK : byte
		{
			Return = 0x0D,
			LShift = 0xA0,
			RShift = 0xA1,
			A = 0x41,
			Z = 0x5A,
			Num_0 = 0x30,
			Num_9 = 0x39
		};

		public static void SendKey(VK key)
		{
			IntPtr win = FindWindow(null, "無題 - メモ帳");
			SetForegroundWindow(win);

			keybd_event((byte)key, 0, 0, (UIntPtr)0);
		}

		public static void SendKeyUp(VK key)
		{
			keybd_event((byte)key, 0, 0x0002, (UIntPtr)0);
		}

		public static void SendString(string keys)
		{
			foreach(var key in keys)
			{
				char lowerAlpha = (char)(key & (char)0xDF);
				if(lowerAlpha >= (char)VK.A && lowerAlpha <= (char)VK.Z)
				{
					bool isLower = (key & (char)0x20) > 0;
					if (isLower)
					{
						SendKey((VK)lowerAlpha);
					}
					else
					{
						SendKey(VK.LShift);
						SendKey((VK)key);
						SendKeyUp(VK.LShift);
					}
				}
				else if(key >= (char)VK.Num_0 && key <= (char)VK.Num_9)
				{
					SendKey((VK)((char)VK.Num_0 + (key - (char)VK.Num_0)));
				}
				else
				{
					MessageBox.Show("未対応キー : " + key.ToString());
				}
			}
		}

		//----------------------------------------------------------
		// その他
		//----------------------------------------------------------

		private static System.Diagnostics.Process notepad;

		private void OpenSoftware(string softPath, string argument = "")
		{
			var psi = new System.Diagnostics.ProcessStartInfo();
			psi.FileName = softPath;
			psi.Arguments = argument;
			psi.UseShellExecute = true;
			psi.CreateNoWindow = true;
			var proc = System.Diagnostics.Process.Start(psi);

			notepad = proc;
		}

		//----------------------------------------------------------
		// WPF イベント
		//----------------------------------------------------------

		public MainWindow()
		{
			InitializeComponent();

			OpenSoftware("notepad.exe");
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			notepad.CloseMainWindow();
		}

		private void ButtonHelloWorld_Click(object sender, RoutedEventArgs e)
		{
			SendString("HelloWorld");
		}

		private void ButtonEnter_Click(object sender, RoutedEventArgs e)
		{
			SendKey(VK.Return);
		}

		private void ButtonUp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonLeft_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonRight_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonDown_Click(object sender, RoutedEventArgs e)
		{

		}


	}
}
