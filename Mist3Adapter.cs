using System;
using UnityEngine;

public class Mist3Adapter : MonoBehaviour
{
	public void CheckMistSettings()
	{
		if (Mist3Adapter.CheckMistSetting())
		{
			if (PostMist.Instance == null)
			{
				PostMist.SetMistPerspect(base.camera);
			}
			else
			{
				PostMist.Instance.EnableMist();
			}
		}
		else if (PostMist.Instance != null)
		{
			PostMist.Instance.DisableMist();
		}
	}

	private static bool CheckMistSetting()
	{
		return false;
	}
}
