using System;

namespace MobaFrame.SkillAction
{
	public class SubPerformReplaceAction : BaseHighEffAction
	{
		public Units targetUnit;

		protected override void StartHighEff()
		{
			if (this.targetUnit == null)
			{
				return;
			}
			if (this.data.param2 != 0f)
			{
				if (this.data.param2 != 3f)
				{
					if (this.data.param2 == 4f)
					{
						if (this.data.strParam2 == string.Empty)
						{
							this.Destroy();
							return;
						}
						if (!this.targetUnit.buffManager.IsHaveBuff(this.data.strParam2))
						{
							return;
						}
					}
				}
			}
			if (this.data.param1 == 1f)
			{
				this.targetUnit.StartActions = StringUtils.GetStringValue(this.data.strParam1, ',');
			}
			else if (this.data.param1 == 2f)
			{
				this.targetUnit.HitActions = StringUtils.GetStringValue(this.data.strParam1, ',');
			}
			else if (this.data.param1 == 3f)
			{
				this.targetUnit.ReadyActions = StringUtils.GetStringValue(this.data.strParam1, ',');
			}
			this.Destroy();
		}

		protected override void StopHighEff()
		{
		}
	}
}
