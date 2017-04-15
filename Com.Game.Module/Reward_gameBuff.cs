using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_gameBuff : RewardItemBase
	{
		public Reward_gameBuff() : base(ERewardType.Reward_gameBuff)
		{
		}

		public override void Init(string[] param)
		{
			base.Init(param);
			if (base.Valid)
			{
				int num;
				if (int.TryParse(param[2], out num))
				{
					base.Num = num;
					SysGameBuffVo dataById = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(param[1]);
					if (dataById != null)
					{
						base.SIcon = dataById.icon;
						base.BIcon = dataById.long_icon;
						base.TypeDes = LanguageManager.Instance.GetStringById(dataById.name);
						base.Des = LanguageManager.Instance.GetStringById(dataById.describe);
						base.Quality = dataById.quality;
						base.Valid = true;
					}
				}
			}
		}
	}
}
