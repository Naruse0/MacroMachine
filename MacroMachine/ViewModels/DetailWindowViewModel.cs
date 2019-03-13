using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MacroMachine.ViewModels
{
	using Commons;
	using Models;

    class DetailWindowViewModel : ViewModelBase
    {
		//----------------------------------------------------------
		// Private Property
		//----------------------------------------------------------

		private DelegateCommand loadedCommand;
		private DelegateCommand	closedCommand;
		private DelegateCommand startCommand;
		private DelegateCommand stopCommand;
		private DelegateCommand clearCommand;
		private bool isRecording = false;
		private string recordedKeys = "";

		public ObservableCollection<BatInfo> batInfos;
		public BatInfo selectedBatInfo;

		//----------------------------------------------------------
		// Indexer
		//----------------------------------------------------------

		public DelegateCommand LoadedCommand
		{
			get
			{
				if (loadedCommand == null) { loadedCommand = new DelegateCommand(loaded); }
				return loadedCommand;
			}
		}
		public DelegateCommand ClosedCommand
		{
			get
			{
				if (closedCommand == null) { closedCommand = new DelegateCommand(closed); }
				return closedCommand;
			}
		}
		public DelegateCommand StartCommand
		{
			get
			{
				if(startCommand == null) { startCommand = new DelegateCommand(start, (object obj) => !isRecording && !KeyboardHook.IsPause); }
				return startCommand;
			}
		}
		public DelegateCommand StopCommand
		{
			get
			{
				if (stopCommand == null) { stopCommand = new DelegateCommand(stop, (object obj)=> isRecording); }
				return stopCommand;
			}
		}
		public DelegateCommand ClearCommand
		{
			get
			{
				if (clearCommand == null) { clearCommand = new DelegateCommand(clear, (object obj) => !isRecording); }
				return clearCommand;
			}
		}

		public string RecordedKeys
		{
			get
			{
				return recordedKeys;
			}
			set
			{
				recordedKeys = value;
				RaisePropertyChanged("RecordedKeys");
			}
		}

		public ObservableCollection<BatInfo> BatInfos
		{
			get { return batInfos; }
			set {
				batInfos = value;
				RaisePropertyChanged("BatInfos");
			}
		}
		public BatInfo SelectedBatInfo
		{
			get { return selectedBatInfo; }
			set
			{
				selectedBatInfo = value;
				RaisePropertyChanged("SelectedBatInfo");

				if (App.SelectedMacro != null)
				{
					App.SelectedMacro.batInfo = selectedBatInfo;
				}
			}
		}


		//----------------------------------------------------------
		// Private Method
		//----------------------------------------------------------

		private void loaded(object obj)
		{
			BatInfos = App.batInfos;

			setRecordedKeyString();
			SelectedBatInfo = App.SelectedMacro.batInfo;
		}

		private void closed(object obj)
		{
			App.SelectedMacro.isShowedDetail = false;
			stop(obj);
		}

		private void start(object obj)
		{
			isRecording = true;
			clear(null);

			KeyboardHook.AddEvent(recording);
		}

		private void stop(object obj)
		{
			isRecording = false;
			KeyboardHook.RemoveEvent(recording);
		}

		private void clear(object obj)
		{
			App.SelectedMacro.keys.Clear();
			setRecordedKeyString();

		}

		//----------------------------------------------------------
		// Inner Method
		//----------------------------------------------------------

		/// <summary>
		/// マクロのキーを登録する際に登録する関数
		/// </summary>
		/// <param name="state"></param>
		private void recording(ref KeyboardHook.KeyboardState state)
		{
			if(App.SelectedMacro == null) { return; }

			// 押下したときのみ
			if (state.Stroke == KeyboardHook.Stroke.KeyDown ||
				state.Stroke == KeyboardHook.Stroke.SyskeyDown)
			{
				ref var macro = ref App.SelectedMacro;

				// 初めて押されたときのみ登録する
				if (!macro.keys.Contains(state.Key))
				{
					macro.keys.Add(state.Key);
				}
			}

			// 文字列を設定
			setRecordedKeyString();

			// キーを無視する
			KeyboardHook.Discard();
		}

		/// <summary>
		/// 表示する文字列を設定する。
		/// </summary>
		private void setRecordedKeyString()
		{
			string str = "";
			foreach (var k in App.SelectedMacro.keys)
			{
				str += k.ToString() + " + ";
			}

			int index = str.LastIndexOf('+');
			if (index > 0)
			{
				RecordedKeys = str.Substring(0, index);
			}
			else
			{
				RecordedKeys = "";
			}
		}
	}
}
