using System;
using System.Collections.Generic;
using UnityEngine;

public class HitGroup : MobaMono
{
	[SerializeField]
	private HitDetector _hitDetector;

	private Dictionary<GameObject, int> _crossedObjs;

	private Dictionary<GameObject, int> _hitInObjs;

	private Dictionary<GameObject, int> _hitOutObjs;

	private void Awake()
	{
		if (this._hitDetector == null)
		{
			this._hitDetector = base.getComponentInParent<HitDetector>();
		}
	}

	public void triggerCrossFunc(GameObject go)
	{
		if (this._crossedObjs == null)
		{
			this._crossedObjs = new Dictionary<GameObject, int>();
		}
		if (this._crossedObjs.ContainsKey(go))
		{
			return;
		}
		this._crossedObjs.Add(go, 0);
		if (this._hitDetector != null)
		{
			this._hitDetector.triggerCross(go);
		}
	}

	public void triggerHitIn(GameObject go)
	{
		if (this._hitInObjs == null)
		{
			this._hitInObjs = new Dictionary<GameObject, int>();
		}
		if (this._hitInObjs.ContainsKey(go))
		{
			return;
		}
		this._hitInObjs.Add(go, 0);
		if (this._hitDetector != null)
		{
			this._hitDetector.triggerInFunc(go);
		}
	}

	public void triggerHitOut(GameObject go)
	{
		if (this._hitOutObjs == null)
		{
			this._hitOutObjs = new Dictionary<GameObject, int>();
		}
		if (this._hitInObjs == null || !this._hitInObjs.ContainsKey(go))
		{
			return;
		}
		if (this._hitOutObjs.ContainsKey(go))
		{
			return;
		}
		this._hitOutObjs.Add(go, 0);
		if (this._hitDetector != null)
		{
			this._hitDetector.triggerOutFunc(go);
		}
	}

	private void OnDisable()
	{
		if (this._hitInObjs != null)
		{
			this._hitInObjs.Clear();
		}
		if (this._hitOutObjs != null)
		{
			this._hitOutObjs.Clear();
		}
		if (this._crossedObjs != null)
		{
			this._crossedObjs.Clear();
		}
	}
}
