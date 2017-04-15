using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using System;
using UnityEngine;

public class SummonerHeadItem : MonoBehaviour
{
	[SerializeField]
	private UILabel S_LV;

	[SerializeField]
	private UITexture S_HeadFrame;

	[SerializeField]
	private UITexture S_Summoner;

	public void Init(string head, string frame, long summonerExp)
	{
		SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(frame);
		if (dataById != null)
		{
			this.S_HeadFrame.mainTexture = ResourceManager.Load<Texture>(dataById.pictureframe_icon, true, true, null, 0, false);
		}
		else
		{
			ClientLogger.Warn("SysSummonerPictureframeVo is NullFrameNumber is" + frame);
			this.S_HeadFrame.mainTexture = ResourceManager.Load<Texture>("pictureframe_0000", true, true, null, 0, false);
		}
		SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(head);
		if (dataById2 == null)
		{
			this.S_Summoner.mainTexture = ResourceManager.Load<Texture>("headportrait_0001", true, true, null, 0, false);
		}
		else
		{
			this.S_Summoner.mainTexture = ResourceManager.Load<Texture>(dataById2.headportrait_icon, true, true, null, 0, false);
		}
		this.S_LV.text = CharacterDataMgr.instance.GetUserLevel(summonerExp).ToString();
	}

	public void ClearResources()
	{
		if (this.S_Summoner != null && this.S_Summoner.mainTexture != null)
		{
			this.S_Summoner.mainTexture = null;
		}
	}
}
