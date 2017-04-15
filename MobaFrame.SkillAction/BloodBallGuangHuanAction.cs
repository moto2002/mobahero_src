using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BloodBallGuangHuanAction : GuangHuanAction
	{
		protected override void StartHighEff()
		{
			base.CreateNode(NodeType.SkillNode, this.higheffId);
			base.gameObject.transform.parent = base.unit.transform;
			base.gameObject.transform.localPosition = Vector3.zero;
			base.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
			BloodBallGuangHuanBehavior bloodBallGuangHuanBehavior = base.gameObject.GetComponent<BloodBallGuangHuanBehavior>();
			if (bloodBallGuangHuanBehavior == null)
			{
				bloodBallGuangHuanBehavior = base.gameObject.AddComponent<BloodBallGuangHuanBehavior>();
			}
			bloodBallGuangHuanBehavior.Init(this.higheffId, this.skillId, base.unit);
			bloodBallGuangHuanBehavior.OnGuangHuanEnd = new Callback(base.OnGuangHuanEnd);
		}
	}
}
