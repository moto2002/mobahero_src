using MobaHeros.Pvp;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamSignalPart : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _signalIcons;

	public void Show(TeamSignalType type)
	{
		foreach (GameObject current in this._signalIcons)
		{
			if (current)
			{
				current.SetActive(false);
			}
		}
		this._signalIcons[(int)type].transform.localEulerAngles = new Vector3(0f, 0f, -base.transform.parent.transform.localEulerAngles.z);
		this._signalIcons[(int)type].SetActive(true);
	}
}
