using System;
using UnityEngine;

public class CharaState
{
	public int count;

	public bool IsInState
	{
		get
		{
			return this.count > 0;
		}
	}

	public CharaState()
	{
		this.count = 0;
	}

	public void SetState(int val)
	{
		this.count = val;
	}

	public void Add()
	{
		this.count++;
	}

	public void Remove()
	{
		this.count--;
		this.count = Mathf.Clamp(this.count, 0, 2147483647);
	}

	public void Reset()
	{
		this.count = 0;
	}
}
