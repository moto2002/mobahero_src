using MobaTools.Move;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class WeiyiSubAction : BaseStateAction
	{
		public const string TASK_NAME = "weiyi task";

		private float moveSpeed;

		private float moveDistance;

		private float moveDuration;

		protected new bool useCollider = true;

		public int colliderRangeType = 1;

		public int colliderParam1 = 5;

		public int colliderParam2 = 2;

		public int colliderParam3 = 2;

		protected new bool isActive;

		protected bool isMoveTarget;

		protected bool isReachedTarget;

		protected Vector3 targetDeathPos;

		protected Quaternion daoDanRot;

		protected Vector3 targetPos;

		protected Vector3 startPos;

		protected float curTime;

		private Task _task;

		private bool _stopped;

		protected override bool IsInState
		{
			get
			{
				return this.targetUnit.WeiYi.IsInState;
			}
		}

		protected override void StartHighEff()
		{
			this.isActive = true;
			this.moveSpeed = this.data.param1;
			this.moveDistance = this.data.param2;
			this.moveDuration = ((this.data.param3 != 0f) ? this.data.param3 : 2.14748365E+09f);
			base.unit.transform.Rotate(0f, this.rotateY, 0f);
			this._task = base.mCoroutineManager.StartCoroutine(this.ChongZhuang_Coroutine(), true);
		}

		protected override void StopHighEff()
		{
			this.isActive = false;
			base.mCoroutineManager.StopAllCoroutine();
			this.RevertState();
		}

		protected override void SetState()
		{
			base.SetState();
			this.targetUnit.WeiYi.Add();
			this.targetUnit.SetCanEnableActionHighEff(false);
			this.targetUnit.ChangeLayer("ChongZhuang");
		}

		protected override void RevertState()
		{
			base.RevertState();
			this.targetUnit.RevertLayer();
			this.targetUnit.SetCanEnableActionHighEff(true);
			this.targetUnit.WeiYi.Remove();
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			return new WeiyiSubAction.<Coroutine>c__Iterator8A();
		}

		[DebuggerHidden]
		private IEnumerator ChongZhuang_Coroutine()
		{
			WeiyiSubAction.<ChongZhuang_Coroutine>c__Iterator8B <ChongZhuang_Coroutine>c__Iterator8B = new WeiyiSubAction.<ChongZhuang_Coroutine>c__Iterator8B();
			<ChongZhuang_Coroutine>c__Iterator8B.<>f__this = this;
			return <ChongZhuang_Coroutine>c__Iterator8B;
		}

		private Vector3 CalcTargetPos(Units inOwner, Vector3 inSrcPos, Vector3 inMoveDir, float inMaxMoveDistance)
		{
			if (inOwner == null || inMaxMoveDistance < 0.01f)
			{
				return inSrcPos;
			}
			Vector3 vector = inSrcPos + inMoveDir * inMaxMoveDistance;
			if (DestinationManager.IsPointWalkable(inOwner, vector))
			{
				return vector;
			}
			Vector3 vector2 = vector - inMoveDir * 0.5f;
			while (Vector3.Dot(inMoveDir, vector2 - inSrcPos) > 0f)
			{
				if (DestinationManager.IsPointWalkable(inOwner, vector2))
				{
					return vector2;
				}
				vector2 -= inMoveDir * 0.5f;
			}
			return inSrcPos;
		}
	}
}
