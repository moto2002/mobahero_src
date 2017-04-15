using Assets.Scripts.Model;
using System;
using UnityEngine;

public class CheatCreator : MonoBehaviour
{
	private bool _noTouch = true;

	private void Update()
	{
		if (Input.touchCount == 0)
		{
			this._noTouch = true;
		}
		else if (this._noTouch && Input.touchCount >= 4 && ModelManager.Instance.Get_IsInWhiteList())
		{
			this._noTouch = false;
			Cheat component = base.gameObject.GetComponent<Cheat>();
			if (component == null)
			{
				base.gameObject.AddComponent<Cheat>();
			}
			else
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}
}
