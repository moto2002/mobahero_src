using Assets.Scripts.Model;
using Com.Game.Data;
using GUIFramework;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class RedPacketView : BaseView<RedPacketView>
	{
		private BottleBookItem m_Item;

		private UILabel m_Describe;

		private UILabel m_Num;

		private Transform m_BG;

		private CoroutineManager cMgr;

		private readonly string _textPrefix = "小梦衷心祝您 ";

		private readonly string[] _textArr = new string[]
		{
			"万事如意",
			"新年快乐",
			"大吉大利",
			"恭喜发财",
			"合家欢乐",
			"财源滚滚",
			"身体健康",
			"新春大吉",
			"吉祥如意",
			"福星高照",
			"鸿运当头",
			"好运连连",
			"恭贺新禧",
			"年年有余",
			"招财进宝",
			"吉星高照",
			"心想事成",
			"花开富贵",
			"幸福安康",
			"十全十美",
			"健康如意",
			"好运常伴",
			"笑口常开",
			"团团圆圆",
			"福禄双喜",
			"六六大顺",
			"事遂心愿"
		};

		public RedPacketView()
		{
			this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/Home/NewYearGift");
		}

		public override void Init()
		{
			base.Init();
			this.m_Describe = this.transform.Find("Describe").GetComponent<UILabel>();
			this.m_Num = this.transform.Find("Num").GetComponent<UILabel>();
			this.m_Item = this.transform.Find("BookItems").GetComponent<BottleBookItem>();
			this.m_BG = this.transform.Find("BG");
		}

		public override void HandleAfterOpenView()
		{
			this.transform.GetComponent<EffectPlayTool>().Play();
			AudioMgr.PlayUI("Play_UI_Reward_L3", null, false, false);
			if (this.cMgr == null)
			{
				this.cMgr = new CoroutineManager();
			}
			this.cMgr.StartCoroutine(this.DelayClick(), true);
		}

		public override void HandleBeforeCloseView()
		{
			this.cMgr.StopAllCoroutine();
		}

		[DebuggerHidden]
		private IEnumerator DelayClick()
		{
			RedPacketView.<DelayClick>c__Iterator14C <DelayClick>c__Iterator14C = new RedPacketView.<DelayClick>c__Iterator14C();
			<DelayClick>c__Iterator14C.<>f__this = this;
			return <DelayClick>c__Iterator14C;
		}

		private void CloseTheWindow(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.RedPacketView);
		}

		public void UpdateGiftData(int _type, int _count, string _name)
		{
			this.UpdateBookItem(_type.ToString(), _count);
			this.m_Num.text = string.Empty;
			this.m_Describe.text = _name + " 给您拜年";
			if (_type == 1)
			{
				ModelManager.Instance.Get_userData_X().Money += (long)_count;
			}
			else
			{
				ModelManager.Instance.Get_userData_X().Diamonds += (long)_count;
			}
			Singleton<MenuTopBarView>.Instance.RefreshUI();
		}

		private void UpdateBookItem(string type, int count)
		{
			this.m_Item.SetData(ItemType.Coin, type, true, true, count);
		}

		public void UpdateGiftData(HeroInfoData heroData)
		{
			this.m_Describe.text = this._textPrefix + this._textArr[UnityEngine.Random.Range(0, this._textArr.Length)];
			SysHeroMainVo sysHeroMainVo = this.m_Item.SetData(heroData, false);
			this.m_Num.text = LanguageManager.Instance.GetStringById(sysHeroMainVo.name);
		}

		public void UpdateGiftData(EquipmentInfoData equipData)
		{
			this.m_Describe.text = this._textPrefix + this._textArr[UnityEngine.Random.Range(0, this._textArr.Length)];
			SysGameItemsVo sysGameItemsVo = this.m_Item.SetData(equipData, true, true);
			this.m_Num.text = string.Empty;
		}

		public void UpdateGiftData(DropItemData dropItem)
		{
			this.m_Describe.text = this._textPrefix + this._textArr[UnityEngine.Random.Range(0, this._textArr.Length)];
			string dropItemStr = string.Format("{0}|{1}|{2}", dropItem.itemType, dropItem.itemId, dropItem.itemCount);
			int num;
			int count;
			ItemType itemType = ToolsFacade.Instance.AnalyzeDropItem(dropItemStr, out num, out count);
			this.m_Item.SetData(itemType, num.ToString(), true, true, count);
			this.m_Num.text = string.Empty;
		}
	}
}
