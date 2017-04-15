using System;
using UnityEngine;

public class CurveMissileObj : MonoBehaviour
{
	public float randTime;

	public int maxRandCount;

	public float startAngle;

	public Vector3 TargetPos;

	public float speed = 18f;

	public bool isStart;

	private int curRandCount;

	private int type;

	private float curtime;

	private float startDistanceToTarget;

	private float distance = 3.40282347E+38f;

	private float deltaTime = 0.033f;

	public void DoStart()
	{
		this.isStart = true;
		this.startDistanceToTarget = Vector3.Distance(base.transform.position, this.TargetPos);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.isStart)
		{
			this.MoveDelta(this.TargetPos, this.speed * this.deltaTime, out this.distance);
			if (this.distance <= 0.2f)
			{
				this.isStart = false;
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
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
		this.curRandCount++;
	}

	protected void MoveDelta(Vector3 targetPos, float moveSpeedDelta, out float currentDistance)
	{
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
