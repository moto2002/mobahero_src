using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;

public class UtilMonsterData : UtilData
{
	public class UnitReward
	{
		public int Exp_kill
		{
			get;
			private set;
		}

		public int Gold_kill
		{
			get;
			private set;
		}

		public int Gold_extra
		{
			get;
			private set;
		}

		public int Exp_share_type
		{
			get;
			private set;
		}

		public int Exp_share_range
		{
			get;
			private set;
		}

		public UnitReward(int expKill, int goldKill, int expShareType, int expShareRange, int goldExtra)
		{
			this.Exp_kill = expKill;
			this.Gold_kill = goldKill;
			this.Exp_share_type = expShareType;
			this.Exp_share_range = expShareRange;
			this.Gold_extra = goldExtra;
		}
	}

	private Dictionary<int, UtilMonsterData.UnitReward> _unitDics;

	private int _startIdx = 1;

	private int _endIdx = 6;

	public UtilMonsterData(string id) : base(id)
	{
	}

	protected override void InitConfig()
	{
		this._unitDics = new Dictionary<int, UtilMonsterData.UnitReward>();
		Dictionary<string, SysBattleAttrRewardVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysBattleAttrRewardVo>();
		foreach (KeyValuePair<string, SysBattleAttrRewardVo> current in typeDicByType)
		{
			SysBattleAttrRewardVo value = current.Value;
			int key = int.Parse(current.Key);
			int expShareType = 0;
			int expShareRange = 0;
			string[] stringValue = StringUtils.GetStringValue(value.exp_share_type, '|');
			if (stringValue.Length == 1)
			{
				expShareType = 1;
			}
			else if (stringValue.Length == 2)
			{
				expShareType = int.Parse(stringValue[0]);
				expShareRange = int.Parse(stringValue[1]);
			}
			this._unitDics.Add(key, new UtilMonsterData.UnitReward(value.exp_kill, value.gold_kill, expShareType, expShareRange, value.gold_extra));
		}
	}

	public UtilMonsterData.UnitReward GetReward(int type)
	{
		UtilMonsterData.UnitReward result = null;
		if (this._unitDics.TryGetValue(type, out result))
		{
			return result;
		}
		return null;
	}
}
