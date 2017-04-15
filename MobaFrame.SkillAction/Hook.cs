using MobaTools.Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class Hook : BasePerformAction
	{
		public Units CasterUnit;

		public Units hookedTargetUnit;

		public Units targetUnits;

		public Vector3? targetPosition;

		protected PlayEffectAction effectAction;

		private bool isForward = true;

		protected BoneAnchorType mBoneType;

		protected float flyDistance = 8f;

		private Vector3 sourcePosition;

		private bool isHookMan;

		private Transform cacheTran;

		private PlayEffectAction HookObj;

		private Skill skill;

		private float minSpeed;

		private float speedAttention = 0.01f;

		public Callback Callback_OnHookBack;

		private float hookBackDelay;

		protected LineRenderer[] mLineRenders
		{
			get
			{
				if (this.effectAction != null && this.effectAction.mEffectPlayer != null)
				{
					return this.effectAction.mEffectPlayer.mLineRender;
				}
				return null;
			}
		}

		public Vector3 prePosition
		{
			get;
			set;
		}

		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			string arg_5A_0 = this.performId;
			Units arg_5A_1 = base.unit;
			Vector3? vector = this.targetPosition;
			this.effectAction = ActionManager.PlayEffect(arg_5A_0, arg_5A_1, new Vector3?(vector.Value), null, true, string.Empty, this.CasterUnit);
			string arg_98_0 = "TufuS1_1";
			Units arg_98_1 = base.unit;
			Vector3? vector2 = this.targetPosition;
			this.HookObj = ActionManager.PlayEffect(arg_98_0, arg_98_1, new Vector3?(vector2.Value), null, true, string.Empty, this.CasterUnit);
			this.SetPosition();
			this.sourcePosition = base.unit.trans.position;
			this.cacheTran = base.transform;
			this.AddAction(this.effectAction);
			this.AddAction(this.HookObj);
			this.ResetLineRender();
			this.isActive = true;
			this.isHookMan = false;
			this.isForward = true;
			this.skill = base.unit.getSkillOrAttackById(this.skillData.skillId);
			Skill expr_11F = this.skill;
			expr_11F.OnTargetHitCallback = (Callback<List<Units>>)Delegate.Combine(expr_11F.OnTargetHitCallback, new Callback<List<Units>>(this.OnTargetHit));
			this.mBoneType = (BoneAnchorType)this.data.effectParam1;
			this.flyDistance = this.data.effectParam2;
			this.minSpeed = this.data.config.effect_speed / 2f;
			Skill expr_185 = this.skill;
			expr_185.DestroyAction = (Callback<string, int>)Delegate.Combine(expr_185.DestroyAction, new Callback<string, int>(this.ExternDestroy));
			this.hookBackDelay = 0f;
			base.mCoroutineManager.StartCoroutine(this.Link_Coroutine(), true);
			return true;
		}

		protected override void OnDestroy()
		{
			if (this.skillData != null)
			{
				Skill skillOrAttackById = base.unit.getSkillOrAttackById(this.skillData.skillId);
				if (skillOrAttackById != null && skillOrAttackById.OnTargetHitCallback != null)
				{
					Skill expr_34 = skillOrAttackById;
					expr_34.OnTargetHitCallback = (Callback<List<Units>>)Delegate.Remove(expr_34.OnTargetHitCallback, new Callback<List<Units>>(this.OnTargetHit));
				}
			}
			base.OnDestroy();
		}

		protected void OnPvpHit(string eventPerformId)
		{
			if (this.performId == eventPerformId)
			{
			}
		}

		protected void ExternDestroy(string eventPerformId, int bulletIndex)
		{
			if (this.performId == eventPerformId)
			{
				if (this.Callback_OnDestroy != null)
				{
					this.Callback_OnDestroy();
				}
				this.Destroy();
			}
		}

		protected void OnTargetHit(List<Units> targets)
		{
			if (targets != null && targets.Count > 0)
			{
				this.hookedTargetUnit = targets[0];
			}
			this.isForward = false;
			this.isHookMan = true;
		}

		[DebuggerHidden]
		public virtual IEnumerator Link_Coroutine()
		{
			Hook.<Link_Coroutine>c__Iterator5A <Link_Coroutine>c__Iterator5A = new Hook.<Link_Coroutine>c__Iterator5A();
			<Link_Coroutine>c__Iterator5A.<>f__this = this;
			return <Link_Coroutine>c__Iterator5A;
		}

		public override void Destroy()
		{
			base.Destroy();
			this.ResetLineRender();
			if (this.HookObj != null)
			{
				this.HookObj.Destroy();
			}
			if (this.effectAction != null)
			{
				this.effectAction.Destroy();
			}
		}

		private void ResetLineRender()
		{
			if (this.mLineRenders == null)
			{
				return;
			}
			for (int i = 0; i < this.mLineRenders.Length; i++)
			{
				this.mLineRenders[i].SetVertexCount(0);
			}
		}

		protected virtual void MoveDelta(Vector3 startDirection, float angle, float moveDelta)
		{
			this.prePosition = this.cacheTran.position;
			this.cacheTran.rotation = Quaternion.LookRotation(startDirection) * Quaternion.AngleAxis(angle, Vector3.up);
			this.cacheTran.position += base.transform.forward * moveDelta;
		}

		protected virtual void SetPosition()
		{
			if (base.transform != null && this.effectAction != null && this.HookObj != null)
			{
				base.transform.position = base.transform.position + this.effectAction.transform.position;
				base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles + this.effectAction.transform.rotation.eulerAngles);
				this.HookObj.transform.rotation = base.transform.rotation;
			}
		}

		public virtual void SetLinkTargets(LineRenderer[] lineRenderers, Units target, Vector3 targetPos)
		{
			if (lineRenderers == null)
			{
				return;
			}
			if (this.HookObj != null)
			{
				this.HookObj.transform.position = targetPos - (targetPos - this.sourcePosition).normalized * 1f + Vector3.up;
			}
			for (int i = 0; i < lineRenderers.Length; i++)
			{
				LineRenderer lineRenderer = lineRenderers[i];
				if (lineRenderer == null)
				{
					return;
				}
				lineRenderer.SetVertexCount(2);
				if (this.isForward)
				{
					lineRenderer.SetPosition(0, base.unit.GetBonePos(this.mBoneType));
				}
				else if (this.isHookMan && base.unit.trans.position != this.sourcePosition)
				{
					lineRenderer.SetPosition(0, this.sourcePosition);
				}
				else
				{
					lineRenderer.SetPosition(0, base.unit.GetBonePos(this.mBoneType));
				}
				lineRenderer.SetPosition(1, this.HookObj.transform.position);
			}
		}

		protected Vector3? GetTargetPosition()
		{
			if (this.targetUnits != null)
			{
				return new Vector3?(this.targetUnits.GetCenter());
			}
			Vector3? vector = this.targetPosition;
			if (vector.HasValue)
			{
				Vector3? vector2 = this.targetPosition;
				return new Vector3?(vector2.Value);
			}
			return null;
		}
	}
}
