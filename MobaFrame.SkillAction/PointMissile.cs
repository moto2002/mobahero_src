using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PointMissile : ParabolaMissileAction
	{
		private float curTime;

		protected override bool doAction()
		{
			bool result = base.doAction();
			this.startDistanceToTarget = Vector3.Distance(this.targetPosition.Value, base.transform.position);
			return result;
		}

		protected override Vector3? GetTargetPosition()
		{
			Vector3? targetPosition = this.targetPosition;
			if (targetPosition.HasValue)
			{
				return this.targetPosition;
			}
			return null;
		}

		protected override void OnHit(BaseAction action, Units target, int count = 1)
		{
			base.OnHit(action, target, count);
			ActionManager.PlayEffect(this.data.effectStrParam1, base.unit, this.targetPosition, null, true, string.Empty, null);
		}

		protected override void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			this.curTime += moveSpeedDelta;
			this.startAngle = this.data.effectParam1;
			float num = Vector3.Distance(base.transform.position, this.targetPosition.Value);
			if (this.curTime < this.data.effectParam2)
			{
				base.transform.LookAt(this.targetPosition.Value);
				float num2 = Mathf.Min(1f, num / Mathf.Clamp(this.startDistanceToTarget, 0.01f, 3.40282347E+38f)) * this.startAngle;
				Quaternion rotation = base.transform.rotation * Quaternion.Euler(Mathf.Clamp(-num2, -num2, num2), 0f, 0f);
				if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z))
				{
					base.transform.rotation = rotation;
				}
				currentDistance = Vector3.Distance(base.transform.position, this.targetPosition.Value);
				base.transform.Translate(Vector3.forward * Mathf.Min(moveSpeedDelta, currentDistance));
				currentDistance = Vector3.Distance(base.transform.position, this.targetPosition.Value);
			}
			else
			{
				Vector3 vector = targetPos - base.transform.position;
				if (vector != Vector3.zero)
				{
					this.daoDanRot = Quaternion.LookRotation(vector);
					base.transform.rotation = this.daoDanRot;
				}
				base.transform.Translate(Vector3.forward * moveSpeedDelta);
				currentDistance = Vector3.Distance(targetPos, base.transform.position);
			}
		}
	}
}
