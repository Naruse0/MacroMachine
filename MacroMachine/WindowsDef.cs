using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMachine
{
	// Windows関係の定義
	namespace WindowsDef
	{
		// WindowsAPIのメッセージ
		public enum WM
		{
			// キー操作関係
			KEYFIRST		= 0x0100,
			KEYDOWN			= 0x0100,
			KEYUP			= 0x0101,
			CHAR			= 0x0102,
			DEADCHAR		= 0x0103,
			SYSKEYDOWN		= 0x0104,
			SYSKEYUP		= 0x0105,
			SYSCHAR			= 0x0106,
			SYSDEADCHAR		= 0x0107,
			UNICHAR			= 0x0109,
			KEYLAST			= 0x0109,

			// マウス関係
			SETCURSOR		= 0x0020,
			MOUSEFIRST		= 0x0200,
			MOUSEMOVE		= 0x0200,
			LBUTTONDOWN		= 0x0201,
			LBUTTONUP		= 0x0202,
			LBUTTONBLCLK	= 0x0203,
			RBUTTONDOWN		= 0x0204,
			RBUTTONUP		= 0x0205,
			RBUTTONBLCLK	= 0x0206,
			MBUTTONDOWN		= 0x0207,
			MBUTTONUP		= 0x0208,
			MBUTTONBLCLK	= 0x0209,
			XBUTTONDOWN		= 0x020B,
			XBUTTONUP		= 0x020C,
			XBUTTONBLCLK	= 0x020D,

			MOUSEWHEEL		= 0x020A,
			MOUSEHWHEEL		= 0x020E,
		}

		// フックのタイプ
		public enum WH
		{
			KEYBOARD_LL		= 13,
			MOUSE_LL		= 14
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-taginput
		/// </summary>
		public enum INPUTTYPE
		{
			MOUSE		= 0,
			KEYBOARD	= 1,
			HARDWARE	= 2,
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagmouseinput
		/// </summary>
		public enum MOUSEEVENTF
		{
			// 分からない（？）
			ABSOLUTE		= 0x8000,
			VIRTUALDESK		= 0x4000,

			// ボタン関係
			MOVE			= 0x0001,
			MOVE_NOCOALESCE	= 0x2000,
			LEFTDOWN		= 0x0002,
			LEFTUP			= 0x0004,
			RIGHTDOWN		= 0x0008,
			RIGHTUP			= 0x0010,
			MIDDLEDOWN		= 0x0020,
			MIDDLEUP		= 0x0040,
			XDOWN			= 0x0080,
			XUP				= 0x0100,

			// ホイール
			WHEEL			= 0x0800,
			HWHEEL			= 0x1000
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/ns-winuser-tagkeybdinput
		/// </summary>
		public enum KEYEVENTF 
		{
			// 入力動作
			KEYDOWN		= 0x0000,
			KEYUP		= 0x0002,

			// わからない（？）
			EXTENDEDKEY	= 0x0001,
			SCANCODE	= 0x0008,
			UNICODE		= 0x0004
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-mapvirtualkeya
		/// </summary>
		public enum MAPVK
		{
			VK_TO_VSC		= 0,
			VSC_TO_VK		= 1,
			VK_TO_CHAR		= 2,
			VSC_TO_VK_EX	= 3
		}
	}
}
