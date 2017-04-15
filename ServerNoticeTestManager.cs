using MobaProtocol.Data;
using System;
using UnityEngine;

public class ServerNoticeTestManager : MonoBehaviour
{
	public UILabel inputContentArea;

	public UILabel lifeTime;

	public UILabel fontStyle;

	public UIButton submit;

	public UIButton reset;

	public ServerNotice comp;

	private NotificationData data = new NotificationData();

	private void Start()
	{
		UIEventListener.Get(this.submit.gameObject).onClick = new UIEventListener.VoidDelegate(this.onSubmit);
		UIEventListener.Get(this.reset.gameObject).onClick = new UIEventListener.VoidDelegate(this.onReset);
	}

	private void Update()
	{
	}

	private void onSubmit(GameObject obj = null)
	{
		this.data.iparam = Convert.ToInt32(this.fontStyle.text);
		this.data.timeVal = Convert.ToInt32(this.lifeTime.text);
		this.data.Content = this.inputContentArea.text;
		MobaMessageManager.ExecuteMsg(MobaMessageManager.GetMessage((ClientMsg)23066, this.data, 0f));
	}

	private void onReset(GameObject obj = null)
	{
		this.fontStyle.text = string.Empty;
		this.lifeTime.text = string.Empty;
		this.inputContentArea.text = string.Empty;
	}
}
