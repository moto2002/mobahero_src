using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class GuangHuanAction : BaseHighEffAction
	{
		protected override void StartHighEff()
		{
			base.CreateNode(NodeType.SkillNode, this.higheffId);
			base.gameObject.transform.parent = base.unit.transform;
			base.gameObject.transform.localPosition = Vector3.zero;
			base.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
			GuangHuanBehavior guangHuanBehavior = base.gameObject.GetComponent<GuangHuanBehavior>();
			if (guangHuanBehavior == null)
			{
				guangHuanBehavior = base.gameObject.AddComponent<GuangHuanBehavior>();
			}
			guangHuanBehavior.Init(this.higheffId, this.skillId, base.unit);
			guangHuanBehavior.OnGuangHuanEnd = new Callback(this.OnGuangHuanEnd);
		}

		protected void OnGuangHuanEnd()
		{
			this.Destroy();
		}
	}
}
