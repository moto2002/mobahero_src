using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class RotationBehavior : MonoBehaviour
	{
		[SerializeField]
		public Transform followTarget;

		[SerializeField]
		public Vector3 offset = Vector3.zero;

		[SerializeField]
		public float speed = 30f;

		[SerializeField]
		public int motionDirection = 1;

		[SerializeField]
		public bool isActive;

		private Vector3 priotPosition;

		private void Awake()
		{
			this.isActive = false;
			this.offset = Vector3.zero;
		}

		private void OnSpawned()
		{
			this.isActive = false;
			this.offset = Vector3.zero;
		}

		private void OnDespawned()
		{
			this.isActive = false;
			this.offset = Vector3.zero;
		}

		private void Update()
		{
			if (this.followTarget == null)
			{
				return;
			}
			base.transform.RotateAround(this.priotPosition, Vector3.up, (float)this.motionDirection * this.speed * Time.deltaTime);
			Vector3 b = this.followTarget.position - this.priotPosition;
			base.transform.position += b;
			this.priotPosition = this.followTarget.position;
		}

		public void SetFollowObj(Transform target, Vector3 offset, float speed = 0f)
		{
			this.followTarget = target;
			this.offset = offset;
			if (speed != 0f)
			{
				this.speed = speed;
			}
			if (this.followTarget != null)
			{
				base.transform.position = this.followTarget.position + offset;
				this.priotPosition = this.followTarget.position;
				this.isActive = true;
			}
		}
	}
}
