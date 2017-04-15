using Com.Game.Utils;
using System;

namespace Com.Game.Module
{
	public class RewardItemBase
	{
		public bool Valid
		{
			get;
			set;
		}

		public int Num
		{
			get;
			set;
		}

		public string BIcon
		{
			get;
			set;
		}

		public string BIconAtlas
		{
			get;
			set;
		}

		public string SIcon
		{
			get;
			set;
		}

		public string Des
		{
			get;
			set;
		}

		public string TypeDes
		{
			get;
			set;
		}

		public int Quality
		{
			get;
			set;
		}

		public bool IsAtlas
		{
			get;
			set;
		}

		public string SIconAtlas
		{
			get;
			set;
		}

		public ERewardType RewardType
		{
			get;
			set;
		}

		public bool Discount
		{
			get;
			set;
		}

		public object ExtraData
		{
			get;
			set;
		}

		public RewardItemBase(ERewardType type)
		{
			this.RewardType = type;
		}

		public virtual void Init(string[] param)
		{
			this.Valid = false;
			if (param.Length != 3)
			{
				ClientLogger.Error("Reward_currency:配置错误");
			}
			else
			{
				this.IsAtlas = false;
				this.Valid = true;
			}
		}
	}
}
