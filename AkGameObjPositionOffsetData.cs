using System;
using UnityEngine;

[Serializable]
public class AkGameObjPositionOffsetData
{
	public Vector3 positionOffset;

	public bool KeepMe;

	public AkGameObjPositionOffsetData(bool IReallyWantToBeConstructed = false)
	{
		this.KeepMe = IReallyWantToBeConstructed;
	}
}
