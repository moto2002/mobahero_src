using System;
using UnityEngine;

public class eventPlayerSound : MobaMono
{
	public bool isplayer;

	public Units units;

	public void loadAnimEvent()
	{
	}

	public void playSound_clip(string strParam)
	{
		if (this.units != null && this.units.m_nVisibleState >= 2)
		{
			return;
		}
		string[] array = strParam.Split(new char[]
		{
			':'
		});
		if (array.Length != 3)
		{
			Debug.LogError("wrong format:" + strParam);
		}
		string eventstr = array[0];
		string text = array[1];
		float num = float.Parse(array[2]);
		if (AudioMgr.Instance.isUsingWWise())
		{
			if (AudioMgr.Instance.isEffMute())
			{
				return;
			}
			if (!string.IsNullOrEmpty(strParam))
			{
				if (!this.isplayer && strParam.Contains("Org_Attack"))
				{
					return;
				}
				AudioMgr.Play(eventstr, base.transform.parent.gameObject, false, false);
			}
		}
	}
}
