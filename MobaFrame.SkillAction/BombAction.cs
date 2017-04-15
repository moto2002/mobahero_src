using MobaTools.Prefab;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BombAction : BasePerformAction
	{
		public Units targetUnit;

		public Vector3? targetPosition;

		protected BombType bombType;

		protected BombFollowType bombFollowType;

		protected ColliderType colliderType;

		protected BoneAnchorType anchorType;

		protected override bool doAction()
		{
			base.CreateNode(NodeType.SkillNode, this.performId);
			this.bombFollowType = (BombFollowType)this.data.effectParam1;
			this.colliderType = (ColliderType)this.data.effectParam2;
			this.SetPostion();
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			if (StringUtils.CheckValid(this.performId))
			{
				PlayEffectAction action = ActionManager.PlayEffect(this.performId, base.unit, null, null, true, string.Empty, null);
				base.AttachSubAction(action);
			}
			if (this.useCollider)
			{
				List<Units> list = new List<Units>();
				list.Add(this.targetUnit);
			}
			return true;
		}

		protected void SetPostion()
		{
			BombFollowType bombFollowType = this.bombFollowType;
			if (bombFollowType != BombFollowType.FollowTarget)
			{
				if (bombFollowType == BombFollowType.FixPosition)
				{
					Transform arg_2E_0 = base.transform;
					Vector3? vector = this.targetPosition;
					arg_2E_0.position = vector.Value;
				}
			}
			else
			{
				base.transform.parent = this.targetUnit.mTransform;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.Euler(Vector3.zero);
			}
		}

		protected Vector3? GetTargetPosition()
		{
			if (this.targetUnit != null)
			{
				return new Vector3?(this.targetUnit.GetCenter());
			}
			Vector3? vector = this.targetPosition;
			if (vector.HasValue)
			{
				Vector3? vector2 = this.targetPosition;
				return new Vector3?(vector2.Value);
			}
			return null;
		}

		protected bool CheckTargetExist()
		{
			if (this.targetUnit != null)
			{
				return this.targetUnit.isLive;
			}
			Vector3? vector = this.targetPosition;
			return vector.HasValue;
		}
	}
}
