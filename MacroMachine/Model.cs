using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MacroMachine
{
	/// <summary>
	/// データを管理するクラス
	/// </summary>
	public class Model
	{
		// キーの登録先
		public List<Key>	keys;
		public bool         isShowedDetail;

		/// <summary>
		/// 初期化用コンストラクタ
		/// </summary>
		public Model()
		{
			keys = new List<Key>();
			isShowedDetail = false;
		}

		/// <summary>
		/// キーが登録されているかどうか
		/// </summary>
		public bool IsRegistered {  get { return keys.Count > 0; } }
	}
}
