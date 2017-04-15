using Assets.Scripts.Model;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Com.Game.Module
{
	public class GameBuff_DoubleCard : Sub_DropItemBase
	{
		public GameBuff_DoubleCard() : base(DropItemID.DoubleCard)
		{
		}

		public override void Init(DropItemData data)
		{
			base.Init(data);
		}

		public override void SetData()
		{
			MobaMessageManagerTools.GetItems_GameBuff(base.ItemID);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetDoubleCard, null, new object[0]);
		}
	}
}
