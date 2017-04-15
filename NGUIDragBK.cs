using System;
using UnityEngine;

public class NGUIDragBK : MonoBehaviour
{
	[SerializeField]
	private GameObject _dragMsgReceiver;

	private void OnDrag(Vector2 delta)
	{
		if (this._dragMsgReceiver)
		{
			this._dragMsgReceiver.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
		}
	}
}
