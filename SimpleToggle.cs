using System;
using UnityEngine;

public class SimpleToggle : MonoBehaviour
{
	public GameObject[] AllObjects;

	public bool TestMode = false;

	private int _activeIndex = -1;

	public int ActiveIndex
	{
		get
		{
			return this._activeIndex;
		}
		set
		{
			if (value < 0 || value >= this.AllObjects.Length)
			{
				this._activeIndex = -1;
			}
			else
			{
				this._activeIndex = value;
			}
			for (int i = 0; i < this.AllObjects.Length; i++)
			{
				this.AllObjects[i].SetActive(i == this._activeIndex);
			}
		}
	}
}
