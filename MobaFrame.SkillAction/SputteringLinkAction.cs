using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class SputteringLinkAction : SputteringMissileAction
	{
		protected float speed = 0.5f;

		protected LineRenderer[] lineRenderer;

		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			SputteringLinkAction.<Missile_Coroutine>c__Iterator5E <Missile_Coroutine>c__Iterator5E = new SputteringLinkAction.<Missile_Coroutine>c__Iterator5E();
			<Missile_Coroutine>c__Iterator5E.<>f__this = this;
			return <Missile_Coroutine>c__Iterator5E;
		}

		[DebuggerHidden]
		private IEnumerator doLinkJianShe()
		{
			SputteringLinkAction.<doLinkJianShe>c__Iterator5F <doLinkJianShe>c__Iterator5F = new SputteringLinkAction.<doLinkJianShe>c__Iterator5F();
			<doLinkJianShe>c__Iterator5F.<>f__this = this;
			return <doLinkJianShe>c__Iterator5F;
		}

		public virtual void LinkTargets(LineRenderer lineRenderer, Units start, Units end)
		{
			if (start == null)
			{
				return;
			}
			if (end == null)
			{
				return;
			}
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, start.GetCenter());
			lineRenderer.SetPosition(1, end.GetCenter());
		}

		protected void OnBack()
		{
			this.Destroy();
		}

		protected bool IsBack()
		{
			return this.targetUnits.Count <= 0 || (this.targetUnits != null && this.curTargetIndex >= this.targetUnits.Count);
		}

		protected Units GetPreTarget()
		{
			if (this.targetUnits == null)
			{
				return null;
			}
			if (this.curTargetIndex == 0)
			{
				return this.targetUnits[this.targetUnits.Count - 1];
			}
			return this.targetUnits[this.curTargetIndex - 1];
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
