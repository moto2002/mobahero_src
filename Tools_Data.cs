using System;
using UnityEngine;

public static class Tools_Data
{
	public static int RoundDownTo1(float _num)
	{
		int num = Mathf.FloorToInt(_num);
		if (num < 1)
		{
			num = 1;
		}
		return num;
	}
}
