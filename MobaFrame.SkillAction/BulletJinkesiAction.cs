using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class BulletJinkesiAction : BulletAction
	{
		private float normalDistance = 0.5f;

		private float curDistance;

		private bool isAddSpeed;

		private TwoParticle setlv2;

		protected override void SetPosition()
		{
			base.SetPosition();
		}

		protected override void MoveDelta(Vector3 startDirection, float angle, float moveDelta)
		{
			if (!this.isAddSpeed)
			{
				if (this.curDistance < this.normalDistance)
				{
					this.curDistance += Time.deltaTime;
				}
				else
				{
					this.isAddSpeed = true;
					this.moveSpeed += 20f;
				}
			}
			base.MoveDelta(startDirection, angle, moveDelta);
		}
	}
}
