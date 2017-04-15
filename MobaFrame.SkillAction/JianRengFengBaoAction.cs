using MobaHeros;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class JianRengFengBaoAction : BaseSkillAction
	{
		public float duration = 5f;

		protected float distance_now;

		protected Vector3 targetDeathPos;

		protected Quaternion daoDanRot;

		protected Vector3? skillPosition;

		protected Vector3 startPos;

		protected float speed = 2f;

		protected new bool isActive = true;

		protected float curTime;

		protected Task actionTask;

		protected PerformAction mStartPerform;

		protected EffectDataType dataType;

		protected override void OnInit()
		{
			base.OnInit();
			this.startPos = base.unit.transform.position;
			this.duration = this.skillData.guideTime;
			this.dataType.GainType = EffectGainType.all;
			this.dataType.MagicType = EffectMagicType.all;
			this.dataType.ImmuneType = EffectImmuneType.igronImmune;
		}

		[DebuggerHidden]
		protected IEnumerator Coroutine()
		{
			JianRengFengBaoAction.<Coroutine>c__Iterator65 <Coroutine>c__Iterator = new JianRengFengBaoAction.<Coroutine>c__Iterator65();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.doLastPerform(false);
			this.isActive = false;
			if (!base.unit.isLive)
			{
				base.unit.ForceDeath();
			}
		}

		protected override bool doAction()
		{
			if (this.isActive)
			{
				base.unit.SetCanAttack(false);
				base.unit.SetCanAIControl(false);
				base.unit.SetForceClickGroundState(true);
				base.unit.ChangeLayer("ChongZhuang");
				if (this.skillData.start_actions.Length > 0)
				{
					this.mStartPerform = ActionManager.PlayPerform(this.skillKey, this.skillData.start_actions[0], base.unit, null, null, true, null);
					this.mStartPerform.OnDamageCallback = new Callback<BaseAction, List<Units>>(this.OnDamage);
					this.mStartPerform.OnDamageEndCallback = new Callback<BaseAction>(this.OnDamageEnd);
					this.AddAction(this.mStartPerform);
				}
			}
			base.mCoroutineManager.StopCoroutine(this.actionTask);
			this.actionTask = base.mCoroutineManager.StartCoroutine(this.StartMoveControl(), true);
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		private IEnumerator StartMoveControl()
		{
			JianRengFengBaoAction.<StartMoveControl>c__Iterator66 <StartMoveControl>c__Iterator = new JianRengFengBaoAction.<StartMoveControl>c__Iterator66();
			<StartMoveControl>c__Iterator.<>f__this = this;
			return <StartMoveControl>c__Iterator;
		}

		private void StartMoveControlQuick()
		{
			base.unit.SetLockAnimState(true);
			base.unit.SetCanMove(true);
		}

		public void doLastPerform(bool isAction)
		{
			base.unit.SetLockAnimState(false);
			base.unit.SetCanMove(true);
			base.unit.RevertLayer();
			base.unit.SetCanAttack(true);
			base.unit.SetCanAIControl(true);
			base.unit.SetForceClickGroundState(false);
			base.DestroyActions();
			this.mStartPerform.Destroy();
			if (isAction && this.skillData.start_actions.Length > 1)
			{
				this.AddAction(ActionManager.PlayPerformMustntLive(this.skillKey, this.skillData.start_actions[1], base.unit, null, null, true, null, null));
			}
			if (!this.skillData.isCanMoveInSkillCastin)
			{
				base.unit.ForceIdle();
			}
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
