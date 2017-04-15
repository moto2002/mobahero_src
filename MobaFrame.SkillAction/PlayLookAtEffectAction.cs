using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PlayLookAtEffectAction : PlayEffectAction
	{
		public Vector3 forward = Vector3.zero;

		protected override void UpdatePosition()
		{
			base.UpdatePosition();
			if (this.forward != Vector3.zero)
			{
				base.transform.LookAt(base.transform.position + this.forward, Vector3.up);
			}
		}
	}
}
