using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaMessageData;
using MobaProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class ChangeAvatarView : BaseView<ChangeAvatarView>
	{
		private Transform CloseBg;

		private Transform AllBtn;

		private Transform OutlookBtn;

		private Transform ActiveBtn;

		private Transform AchievBtn;

		private UILabel TitleLabel;

		private UIGrid CenterGrid;

		private UISprite SampleAvatar;

		private UISprite SampleFrame;

		private UILabel SampleName;

		private UILabel SampleDesc;

		private Transform UseBtn;

		private Transform BuyBtn;

		private Transform OtherBtn;

		private bool isHeadPortraitPanel;

		private string selectPortraitName;

		private string selectFrameName;

		private Dictionary<string, SysSummonersHeadportraitVo> HeadportraitVo = new Dictionary<string, SysSummonersHeadportraitVo>();

		private Dictionary<string, SysSummonersPictureframeVo> PictureframeVo = new Dictionary<string, SysSummonersPictureframeVo>();

		private List<SysSummonersHeadportraitVo> heroPortraitList = new List<SysSummonersHeadportraitVo>();

		private List<string> OwnAvatar = new List<string>();

		private List<string[]> AvatarStateLst = new List<string[]>();

		private List<string> OwnFrame = new List<string>();

		private List<string[]> FrameStateLst = new List<string[]>();

		private List<SysSummonersPictureframeVo> FrameList = new List<SysSummonersPictureframeVo>();

		private ChangeAvatarItem _avatarItem;

		public int Avatar;

		public string PictureFrame;

		private string skipPage = string.Empty;

		private string[] skipToActive;

		public ChangeAvatarView()
		{
			this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/Summoner/ChangeAvatarView");
		}

		public override void Init()
		{
			base.Init();
			this.CloseBg = this.transform.Find("BackBtn");
			this.AllBtn = this.transform.Find("TypeButton/All");
			this.OutlookBtn = this.transform.Find("TypeButton/1");
			this.ActiveBtn = this.transform.Find("TypeButton/2");
			this.AchievBtn = this.transform.Find("TypeButton/3");
			this.TitleLabel = this.transform.Find("Title").GetComponent<UILabel>();
			this.CenterGrid = this.transform.Find("Panel/Center/Grid").GetComponent<UIGrid>();
			this.SampleAvatar = this.transform.Find("SampleAvatar").GetComponent<UISprite>();
			this.SampleFrame = this.transform.Find("SampleFrame").GetComponent<UISprite>();
			this.SampleName = this.transform.Find("SampleName").GetComponent<UILabel>();
			this.SampleDesc = this.transform.Find("SampleDescribe").GetComponent<UILabel>();
			this.UseBtn = this.transform.Find("ButtonMag/UseBtn");
			this.BuyBtn = this.transform.Find("ButtonMag/BuyBtn");
			this.OtherBtn = this.transform.Find("ButtonMag/OtherBtn");
			this._avatarItem = Resources.Load<ChangeAvatarItem>("Prefab/UI/Summoner/ChangeAvatarItem");
			UIEventListener.Get(this.AllBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_SelectType);
			UIEventListener.Get(this.OutlookBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_SelectType);
			UIEventListener.Get(this.ActiveBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_SelectType);
			UIEventListener.Get(this.AchievBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_SelectType);
			UIEventListener.Get(this.UseBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_UseBtn);
			UIEventListener.Get(this.CloseBg.gameObject).onClick = new UIEventListener.VoidDelegate(this.Click_CloseView);
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.ModfiyAvatar, new MobaMessageFunc(this.OnGetMsg_ModfiyAvatar));
			MVC_MessageManager.AddListener_view(MobaGameCode.ModfiyAvatar, new MobaMessageFunc(this.OnGetMsg_ModfiyAvatar2));
			MVC_MessageManager.AddListener_view(MobaGameCode.ShowIconFrame, new MobaMessageFunc(this.OnGetMsg_ShowIconFrame));
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ModfiyAvatar, new MobaMessageFunc(this.OnGetMsg_ModfiyAvatar));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ModfiyAvatar, new MobaMessageFunc(this.OnGetMsg_ModfiyAvatar2));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ShowIconFrame, new MobaMessageFunc(this.OnGetMsg_ShowIconFrame));
			this.AvatarStateLst.Clear();
			this.FrameStateLst.Clear();
		}

		public void ReFreshUI(bool isHead, int avatar, int frame)
		{
			this.isHeadPortraitPanel = isHead;
			this.Avatar = avatar;
			this.PictureFrame = frame.ToString();
			this.AllBtn.GetComponent<UIToggle>().startsActive = true;
			if (isHead)
			{
				this.TitleLabel.text = "召唤师头像";
				this.SampleAvatar.gameObject.SetActive(true);
				this.SampleFrame.gameObject.SetActive(false);
				this.AllBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_all");
				this.OutlookBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_appearanc");
				this.ActiveBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_activity");
				this.AchievBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_achievement");
				this.HeadportraitVo = BaseDataMgr.instance.GetTypeDicByType<SysSummonersHeadportraitVo>();
				this.OwnAvatar = CharacterDataMgr.instance.GetPortrait(CharacterDataMgr.Portrait.CommonPortrait);
				this.selectPortraitName = this.Avatar.ToString();
				for (int i = 0; i < this.OwnAvatar.Count; i++)
				{
					string[] array = this.OwnAvatar[i].Split(new char[]
					{
						'_'
					});
					if (array.Length == 2)
					{
						this.AvatarStateLst.Add(array);
					}
				}
				this.ReFreshLeftBtn();
			}
			else
			{
				this.TitleLabel.text = "召唤师头像框";
				this.SampleAvatar.gameObject.SetActive(false);
				this.SampleFrame.gameObject.SetActive(true);
				this.AllBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_all");
				this.OutlookBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_appearanc");
				this.ActiveBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_activity");
				this.AchievBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("headportrait_classify_achievement");
				this.PictureframeVo = BaseDataMgr.instance.GetTypeDicByType<SysSummonersPictureframeVo>();
				this.OwnFrame = CharacterDataMgr.instance.GetSummonerFrame();
				this.selectPortraitName = this.PictureFrame;
				for (int j = 0; j < this.OwnFrame.Count; j++)
				{
					string[] array2 = this.OwnFrame[j].Split(new char[]
					{
						'_'
					});
					if (array2.Length == 2)
					{
						this.FrameStateLst.Add(array2);
					}
				}
				this.ReFreshLeftBtnFrame();
				SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(this.PictureFrame);
				if (dataById != null)
				{
					this.SampleFrame.spriteName = dataById.pictureframe_icon;
				}
				else
				{
					this.SampleFrame.spriteName = "pictureframe_0000";
				}
			}
			SysSummonersHeadportraitVo headportraitVo = this.GetHeadportraitVo(this.Avatar.ToString());
			if (headportraitVo == null)
			{
				this.SampleAvatar.spriteName = "headportrait_0001";
			}
			else
			{
				this.SampleAvatar.spriteName = headportraitVo.headportrait_icon;
			}
			if (isHead)
			{
				this.SampleName.text = LanguageManager.Instance.GetStringById(headportraitVo.headportrait_name);
				this.SampleDesc.text = LanguageManager.Instance.GetStringById(headportraitVo.headportrait_describe);
			}
			else
			{
				SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(this.PictureFrame);
				if (dataById2 == null)
				{
					return;
				}
				this.SampleName.text = LanguageManager.Instance.GetStringById(dataById2.pictureframe_name);
				this.SampleDesc.text = LanguageManager.Instance.GetStringById(dataById2.pictureframe_describe);
			}
			this.UpdateUseBtn(true);
			this.Click_SelectType(this.AllBtn.gameObject);
		}

		private void ReFreshLeftBtn()
		{
			bool flag = false;
			this.transform.Find("TypeButton/1/NewMark").gameObject.SetActive(false);
			this.transform.Find("TypeButton/2/NewMark").gameObject.SetActive(false);
			this.transform.Find("TypeButton/3/NewMark").gameObject.SetActive(false);
			for (int i = 0; i < this.AvatarStateLst.Count; i++)
			{
				if (this.AvatarStateLst[i][1] == "1")
				{
					SysSummonersHeadportraitVo headportraitVo = this.GetHeadportraitVo(this.AvatarStateLst[i][0]);
					if (headportraitVo != null)
					{
						int headportrait_classify = headportraitVo.headportrait_classify;
						this.transform.Find("TypeButton/" + headportrait_classify + "/NewMark").gameObject.SetActive(true);
						flag = true;
						break;
					}
				}
				else
				{
					flag = false;
				}
			}
			this.transform.Find("TypeButton/All/NewMark").gameObject.SetActive(flag);
			Singleton<SummonerView>.Instance.passportView.SwitchAvatarNewMark(flag);
		}

		private void ReFreshLeftBtnFrame()
		{
			bool flag = false;
			this.transform.Find("TypeButton/1/NewMark").gameObject.SetActive(false);
			this.transform.Find("TypeButton/2/NewMark").gameObject.SetActive(false);
			this.transform.Find("TypeButton/3/NewMark").gameObject.SetActive(false);
			for (int i = 0; i < this.FrameStateLst.Count; i++)
			{
				if (this.FrameStateLst[i][1] == "1")
				{
					SysSummonersPictureframeVo pictureframeVo = this.GetPictureframeVo(this.FrameStateLst[i][0]);
					if (pictureframeVo != null)
					{
						int pictureframe_classify = pictureframeVo.pictureframe_classify;
						this.transform.Find("TypeButton/" + pictureframe_classify + "/NewMark").gameObject.SetActive(true);
						flag = true;
						break;
					}
				}
				else
				{
					flag = false;
				}
			}
			this.transform.Find("TypeButton/All/NewMark").gameObject.SetActive(flag);
			Singleton<SummonerView>.Instance.passportView.SwitchFrameNewMark(flag);
		}

		private SysSummonersHeadportraitVo GetHeadportraitVo(string idx)
		{
			return BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(idx);
		}

		private SysSummonersPictureframeVo GetPictureframeVo(string idx)
		{
			return BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(idx);
		}

		private void Click_CloseView(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.ChangeAvatarView);
		}

		private void Click_SelectType(GameObject go)
		{
			if (this.isHeadPortraitPanel)
			{
				this.SampleFrame.gameObject.SetActive(false);
				this.heroPortraitList = new List<SysSummonersHeadportraitVo>(this.HeadportraitVo.Values);
				this.heroPortraitList = (from item in this.heroPortraitList
				orderby item.headportrait_quality, item.headportrait_order
				select item).ToList<SysSummonersHeadportraitVo>();
			}
			else
			{
				this.SampleFrame.gameObject.SetActive(true);
				this.FrameList = new List<SysSummonersPictureframeVo>(this.PictureframeVo.Values);
				this.FrameList = (from item in this.FrameList
				orderby item.pictureframe_quality, item.pictureframe_order
				select item).ToList<SysSummonersPictureframeVo>();
			}
			string name = go.name;
			switch (name)
			{
			case "1":
				if (this.isHeadPortraitPanel)
				{
					this.heroPortraitList = this.heroPortraitList.FindAll((SysSummonersHeadportraitVo item) => item.headportrait_classify == 1);
				}
				else
				{
					this.FrameList = this.FrameList.FindAll((SysSummonersPictureframeVo item) => item.pictureframe_classify == 1);
				}
				break;
			case "2":
				if (this.isHeadPortraitPanel)
				{
					this.heroPortraitList = this.heroPortraitList.FindAll((SysSummonersHeadportraitVo item) => item.headportrait_classify == 2);
				}
				else
				{
					this.FrameList = this.FrameList.FindAll((SysSummonersPictureframeVo item) => item.pictureframe_classify == 2);
				}
				break;
			case "3":
				if (this.isHeadPortraitPanel)
				{
					this.heroPortraitList = this.heroPortraitList.FindAll((SysSummonersHeadportraitVo item) => item.headportrait_classify == 3);
				}
				else
				{
					this.FrameList = this.FrameList.FindAll((SysSummonersPictureframeVo item) => item.pictureframe_classify == 3);
				}
				break;
			}
			if (this.isHeadPortraitPanel)
			{
				this.UpdatePortrait(this.heroPortraitList);
			}
			else
			{
				this.UpdatePortraitFrame(this.FrameList);
			}
			if (this.selectPortraitName != null)
			{
				this.UpdateClickFrame(this.selectPortraitName);
			}
			this.transform.Find("Panel/Center").GetComponent<UIScrollView>().ResetPosition();
			this.transform.Find("Panel/Center").GetComponent<UIScrollView>().considerInactive = false;
			this.transform.Find("Panel/Center").GetComponent<UIScrollView>().mCalculatedBounds = false;
		}

		public void UpdateSample(string _id)
		{
			this.UpdateClickFrame(_id);
			this.selectPortraitName = _id;
			if (this.isHeadPortraitPanel)
			{
				SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(_id);
				this.SampleAvatar.spriteName = dataById.headportrait_icon;
				this.SampleName.text = LanguageManager.Instance.GetStringById(dataById.headportrait_name);
				this.SampleDesc.text = LanguageManager.Instance.GetStringById(dataById.headportrait_describe);
				if (this.AvatarStateLst.Find((string[] item) => item[0] == _id) != null)
				{
					if (_id == this.Avatar.ToString())
					{
						this.UpdateUseBtn(true);
					}
					else
					{
						this.UpdateUseBtn(false);
					}
				}
				else
				{
					this.UpdateBtnMng(dataById.headportrait_skip);
				}
			}
			else
			{
				SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(_id);
				this.SampleFrame.spriteName = dataById2.pictureframe_icon;
				this.SampleName.text = LanguageManager.Instance.GetStringById(dataById2.pictureframe_name);
				this.SampleDesc.text = LanguageManager.Instance.GetStringById(dataById2.pictureframe_describe);
				if (this.FrameStateLst.Find((string[] item) => item[0] == _id) != null)
				{
					if (_id == this.PictureFrame.ToString())
					{
						this.UpdateUseBtn(true);
					}
					else
					{
						this.UpdateUseBtn(false);
					}
				}
				else
				{
					this.UpdateBtnMng(dataById2.pictureframe_skip);
				}
			}
		}

		private void UpdateClickFrame(string _id)
		{
			if (this.selectPortraitName != _id)
			{
				if (null != this.CenterGrid.transform.Find(this.selectPortraitName))
				{
					this.CenterGrid.transform.Find(this.selectPortraitName + "/Click").gameObject.SetActive(false);
				}
				this.CenterGrid.transform.Find(_id + "/Click").gameObject.SetActive(true);
			}
		}

		private void UpdateUseBtn(bool isusing)
		{
			if (isusing)
			{
				this.UseBtn.gameObject.SetActive(false);
				this.BuyBtn.gameObject.SetActive(false);
				this.OtherBtn.gameObject.SetActive(false);
			}
			else
			{
				this.UseBtn.gameObject.SetActive(true);
				this.BuyBtn.gameObject.SetActive(false);
				this.OtherBtn.gameObject.SetActive(false);
			}
		}

		private void UpdateBtnMng(string skip)
		{
			this.UseBtn.gameObject.SetActive(false);
			this.BuyBtn.gameObject.SetActive(false);
			this.OtherBtn.gameObject.SetActive(true);
			this.OtherBtn.GetComponent<BoxCollider>().enabled = true;
			string[] array = skip.Split(new char[]
			{
				'|'
			});
			string text = array[0];
			switch (text)
			{
			case "1":
				this.UseBtn.gameObject.SetActive(false);
				this.BuyBtn.gameObject.SetActive(true);
				this.OtherBtn.gameObject.SetActive(false);
				UIEventListener.Get(this.BuyBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToShop);
				break;
			case "2":
				this.OtherBtn.Find("Label").GetComponent<UILabel>().text = "前往签到";
				UIEventListener.Get(this.OtherBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToSign);
				this.skipPage = array[1];
				break;
			case "3":
				this.OtherBtn.Find("Label").GetComponent<UILabel>().text = "前往活动";
				UIEventListener.Get(this.OtherBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToActvityView);
				this.skipToActive = array;
				break;
			case "4":
				this.OtherBtn.Find("Label").GetComponent<UILabel>().text = "前往成就";
				UIEventListener.Get(this.OtherBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToTaskView);
				this.skipPage = array[1];
				break;
			case "5":
				this.OtherBtn.Find("Label").GetComponent<UILabel>().text = "前往小魔瓶";
				UIEventListener.Get(this.OtherBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToMaggicBottle);
				break;
			case "6":
				this.OtherBtn.Find("Label").GetComponent<UILabel>().text = "前往排行榜";
				UIEventListener.Get(this.OtherBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToRank);
				this.skipPage = array[1];
				break;
			case "7":
				if (array[1] != null)
				{
					this.OtherBtn.Find("Label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById(array[1]);
				}
				this.OtherBtn.GetComponent<BoxCollider>().enabled = false;
				break;
			}
		}

		private void GoToShop(GameObject go)
		{
			CtrlManager.OpenWindow(WindowID.PurchasePopupView, null);
			Singleton<PurchasePopupView>.Instance.onSuccess.Add(new Callback(this.AfterBuyAvatar));
			if (this.isHeadPortraitPanel)
			{
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.HeadPortrait, this.selectPortraitName, 1, false);
			}
			else
			{
				Singleton<PurchasePopupView>.Instance.Show(GoodsSubject.HeadPortraitFrame, this.selectPortraitName, 1, false);
			}
		}

		private void AfterBuyAvatar()
		{
			this.BuyBtn.gameObject.SetActive(false);
			this.UseBtn.gameObject.SetActive(true);
			Transform transform = this.CenterGrid.transform.Find(this.selectPortraitName + "/State");
			transform.gameObject.SetActive(true);
			transform.GetComponent<UILabel>().text = "已拥有";
			transform.GetComponent<UILabel>().color = new Color32(11, 233, 0, 255);
			this.CenterGrid.transform.Find(this.selectPortraitName + "/PriceTag").gameObject.SetActive(false);
			if (this.isHeadPortraitPanel)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ShowIconFrame, param, new object[]
				{
					int.Parse(this.selectPortraitName),
					1
				});
			}
			else
			{
				SendMsgManager.SendMsgParam param2 = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ShowIconFrame, param2, new object[]
				{
					int.Parse(this.selectPortraitName),
					2
				});
			}
		}

		private void GoToSign(GameObject go)
		{
			if (this.CanJumpToSign())
			{
				CtrlManager.OpenWindow(WindowID.SignView, null);
			}
			else
			{
				Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
			}
		}

		private bool CanJumpToSign()
		{
			bool result = false;
			int week = ModelManager.Instance.Get_GetSignDay_X().week;
			for (int i = 1; i <= 7; i++)
			{
				int num = i + week * 7;
				SysAttendanceRewardsVo dataById = BaseDataMgr.instance.GetDataById<SysAttendanceRewardsVo>(num.ToString());
				if (dataById != null)
				{
					SysDropRewardsVo dataById2 = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(dataById.rewards.ToString());
					if (dataById2 != null)
					{
						SysDropItemsVo dataById3 = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(dataById2.drop_items);
						if (dataById3 != null)
						{
							string[] array = dataById3.rewards.Split(new char[]
							{
								'|'
							});
							if ("3" == array[0] && (array[1] == "3" || array[1] == "4") && this.skipPage == array[2])
							{
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private void GoToTaskView(GameObject go)
		{
			CtrlManager.OpenWindow(WindowID.TaskView, null);
			Singleton<TaskView>.Instance.ClickPanel(this.skipPage);
		}

		private void GoToActvityView(GameObject go)
		{
			int num = 0;
			int activity_typeID = 100;
			if (this.skipToActive.Length > 2)
			{
				num = int.Parse(this.skipToActive[1]);
				activity_typeID = int.Parse(this.skipToActive[2]);
			}
			if (!this.CanJumpToActive(num))
			{
				Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
				return;
			}
			CtrlManager.OpenWindow(WindowID.ActivityView, null);
			MsgData_Activity_setCurActivity param = new MsgData_Activity_setCurActivity
			{
				activity_typeID = activity_typeID,
				activity_id = num
			};
			MobaMessageManagerTools.SendClientMsg(ClientV2V.Activity_setCurActivity, param, false);
		}

		private bool CanJumpToActive(int actId)
		{
			bool result = false;
			DateTime serverCurrentTime = ToolsFacade.ServerCurrentTime;
			SysActivityVo dataById = BaseDataMgr.instance.GetDataById<SysActivityVo>(actId.ToString());
			if (dataById != null)
			{
				string[] array = dataById.start_time.Split(new char[]
				{
					'|'
				});
				string[] array2 = dataById.end_time.Split(new char[]
				{
					'|'
				});
				DateTime t = new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]));
				DateTime t2 = new DateTime(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]));
				if (serverCurrentTime > t && serverCurrentTime < t2)
				{
					result = true;
				}
				else if ("[]" == dataById.start_time)
				{
					result = true;
				}
			}
			return result;
		}

		private void GoToMaggicBottle(GameObject go)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
		}

		private void GoToRank(GameObject go)
		{
			if (this.skipPage == "1")
			{
				Singleton<RankView>.Instance.selectBtn = "Ladder";
			}
			else if (this.skipPage == "2")
			{
				Singleton<RankView>.Instance.selectBtn = "MagicBottle";
			}
			else
			{
				Singleton<RankView>.Instance.selectBtn = "Charming";
			}
			CtrlManager.OpenWindow(WindowID.RankView, null);
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
		}

		private void UpdatePortrait(List<SysSummonersHeadportraitVo> Lst)
		{
			bool isOwn = true;
			bool isNew = true;
			bool isVisiable = true;
			GridHelper.FillGrid<ChangeAvatarItem>(this.CenterGrid, this._avatarItem, Lst.Count, delegate(int idx, ChangeAvatarItem comp)
			{
				if (this.AvatarStateLst.Find((string[] item) => item[0] == Lst[idx].headportrait_id.ToString()) != null)
				{
					isOwn = true;
					string[] array = this.AvatarStateLst.Find((string[] item) => item[0] == Lst[idx].headportrait_id.ToString());
					if (array[1] == "0")
					{
						isNew = false;
					}
					else
					{
						isNew = true;
					}
				}
				else
				{
					isOwn = false;
					isNew = false;
				}
				if (Lst[idx].headportrait_classify == 3)
				{
					string[] array2 = Lst[idx].headportrait_skip.Split(new char[]
					{
						'|'
					});
					if (array2 != null && array2.Length == 3)
					{
						string unikey = array2[1];
						SysActivityVo dataById = BaseDataMgr.instance.GetDataById<SysActivityVo>(unikey);
						DateTime dateTime = ActivityTools.GetDateTime(dataById.start_time, true);
						DateTime dateTime2 = ActivityTools.GetDateTime(dataById.end_time, true);
						isVisiable = ToolsFacade.Instance.IsInTimeInterval(dateTime, dateTime2);
					}
				}
				if ((Lst[idx].is_hidden == 1 && isVisiable) || isOwn)
				{
					comp.UpdateAvatarItem(Lst[idx], isOwn, isNew);
					comp.transform.name = Lst[idx].headportrait_id.ToString();
					comp.transform.gameObject.SetActive(true);
					if (Lst[idx].headportrait_id.ToString() == this.selectPortraitName)
					{
						comp.transform.Find("Click").gameObject.SetActive(true);
					}
					else
					{
						comp.transform.Find("Click").gameObject.SetActive(false);
					}
				}
				else
				{
					comp.gameObject.SetActive(false);
				}
			});
			this.CenterGrid.Reposition();
		}

		private void UpdatePortraitFrame(List<SysSummonersPictureframeVo> Lst)
		{
			bool isOwn = true;
			bool isNew = true;
			bool isVisiable = true;
			GridHelper.FillGrid<ChangeAvatarItem>(this.CenterGrid, this._avatarItem, Lst.Count, delegate(int idx, ChangeAvatarItem comp)
			{
				if (this.FrameStateLst.Find((string[] item) => item[0] == Lst[idx].pictureframe_id.ToString()) != null)
				{
					isOwn = true;
					string[] array = this.FrameStateLst.Find((string[] item) => item[0] == Lst[idx].pictureframe_id.ToString());
					if (array[1] == "0")
					{
						isNew = false;
					}
					else
					{
						isNew = true;
					}
				}
				else
				{
					isOwn = false;
					isNew = false;
				}
				if (Lst[idx].pictureframe_classify == 3)
				{
					string[] array2 = Lst[idx].pictureframe_skip.Split(new char[]
					{
						'|'
					});
					if (array2 != null && array2.Length == 3)
					{
						string unikey = array2[1];
						SysActivityVo dataById = BaseDataMgr.instance.GetDataById<SysActivityVo>(unikey);
						DateTime dateTime = ActivityTools.GetDateTime(dataById.start_time, true);
						DateTime dateTime2 = ActivityTools.GetDateTime(dataById.end_time, true);
						isVisiable = ToolsFacade.Instance.IsInTimeInterval(dateTime, dateTime2);
					}
				}
				if ((Lst[idx].is_hidden == 1 && isVisiable) || isOwn)
				{
					comp.UpdateAvatarItem(Lst[idx], isOwn, isNew);
					comp.transform.name = Lst[idx].pictureframe_id.ToString();
					comp.transform.gameObject.SetActive(true);
					if (Lst[idx].pictureframe_id.ToString() == this.selectPortraitName)
					{
						comp.transform.Find("Click").gameObject.SetActive(true);
					}
					else
					{
						comp.transform.Find("Click").gameObject.SetActive(false);
					}
				}
				else
				{
					comp.gameObject.SetActive(false);
				}
			});
			this.CenterGrid.Reposition();
		}

		private void UseBtnState(bool isAvail)
		{
			UILabel component = this.UseBtn.Find("Label").GetComponent<UILabel>();
			if (isAvail)
			{
				component.applyGradient = true;
				component.effectColor = new Color32(127, 60, 6, 255);
				this.UseBtn.transform.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				component.applyGradient = false;
				component.effectColor = new Color32(75, 69, 70, 255);
				this.UseBtn.transform.GetComponent<BoxCollider>().enabled = false;
			}
		}

		private void Click_UseBtn(GameObject go)
		{
			if (this.isHeadPortraitPanel)
			{
				for (int i = 0; i < this.selectPortraitName.ToCharArray().Length; i++)
				{
					if (this.selectPortraitName.ToCharArray()[i] < '\0' || this.selectPortraitName.ToCharArray()[i] > 'c')
					{
						return;
					}
				}
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ModfiyAvatar, param, new object[]
				{
					int.Parse(this.selectPortraitName),
					1
				});
			}
			else
			{
				SendMsgManager.SendMsgParam param2 = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ModfiyAvatar, param2, new object[]
				{
					int.Parse(this.selectPortraitName),
					2
				});
			}
		}

		private void ChangeAvatat(string _id)
		{
			UILabel component = this.CenterGrid.transform.Find(_id + "/State").GetComponent<UILabel>();
			component.text = "使用中";
			component.color = new Color32(255, 246, 10, 255);
			if (this.isHeadPortraitPanel)
			{
				if (null != this.CenterGrid.transform.Find(this.Avatar + "/State"))
				{
					UILabel component2 = this.CenterGrid.transform.Find(this.Avatar + "/State").GetComponent<UILabel>();
					component2.text = "已拥有";
					component2.color = new Color32(11, 233, 0, 255);
				}
				this.Avatar = int.Parse(_id);
			}
			else
			{
				if (null != this.CenterGrid.transform.Find(this.PictureFrame + "/State"))
				{
					UILabel component2 = this.CenterGrid.transform.Find(this.PictureFrame + "/State").GetComponent<UILabel>();
					component2.text = "已拥有";
					component2.color = new Color32(11, 233, 0, 255);
				}
				this.PictureFrame = _id;
			}
			this.UseBtn.gameObject.SetActive(false);
		}

		private void OnGetMsg_ModfiyAvatar(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.TryModfiyIconCallBack(num, operationResponse.DebugMessage);
			}
			else
			{
				this.TryModfiyIconCallBack(num, operationResponse.DebugMessage);
			}
		}

		private void TryModfiyIconCallBack(int i = 0, string str = null)
		{
			if (i != 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText("未知错误", 1f);
				Singleton<SummonerView>.Instance.passportView.RefreshUI();
			}
			else
			{
				this.UseBtn.gameObject.SetActive(false);
				this.ChangeAvatat(this.selectPortraitName);
				Singleton<MenuTopBarView>.Instance.RefreshUI();
				Singleton<SummonerView>.Instance.passportView.RefreshUI();
			}
		}

		private void OnGetMsg_ModfiyAvatar2(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.ClickHeroHeadCallBack(num, operationResponse.DebugMessage);
			}
			else
			{
				this.ClickHeroHeadCallBack(num, operationResponse.DebugMessage);
			}
		}

		private void ClickHeroHeadCallBack(int i = 0, string str = null)
		{
			if (i != 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText("未知错误", 1f);
				Singleton<MenuTopBarView>.Instance.RefreshUI();
				Singleton<SummonerView>.Instance.passportView.RefreshUI();
			}
			else
			{
				Singleton<MenuTopBarView>.Instance.RefreshUI();
				Singleton<SummonerView>.Instance.passportView.RefreshUI();
			}
		}

		private void OnGetMsg_ShowIconFrame(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				int num2 = (int)operationResponse.Parameters[58];
				byte b = (byte)operationResponse.Parameters[10];
				string text = null;
				if (b == 1)
				{
					this.OwnAvatar = CharacterDataMgr.instance.GetPortrait(CharacterDataMgr.Portrait.CommonPortrait);
					this.AvatarStateLst.Clear();
					for (int i = 0; i < this.OwnAvatar.Count; i++)
					{
						string[] array = this.OwnAvatar[i].Split(new char[]
						{
							'_'
						});
						if (array.Length == 2)
						{
							this.AvatarStateLst.Add(array);
						}
					}
					for (int j = 0; j < this.AvatarStateLst.Count; j++)
					{
						if (this.AvatarStateLst[j][0] == num2.ToString())
						{
							this.AvatarStateLst[j][1] = "0";
						}
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							this.AvatarStateLst[j][0],
							"_",
							this.AvatarStateLst[j][1],
							"|"
						});
					}
					ModelManager.Instance.Get_userData_X().OwnIconStr = text;
					this.ReFreshLeftBtn();
				}
				else
				{
					this.OwnFrame = CharacterDataMgr.instance.GetSummonerFrame();
					this.FrameStateLst.Clear();
					for (int k = 0; k < this.OwnFrame.Count; k++)
					{
						string[] array2 = this.OwnFrame[k].Split(new char[]
						{
							'_'
						});
						if (array2.Length == 2)
						{
							this.FrameStateLst.Add(array2);
						}
					}
					for (int l = 0; l < this.FrameStateLst.Count; l++)
					{
						if (this.FrameStateLst[l][0] == num2.ToString())
						{
							this.FrameStateLst[l][1] = "0";
						}
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							this.FrameStateLst[l][0],
							"_",
							this.FrameStateLst[l][1],
							"|"
						});
					}
					ModelManager.Instance.Get_userData_X().OwnPictureFrame = text;
					this.ReFreshLeftBtnFrame();
				}
				this.CenterGrid.transform.Find(num2 + "/NewPoint").gameObject.SetActive(false);
			}
		}
	}
}
