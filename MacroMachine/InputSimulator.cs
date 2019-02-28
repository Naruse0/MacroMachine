using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MacroMachine
{
	using WindowsDef;

	/// <summary>
	/// マウス・キーボードの入力操作のシミュレーション関係のクラス
	/// </summary>
    public static class InputSimulator
    {
		/// <summary>
		/// プラットフォーム呼び出し関係をまとめたクラス
		/// </summary>
		private static class PlatformInvoke
		{
			/// <summary>
			/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-mapvirtualkeya
			/// </summary>
			[DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
			public extern static int MapVirtualKey(int uCode, int uMapType);

			/// <summary>
			/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-sendinput
			/// </summary>
			[DllImport("user32.dll")]
			public static extern uint SendInput(int nInputs, INPUT[] pInputs, int cbSize);
		}

		#region PlatformInvoke related struct 

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmouseinput
		/// </summary>
		public struct MOUSEINPUT
		{
			public int     dx;
			public int     dy;
			public uint    mouseData;
			public uint    dwFlags;
			public uint    time;
			public UIntPtr dwExtraInfo;
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagkeybdinput
		/// </summary>
		public struct KEYBDINPUT
		{
			public short   wVk;
			public short   wScan;
			public uint    dwFlags;
			public uint    time;
			public UIntPtr dwExtraInfo;
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-taghardwareinput
		/// </summary>
		public struct HARDWAREINPUT
		{
			public uint    uMsg;
			public short   wParamL;
			public short   wParamH;
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-taginput
		/// </summary>
		/// <remarks>
		/// union { mi, ki, hi }	
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT
		{
			[FieldOffset(0)]
			public int              type;

			[FieldOffset(4)]
			public MOUSEINPUT       mi;

			[FieldOffset(4)]
			public KEYBDINPUT       ki;

			[FieldOffset(4)]
			public HARDWAREINPUT    hi;

		}

		#endregion

		#region Public functions

		/// <summary>
		/// 入力イベントの実行
		/// </summary>
		/// <param name="inputs">実行する入力群</param>
		public static void SendInput(List<INPUT> inputs)
		{
			INPUT[] inputArray = inputs.ToArray();
			SendInput(inputArray);
		}

		/// <summary>
		/// 文字列の入力イベントを追加する
		/// </summary>
		/// <param name="inputs">追加する対象</param>
		/// <param name="srcStr">シミュレートする文字列(a~z, A~Z, 0~9, Space)</param>
		public static void AddKeyboardInput(ref List<INPUT> inputs, string srcStr)
		{
			if (String.IsNullOrEmpty(srcStr)) { return; }

			KEYBDINPUT ki;
			ki.time = 0;
			ki.dwExtraInfo = UIntPtr.Zero;

			foreach (var c in srcStr)
			{
				char key = c;
				if(!Char.IsLetterOrDigit(c) && !Char.IsWhiteSpace(c))
				{
					continue;
				}

				if(Char.IsLower(c))
				{
					key = Char.ToUpper(c);
				}

				ki.wVk = (short)(key);
				ki.wScan = (short)PlatformInvoke.MapVirtualKey(ki.wVk, (int)MAPVK.VK_TO_VSC);

				// DOWN
				ki.dwFlags = (int)(KEYEVENTF.KEYDOWN | KEYEVENTF.EXTENDEDKEY);
				AddKeyboardInput(ref inputs, ki);

				// UP
				ki.dwFlags = (int)(KEYEVENTF.KEYUP | KEYEVENTF.EXTENDEDKEY);
				AddKeyboardInput(ref inputs, ki);
			}

		}

		/// <summary>
		/// キーボード入力イベントを追加する
		/// </summary>
		/// <param name="inputs">追加する対象</param>
		/// <param name="flags">キーの状態</param>
		/// <param name="key">入力するキー</param>
		public static void AddKeyboardInput(ref List<INPUT> inputs, KEYEVENTF flags, Key key)
		{

			uint	keyboardFlags = (uint)flags | (uint)KEYEVENTF.EXTENDEDKEY;
			short   vk = (short)KeyInterop.VirtualKeyFromKey(key); 
			short   scanCode = (short)PlatformInvoke.MapVirtualKey(vk, (int)MAPVK.VK_TO_VSC);

			// データを作成
			KEYBDINPUT ki;
			ki.dwFlags = keyboardFlags;
			ki.wVk = vk;
			ki.wScan = scanCode;
			ki.time = 0;
			ki.dwExtraInfo = UIntPtr.Zero;

			// 追加
			AddKeyboardInput(ref inputs, ki);
		}

		#endregion

		#region LowLevel wrapper

		/// <summary>
		/// 入力イベントの実行
		/// </summary>
		/// <param name="inputs">実行する入力群</param>
		private static void SendInput(INPUT[] inputs)
		{
			PlatformInvoke.SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
		}
		
		/// <summary>
		/// キー入力イベントの追加
		/// </summary>
		/// <param name="inputs"></param>
		/// <param name="keybdinput"></param>
		private static void AddKeyboardInput(ref List<INPUT> inputs, KEYBDINPUT keybdinput)
		{
			var input = new INPUT();
			input.type = (int)INPUTTYPE.KEYBOARD;
			input.ki = keybdinput;

			inputs.Add(input);
		}	

		#endregion

	}
}
