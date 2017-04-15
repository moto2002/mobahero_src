using Assets.Scripts.Model;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class BattleEquip_Top : MonoBehaviour
	{
		private UILabel lobel_money;

		private Transform trans_back;

		private int money;

		private void Awake()
		{
			this.Init();
			MobaMessageManager.RegistMessage((ClientMsg)23021, new MobaMessageFunc(this.OnMsg_walletChanged));
		}

		private void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23021, new MobaMessageFunc(this.OnMsg_walletChanged));
		}

		private void Init()
		{
			this.lobel_money = base.transform.parent.Find("GameObject/Coin/Money").GetComponent<UILabel>();
			this.trans_back = base.transform.Find("BackBtn");
			UIEventListener.Get(this.trans_back.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickBack);
			this.money = ModelManager.Instance.Get_BattleShop_money();
			this.RefreshUI_money();
		}

		private void OnClickBack(GameObject go)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickBack, null, false);
		}

		private void OnMsg_walletChanged(MobaMessage msg)
		{
			if (this.money != (int)msg.Param)
			{
				this.money = (int)msg.Param;
				this.RefreshUI_money();
			}
		}

		private void RefreshUI_money()
		{
			this.lobel_money.text = this.money.ToString();
		}
	}
}
