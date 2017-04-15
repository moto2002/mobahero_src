using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class TowSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.QianYin.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.QianYin.Add();
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.QianYin.Remove();
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			TowSubAction.<Coroutine>c__Iterator89 <Coroutine>c__Iterator = new TowSubAction.<Coroutine>c__Iterator89();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}
	}
}
