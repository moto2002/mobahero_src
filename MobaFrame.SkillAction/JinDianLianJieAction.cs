using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class JinDianLianJieAction : FaLisheQuAction
	{
		protected override bool CheckTargetsExist()
		{
			return !(this.target == null) && this.target.isLive && Vector3.Distance(this.target.mTransform.position, base.unit.mTransform.position) <= this.skillData.config.distance * 3f;
		}
	}
}
