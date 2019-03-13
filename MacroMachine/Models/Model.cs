using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MacroMachine.Models
{
	/// <summary>
	/// 実行するBatファイルの情報を保持するクラス
	/// </summary>
	public class BatInfo
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
	}

	/// <summary>
	/// マクロ情報を保持するクラス
	/// </summary>
	public class MacroInfo
	{
		// キーの登録先
		public List<Key>	keys;
		public bool         isShowedDetail;
		public BatInfo      batInfo;

		/// <summary>
		/// 初期化用コンストラクタ
		/// </summary>
		public MacroInfo()
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
