using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class DetailsInfo : BaseView<DetailsInfo>
	{
		public UILabel Name;

		public UILabel TeamName;

		public UITexture Portrait;

		public UISprite PortraitFrame;

		public UILabel SummonerLv;

		public UILabel MobaID;

		public UILabel LikeCount;

		public UITexture LadderIcon;

		public UITexture MagicIcon;

		public UILabel LadderName;

		public UILabel LadderScore;

		public UILabel MagicRank;

		public UILabel Charm;

		public UILabel LadderCount;

		public UISprite LadderBar;

		public UILabel LadderPercent;

		public UILabel ChampionCount;

		public UISprite ChampionBar;

		public UILabel ChampionPercent;

		public UILabel FiveCount;

		public UISprite FiveBar;

		public UILabel FivePercent;

		public UILabel ThreeCount;

		public UISprite ThreeBar;

		public UILabel ThreePercent;

		public UILabel TripleKill_;

		public UILabel QuataryKill;

		public UILabel PentaKill;

		public UILabel Legendary;

		public Transform BackBtn;

		public Transform LikeBth;

		public DetailsInfo()
		{
			this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/Rank/DetailsInfo");
		}

		public override void Init()
		{
			base.Init();
			this.Name = this.transform.Find("Name").GetComponent<UILabel>();
			this.TeamName = this.transform.Find("TeamName").GetComponent<UILabel>();
			this.Portrait = this.transform.Find("Portrait").GetComponent<UITexture>();
			this.PortraitFrame = this.transform.Find("Portrait/Frame").GetComponent<UISprite>();
			this.SummonerLv = this.transform.Find("Portrait/GradeBack/gradeNumber").GetComponent<UILabel>();
			this.MobaID = this.transform.Find("MobaID").GetComponent<UILabel>();
			this.LikeCount = this.transform.Find("Like").GetComponent<UILabel>();
			this.LadderIcon = this.transform.Find("LadderIcon").GetComponent<UITexture>();
			this.MagicIcon = this.transform.Find("MaggicIcon").GetComponent<UITexture>();
			this.LadderName = this.transform.Find("LadderName").GetComponent<UILabel>();
			this.LadderScore = this.transform.Find("LadderScore").GetComponent<UILabel>();
			this.MagicRank = this.transform.Find("MagicRank").GetComponent<UILabel>();
			this.Charm = this.transform.Find("Charm").GetComponent<UILabel>();
			this.LadderCount = this.transform.Find("Ladder/LadderCount").GetComponent<UILabel>();
			this.LadderBar = this.transform.Find("Ladder/LadderBar").GetComponent<UISprite>();
			this.LadderPercent = this.transform.Find("Ladder/LadderPercent").GetComponent<UILabel>();
			this.ChampionCount = this.transform.Find("ChampionShip/ChampionCount").GetComponent<UILabel>();
			this.ChampionBar = this.transform.Find("ChampionShip/ChampionBar").GetComponent<UISprite>();
			this.ChampionPercent = this.transform.Find("ChampionShip/ChampionPercent").GetComponent<UILabel>();
			this.FiveCount = this.transform.Find("5v5/FiveCount").GetComponent<UILabel>();
			this.FiveBar = this.transform.Find("5v5/FiveBar").GetComponent<UISprite>();
			this.FivePercent = this.transform.Find("5v5/FivePercent").GetComponent<UILabel>();
			this.ThreeCount = this.transform.Find("3v3/ThreeCount").GetComponent<UILabel>();
			this.ThreeBar = this.transform.Find("3v3/ThreeBar").GetComponent<UISprite>();
			this.ThreePercent = this.transform.Find("3v3/ThreePercent").GetComponent<UILabel>();
			this.TripleKill_ = this.transform.Find("TripleKill").GetComponent<UILabel>();
			this.QuataryKill = this.transform.Find("QuataryKill").GetComponent<UILabel>();
			this.PentaKill = this.transform.Find("PentaKill").GetComponent<UILabel>();
			this.Legendary = this.transform.Find("Legendary").GetComponent<UILabel>();
			this.BackBtn = this.transform.Find("ClosePanel");
			this.LikeBth = this.transform.Find("LikeBtn");
			UIEventListener.Get(this.BackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnCloseBtn);
			UIEventListener.Get(this.LikeBth.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLike);
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
			this.LikeBth.transform.GetComponent<UISprite>().spriteName = "Settlement_praise_01";
			UIEventListener.Get(this.LikeBth.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickLike);
		}

		private void OnCloseBtn(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.DetailsInfo);
		}

		private void OnClickLike(GameObject go)
		{
			this.ClickLikeBtn(go);
		}

		public void ClickLikeBtn(GameObject go)
		{
			string value = ToolsFacade.ServerCurrentTime.ToString("y-M-d");
			string name = go.transform.parent.name;
			go.GetComponent<TweenScale>().PlayForward();
			go.transform.Find("fly").gameObject.SetActive(true);
			TweenPosition component = go.transform.Find("fly").GetComponent<TweenPosition>();
			component.ResetToBeginning();
			TweenAlpha component2 = go.transform.Find("fly").GetComponent<TweenAlpha>();
			component2.ResetToBeginning();
			component.PlayForward();
			component2.PlayForward();
			go.transform.GetComponent<UISprite>().spriteName = "Settlement_praise_02";
			UIEventListener.Get(go).onClick = null;
			if (SendMsgManager.Instance.SendMsg(MobaGameCode.SayGoodToSomeOne, null, new object[]
			{
				name
			}))
			{
				PlayerPrefs.SetString(name, value);
				PlayerPrefs.Save();
				this.LikeCount.text = ToolsFacade.Instance.GetMillionsSuffix(ModelManager.Instance.Get_GetPlayerData_X().likeCount + 1);
			}
		}

		public void ShowDetailsInfo_withoutBottleRank(string userId)
		{
			PlayerData playerData = ModelManager.Instance.Get_GetPlayerData_X();
			if (playerData == null)
			{
				return;
			}
			string b = ToolsFacade.ServerCurrentTime.ToString("y-M-d");
			CtrlManager.OpenWindow(WindowID.DetailsInfo, null);
			this.transform.localPosition = new Vector3(-373.8f, 84.21f, 0f);
			this.transform.name = userId;
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(playerData.Icon.ToString());
			if (dataById == null)
			{
				ClientLogger.Error("  Can't find Headportrait id=" + playerData.Icon + " in SysSummonersHeadportraitVo");
			}
			else
			{
				this.Portrait.mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon.ToString(), true, true, null, 0, false);
			}
			SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(playerData.Icon2.ToString());
			if (dataById2 == null)
			{
				ClientLogger.Error("  Can't find Headportrait frame id=" + playerData.Icon2 + " in SysSummonersPictureframeVo");
			}
			else
			{
				this.PortraitFrame.spriteName = dataById2.pictureframe_icon;
			}
			this.Name.text = playerData.SummName;
			this.Name.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(playerData.CharmRankvalue);
			this.SummonerLv.text = CharacterDataMgr.instance.GetUserLevel(playerData.SummLevel).ToString();
			this.MobaID.text = "(魔霸号：" + playerData.SummId + ")";
			this.LikeCount.text = ModelManager.Instance.Get_GetPlayerData_X().likeCount.ToString();
			if (userId == ModelManager.Instance.Get_userData_X().UserId)
			{
				this.LikeBth.gameObject.SetActive(false);
				this.LikeCount.gameObject.SetActive(false);
			}
			else
			{
				this.LikeBth.gameObject.SetActive(true);
				this.LikeCount.gameObject.SetActive(true);
			}
			this.GetState((double)playerData.LadderScore, playerData.LadderRank);
			int level = (playerData.bottlelevel != 0) ? playerData.bottlelevel : 1;
			this.MagicIcon.mainTexture = ResourceManager.Load<Texture>(Tools_ParsePrice.BottleLevelParse(level), true, true, null, 0, false);
			if (playerData.battleinfos != null)
			{
				this.BattleInfo(playerData.battleinfos);
			}
			this.Charm.text = playerData.usercp.ToString();
			this.MagicRank.text = level.ToString();
			this.MagicRank.transform.FindChild("Label").GetComponent<UILabel>().text = "小魔瓶等级";
			this.PentaKill.text = playerData.pentaKill.ToString();
			this.QuataryKill.text = playerData.quadraKill.ToString();
			this.TripleKill_.text = playerData.tripleKill.ToString();
			this.Legendary.text = playerData.godlike.ToString();
			if (PlayerPrefs.GetString(userId) == b)
			{
				this.LikeBth.GetComponent<UISprite>().spriteName = "Settlement_praise_02";
				UIEventListener.Get(this.LikeBth.gameObject).onClick = null;
			}
			else
			{
				this.LikeBth.GetComponent<UISprite>().spriteName = "Settlement_praise_01";
				UIEventListener.Get(this.LikeBth.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickLikeBtn);
			}
		}

		public void ShowDetailsInfo(string userinfo)
		{
			string text;
			string text2;
			if (userinfo.Contains('|'))
			{
				string[] array = userinfo.Split(new char[]
				{
					'|'
				});
				text = array[0];
				text2 = array[1];
			}
			else
			{
				text = userinfo;
				text2 = string.Empty;
			}
			PlayerData playerData = ModelManager.Instance.Get_GetPlayerData_X();
			if (playerData == null)
			{
				return;
			}
			string b = ToolsFacade.ServerCurrentTime.ToString("y-M-d");
			CtrlManager.OpenWindow(WindowID.DetailsInfo, null);
			this.transform.localPosition = new Vector3(-373.8f, 84.21f, 0f);
			this.transform.name = text;
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(playerData.Icon.ToString());
			if (dataById == null)
			{
				ClientLogger.Error("  Can't find Headportrait id=" + playerData.Icon + " in SysSummonersHeadportraitVo");
			}
			else
			{
				this.Portrait.mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon.ToString(), true, true, null, 0, false);
			}
			SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(playerData.Icon2.ToString());
			if (dataById2 == null)
			{
				ClientLogger.Error("  Can't find Headportrait frame id=" + playerData.Icon2 + " in SysSummonersPictureframeVo");
			}
			else
			{
				this.PortraitFrame.spriteName = dataById2.pictureframe_icon;
			}
			this.Name.text = playerData.SummName;
			this.Name.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(playerData.CharmRankvalue);
			this.SummonerLv.text = CharacterDataMgr.instance.GetUserLevel(playerData.SummLevel).ToString();
			this.MobaID.text = "(魔霸号：" + playerData.SummId + ")";
			this.LikeCount.text = ModelManager.Instance.Get_GetPlayerData_X().likeCount.ToString();
			if (text == ModelManager.Instance.Get_userData_X().UserId)
			{
				this.LikeBth.gameObject.SetActive(false);
				this.LikeCount.gameObject.SetActive(false);
			}
			else
			{
				this.LikeBth.gameObject.SetActive(true);
				this.LikeCount.gameObject.SetActive(true);
			}
			this.GetState((double)playerData.LadderScore, playerData.LadderRank);
			int level = (playerData.bottlelevel != 0) ? playerData.bottlelevel : 1;
			this.MagicIcon.mainTexture = ResourceManager.Load<Texture>(Tools_ParsePrice.BottleLevelParse(level), true, true, null, 0, false);
			if (playerData.battleinfos != null)
			{
				this.BattleInfo(playerData.battleinfos);
			}
			this.Charm.text = playerData.usercp.ToString();
			this.MagicRank.text = text2;
			this.PentaKill.text = playerData.pentaKill.ToString();
			this.QuataryKill.text = playerData.quadraKill.ToString();
			this.TripleKill_.text = playerData.tripleKill.ToString();
			this.Legendary.text = playerData.godlike.ToString();
			if (PlayerPrefs.GetString(text) == b)
			{
				this.LikeBth.GetComponent<UISprite>().spriteName = "Settlement_praise_02";
				UIEventListener.Get(this.LikeBth.gameObject).onClick = null;
			}
			else
			{
				this.LikeBth.GetComponent<UISprite>().spriteName = "Settlement_praise_01";
				UIEventListener.Get(this.LikeBth.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickLikeBtn);
			}
		}

		private void GetState(double score, int rank)
		{
			string id = string.Empty;
			SysRankStageVo sysRankStageVo = ModelManager.Instance.Get_LadderLevel(score, rank);
			if (sysRankStageVo == null)
			{
				return;
			}
			id = sysRankStageVo.StageName;
			this.LadderName.text = LanguageManager.Instance.GetStringById(id);
			this.LadderIcon.mainTexture = ResourceManager.Load<Texture>(sysRankStageVo.StageImg.ToString(), true, true, null, 0, false);
			this.LadderScore.text = score.ToString("F0");
		}

		private void BattleInfo(BattleInfoData[] battleinfos)
		{
			BattleInfoData battleInfoData = new BattleInfoData();
			for (int i = 0; i < battleinfos.Length; i++)
			{
				if (battleinfos[i].battleid == "80006")
				{
					int num = battleinfos[i].losecount + battleinfos[i].wincount;
					if (num == 0)
					{
						this.LadderCount.text = "0场";
						this.LadderBar.gameObject.SetActive(false);
						this.LadderPercent.text = "胜0%";
					}
					else
					{
						this.LadderCount.text = num.ToString() + "场";
						this.LadderPercent.text = "胜" + ((double)battleinfos[i].wincount / (double)num * 100.0).ToString("F0") + "%";
						this.LadderBar.gameObject.SetActive(true);
						this.LadderBar.width = (int)((double)battleinfos[i].wincount / (double)num * 331.0);
					}
				}
				else if (battleinfos[i].battleid == "800055")
				{
					int num2 = battleinfos[i].losecount + battleinfos[i].wincount;
					if (num2 == 0)
					{
						this.FiveCount.text = "0场";
						this.FiveBar.gameObject.SetActive(false);
						this.FivePercent.text = "胜0%";
					}
					else
					{
						this.FiveCount.text = num2.ToString() + "场";
						this.FivePercent.text = "胜" + ((double)battleinfos[i].wincount / (double)num2 * 100.0).ToString("F0") + "%";
						this.FiveBar.gameObject.SetActive(true);
						this.FiveBar.width = (int)((double)battleinfos[i].wincount / (double)num2 * 331.0);
					}
				}
				else if (battleinfos[i].battleid == "80003")
				{
					int num3 = battleinfos[i].losecount + battleinfos[i].wincount;
					if (num3 == 0)
					{
						this.ThreeCount.text = "0场";
						this.ThreeBar.gameObject.SetActive(false);
						this.ThreePercent.text = "胜0%";
					}
					else
					{
						this.ThreeCount.text = num3.ToString() + "场";
						this.ThreePercent.text = "胜" + ((double)battleinfos[i].wincount / (double)num3 * 100.0).ToString("F0") + "%";
						this.ThreeBar.gameObject.SetActive(true);
						this.ThreeBar.width = (int)((double)battleinfos[i].wincount / (double)num3 * 331.0);
					}
				}
				else if (battleinfos[i].battleid == "80022" || battleinfos[i].battleid == "80007")
				{
					battleInfoData.wincount += battleinfos[i].wincount;
					battleInfoData.losecount += battleinfos[i].losecount;
				}
			}
			int num4 = battleInfoData.losecount + battleInfoData.wincount;
			if (num4 == 0)
			{
				this.ChampionCount.text = "0场";
				this.ChampionBar.gameObject.SetActive(false);
				this.ChampionPercent.text = "胜0%";
			}
			else
			{
				this.ChampionCount.text = num4.ToString() + "场";
				this.ChampionPercent.text = "胜" + ((double)battleInfoData.wincount / (double)num4 * 100.0).ToString("F0") + "%";
				this.ChampionBar.gameObject.SetActive(true);
				this.ChampionBar.width = (int)((double)battleInfoData.wincount / (double)num4 * 331.0);
			}
		}
	}
}
