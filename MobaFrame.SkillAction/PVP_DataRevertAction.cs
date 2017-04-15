using MobaHeros;
using System;

namespace MobaFrame.SkillAction
{
	public class PVP_DataRevertAction : BaseDataAction
	{
		public float addHp;

		public float addMp;

		protected override void OnRecordStart()
		{
		}

		protected override void OnSendStart()
		{
		}

		protected override bool doAction()
		{
			if (base.unit == null)
			{
				return false;
			}
			base.unit.ChangeAttr(AttrType.Hp, OpType.Add, this.addHp);
			base.unit.ChangeAttr(AttrType.Mp, OpType.Add, this.addMp);
			return true;
		}
	}
}
