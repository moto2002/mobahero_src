using Com.Game.Module;
using MobaHeros.Pvp;
using MobaTools.Move;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class TengYunTuJiAction : BaseHighEffAction
	{
		protected bool isReachedTarget;

		protected float speed;

		protected Vector3 sourceCenter;

		protected float _moveTotalTime = 1f;

		protected float _pastTime;

		private List<TengYunTuJiReplicateInfo> _replicateInfos = new List<TengYunTuJiReplicateInfo>();

		protected override void OnInit()
		{
			base.OnInit();
			this.speed = this.data.param1;
			this._replicateInfos.Clear();
		}

		protected override void StartHighEff()
		{
			base.StartHighEff();
			if (base.unit != null && base.unit.m_nVisibleState == 2)
			{
				return;
			}
			base.mCoroutineManager.StartCoroutine(this.CreateSelfReplicateCoroutine(), true);
			base.mCoroutineManager.StartCoroutine(this.MoveToTargetCoroutine(), true);
		}

		protected override void StopHighEff()
		{
			base.mCoroutineManager.StopAllCoroutine();
			this.DestroySelfReplicate();
			base.unit.ForceIdle();
		}

		[DebuggerHidden]
		protected IEnumerator MoveToTargetCoroutine()
		{
			TengYunTuJiAction.<MoveToTargetCoroutine>c__Iterator87 <MoveToTargetCoroutine>c__Iterator = new TengYunTuJiAction.<MoveToTargetCoroutine>c__Iterator87();
			<MoveToTargetCoroutine>c__Iterator.<>f__this = this;
			return <MoveToTargetCoroutine>c__Iterator;
		}

		protected void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
		{
			if (base.unit == null)
			{
				currentDistance = 0f;
				return;
			}
			currentDistance = Vector3.Distance(targetPos, base.unit.mTransform.position);
			if (currentDistance < 0.2f)
			{
				return;
			}
		}

		private void DoStop()
		{
			base.unit.PlayAnim(AnimationType.Conjure, true, 0, false, false);
			base.unit.ForceIdle();
			base.unit.RevertLayer();
			this.isReachedTarget = true;
			this.isActive = false;
		}

		private void DoStart()
		{
			this.isReachedTarget = false;
			this.isActive = true;
			base.unit.ChangeLayer("ChongZhuang");
			if (!Singleton<PvpManager>.Instance.IsInPvp)
			{
			}
			if (this.targetUnits != null && this.targetUnits.Count > 0)
			{
				this.skillPosition = new Vector3?(this.GetTargetPos(this.targetUnits[0]));
				if (base.unit != null && this.speed > 0.001f)
				{
					this._moveTotalTime = (this.skillPosition.Value - base.unit.transform.position).magnitude / this.speed;
				}
			}
			this._pastTime = 0f;
		}

		private Vector3 GetTargetPos(Units targetUnit)
		{
			if (targetUnit == null || targetUnit == base.unit)
			{
				return this.skillPosition.Value;
			}
			Vector3 vector = new Vector3(targetUnit.transform.position.x, base.unit.mTransform.position.y, targetUnit.transform.position.z);
			Vector3 vector2 = vector - base.unit.mTransform.position;
			if (vector2.sqrMagnitude < 0.5f)
			{
				return vector;
			}
			Vector3 normalized = vector2.normalized;
			Vector3 vector3 = vector - normalized * 0.5f;
			bool flag = false;
			while (Vector3.Dot(normalized, vector3 - base.unit.mTransform.position) >= 0f)
			{
				if (DestinationManager.IsPointWalkable(base.unit, vector3))
				{
					flag = true;
					break;
				}
				vector3 -= normalized * 0.5f;
			}
			if (!flag)
			{
				vector3 = vector - normalized * 0.5f;
			}
			return vector3;
		}

		protected override void OnDestroy()
		{
			this.DoStop();
			base.OnDestroy();
		}

		[DebuggerHidden]
		private IEnumerator CreateSelfReplicateCoroutine()
		{
			TengYunTuJiAction.<CreateSelfReplicateCoroutine>c__Iterator88 <CreateSelfReplicateCoroutine>c__Iterator = new TengYunTuJiAction.<CreateSelfReplicateCoroutine>c__Iterator88();
			<CreateSelfReplicateCoroutine>c__Iterator.<>f__this = this;
			return <CreateSelfReplicateCoroutine>c__Iterator;
		}

		private void DestroySelfReplicate()
		{
			if (this._replicateInfos == null)
			{
				return;
			}
			if (this._replicateInfos.Count > 0)
			{
				for (int i = 0; i < this._replicateInfos.Count; i++)
				{
					if (this._replicateInfos[i] != null)
					{
						MapManager.Instance.DespawnTengYunTuJiReplicateUnit(this._replicateInfos[i].replicateUnit);
					}
				}
				this._replicateInfos.Clear();
			}
			this._replicateInfos = null;
		}

		private void ReplicateUnitMoveDelta()
		{
			if (this._replicateInfos == null || this._replicateInfos.Count < 1)
			{
				return;
			}
			for (int i = 0; i < this._replicateInfos.Count; i++)
			{
				if (!this._replicateInfos[i].isStopped)
				{
					Units replicateUnit = this._replicateInfos[i].replicateUnit;
					if (!(replicateUnit == null))
					{
						float num = (this._moveTotalTime <= 0.001f) ? 1f : (this._pastTime / this._moveTotalTime);
						num = Mathf.Clamp01(num);
						if (num > 0.95f)
						{
							this._replicateInfos[i].isStopped = true;
							replicateUnit.transform.position = this._replicateInfos[i].targetPos;
						}
						else
						{
							replicateUnit.transform.position = Vector3.Lerp(this._replicateInfos[i].srcPos, this._replicateInfos[i].targetPos, num);
						}
					}
				}
			}
		}

		private void TrySelfReplicatePlayAnimation(Units inUnitInst)
		{
			if (inUnitInst == null)
			{
				return;
			}
			if (!StringUtils.CheckValid(this.skillId))
			{
				return;
			}
			Skill skillById = base.unit.getSkillById(this.skillId);
			if (skillById == null || skillById.Data == null)
			{
				return;
			}
			if (skillById.Data.start_actions == null || skillById.Data.start_actions.Length < 1)
			{
				return;
			}
			for (int i = 0; i < skillById.Data.start_actions.Length; i++)
			{
				if (StringUtils.CheckValid(skillById.Data.start_actions[i]))
				{
					PerformData vo = Singleton<PerformDataManager>.Instance.GetVo(skillById.Data.start_actions[i]);
					if (vo != null && vo.action_type == AnimationType.Conjure)
					{
						inUnitInst.PlayAnim(vo.action_type, true, vo.action_index, false, false);
						break;
					}
				}
			}
		}
	}
}
