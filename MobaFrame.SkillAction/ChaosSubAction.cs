using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class ChaosSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.HunLuan.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.HunLuan.Add();
			this.targetUnit.SetCanRotate(false);
			base.EnableAction(this.targetUnit, false, this.data.param1);
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.SetCanRotate(true);
			base.EnableAction(this.targetUnit, true, this.data.param1);
			this.targetUnit.HunLuan.Remove();
			if (this.targetUnit.isPlayer)
			{
				this.targetUnit.skillManager.UpdateSkillUI();
			}
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			ChaosSubAction.<Coroutine>c__Iterator7A <Coroutine>c__Iterator7A = new ChaosSubAction.<Coroutine>c__Iterator7A();
			<Coroutine>c__Iterator7A.<>f__this = this;
			return <Coroutine>c__Iterator7A;
		}
	}
}
