using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace MacroMachine
{
	using WindowsDef;

	/// <summary>
	/// マウスのグローバルフックを行うクラス
	/// </summary>
	public static class MouseHook
	{
		/// <summary>
		/// プラットフォーム呼び出し関係のクラス
		/// </summary>
		private static class PlatformInvoke
		{
			/// <summary>
			/// Windows.h tagPoint構造体
			/// </summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct POINT
			{
				public int x;
				public int y;
			}

			/// <summary>
			/// Windows.h tagMSLLHOOKSTRUCT 構造体
			/// </summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct MSLLHOOKSTRUCT
			{
				public POINT	pt;
				public uint     mouseData;
				public uint     flags;
				public uint     time;
				public IntPtr   dwExtraInfo;
			};

			/// <summary>
			/// フックプロシージャのデリゲート（関数ポインタに設定する型）
			/// </summary>
			public delegate IntPtr MouseHookCallback(int nCode, uint msg, ref MSLLHOOKSTRUCT msllHookStruct);

			/// <summary>
			/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowshookexa
			/// </summary>
			[DllImport("user32.dll")]
			public static extern IntPtr SetWindowsHookEx(int idHook, MouseHookCallback lpfn, IntPtr hmod, uint dwThreadID);

			/// <summary>
			/// https://docs.microsoft.com/ja-jp/windows/desktop/api/Winuser/nf-winuser-callnexthookex
			/// </summary>
			[DllImport("user32.dll")]
			public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);

			/// <summary>
			/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-unhookwindowshookex
			/// </summary>
			/// <remarks>
			/// <c>[return: MarshalAs(UnmanagedType.Bool)]</c>は
			/// DLL側（C++）ではbool型を1byteとして扱うが、C#では4byteとして扱うので、その差を吸収している。
			/// </remarks>
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool UnhookWindowsHookEx(IntPtr hhk);
		}



		/// <summary>
		/// マウスの挙動を示す列挙型
		/// </summary>
		public enum Stroke
		{
			Move,
			LeftDown,
			LeftUp,
			RightDown,
			RightUp,
			MiddleDown,
			MiddleUp,
			WheelDown,
			WheelUp,
			X1Down,
			X1Up,
			X2Down,
			X2Up,
			Unknown
		}

		/// <summary>
		/// マウスの状態を扱う構造体
		/// </summary>
		public struct MouseState
		{
			public Stroke	Stroke;
			public int      X;
			public int      Y;
			public uint     Data;
			public uint     Flags;
			public uint     Time;
			public IntPtr   ExtraInfo;
		}

		/// <summary>
		/// マウスのグローバルフックを行っているかどうか
		/// </summary>
		public static bool IsHooking { get; private set; }

		/// <summary>
		/// マウスのグローバルフックをポーズしているかどうか
		/// </summary>
		public static bool IsPause { get; private set; }

		/// <summary>
		/// マウスの状態を保持する
		/// </summary>
		public static MouseState State;

		/// <summary>
		/// フックプロシージャ内のイベント用デリゲート
		/// </summary>
		public delegate void HookHandler(ref MouseState state);

		/// <summary>
		/// 入力を破棄するかどうか（次のプロシージャへ渡さなくする）
		/// </summary>
		private static bool WillDiscard;

		/// <summary>
		/// フックプロシージャのハンドル
		/// </summary>
		private static IntPtr HookHandle;

		/// <summary>
		/// 登録されたコールバックメソッドを保持する
		/// </summary>
		/// <remarks>
		/// 保持せずにローカル変数や関数に直接設定すると、ガベージコレクションされ例外が発生する
		/// </remarks>
		private static event PlatformInvoke.MouseHookCallback RegisteredHookCallback;

		/// <summary>
		/// イベントの登録先
		/// </summary>
		private static event HookHandler HookEvent;

		/// <summary>
		/// イベントに登録されたHookHandlerを保持するリスト
		/// </summary>
		/// <remarks>
		/// 登録したイベントを削除するために使用する
		/// </remarks>
		private static List<HookHandler> RegisteredHookEvents;



		/// <summary>
		/// マウスのグローバルフックを開始
		/// </summary>
		public static void Start()
		{
			if (IsHooking) { return; }
			IsHooking = true;
			IsPause = false;

			// ウィンドウのハンドルインスタンスを取得
			IntPtr h = Marshal.GetHINSTANCE(typeof(MouseHook).Assembly.GetModules()[0]);

			// 登録するメソッドを保持する
			RegisteredHookCallback = HookProcedure;

			// グローバルフックを行いハンドルを取得
			HookHandle = PlatformInvoke.SetWindowsHookEx((int)WH.MOUSE_LL, RegisteredHookCallback, h, 0);

			// ハンドル取得失敗時
			if (HookHandle == IntPtr.Zero)
			{ 
				IsHooking = false;
				throw new System.ComponentModel.Win32Exception();
			}
		}

		/// <summary>
		/// マウスのグローバルフックを終了
		/// </summary>
		public static void Stop()
		{
			if(!IsHooking) { return; }

			if(HookHandle != IntPtr.Zero)
			{
				IsHooking = false;
				IsPause = false;

				// フックを解除
				PlatformInvoke.UnhookWindowsHookEx(HookHandle);

				// 初期化
				HookHandle = IntPtr.Zero;
				RegisteredHookCallback -= HookProcedure;
			}
		}

		/// <summary>
		/// フックをポーズ
		/// </summary>
		public static void Pause()
		{
			IsPause = true;
		}

		/// <summary>
		/// フックのポーズを解除
		/// </summary>
		public static void Unpause()
		{
			IsPause = false;
		}

		/// <summary>
		/// フックのポーズのトグル
		/// </summary>
		public static void TogglePause()
		{
			IsPause = !IsPause;
		}

		/// <summary>
		/// 入力を破棄する
		/// </summary>
		public static void Discard()
		{
			WillDiscard = true;
		}

		/// <summary>
		/// イベントの追加
		/// </summary>
		public static void AddEvent(HookHandler hookHandler)
		{
			if (RegisteredHookEvents == null)
			{
				RegisteredHookEvents = new List<HookHandler>();
			}

			HookEvent += hookHandler;
			RegisteredHookEvents.Add(hookHandler);
		}

		/// <summary>
		/// イベントの削除
		/// </summary>
		/// <param name="hookHandler"></param>
		public static void RemoveEvent(HookHandler hookHandler)
		{
			if (RegisteredHookEvents == null) { return; }

			HookEvent -= hookHandler;
			RegisteredHookEvents.Remove(hookHandler);
		}

		/// <summary>
		/// イベントの全削除
		/// </summary>
		public static void ClearEvent()
		{
			if (RegisteredHookEvents == null) { return; }

			foreach (var e in RegisteredHookEvents)
			{
				HookEvent -= e;
			}

			RegisteredHookEvents.Clear();
		}



		/// <summary>
		/// コールバックに登録するプロシージャ
		/// </summary>
		private static IntPtr HookProcedure(int nCode, uint msg, ref PlatformInvoke.MSLLHOOKSTRUCT s)
		{
			if (nCode >= 0 && HookEvent != null && !IsPause)
			{
				// メッセージから入力状態を取得
				State.Stroke = GetStroke(msg, ref s);
				State.X = s.pt.x;
				State.Y = s.pt.y;
				State.Data = s.mouseData;
				State.Flags = s.flags;
				State.Time = s.time;
				State.ExtraInfo = s.dwExtraInfo;

				// 登録されているイベントを実行
				HookEvent(ref State);

				// 入力のキャンセル
				if (WillDiscard)
				{
					// 状態をリセット
					WillDiscard = false;	

					// メッセージを破棄
					return (IntPtr)1;
				}
			}

			// 次のプロシージャへ渡す
			return PlatformInvoke.CallNextHookEx(HookHandle, nCode, msg, ref s);
		}

		/// <summary>
		/// マウスの挙動を取得する
		/// </summary>
		private static Stroke GetStroke(uint msg, ref PlatformInvoke.MSLLHOOKSTRUCT s)
		{
			switch(msg)
			{
				case (int)WM.MOUSEMOVE:
					return Stroke.Move;

				case (int)WM.LBUTTONDOWN:
					return Stroke.LeftDown;

				case (int)WM.LBUTTONUP:
					return Stroke.LeftUp;

				case (int)WM.RBUTTONDOWN:
					return Stroke.RightDown;

				case (int)WM.RBUTTONUP:
					return Stroke.RightUp;

				case (int)WM.MBUTTONDOWN:
					return Stroke.MiddleDown;

				case (int)WM.MBUTTONUP:
					return Stroke.MiddleUp;

				case (int)WM.MOUSEWHEEL:
					if ((short)((s.mouseData >> 16) & 0xFFFF) > 0) {
						return Stroke.WheelUp;
					}
					else {
						return Stroke.WheelDown;
					}

				case (int)WM.XBUTTONDOWN:
					switch(s.mouseData >> 16)
					{
						case 1:
							return Stroke.X1Down;

						case 2:
							return Stroke.X2Down;

						default:
							return Stroke.Unknown;
					}

				case (int)WM.XBUTTONUP:
					switch (s.mouseData >> 16)
					{
						case 1:
							return Stroke.X1Up;

						case 2:
							return Stroke.X2Up;

						default:
							return Stroke.Unknown;
					}

				default:
					return Stroke.Unknown;
			}
		}
	}
}
