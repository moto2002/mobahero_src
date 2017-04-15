using Assets.Scripts.Model;
using Com.Game.Module;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleSettlement
{
	public class Settlement_Surprise : MonoBehaviour
	{
		private void Awake()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23034, new MobaMessageFunc(this.onMsg_show));
		}

		private void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23034, new MobaMessageFunc(this.onMsg_show));
		}

		private void onMsg_show(MobaMessage msg)
		{
			CtrlManager.OpenWindow(WindowID.GetItemsView, null);
			ModelManager.Instance.Set_settle_2GetItemsView();
			ModelManager.Instance.Get_Settle_SavingData();
		}
	}
}
