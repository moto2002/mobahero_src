using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PassiveLinkAction : SputteringLinkAction
	{
		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			PassiveLinkAction.<Missile_Coroutine>c__Iterator60 <Missile_Coroutine>c__Iterator = new PassiveLinkAction.<Missile_Coroutine>c__Iterator60();
			<Missile_Coroutine>c__Iterator.<>f__this = this;
			return <Missile_Coroutine>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator doLinkJianShe()
		{
			PassiveLinkAction.<doLinkJianShe>c__Iterator61 <doLinkJianShe>c__Iterator = new PassiveLinkAction.<doLinkJianShe>c__Iterator61();
			<doLinkJianShe>c__Iterator.<>f__this = this;
			return <doLinkJianShe>c__Iterator;
		}

		public virtual void LinkTargets(LineRenderer lineRenderer)
		{
			for (int i = 0; i < this.targetUnits.Count; i++)
			{
				lineRenderer.SetPosition(i, this.targetUnits[i].GetCenter());
			}
		}

		protected new bool IsBack()
		{
			return this.targetUnits.Count <= 0 || (this.targetUnits != null && this.curTargetIndex >= this.targetUnits.Count);
		}

		protected new Units GetPreTarget()
		{
			if (this.curTargetIndex + 1 >= this.targetUnits.Count)
			{
				return null;
			}
			if (this.targetUnits != null)
			{
				return this.targetUnits[this.curTargetIndex + 1];
			}
			return null;
		}

		protected override Units GetCurTarget()
		{
			if (this.curTargetIndex >= this.targetUnits.Count)
			{
				return null;
			}
			return this.targetUnits[this.curTargetIndex];
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.highEffAction != null)
			{
				this.highEffAction.Destroy();
			}
		}
	}
}
