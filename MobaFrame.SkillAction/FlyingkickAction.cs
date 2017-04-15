using Com.Game.Module;
using MobaHeros.Pvp;
using MobaTools.Move;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class FlyingkickAction : BaseHighEffAction
	{
		protected bool isMoveTarget;

		protected Quaternion daoDanRot;

		protected Vector3 targetDeathPos;

		protected bool isReachedTarget;

		protected float speed;

		protected Vector3 sourceCenter;

		protected override void OnInit()
		{
			base.OnInit();
			this.speed = this.data.param1;
		}

		protected override void StartHighEff()
		{
			base.StartHighEff();
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
		}

		protected override void StopHighEff()
		{
			base.mCoroutineManager.StopAllCoroutine();
			base.unit.ForceIdle();
		}

		[DebuggerHidden]
		protected IEnumerator Coroutine()
		{
			FlyingkickAction.<Coroutine>c__Iterator7C <Coroutine>c__Iterator7C = new FlyingkickAction.<Coroutine>c__Iterator7C();
			<Coroutine>c__Iterator7C.<>f__this = this;
			return <Coroutine>c__Iterator7C;
		}

		protected void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance, out bool outIsHitSide)
		{
			currentDistance = Vector3.Distance(targetPos, base.unit.mTransform.position);
			outIsHitSide = false;
			Vector3 normalized = (targetPos - base.unit.mTransform.position).normalized;
			if (currentDistance < (normalized * moveSpeedDelta).magnitude)
			{
				outIsHitSide = true;
				return;
			}
			CollisionFlags collisionFlags = CollisionFlags.None;
			if ((collisionFlags & CollisionFlags.Sides) != CollisionFlags.None)
			{
				outIsHitSide = true;
			}
		}

		private void DoStop()
		{
			base.unit.PlayAnim(AnimationType.Conjure, true, 0, false, false);
			base.unit.ForceIdle();
			base.unit.RevertLayer();
			this.isReachedTarget = true;
			this.isActive = false;
			if (Singleton<PvpManager>.Instance.IsInPvp && base.unit.isPlayer)
			{
				PvpEvent.SendFlashTo(base.unit.unique_id, base.unit.trans.position);
			}
		}

		private void DoStart()
		{
			this.isMoveTarget = true;
			this.isActive = true;
			base.unit.ChangeLayer("ChongZhuang");
			base.unit.StopMove();
			if (this.targetUnits != null && this.targetUnits.Count > 0)
			{
				this.skillPosition = new Vector3?(this.GetTargetPos(this.targetUnits[0]));
			}
		}

		private Vector3 GetTargetPos(Units targetUnit)
		{
			if (targetUnit == null || targetUnit == base.unit)
			{
				return this.skillPosition.Value;
			}
			Vector3 a = new Vector3(targetUnit.transform.position.x, base.unit.mTransform.position.y, targetUnit.transform.position.z);
			Vector3 lhs = a - base.unit.mTransform.position;
			Vector3 b = lhs.normalized * 1.5f;
			Vector3 a2 = base.unit.mTransform.position + b;
			bool flag = false;
			Vector3 vector = a - lhs.normalized * 0.5f;
			while (!DestinationManager.IsPointWalkable(base.unit, vector))
			{
				vector -= lhs.normalized * 0.5f;
				if (Vector3.Dot(lhs, vector - base.unit.mTransform.position) < 0f)
				{
					IL_137:
					if (!flag)
					{
						vector = a + lhs.normalized * 0.5f;
						while (!DestinationManager.IsPointWalkable(base.unit, vector))
						{
							vector += lhs.normalized * 0.5f;
							if (Vector3.Dot(lhs, a2 - vector) < 0f)
							{
								goto IL_1B2;
							}
						}
						flag = true;
					}
					IL_1B2:
					if (!flag)
					{
						vector = a - 0.5f * lhs.normalized;
					}
					return vector;
				}
			}
			flag = true;
			goto IL_137;
		}

		protected override void OnDestroy()
		{
			this.DoStop();
			base.OnDestroy();
		}
	}
}
