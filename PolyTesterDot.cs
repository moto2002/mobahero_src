using System;
using UnityEngine;

public class PolyTesterDot : MobaMono
{
	[SerializeField]
	private PolyTesterDot _nextDot;

	public PolyTesterDot nextDot
	{
		get
		{
			return this._nextDot;
		}
		set
		{
			this._nextDot = value;
		}
	}
}
