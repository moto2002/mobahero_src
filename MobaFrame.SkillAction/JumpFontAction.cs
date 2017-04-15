using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;

namespace MobaFrame.SkillAction
{
	public class JumpFontAction : BaseAction
	{
		public JumpFontInfo jumpFontInfo;

		protected override void OnSendStart()
		{
			this.jumpFontInfo.unitId = base.unit.unique_id;
			PvpEvent.SendJumFontEvent(this.jumpFontInfo);
		}

		protected override bool doAction()
		{
			base.unit.jumpFont((JumpFontType)this.jumpFontInfo.type, this.jumpFontInfo.text, null, true);
			return true;
		}
	}
}
