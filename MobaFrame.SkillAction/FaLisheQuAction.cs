using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class FaLisheQuAction : SimpleSkillAction
	{
		protected Units target;

		protected override bool doAction()
		{
			base.doAction();
			this.SetTarget();
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
			return true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.mCoroutineManager.StopAllCoroutine();
		}

		[DebuggerHidden]
		protected virtual IEnumerator Coroutine()
		{
			FaLisheQuAction.<Coroutine>c__Iterator62 <Coroutine>c__Iterator = new FaLisheQuAction.<Coroutine>c__Iterator62();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}

		protected virtual bool CheckTargetsExist()
		{
			return !(this.target == null) && this.target.isLive && Vector3.Distance(this.target.mTransform.position, base.unit.mTransform.position) <= this.skillData.config.distance + 2.5f;
		}

		private void SetTarget()
		{
			if (this.targetUnits == null)
			{
				this.target = null;
				return;
			}
			if (this.targetUnits.Count == 0)
			{
				this.target = null;
				return;
			}
			this.target = this.targetUnits[0];
		}
	}
}
