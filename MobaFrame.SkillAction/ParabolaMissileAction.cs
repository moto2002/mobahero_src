using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ParabolaMissileAction : MissileAction
	{
		public float startDistanceToTarget;

		protected float startAngle = 60f;

		protected float minDistance;

		protected override void OnInit()
		{
			base.OnInit();
			this.minDistance = this.data.effectParam2;
		}

		protected override void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			this.startAngle = this.data.effectParam1;
			base.transform.LookAt(targetPos);
			float num = Mathf.Min(1f, Vector3.Distance(base.transform.position, targetPos) / Mathf.Clamp(this.startDistanceToTarget, 0.01f, 3.40282347E+38f)) * this.startAngle;
			Quaternion rotation = base.transform.rotation * Quaternion.Euler(Mathf.Clamp(-num, -num, num), 0f, 0f);
			if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z))
			{
				base.transform.rotation = rotation;
			}
			currentDistance = Vector3.Distance(base.transform.position, targetPos);
			base.transform.Translate(Vector3.forward * Mathf.Min(moveSpeedDelta, currentDistance));
			currentDistance = Vector3.Distance(base.transform.position, targetPos);
		}

		protected override float MinDistance(float moveSpeedDelta)
		{
			if (this.minDistance == 0f)
			{
				return base.MinDistance(moveSpeedDelta);
			}
			return this.minDistance;
		}

		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			ParabolaMissileAction.<Missile_Coroutine>c__Iterator57 <Missile_Coroutine>c__Iterator = new ParabolaMissileAction.<Missile_Coroutine>c__Iterator57();
			<Missile_Coroutine>c__Iterator.<>f__this = this;
			return <Missile_Coroutine>c__Iterator;
		}
	}
}
