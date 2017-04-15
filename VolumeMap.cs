using System;
using UnityEngine;

public class VolumeMap : MonoBehaviour
{
	public AnimationCurve map;

	private static VolumeMap _instance;

	public static VolumeMap Instance
	{
		get
		{
			if (VolumeMap._instance == null)
			{
				VolumeMap.Init();
			}
			return VolumeMap._instance;
		}
	}

	private static void Init()
	{
		GameObject gameObject = Resources.Load("Audio/VolumeMap") as GameObject;
		VolumeMap._instance = gameObject.GetComponent<VolumeMap>();
	}

	public static float DecayOnMap(float t)
	{
		return VolumeMap.Instance.map.Evaluate(t);
	}
}
