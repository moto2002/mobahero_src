using System;
using UnityEngine;

public class EditorMissilePerform : EditorPerform
{
	private Vector3 prePos;

	protected override void Update()
	{
		if (this.targets == null)
		{
			return;
		}
		if (this.trans == null)
		{
			return;
		}
		if (this.targets.Count == 0)
		{
			return;
		}
		Vector3 normalized = (this.targets[0].Trans.position + Vector3.up - this.trans.position).normalized;
		this.prePos = this.trans.position;
		this.trans.position += normalized * this.data.config.effect_speed * Time.deltaTime;
		if (this.CheackHit())
		{
			this.OnHit();
			base.doDestroy();
		}
	}

	protected override bool CheackHit()
	{
		if (this.data.useCollider)
		{
			if (Vector3.Distance(this.trans.position, this.targets[0].Trans.position + Vector3.up) <= 0.3f && this.hitCount <= 0)
			{
				return true;
			}
			if (Vector3.Dot((this.targets[0].Trans.position + Vector3.up - this.prePos).normalized, (this.targets[0].Trans.position + Vector3.up - this.trans.position).normalized) < 0f)
			{
				return true;
			}
		}
		return false;
	}
}
