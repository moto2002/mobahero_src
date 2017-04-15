using MobaTools.Prefab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class LinkAction : BasePerformAction
	{
		public Units CasterUnit;

		public List<Units> targetUnits;

		public Vector3? targetPosition;

		protected float effect_time;

		protected float cur_time;

		protected List<Units> mLinkTargets;

		protected LinkType mLinkType;

		protected BoneAnchorType mBoneType;

		protected PlayEffectAction effectAction;

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

		protected override void OnStop()
		{
			this.isActive = false;
			base.OnStop();
		}

		protected override void OnDestroy()
		{
			this.cur_time = 0f;
			this.effect_time = 0f;
			base.OnDestroy();
		}

		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.mLinkType = (LinkType)this.data.effectParam1;
			this.mBoneType = (BoneAnchorType)this.data.effectParam2;
			this.effect_time = this.data.config.effect_time + this.data.config.effect_delay;
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			string arg_A5_0 = this.performId;
			Units arg_A5_1 = base.unit;
			Vector3? vector = this.targetPosition;
			this.effectAction = ActionManager.PlayEffect(arg_A5_0, arg_A5_1, new Vector3?(vector.Value), null, true, string.Empty, this.CasterUnit);
			this.AddAction(this.effectAction);
			if (this.useCollider)
			{
			}
			this.isActive = true;
			base.mCoroutineManager.StartCoroutine(this.Link_Coroutine(this.targetUnits), true);
			return true;
		}

		[DebuggerHidden]
		public virtual IEnumerator Link_Coroutine(List<Units> linkTargets)
		{
			LinkAction.<Link_Coroutine>c__Iterator5B <Link_Coroutine>c__Iterator5B = new LinkAction.<Link_Coroutine>c__Iterator5B();
			<Link_Coroutine>c__Iterator5B.linkTargets = linkTargets;
			<Link_Coroutine>c__Iterator5B.<$>linkTargets = linkTargets;
			<Link_Coroutine>c__Iterator5B.<>f__this = this;
			return <Link_Coroutine>c__Iterator5B;
		}

		public virtual void SetLinkTargets(LineRenderer lineRenderer, List<Units> targets)
		{
			if (targets == null)
			{
				return;
			}
			if (lineRenderer == null)
			{
				return;
			}
			lineRenderer.SetVertexCount(targets.Count + 1);
			if (this.mLinkType == LinkType.LinkWithTargetAndSelf)
			{
				lineRenderer.SetPosition(0, base.unit.GetBonePos(this.mBoneType) + new Vector3(this.data.offset_x, this.data.offset_y, this.data.offset_z));
			}
			else
			{
				lineRenderer.SetPosition(0, targets[0].GetCenter());
			}
			for (int i = 0; i < targets.Count; i++)
			{
				if (targets[i] != null)
				{
					lineRenderer.SetPosition(i + 1, targets[i].GetCenter());
				}
			}
		}
	}
}
