using MobaTools.Prefab;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class FenLieJianAction : BasePerformAction
	{
		public List<Units> targetUnits;

		public Vector3? targetPosition;

		protected BombType bombType;

		protected ColliderType colliderType;

		protected BoneAnchorType anchorType;

		protected override bool doAction()
		{
			for (int i = 0; i < this.targetUnits.Count; i++)
			{
				if (this.targetUnits[i] != null && this.targetUnits[i].isLive)
				{
					float distanceToTarget = Vector3.Distance(this.targetUnits[i].mTransform.position, base.unit.mTransform.position);
					ParabolaMissileAction parabolaMissileAction = ActionManager.ParabolaMissile(this.skillKey, this.performId, base.unit, distanceToTarget, this.targetUnits[i], this.targetPosition);
					parabolaMissileAction.OnDamageCallback = new Callback<BaseAction, List<Units>>(this.OnDamage);
					this.AddAction(parabolaMissileAction);
				}
			}
			return true;
		}
	}
}
