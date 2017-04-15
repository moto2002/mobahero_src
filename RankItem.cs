using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RankItem : MonoBehaviour
{
	public UISprite Bg;

	public UISprite RankSprite;

	public UILabel RankLabel;

	public UISprite Equal;

	public UISprite RankChange;

	public UILabel ChangeNum;

	public UISprite Portrait;

	public UISprite Frame;

	public UILabel SummonerLv;

	public Transform RankPlay;

	public UILabel _rName;

	public UILabel _rTeamLabel;

	public UISprite RankIcon;

	public UILabel RankLv;

	public UILabel RankPoint;

	public Transform MagicBottle;

	public UILabel _mName;

	public UILabel MagicBottleLv;

	public UISprite MagicBottleIcon;

	public UILabel MagicBottleExp;

	public Transform Charming;

	public UILabel CharmNum;

	public UILabel _cName;

	public Transform Team;

	public Transform Championship;

	public GameObject _pressMask;

	private void Awake()
	{
	}

	private void Start()
	{
		UIEventListener.Get(base.gameObject).onPress = new UIEventListener.BoolDelegate(this.ShowPressMask);
	}

	private void Update()
	{
	}

	public void ClearResources()
	{
	}

	private void ShowPressMask(GameObject obj, bool isPress)
	{
		this._pressMask.SetActive(isPress);
	}

	public void SetAndUpdateType(string name, int index, Dictionary<string, object> rankStageDic)
	{
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ChickDetailsInfo);
		switch (name)
		{
		case "Ladder":
		{
			this.RankPlay.gameObject.SetActive(true);
			this.MagicBottle.gameObject.SetActive(false);
			this.Charming.gameObject.SetActive(false);
			this.Team.gameObject.SetActive(false);
			this.Championship.gameObject.SetActive(false);
			this.SummonerLv.gameObject.SetActive(true);
			List<SummonerLadderRankData> rankList = ModelManager.Instance.Get_GetSummonerLadderRankList_X().rankList;
			this.SetHeroInfo(rankList[index]);
			this.GetState(rankList[index].LadderScore, rankStageDic);
			break;
		}
		case "MagicBottle":
		{
			this.RankPlay.gameObject.SetActive(false);
			this.MagicBottle.gameObject.SetActive(true);
			this.Charming.gameObject.SetActive(false);
			this.Team.gameObject.SetActive(false);
			this.Championship.gameObject.SetActive(false);
			this.SummonerLv.gameObject.SetActive(true);
			MagicBottleRankList magicBottleRankList = ModelManager.Instance.Get_GetMagicBottleRankList_X();
			List<MagicBottleRankData> list = magicBottleRankList.list;
			this.SetMagicInfo(list[index]);
			this.Equal.gameObject.SetActive(false);
			this.RankChange.gameObject.SetActive(false);
			break;
		}
		case "Charming":
		{
			this.RankPlay.gameObject.SetActive(false);
			this.MagicBottle.gameObject.SetActive(false);
			this.Charming.gameObject.SetActive(true);
			this.Team.gameObject.SetActive(false);
			this.Championship.gameObject.SetActive(false);
			this.SummonerLv.gameObject.SetActive(false);
			List<CharmRankData> charmRankList = ModelManager.Instance.Get_GetRankList_X().CharmRankList;
			this.SetCharmInfo(charmRankList[index]);
			this.Equal.gameObject.SetActive(false);
			this.RankChange.gameObject.SetActive(false);
			break;
		}
		case "Team":
			this.RankPlay.gameObject.SetActive(false);
			this.MagicBottle.gameObject.SetActive(false);
			this.Charming.gameObject.SetActive(false);
			this.Team.gameObject.SetActive(true);
			this.Championship.gameObject.SetActive(false);
			break;
		case "Championship":
			this.RankPlay.gameObject.SetActive(false);
			this.MagicBottle.gameObject.SetActive(false);
			this.Charming.gameObject.SetActive(false);
			this.Team.gameObject.SetActive(false);
			this.Championship.gameObject.SetActive(true);
			break;
		}
	}

	private void SetHeroInfo(SummonerLadderRankData summonerLadderRankData)
	{
		SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(summonerLadderRankData.IconId.ToString());
		if (dataById != null)
		{
			this.Portrait.spriteName = dataById.headportrait_icon.ToString();
		}
		SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(summonerLadderRankData.PictureFrame.ToString());
		if (dataById2 != null)
		{
			this.Frame.spriteName = dataById2.pictureframe_icon;
		}
		this._rName.text = summonerLadderRankData.NickName;
		this.SummonerLv.text = CharacterDataMgr.instance.GetUserLevel(summonerLadderRankData.Exp).ToString();
		base.gameObject.name = summonerLadderRankData.UserId + "|" + summonerLadderRankData.MagicBottleRank.ToString();
		if (summonerLadderRankData.Rank == 1)
		{
			this.Bg.spriteName = "Ranking_list_bg_02";
			this._rName.color = new Color32(212, 254, 249, 255);
			this._rTeamLabel.color = new Color32(253, 236, 120, 255);
			this.RankIcon.transform.Find("bg").GetComponent<UISprite>().color = new Color32(52, 12, 0, 155);
			this.RankPoint.transform.Find("bg").GetComponent<UISprite>().color = new Color32(52, 12, 0, 155);
		}
		else
		{
			this.Bg.spriteName = "Ranking_list_bg_01";
			this._rName.color = new Color32(212, 254, 249, 255);
			this._rTeamLabel.color = new Color32(0, 214, 253, 255);
			this.RankIcon.transform.Find("bg").GetComponent<UISprite>().color = new Color32(0, 18, 52, 155);
			this.RankPoint.transform.Find("bg").GetComponent<UISprite>().color = new Color32(0, 18, 52, 155);
		}
		if (summonerLadderRankData.Rank <= 3 && summonerLadderRankData.Rank > 0)
		{
			this.RankSprite.gameObject.SetActive(true);
			this.RankLabel.gameObject.SetActive(false);
			this.RankSprite.spriteName = "Ranking_images_number_" + summonerLadderRankData.Rank.ToString();
		}
		else
		{
			this.RankSprite.gameObject.SetActive(false);
			this.RankLabel.gameObject.SetActive(true);
			this.RankLabel.text = summonerLadderRankData.Rank.ToString();
		}
		if (summonerLadderRankData.UserId == ModelManager.Instance.Get_userData_X().UserId)
		{
			this._rName.color = new Color32(214, 7, 182, 255);
			this._rName.text = ModelManager.Instance.Get_userData_filed_X("NickName");
		}
		this._rName.GetComponent<AllochroicLabelChecker>().RenderLabel(summonerLadderRankData.CharmRankValue);
		this.SetRankUp(summonerLadderRankData.RankUp);
	}

	private void ChickDetailsInfo(GameObject go)
	{
		string[] array = go.name.Split(new char[]
		{
			'|'
		});
		string text = array[0];
		SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得数据", true, 15f);
		if (SendMsgManager.Instance.SendMsg(MobaGameCode.GetPlayerData, param, new object[]
		{
			text
		}))
		{
			Singleton<RankView>.Instance.targetId = go.name;
		}
	}

	private void SetMagicInfo(MagicBottleRankData rankData)
	{
		base.gameObject.name = rankData.userid + "|" + rankData.rank.ToString();
		SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(rankData.icon.ToString());
		if (dataById != null)
		{
			this.Portrait.spriteName = dataById.headportrait_icon;
		}
		SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(rankData.pictureFrame.ToString());
		if (dataById2 != null)
		{
			this.Frame.spriteName = dataById2.pictureframe_icon;
		}
		this.SummonerLv.text = rankData.level.ToString();
		this._mName.text = rankData.name;
		this.MagicBottleLv.text = rankData.magicbottlelevel.ToString() + "级";
		this.MagicBottleExp.text = rankData.todayexp.ToString();
		this.MagicBottleIcon.spriteName = Tools_ParsePrice.BottleLevelParse(rankData.magicbottlelevel);
		if (rankData.rank == 1)
		{
			this.Bg.spriteName = "Ranking_list_bg_02";
			this._mName.color = new Color32(212, 254, 249, 255);
			this.MagicBottleIcon.transform.Find("bg").GetComponent<UISprite>().color = new Color32(52, 12, 0, 155);
			this.MagicBottleExp.transform.Find("bg").GetComponent<UISprite>().color = new Color32(52, 12, 0, 155);
		}
		else
		{
			this.Bg.spriteName = "Ranking_list_bg_01";
			this._mName.color = new Color32(212, 254, 249, 255);
			this.MagicBottleIcon.transform.Find("bg").GetComponent<UISprite>().color = new Color32(0, 18, 52, 155);
			this.MagicBottleExp.transform.Find("bg").GetComponent<UISprite>().color = new Color32(0, 18, 52, 155);
		}
		if (rankData.rank <= 3 && rankData.rank > 0)
		{
			this.RankSprite.gameObject.SetActive(true);
			this.RankLabel.gameObject.SetActive(false);
			this.RankSprite.spriteName = "Ranking_images_number_" + rankData.rank.ToString();
		}
		else
		{
			this.RankSprite.gameObject.SetActive(false);
			this.RankLabel.gameObject.SetActive(true);
			this.RankLabel.text = rankData.rank.ToString();
		}
		if (rankData.userid == ModelManager.Instance.Get_userData_X().UserId)
		{
			this._mName.color = new Color32(214, 7, 182, 255);
			this._mName.text = ModelManager.Instance.Get_userData_filed_X("NickName");
		}
		this._mName.GetComponent<AllochroicLabelChecker>().RenderLabel(rankData.CharmRankValue);
		this._mName.text = rankData.name;
	}

	private void SetCharmInfo(CharmRankData charmData)
	{
		base.gameObject.name = charmData.UserId + "|" + charmData.MagicBottleRank.ToString();
		SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(charmData.IconId.ToString());
		if (dataById != null)
		{
			this.Portrait.spriteName = dataById.headportrait_icon.ToString();
		}
		SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(charmData.PictureFrame.ToString());
		if (dataById2 != null)
		{
			this.Frame.spriteName = dataById2.pictureframe_icon;
		}
		this._cName.text = charmData.NickName;
		this.CharmNum.text = charmData.Charm.ToString();
		if (charmData.Rank == 1)
		{
			this.Bg.spriteName = "Ranking_list_bg_02";
			this._cName.color = new Color32(212, 254, 249, 255);
			this.CharmNum.transform.Find("bg").GetComponent<UISprite>().color = new Color32(52, 12, 0, 155);
		}
		else
		{
			this.Bg.spriteName = "Ranking_list_bg_01";
			this._cName.color = new Color32(212, 254, 249, 255);
			this.CharmNum.transform.Find("bg").GetComponent<UISprite>().color = new Color32(0, 18, 52, 155);
		}
		if (charmData.Rank <= 3 && charmData.Rank > 0)
		{
			this.RankSprite.gameObject.SetActive(true);
			this.RankLabel.gameObject.SetActive(false);
			this.RankSprite.spriteName = "Ranking_images_number_" + charmData.Rank.ToString();
		}
		else
		{
			this.RankSprite.gameObject.SetActive(false);
			this.RankLabel.gameObject.SetActive(true);
			this.RankLabel.text = charmData.Rank.ToString();
		}
		if (charmData.UserId == ModelManager.Instance.Get_userData_X().UserId)
		{
			this._cName.color = new Color32(214, 7, 182, 255);
			this._cName.text = ModelManager.Instance.Get_userData_filed_X("NickName");
		}
		this._cName.GetComponent<AllochroicLabelChecker>().RenderLabel(charmData.CharmRankValue);
	}

	private void SetRankUp(int upValue)
	{
		if (upValue == 0)
		{
			this.Equal.gameObject.SetActive(true);
			this.RankChange.gameObject.SetActive(false);
		}
		else if (upValue > 0)
		{
			this.Equal.gameObject.SetActive(false);
			this.RankChange.gameObject.SetActive(true);
			this.ChangeNum.text = Mathf.Abs(upValue).ToString();
		}
		else
		{
			this.Equal.gameObject.SetActive(false);
			this.RankChange.gameObject.SetActive(true);
			this.ChangeNum.text = Mathf.Abs(upValue).ToString();
		}
	}

	private void GetState(double score, Dictionary<string, object> rankStageDic)
	{
		string id = string.Empty;
		double num = score;
		rankStageDic = BaseDataMgr.instance.GetDicByType<SysRankStageVo>();
		int num2 = 0;
		for (int i = 1; i <= rankStageDic.Keys.Count; i++)
		{
			if (num >= (double)(rankStageDic[i.ToString()] as SysRankStageVo).StageScore)
			{
				num2 = i;
			}
		}
		if (num2 == 0)
		{
			num2 = 1;
		}
		SysRankStageVo sysRankStageVo = rankStageDic[num2.ToString()] as SysRankStageVo;
		id = sysRankStageVo.StageName;
		this.RankLv.text = LanguageManager.Instance.GetStringById(id);
		this.RankLv.gradientTop = ModelManager.Instance.Get_ColorByString_X(sysRankStageVo.GradientTop);
		this.RankLv.gradientBottom = ModelManager.Instance.Get_ColorByString_X(sysRankStageVo.GradientBottom);
		this.RankIcon.spriteName = sysRankStageVo.StageImg.ToString();
		this.RankPoint.text = score.ToString("F0");
	}
}
