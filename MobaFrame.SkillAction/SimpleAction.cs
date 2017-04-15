using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class SimpleAction : BasePerformAction
	{
		public Units CasterUnit;

		public Vector3? targetPosition;

		protected override bool doAction()
		{
			this.AddAction(ActionManager.PlayAnim(this.performId, base.unit, true));
			if (StringUtils.CheckValid(this.performId))
			{
				PlayEffectAction action = ActionManager.PlayEffect(this.performId, base.unit, this.targetPosition, new Vector3?(base.unit.mTransform.rotation.eulerAngles), true, this.skillKey.SkillID, this.CasterUnit);
				this.AddAction(action);
			}
			return true;
		}
	}
}
