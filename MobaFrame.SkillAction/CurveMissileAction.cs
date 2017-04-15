using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class CurveMissileAction : ParabolaMissileAction
	{
		private int type;

		private float curtime;

		private float randTime = 0.1f;

		private float maxRandCount = 2.14748365E+09f;

		private float curRandCount;

		protected override bool doAction()
		{
			this.doRandom();
			return base.doAction();
		}

		private void doRandom()
		{
			if (this.curRandCount > this.maxRandCount)
			{
				return;
			}
			if (MathUtils.Rand(20f))
			{
				this.type = 0;
			}
			else if (MathUtils.Rand(30f))
			{
				this.type = 1;
			}
			else if (MathUtils.Rand(30f))
			{
				this.type = 2;
			}
			else
			{
				this.type = -1;
			}
			this.curRandCount += 1f;
		}

		protected override void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			this.startAngle = this.data.effectParam1;
			this.randTime = this.data.effectParam2;
			this.maxRandCount = this.data.effectParam3;
			if (this.curtime > this.randTime)
			{
				this.curtime = 0f;
				this.doRandom();
			}
			this.curtime += Time.deltaTime;
			float num = Vector3.Distance(base.transform.position, targetPos);
			base.transform.LookAt(targetPos);
			float num2 = Mathf.Min(1f, num / Mathf.Clamp(this.startDistanceToTarget, 0.01f, 3.40282347E+38f)) * this.startAngle;
			float num3 = Mathf.Clamp(-num2, -num2, num2);
			Quaternion rotation;
			if (this.type == 0)
			{
				rotation = base.transform.rotation * Quaternion.Euler(num3, 0f, 0f);
			}
			else if (this.type == 1)
			{
				rotation = base.transform.rotation * Quaternion.Euler(0f, num3, 0f);
			}
			else if (this.type == 2)
			{
				rotation = base.transform.rotation * Quaternion.Euler(0f, -num3, 0f);
			}
			else
			{
				rotation = base.transform.rotation * Quaternion.Euler(0f, 0f, num3);
			}
			if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z))
			{
				base.transform.rotation = rotation;
			}
			currentDistance = Vector3.Distance(base.transform.position, targetPos);
			base.transform.Translate(Vector3.forward * Mathf.Min(moveSpeedDelta, currentDistance));
			currentDistance = Vector3.Distance(base.transform.position, targetPos);
		}
	}
}
