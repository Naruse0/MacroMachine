using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Collections.ObjectModel;

namespace MacroMachine
{
	using Commons;
	using Commons.WindowsDef;
	using Models;
	using Views;

	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// アプリケーション全体でウィンドウを管理する変数
		/// </summary>
		private static MainWindow	window;

		/// <summary>
		/// タスクバーに表示するための変数
		/// </summary>
		private NotifyIconWrapper	notifyIcon;

		/// <summary>
		/// 同時実行を制御するための変数
		/// </summary>
		private Mutex               mutex;

		/// <summary>
		/// キーごとのマクロデータ
		/// </summary>
		public static SortedDictionary<Key, MacroInfo>  Macros;

		/// <summary>
		/// 選択されたキーの参照
		/// </summary>
		public static MacroInfo                         SelectedMacro;

		/// <summary>
		/// マクロを実行する際のトリガーとなるキー
		/// </summary>
		/// <remarks>
		/// このキーと指定のキーを同時押ししてマクロを実行する
		/// 実行中はこのキーが使えなくなるので注意
		/// </remarks>
		public static readonly Key  executionTriggerKey = Key.Capital;

		/// <summary>
		/// キー実行コマンドが押されているかどうか
		/// </summary>
		public static bool          isTriggered = false;

		/// <summary>
		/// 全Bat情報を保持する
		/// </summary>
		public static ObservableCollection<BatInfo>	batInfos;

		//----------------------------------------------------------
		// Indexer
		//----------------------------------------------------------

		/// <summary>
		/// メインウィンドウへのアクセス
		/// </summary>
		public static MainWindow Window
		{
			get { return window; }
		}

		//----------------------------------------------------------
		// WPF Event
		//----------------------------------------------------------

		public App()
		{
			// 初期化
			Macros = new SortedDictionary<Key, MacroInfo>();
			SelectedMacro = null;

			InitBatInfos();
		}

		/// <summary>
		/// アプリケーション開始時のイベント
		/// </summary>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			mutex = new Mutex(false, "MacroMachineMutex");

			// 一つ目の起動時
			if (mutex.WaitOne(0, false))
			{
				this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

				// ウィンドウを生成
				ShowWindow();

				// マクロ実行のフックをスタート
				KeyboardHook.AddEvent(ProcUpdateExecutionTrigger);
				KeyboardHook.AddEvent(ProcWaitExecuteMacro);
				KeyboardHook.Start();

				this.notifyIcon = new NotifyIconWrapper();
			}
			// 二重起動時
			else
			{
				MessageBox.Show("既に起動しています。", "Infomation", MessageBoxButton.OK, MessageBoxImage.Information);

				mutex.Close();
				mutex = null;
				this.Shutdown();
			}
		}

		/// <summary>
		/// アプリケーション終了時のイベント
		/// </summary>
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			notifyIcon.Dispose();

			// イベントを削除
			KeyboardHook.RemoveEvent(ProcWaitExecuteMacro);
			KeyboardHook.RemoveEvent(ProcUpdateExecutionTrigger);

			// フックを止める
			MouseHook.Stop();
			KeyboardHook.Stop();

			// Mutexの解放処理
			if (mutex != null)
			{
				mutex.ReleaseMutex(); 
				mutex.Close();
			}
		}

		//----------------------------------------------------------
		// Public
		//----------------------------------------------------------

		/// <summary>
		/// ウィンドウの表示（生成）を行う
		/// </summary>
		public static void ShowWindow()
		{
			if(window == null)
			{
				window = new MainWindow();
				window.Show();
			}
			else
			{
				window.Focus();
			}
		}

		/// <summary>
		/// ウィンドウの終了処理
		/// </summary>
		public static void FinalizeWindow()
		{
			window = null;
		}

		/// <summary>
		/// 指定したキーのマクロを選択
		/// </summary>
		public static void SelectMacro(Key k)
		{
			// キーがなければ追加
			if (!Macros.ContainsKey(k))
			{
				Macros.Add(k, new MacroInfo());
			}
			SelectedMacro = Macros[k];
		}

		/// <summary>
		/// キーに登録されているマクロを実行する
		/// </summary>
		public void ExecuteMacro(Key k)
		{
			// キーにマクロが登録されていない場合は無視
			if (!Macros.ContainsKey(k)) { return; }
			// マクロが登録されていない場合は無視
			if(Macros[k].keys.Count() <= 0) { return; }

			// 再度イベントを中断
			KeyboardHook.RemoveEvent(ProcWaitExecuteMacro);

			// キー送信
			List<InputSimulator.INPUT> inputs = new List<InputSimulator.INPUT>();

			// 押下状態を送信
			foreach (var macroKey in Macros[k].keys)
			{
				InputSimulator.AddKeyboardInput(ref inputs, KEYEVENTF.KEYDOWN, macroKey);
			}
			// 押上状態を創始音
			foreach (var macroKey in Macros[k].keys)
			{
				InputSimulator.AddKeyboardInput(ref inputs, KEYEVENTF.KEYUP, macroKey);
			}

			// キーを送信
			InputSimulator.SendInput(inputs);

			// 再度イベントを実行
			KeyboardHook.AddEvent(ProcWaitExecuteMacro);

			// 実行時のキーは破棄する
			KeyboardHook.Discard();
		}

		//----------------------------------------------------------
		// Private
		//----------------------------------------------------------
	
		/// <summary>
		/// Bat情報を初期化
		/// </summary>
		private void InitBatInfos()
		{
			batInfos = new ObservableCollection<BatInfo>()
			{
				new BatInfo {Id = -1, Name= "----"},
				new BatInfo {Id = 0, Name= "Hello"},
				new BatInfo {Id = 1, Name= "GoodBye"}
			};	
		}

		/// <summary>
		/// コマンド実行トリガーキーが押されているかの状態を更新する
		/// </summary>
		/// <param name="state"></param>
		private void ProcUpdateExecutionTrigger(ref KeyboardHook.KeyboardState state)
		{
			if (state.Key == executionTriggerKey)
			{
				if(state.Stroke == KeyboardHook.Stroke.KeyDown)
				{
					isTriggered = true;
				}
				else if(state.Stroke == KeyboardHook.Stroke.KeyUp)
				{
					isTriggered = false;
				}

				KeyboardHook.Discard();
			}
		}

		/// <summary>
		/// マクロ実行のため常に待機しているキーボードフック
		/// </summary>
		private void ProcWaitExecuteMacro(ref KeyboardHook.KeyboardState state)
		{
			// トリガーキーが押されていない場合は無視
			if(!isTriggered) { return; }

			// 押されたキーのマクロを実行
			if (state.Stroke == KeyboardHook.Stroke.KeyDown)
			{
				Key curKey = state.Key;
				if (curKey >= Key.A && curKey <= Key.Z)
				{
					ExecuteMacro(curKey);
				}
			}
		}
	}
}
