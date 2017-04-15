using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_bottleExp : RewardItemBase
	{
		public Reward_bottleExp() : base(ERewardType.Reward_bottleExp)
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
					SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>("7777");
					if (dataById != null)
					{
						base.SIcon = dataById.icon;
						base.BIcon = dataById.Long_icon;
						base.TypeDes = LanguageManager.Instance.GetStringById("GameItems_Name_7777");
						base.Des = LanguageManager.Instance.GetStringById("GameItems_Describe_7777");
						base.Quality = dataById.quality;
						base.Valid = true;
					}
				}
			}
		}
	}
}
