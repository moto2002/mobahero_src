using System;
using UnityEngine;

[AddComponentMenu("Game/HitDetector")]
public class HitDetector : MobaMono
{
	private Callback<Collider> _onTargetIn;

	private Callback<Collider> _onTargetOut;

	private Callback<Collider> _onHitTarget;

	public Callback<Collider> onTargetIn
	{
		get
		{
			return this._onTargetIn;
		}
		set
		{
			this._onTargetIn = value;
		}
	}

	public Callback<Collider> onTargetOut
	{
		get
		{
			return this._onTargetOut;
		}
		set
		{
			this._onTargetOut = value;
		}
	}

	public Callback<Collider> onHitTarget
	{
		get
		{
			return this._onHitTarget;
		}
		set
		{
			this._onHitTarget = value;
		}
	}

	public void triggerCross(GameObject go)
	{
		if (this._onHitTarget != null)
		{
			this._onHitTarget(go.collider);
		}
	}

	public void triggerInFunc(GameObject go)
	{
		if (this._onTargetIn != null)
		{
			this._onTargetIn(go.collider);
		}
	}

	public void triggerOutFunc(GameObject go)
	{
		if (this._onTargetOut != null)
		{
			this._onTargetOut(go.collider);
		}
	}
}
