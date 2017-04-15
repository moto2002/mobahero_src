using Com.Game.Module;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class WaltzAction : BaseSkillAction
	{
		private Vector3 sourcePos;

		private Quaternion sourceRot;

		protected override bool doAction()
		{
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		protected IEnumerator Coroutine()
		{
			WaltzAction.<Coroutine>c__Iterator6B <Coroutine>c__Iterator6B = new WaltzAction.<Coroutine>c__Iterator6B();
			<Coroutine>c__Iterator6B.<>f__this = this;
			return <Coroutine>c__Iterator6B;
		}

		private void DoDestroy()
		{
			base.unit.PlayAnim(AnimationType.Move, false, 0, true, false);
			base.unit.EnableAllRenders(true);
			base.unit.EnableAction(true);
			base.unit.RevertLayer();
			if (base.unit.isPlayer)
			{
				Singleton<SkillView>.Instance.CheckIconToGrayByCanUseAll(null);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.OnActionEnd();
			this.DoDestroy();
		}

		protected override void OnSkillDamage(BaseSkillAction action, List<Units> targets)
		{
			this.AddAction(ActionManager.HitSkill(action.skillKey, base.unit, targets, true));
			base.AddHighEff(action.skillKey, SkillPhrase.Hit, targets, this.targetPosition);
			base.AddBuff(action.skillKey, SkillPhrase.Hit, targets);
			base.OnSkillDamage(action, targets);
		}

		protected override void OnSkillEnd(BaseSkillAction action)
		{
			ActionManager.EndSkill(action.skillKey, base.unit, true);
			base.OnSkillEnd(action);
		}
	}
}
