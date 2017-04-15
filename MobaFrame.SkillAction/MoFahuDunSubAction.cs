using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class MoFahuDunSubAction : BaseStateAction
	{
		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.MoFaHuDun.IsInState;
			}
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.MoFaHuDun.Add();
			if (StringUtils.CheckValid(this.data.strParam1))
			{
				ActionManager.AddHighEffect(this.data.strParam1, this.skillId, this.targetUnit, this.targetUnit, null, true);
			}
			GlobalObject.Instance.StartCoroutine(this.UpdateCoroutine());
			base.PlayEffects(this.targetUnit);
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.moFaHuDunCoverProportion = 0f;
			if (StringUtils.CheckValid(this.data.strParam1))
			{
				this.targetUnit.highEffManager.RemoveHighEffect(this.data.strParam1);
			}
			this.targetUnit.MoFaHuDun.Remove();
			GlobalObject.Instance.StopCoroutine(this.UpdateCoroutine());
		}

		[DebuggerHidden]
		private IEnumerator UpdateCoroutine()
		{
			MoFahuDunSubAction.<UpdateCoroutine>c__Iterator81 <UpdateCoroutine>c__Iterator = new MoFahuDunSubAction.<UpdateCoroutine>c__Iterator81();
			<UpdateCoroutine>c__Iterator.<>f__this = this;
			return <UpdateCoroutine>c__Iterator;
		}
	}
}
