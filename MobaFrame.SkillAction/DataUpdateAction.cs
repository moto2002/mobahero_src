using System;

namespace MobaFrame.SkillAction
{
	public class DataUpdateAction : BaseDataAction
	{
		protected override void OnSendStart()
		{
		}

		protected override void OnRecordStart()
		{
		}

		protected override bool doAction()
		{
			if (this.dataUpdateInfo == null || null == base.unit || base.unit.data == null)
			{
				return false;
			}
			base.unit.data.SetAttrVal(this.dataUpdateInfo);
			return true;
		}
	}
}
