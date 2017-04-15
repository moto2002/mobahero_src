using System;
using UnityEngine;

public class MissilePlayer : MonoBehaviour
{
	public float speed = 12f;

	public float distance = 10f;

	public Vector3 dir = Vector3.forward;

	public bool Play;

	private bool isRevert;

	private Vector3 sourcePos;

	private Vector3 targetPos;

	private void Awake()
	{
		this.sourcePos = base.transform.localPosition;
		Application.targetFrameRate = 20;
	}

	private void Update()
	{
		if (this.Play)
		{
			this.targetPos = this.sourcePos + this.dir.normalized * this.distance;
			if (!this.isRevert)
			{
				base.transform.LookAt(this.targetPos);
				base.transform.Translate((this.targetPos - this.sourcePos).normalized * this.speed * 0.033f, Space.World);
				if (Vector3.Distance(base.transform.localPosition, this.targetPos) <= 1f)
				{
					this.isRevert = true;
				}
			}
			else
			{
				base.transform.LookAt(this.sourcePos);
				base.transform.Translate((this.sourcePos - this.targetPos).normalized * this.speed * 0.033f, Space.World);
				if (Vector3.Distance(base.transform.localPosition, this.sourcePos) <= 1f)
				{
					this.isRevert = false;
				}
			}
		}
	}
}
