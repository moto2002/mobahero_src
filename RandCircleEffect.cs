using System;
using UnityEngine;

public class RandCircleEffect : RandSphereEffect
{
	protected override void Update()
	{
		if (Time.time - this.startTime <= this.delay)
		{
			return;
		}
		if (this.parentObj == null)
		{
			return;
		}
		if ((Mathf.Abs(this.tempPos.x - this.mTrans.position.x) >= this.range || Mathf.Abs(this.tempPos.z - this.mTrans.position.z) >= this.range || this.mTrans.position == Vector3.zero) && !this.isStop)
		{
			this.tempPos = MathUtils.RadomInsideCirclePoint(this.parentObj.position, this.range);
			this.tempdir = (this.tempPos - this.mTrans.position).normalized;
			this.mTrans.position += this.tempdir * Time.deltaTime * this.speed;
		}
		else if (this.isStop)
		{
			this.mTrans.localPosition = Vector3.Lerp(this.mTrans.localPosition, this.tempPos, Time.deltaTime);
		}
		else
		{
			this.mTrans.position += this.tempdir * Time.deltaTime * this.speed;
		}
		if (this.exitTime != 0f && Time.time - this.startTime >= this.exitTime && !this.isStop)
		{
			this.isStop = true;
			this.tempPos = Vector3.zero;
		}
	}
}
