using System;
using UnityEngine;

public class EditorBulletPerform : EditorPerform
{
	private Vector3 dir;

	protected override void Update()
	{
		if (this.targets == null)
		{
			return;
		}
		Vector3? pos = this.pos;
		if (!pos.HasValue)
		{
			return;
		}
		if (this.trans == null)
		{
			return;
		}
		this.dir = (this.pos.Value + Vector3.up - this.trans.position).normalized;
		this.trans.position += this.dir * this.data.config.effect_speed * Time.deltaTime;
		if (this.CheackHit())
		{
			this.OnHit();
		}
	}

	protected override void OnPlayEffect()
	{
		this.dir = (this.pos.Value + Vector3.up - this.trans.position).normalized;
		this.trans.LookAt(this.dir);
	}

	protected override bool CheackHit()
	{
		return Vector3.Distance(this.trans.position, this.pos.Value + Vector3.up) <= 0.3f && this.hitCount <= 0 && this.data.useCollider;
	}
}
