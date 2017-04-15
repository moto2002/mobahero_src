using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ShanShuoAction : BaseSkillAction
	{
		public new Vector3? targetPosition;

		protected override bool doAction()
		{
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		protected virtual IEnumerator Coroutine()
		{
			ShanShuoAction.<Coroutine>c__Iterator67 <Coroutine>c__Iterator = new ShanShuoAction.<Coroutine>c__Iterator67();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}
	}
}
