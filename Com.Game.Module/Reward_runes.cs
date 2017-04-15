using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public class Reward_runes : RewardItemBase
	{
		public Reward_runes() : base(ERewardType.Reward_runes)
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
						base.IsAtlas = true;
						base.SIconAtlas = "Texture/Runes/RuneIconsAtlas";
						base.SIcon = dataById.icon;
						base.BIcon = dataById.icon;
						base.BIconAtlas = "Texture/Runes/RunIconAtlas";
						base.Des = LanguageManager.Instance.GetStringById(dataById.name);
						base.Quality = dataById.quality;
						base.TypeDes = LanguageManager.Instance.GetStringById("Currency_Rune");
						base.Valid = true;
					}
				}
			}
		}
	}
}
