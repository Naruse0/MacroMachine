using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMachine
{
    class DetailWindowViewModel : ViewModelBase
    {
		//----------------------------------------------------------
		// Private Property
		//----------------------------------------------------------

		private DelegateCommand loadedCommand;
		private DelegateCommand startCommand;
		private DelegateCommand stopCommand;
		private DelegateCommand clearCommand;
		private bool isRecording = false;
		private string recordedKeys = "";

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
		public DelegateCommand StartCommand
		{
			get
			{
				if(startCommand == null) { startCommand = new DelegateCommand(start, (object obj) => !isRecording); }
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

		//----------------------------------------------------------
		// Private Method
		//----------------------------------------------------------

		private void loaded(object obj)
		{
			setRecordedKeyString();
		}

		private void start(object obj)
		{
			isRecording = true;
			clear(null);

			KeyboardHook.AddEvent(recording);
			KeyboardHook.Start();
		}

		private void stop(object obj)
		{
			isRecording = false;
			KeyboardHook.RemoveEvent(recording);
			KeyboardHook.Stop();
		}

		private void clear(object obj)
		{
			MainWindow.SelectedMacro.keys.Clear();
			setRecordedKeyString();
		}

		//----------------------------------------------------------
		// Inner Method
		//----------------------------------------------------------

		private void recording(ref KeyboardHook.KeyboardState state)
		{
			if(MainWindow.SelectedMacro == null) { return; }

			// 押下したときのみ
			if (state.Stroke == KeyboardHook.Stroke.KeyDown ||
				state.Stroke == KeyboardHook.Stroke.SyskeyDown)
			{
				ref var macro = ref MainWindow.SelectedMacro;

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

		private void setRecordedKeyString()
		{
			string str = "";
			foreach (var k in MainWindow.SelectedMacro.keys)
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
