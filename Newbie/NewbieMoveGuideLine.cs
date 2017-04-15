using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newbie
{
	public class NewbieMoveGuideLine : MonoBehaviour
	{
		private float CArrowIntervalDist = 2f;

		private Vector3 CHidePos = new Vector3(0f, -999999f, 0f);

		private Vector3 _targetPos = Vector3.one;

		private Units _playerInst;

		private GameObject _lineArrowResObj;

		private bool _isDoUpdate;

		private float _playerRadius = 0.5f;

		private float _targetRadius = 1f;

		private float _dist = 1f;

		private Vector3 _dir = Vector3.one;

		private Vector3 _startPos = Vector3.one;

		private List<Transform> _arrows = new List<Transform>();

		private void Update()
		{
			if (!this._isDoUpdate)
			{
				return;
			}
			this.UpdateArrowHints();
		}

		public void ShowMoveGuideLine(Vector3 inTargetPos)
		{
			this._targetPos = inTargetPos;
			this._playerInst = PlayerControlMgr.Instance.GetPlayer();
			this._lineArrowResObj = NewbieManager.Instance.GetMoveGuideLineArrowResHandle();
			this._isDoUpdate = true;
			this.CreateArrowHints();
		}

		public void HideMoveGuideLine()
		{
			this._isDoUpdate = false;
			base.enabled = false;
			for (int i = 0; i < this._arrows.Count; i++)
			{
				Transform transform = this._arrows[i];
				if (transform != null)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			this._arrows.Clear();
		}

		private void CreateArrowHints()
		{
			this._dist = (this._targetPos - this._playerInst.mTransform.position).magnitude - this._playerRadius - this._targetRadius;
			if (this._dist < 2f * this.CArrowIntervalDist)
			{
				return;
			}
			int num = Mathf.CeilToInt(this._dist / this.CArrowIntervalDist) - 1;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this._lineArrowResObj) as GameObject;
				if (gameObject != null)
				{
					this._arrows.Add(gameObject.transform);
				}
			}
			this._dir = (this._targetPos - this._playerInst.mTransform.position).normalized;
			this._startPos = this._targetPos - this._dir * (this._targetRadius + this.CArrowIntervalDist);
			this._startPos = new Vector3(this._startPos.x, 0.5f, this._startPos.z);
			this.AjustArrowTrans(num, this._startPos, this._dir);
		}

		private void UpdateArrowHints()
		{
			if (this._playerInst == null || this._playerInst.mTransform == null)
			{
				return;
			}
			if (this._arrows == null)
			{
				return;
			}
			if (this._lineArrowResObj == null)
			{
				return;
			}
			this._dist = (this._targetPos - this._playerInst.mTransform.position).magnitude - this._playerRadius - this._targetRadius;
			int num = Mathf.CeilToInt(this._dist / this.CArrowIntervalDist) - 1;
			if (num > this._arrows.Count)
			{
				int num2 = num - this._arrows.Count;
				for (int i = 0; i < num2; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(this._lineArrowResObj) as GameObject;
					if (gameObject != null)
					{
						this._arrows.Add(gameObject.transform);
					}
				}
			}
			this._dir = (this._targetPos - this._playerInst.mTransform.position).normalized;
			this._startPos = this._targetPos - this._dir * (this._targetRadius + this.CArrowIntervalDist);
			this._startPos = new Vector3(this._startPos.x, 0.5f, this._startPos.z);
			this.AjustArrowTrans(num, this._startPos, this._dir);
		}

		private void AjustArrowTrans(int inArrowCount, Vector3 inStartPos, Vector3 inPlayerPointTargetDir)
		{
			if (this._arrows == null)
			{
				return;
			}
			for (int i = 0; i < this._arrows.Count; i++)
			{
				Transform transform = this._arrows[i];
				if (!(transform == null))
				{
					if (i < inArrowCount)
					{
						transform.position = inStartPos - inPlayerPointTargetDir * this.CArrowIntervalDist * (float)i;
						transform.LookAt(this._targetPos);
					}
					else
					{
						this.HideArrow(transform);
					}
				}
			}
		}

		private void HideArrow(Transform inTrans)
		{
			if (inTrans != null)
			{
				inTrans.position = this.CHidePos;
			}
		}
	}
}
