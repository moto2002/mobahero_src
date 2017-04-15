using Com.Game.Module;
using MobaHeros;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ChongZhuangAction : MissileAction
	{
		private float curTime;

		private float totalTime;

		private float speed;

		private float curDistance;

		private float totalDistance;

		private float m_fCostDamage;

		private float m_fCostMp;

		private int m_nCostUseCount = 1;

		private Vector3 m_StartPos = Vector3.zero;

		private float m_nMP;

		protected override void OnInit()
		{
			base.OnInit();
			this.curTime = 0f;
			this.curDistance = 0f;
			this.m_nMP = base.unit.GetAttr(AttrType.Mp);
			this.m_nCostUseCount = 1;
			if (this.skillData != null && this.skillData.cost_ids.Length > 1)
			{
				int id = this.skillData.cost_ids[1];
				DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(id);
				if (vo != null)
				{
					float mp_max = base.unit.mp_max;
					this.m_fCostDamage = -(mp_max * vo.damageParam3 + vo.damageParam2);
				}
				id = this.skillData.cost_ids[0];
				vo = Singleton<DamageDataManager>.Instance.GetVo(id);
				if (vo != null)
				{
					float mp_max2 = base.unit.mp_max;
					this.m_fCostMp = -(mp_max2 * vo.damageParam3 + vo.damageParam2);
					this.m_nMP -= this.m_fCostMp;
				}
			}
			this.m_StartPos.x = base.unit.transform.position.x;
			this.m_StartPos.y = base.unit.transform.position.y;
			this.m_StartPos.z = base.unit.transform.position.z;
		}

		protected override void OnStop()
		{
			base.OnStop();
		}

		[DebuggerHidden]
		protected override IEnumerator Missile_Coroutine()
		{
			ChongZhuangAction.<Missile_Coroutine>c__Iterator56 <Missile_Coroutine>c__Iterator = new ChongZhuangAction.<Missile_Coroutine>c__Iterator56();
			<Missile_Coroutine>c__Iterator.<>f__this = this;
			return <Missile_Coroutine>c__Iterator;
		}

		protected override void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			currentDistance = Vector3.Distance(targetPos, base.unit.mTransform.position);
		}

		protected virtual void OnHit(BaseAction action, Units target)
		{
			action.Destroy();
		}

		public virtual void OnMoveStart()
		{
			base.unit.SetCanAction(false);
		}

		public virtual void OnMoveEnd()
		{
			base.unit.SetCanAction(true);
			base.unit.ForceIdle();
			base.mCoroutineManager.StopAllCoroutine();
		}

		protected override void OnDestroy()
		{
			this.OnMoveEnd();
			base.OnDestroy();
		}

		protected override Vector3? GetTargetPosition()
		{
			Vector3? targetPosition = this.targetPosition;
			if (targetPosition.HasValue)
			{
				Vector3? targetPosition2 = this.targetPosition;
				return new Vector3?(targetPosition2.Value);
			}
			return null;
		}

		private new bool CheckTargetExist()
		{
			if (this.targetUnit != null)
			{
				return this.targetUnit.isLive;
			}
			Vector3? targetPosition = this.targetPosition;
			return targetPosition.HasValue;
		}
	}
}
