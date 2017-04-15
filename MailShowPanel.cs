using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MailShowPanel : BaseView<MailShowPanel>
{
	private Transform infoPanelBg;

	private Transform closeButton;

	private Transform getRewardButton;

	private Transform mailInfoPanel;

	private UILabel rd_title;

	private UILabel rd_content;

	private UILabel rd_addresser;

	private Transform rd_main;

	private Transform rewardPanel;

	private UITable rewardTable;

	private Transform moneyReward;

	private Transform dimReward;

	private Transform capReward;

	private Transform boxReward;

	private UILabel moneyCount;

	private UILabel dimCount;

	private UILabel capCount;

	private UILabel boxCount;

	private List<Transform> rewardChilden = new List<Transform>();

	private List<RewardModel> rewardModelList;

	public MailShowPanel()
	{
		this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/Mail/MailShowPanel");
	}

	public override void Init()
	{
		base.Init();
		this.mailInfoPanel = this.transform.transform.Find("Show/MailInfoPanel");
		this.rd_title = this.transform.FindChild("Show/Title").GetComponent<UILabel>();
		this.rd_content = this.mailInfoPanel.FindChild("Content").GetComponent<UILabel>();
		this.rd_addresser = this.mailInfoPanel.FindChild("Addresser").GetComponent<UILabel>();
		this.rd_main = this.mailInfoPanel.FindChild("Panel/Reward");
		this.getRewardButton = this.transform.Find("Show/Button");
		this.closeButton = this.transform.Find("Show/CloseButton");
		this.infoPanelBg = this.transform.Find("Show/Anchor/ShowBg");
		this.rewardPanel = this.mailInfoPanel.Find("Panel");
		this.rewardTable = this.rewardPanel.Find("Table").GetComponent<UITable>();
		this.moneyReward = this.rewardPanel.Find("Table/01");
		this.dimReward = this.rewardPanel.Find("Table/02");
		this.capReward = this.rewardPanel.Find("Table/03");
		this.boxReward = this.rewardPanel.Find("Table/04");
		this.moneyCount = this.rewardPanel.Find("Table/01/Label").GetComponent<UILabel>();
		this.dimCount = this.rewardPanel.Find("Table/02/Label").GetComponent<UILabel>();
		this.capCount = this.rewardPanel.Find("Table/03/Label").GetComponent<UILabel>();
		this.boxCount = this.rewardPanel.Find("Table/04/Label").GetComponent<UILabel>();
		UIEventListener expr_1CF = UIEventListener.Get(this.getRewardButton.gameObject);
		expr_1CF.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_1CF.onClick, new UIEventListener.VoidDelegate(this.GetReward1));
		UIEventListener expr_200 = UIEventListener.Get(this.closeButton.gameObject);
		expr_200.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_200.onClick, new UIEventListener.VoidDelegate(this.GetReward1));
	}

	public override void HandleAfterOpenView()
	{
	}

	public override void HandleBeforeCloseView()
	{
	}

	public override void RegisterUpdateHandler()
	{
	}

	public override void CancelUpdateHandler()
	{
	}

	public override void RefreshUI()
	{
	}

	public void ShowInfoPanel(MailData mailData, GameObject obj)
	{
		this.moneyReward.gameObject.SetActive(false);
		this.dimReward.gameObject.SetActive(false);
		this.capReward.gameObject.SetActive(false);
		this.boxReward.gameObject.SetActive(false);
		if (mailData == null)
		{
			return;
		}
		if (mailData.TemplateId == 0)
		{
			string[] array = mailData.Mail_Param.Split(new char[]
			{
				'|'
			});
			this.rd_title.text = array[0];
			this.rd_content.text = array[1];
			this.rd_addresser.text = array[2];
		}
		else
		{
			SysMailInfoVo dataById = BaseDataMgr.instance.GetDataById<SysMailInfoVo>(mailData.TemplateId.ToString());
			this.rd_title.text = LanguageManager.Instance.GetStringById(dataById.mail_title);
			string text = LanguageManager.Instance.GetStringById(dataById.mail_content);
			string[] array2 = mailData.Mail_Param.Split(new char[]
			{
				'|'
			});
			string[] array3 = text.Split(new char[]
			{
				'*'
			});
			text = string.Empty;
			for (int i = 0; i < array3.Length; i++)
			{
				if (i == array3.Length - 1)
				{
					text += array3[i];
				}
				else
				{
					text = text + array3[i] + array2[i];
				}
			}
			this.rd_content.text = text;
			this.rd_addresser.text = LanguageManager.Instance.GetStringById(dataById.addresser);
		}
		if (mailData.Mail_AwardList != null)
		{
			this.closeButton.gameObject.SetActive(false);
			this.getRewardButton.gameObject.SetActive(true);
			this.rd_main.gameObject.SetActive(true);
			foreach (RewardModel current in mailData.Mail_AwardList)
			{
				int type = current.Type;
				if (type != 1)
				{
					this.boxReward.gameObject.SetActive(true);
				}
				else if (current.Id == "1")
				{
					this.moneyCount.text = "x" + current.Count;
					this.moneyReward.gameObject.SetActive(true);
				}
				else if (current.Id == "2")
				{
					this.dimCount.text = "x" + current.Count;
					this.dimReward.gameObject.SetActive(true);
				}
				else if (current.Id == "9")
				{
					this.capCount.text = "x" + current.Count;
					this.capReward.gameObject.SetActive(true);
				}
			}
		}
		else
		{
			this.rd_main.gameObject.SetActive(false);
			this.closeButton.gameObject.SetActive(true);
			this.getRewardButton.gameObject.SetActive(false);
		}
		this.rewardTable.Reposition();
		this.rewardChilden = this.rewardTable.children;
		if (this.rewardChilden.Count == 1)
		{
			this.rd_main.GetComponent<UISprite>().height = 121;
		}
		else if (this.rewardChilden.Count > 1)
		{
			this.rd_main.GetComponent<UISprite>().height = this.rewardChilden.Count * 100;
		}
		this.GetReward(mailData);
	}

	private void GetReward(MailData mailData)
	{
		this.rewardModelList = mailData.Mail_AwardList;
		Singleton<MailView>.Instance.GetRewardResult(this.rewardModelList);
		Singleton<MenuBottomBarView>.Instance.CheckHeroState();
	}

	private void GetReward1(GameObject obj = null)
	{
		Singleton<MenuTopBarView>.Instance.RefreshUI();
		if (this.rewardModelList != null)
		{
			CtrlManager.OpenWindow(WindowID.GetItemsView, null);
			foreach (RewardModel item in this.rewardModelList)
			{
				switch (item.Type)
				{
				case 1:
					if (item.Id == "1")
					{
						MobaMessageManagerTools.GetItems_Coin(item.Count);
					}
					else if (item.Id == "2")
					{
						MobaMessageManagerTools.GetItems_Diamond(item.Count);
					}
					else if (item.Id == "9")
					{
						MobaMessageManagerTools.GetItems_Caps(item.Count);
					}
					else if (item.Id == "11")
					{
						ModelManager.Instance.Get_userData_X().Speaker += item.Count;
						MobaMessageManagerTools.GetItems_Speaker(item.Count);
					}
					break;
				case 2:
				{
					this.boxReward.gameObject.SetActive(true);
					SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(item.Id);
					if (dataById != null)
					{
						if (dataById.type == 4)
						{
							MobaMessageManagerTools.GetItems_Rune(int.Parse(item.Id));
						}
						else if (item.Id == "7777")
						{
							MobaMessageManagerTools.GetItems_Bottle(item.Count);
						}
						else
						{
							MobaMessageManagerTools.GetItems_GameItem(item.Id);
						}
					}
					break;
				}
				case 3:
					if (item.Id == "1")
					{
						Dictionary<string, SysHeroMainVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysHeroMainVo>();
						SysHeroMainVo sysHeroMainVo = typeDicByType.Values.FirstOrDefault((SysHeroMainVo val) => val.hero_id == item.Count);
						if (sysHeroMainVo != null)
						{
							string npc_id = sysHeroMainVo.npc_id;
							if (CharacterDataMgr.instance.OwenHeros.Contains(npc_id))
							{
								MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Hero, npc_id, false);
							}
							else
							{
								MobaMessageManagerTools.GetItems_Hero(npc_id);
								CharacterDataMgr.instance.UpdateHerosData();
							}
						}
						else
						{
							ClientLogger.Error("Unknown hero id: " + item.Count);
						}
					}
					else if (item.Id == "2")
					{
						if (ModelManager.Instance.IsPossessSkin(item.Count))
						{
							MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeroSkin, item.Count.ToString(), false);
						}
						else
						{
							ModelManager.Instance.GetNewHeroSkin(item.Count);
							MobaMessageManagerTools.GetItems_HeroSkin(item.Count);
						}
					}
					else if (item.Id == "3")
					{
						if (ModelManager.Instance.IsRepeatAvatar(item.Id, item.Count.ToString()))
						{
							MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.HeadPortrait, item.Count.ToString(), true);
						}
						else
						{
							MobaMessageManagerTools.GetItems_HeadPortrait(item.Count);
							ModelManager.Instance.GetNewAvatar(item.Id, item.Count.ToString());
						}
					}
					else if (item.Id == "4")
					{
						if (ModelManager.Instance.IsRepeatAvatar(item.Id, item.Count.ToString()))
						{
							MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.PortraitFrame, item.Count.ToString(), true);
						}
						else
						{
							MobaMessageManagerTools.GetItems_PortraitFrame(item.Count.ToString());
							ModelManager.Instance.GetNewAvatar(item.Id, item.Count.ToString());
						}
					}
					else if (item.Id == "5")
					{
						if (ModelManager.Instance.IsCouponRepeated(item.Count))
						{
							MobaMessageManagerTools.GetItems_Exchange(Com.Game.Module.ItemType.Coupon, item.Count.ToString(), true);
						}
						else
						{
							ModelManager.Instance.GetNewCoupon(item.Count.ToString());
							MobaMessageManagerTools.GetItems_Coupon(item.Count.ToString());
						}
					}
					break;
				case 4:
					MobaMessageManagerTools.GetItems_Exp(item.Count, ModelManager.Instance.Get_userData_X().Exp);
					break;
				case 6:
				{
					this.boxReward.gameObject.SetActive(true);
					SysGameBuffVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameBuffVo>(item.Id);
					if (dataById2 != null)
					{
						MobaMessageManagerTools.GetItems_GameBuff(Convert.ToInt32(item.Id));
						SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
					}
					break;
				}
				}
			}
			Singleton<GetItemsView>.Instance.Play();
		}
		CtrlManager.CloseWindow(WindowID.MailShowPanel);
	}
}
