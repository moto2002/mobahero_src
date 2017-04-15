using Com.Game.Utils;
using System;
using UnityEngine;

public class EditorDartsPerform : EditorPerform
{
	private Vector3 dir;

	private bool inProcess;

	private bool isForward = true;

	private float distance = 1E-06f;

	private Vector3 startPos;

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
		if (!this.inProcess)
		{
			this.startPos = this.trans.position;
			this.dir = (this.pos.Value + Vector3.up - this.trans.position).normalized;
			this.inProcess = true;
		}
		this.trans.position += this.dir * this.data.config.effect_speed * Time.deltaTime;
		this.distance = Vector3.Distance(this.startPos, this.trans.position);
		if (this.distance >= 7.5f && this.isForward)
		{
			this.dir = -this.dir;
			this.isForward = false;
		}
		ClientLogger.Info(this.distance);
		if (this.distance <= 0.3f && !this.isForward)
		{
			base.doDestroy();
		}
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
