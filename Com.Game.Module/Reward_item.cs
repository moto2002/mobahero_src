using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_item : RewardItemBase
	{
		public Reward_item() : base(ERewardType.Reward_item)
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
					SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(param[1]);
					if (dataById != null)
					{
						base.SIcon = dataById.icon;
						base.BIcon = dataById.Long_icon;
						base.Des = LanguageManager.Instance.GetStringById(dataById.name);
						base.Quality = dataById.quality;
						string id = string.Empty;
						id = "Currency_Items";
						base.TypeDes = LanguageManager.Instance.GetStringById(id);
						base.Valid = true;
					}
				}
			}
		}
	}
}
