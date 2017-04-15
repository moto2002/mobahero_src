using MobaProtocol.Data;
using System;
using UnityEngine;

public class ScreenShotItem : MonoBehaviour
{
	public UITexture ScreenShotPic;

	public UILabel KillTypeLabel;

	public UILabel TimeLabel;

	public UILabel DateLabel;

	public void Init(KdaUserHonorData data)
	{
		this.KillTypeLabel.text = this.KillTypeToString(data.killtype);
		this.TimeLabel.text = data.timerecord.ToString("HH:mm");
		this.DateLabel.text = data.timerecord.ToString("M/d/y");
	}

	private string KillTypeToString(KillType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case KillType.TripleKill:
			result = "三杀";
			break;
		case KillType.QuadraKill:
			result = "四杀";
			break;
		case KillType.PentaKill:
			result = "五杀";
			break;
		default:
			if (type == KillType.GodLike)
			{
				result = "超神";
			}
			break;
		}
		return result;
	}
}
