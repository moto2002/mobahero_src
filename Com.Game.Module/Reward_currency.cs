using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_currency : RewardItemBase
	{
		public Reward_currency() : base(ERewardType.Reward_currency)
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
					SysCurrencyVo dataById = BaseDataMgr.instance.GetDataById<SysCurrencyVo>(param[1]);
					if (dataById != null)
					{
						base.SIcon = dataById.icon_gameresid;
						base.BIcon = dataById.Longicon_gameresid;
						base.TypeDes = LanguageManager.Instance.GetStringById(dataById.name);
						base.Des = LanguageManager.Instance.GetStringById(dataById.description);
						base.Quality = dataById.quality;
						base.Valid = true;
					}
				}
			}
		}
	}
}
