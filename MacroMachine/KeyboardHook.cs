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
	/// キーボードのグローバルフックに関するクラス
	/// </summary>
    public static class KeyboardHook
    {
		/// <summary>
		/// プラットフォーム呼び出し関係のクラス
		/// </summary>
		private static class PlatformInvoke
		{
			/// <summary>
			/// Windwos.h tagKBDLLHOOKSTRUCT 構造体
			/// </summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct KBDLLHOOKSTRUCT
			{
				public uint		vkCode;
				public uint		scanCode;
				public uint		flags;
				public uint		time;
				public IntPtr   dwExtraInfo;
			}

			/// <summary>
			/// フックプロシージャのデリゲート
			/// </summary>
			public delegate IntPtr KeyboardHookCallback(int nCode, uint msg, ref KBDLLHOOKSTRUCT kbdllhookstruct);

			/// <summary>
			/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-setwindowshookexa
			/// </summary>
			[DllImport("user32.dll")]
			public static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookCallback lpfn, IntPtr hMod, uint dwThreadID);

			/// <summary>
			/// https://docs.microsoft.com/ja-jp/windows/desktop/api/Winuser/nf-winuser-callnexthookex
			/// </summary>
			[DllImport("user32.dll")]
			public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, uint msg, ref KBDLLHOOKSTRUCT kbdllhookstruct);

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
		/// キーボードの状態を示す列挙型
		/// </summary>
		public enum Stroke
		{
			KeyDown,
			KeyUp,
			SyskeyDown,
			SyskeyUp,
			Unknown
		};

		/// <summary>
		/// キーボードの状態を扱う構造体
		/// </summary>
		public struct KeyboardState
		{
			public Stroke       Stroke;
			public uint         RawKey;     // Forms.Keys (KBDLLHOOKSTRUCTから送られてくるキーコード)
			public Key          Key;        // RawKeyをWPFように変換したもの
			public List<Key>    Keys;		// 押されている通常キーを保持する
			public uint         ScanCode;
			public uint         Flags;
			public uint         Time;
			public IntPtr       ExtraInfo;
		}

		/// <summary>
		/// キーボードのグローバルフックを行っているかどうか
		/// </summary>
		public static bool IsHooking { get; private set; }

		/// <summary>
		/// 入力を破棄するかどうか（次のプロシージャへ渡さなくする）
		/// </summary>
		public static bool WillDiscard { get; private set; }

		/// <summary>
		/// キーボードの状態を保持する
		/// </summary>
		public static KeyboardState State;

		/// <summary>
		/// フックプロシージャ内のイベント用デリゲート
		/// </summary>
		public delegate void HookHandler(ref KeyboardState state);

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
		private static event PlatformInvoke.KeyboardHookCallback RegisteredHookCallback;

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

			// インスタンス化
			State.Keys = new List<Key>();

			// ウィンドウのハンドルインスタンスを取得
			IntPtr h = Marshal.GetHINSTANCE(typeof(KeyboardHook).Assembly.GetModules()[0]);

			// 登録するメソッドを保持する
			RegisteredHookCallback = HookProcedure;

			// グローバルフックを行いハンドルを取得
			HookHandle = PlatformInvoke.SetWindowsHookEx((int)WH.KEYBOARD_LL, RegisteredHookCallback, h, 0);

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
			if (!IsHooking) { return; }

			if (HookHandle != IntPtr.Zero)
			{
				IsHooking = false;

				// フックを解除
				PlatformInvoke.UnhookWindowsHookEx(HookHandle);

				// 初期化
				HookHandle = IntPtr.Zero;
				RegisteredHookCallback -= HookProcedure;

				// インスタンス削除
				State.Keys = null;
			}
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
		private static IntPtr HookProcedure(int nCode, uint msg, ref PlatformInvoke.KBDLLHOOKSTRUCT s)
		{
			if (nCode >= 0 && HookEvent != null)
			{
				// メッセージから入力状態を取得
				State.Stroke = GetStroke(msg);
				State.RawKey = s.vkCode;
				State.Key = KeyInterop.KeyFromVirtualKey((int)s.vkCode);
				State.ScanCode = s.scanCode;
				State.Flags = s.flags;
				State.Time = s.time;
				State.ExtraInfo = s.dwExtraInfo;

				// 更新
				UpdateKeys(ref State, State.Stroke, State.Key);

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
		/// キーボードの挙動を取得する
		/// </summary>
		private static Stroke GetStroke(uint msg)
		{
			switch (msg)
			{
				case (int)WM.KEYDOWN:
					return Stroke.KeyDown;

				case (int)WM.KEYUP:
					return Stroke.KeyUp;

				case (int)WM.SYSKEYDOWN:
					return Stroke.SyskeyDown;

				case (int)WM.SYSKEYUP:
					return Stroke.SyskeyUp;

				default:
					return Stroke.Unknown;
			}
		}

		/// <summary>
		/// キーの同時押し情報を更新する
		/// </summary>
		/// <param name="state">代入先</param>
		/// <param name="stroke">キーのストローク状態</param>
		/// <param name="key">ストロークされたキー</param>
		private static void UpdateKeys(ref KeyboardState state, Stroke stroke, Key key)
		{
			switch (stroke)
			{
				case Stroke.KeyDown:
				case Stroke.SyskeyDown:
					// キーリピートを無視
					if (!state.Keys.Contains(key))
					{
						state.Keys.Add(key);
					}
					break;

				case Stroke.KeyUp:
				case Stroke.SyskeyUp:
					state.Keys.RemoveAll((Key k) => { return k == key; });
					break;

				default:
					return;
			}
		}
	}
}
