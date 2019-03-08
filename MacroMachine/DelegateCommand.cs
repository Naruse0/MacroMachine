using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroMachine
{
	using System;
	using System.Windows.Input;

	//!	@brief	コマンドの基底クラス
	class DelegateCommand : ICommand
	{
		private Action<object> ExecuteHandler { get; set; }
		private Func<object, bool> CanExecuteHandler { get; set; }

		//!	@brief	イベントの管理
		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		//!	@brief	コンストラクタ
		public DelegateCommand(Action<object> execute) : this(execute, (object obj) => true)
		{
		}

		public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
		{
			if (execute == null) { throw new ArgumentNullException("execute"); }
			if (canExecute == null) { throw new ArgumentNullException("canExecute"); }

			ExecuteHandler = execute;
			CanExecuteHandler = canExecute;
		}


		//! @brief	実行可否状態を取得
		bool ICommand.CanExecute(object parameter)
		{
			return CanExecuteHandler(parameter);
		}

		//!	@brieff	実行
		public void Execute(object parameter)
		{
			ExecuteHandler(parameter);
		}
	}
}
