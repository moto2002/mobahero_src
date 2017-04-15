using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_Toggle : IComparable<GameVideo_Toggle>
	{
		public byte Priority;

		public string TabName;

		public string IconSpriteName;

		public ClientMsg ClickMsg;

		public UIToggle ComponentRef;

		public int CompareTo(GameVideo_Toggle other)
		{
			return this.Priority.CompareTo(other.Priority);
		}

		public void SendMsg()
		{
			MobaMessage message = MobaMessageManager.GetMessage(this.ClickMsg, null, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		public bool CheckGameObj(GameObject _target)
		{
			return this.ComponentRef.gameObject == _target;
		}
	}
}
