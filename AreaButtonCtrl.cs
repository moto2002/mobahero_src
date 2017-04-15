using Assets.Scripts.Model;
using Com.Game.Module;
using System;
using UnityEngine;

public class AreaButtonCtrl : MonoBehaviour
{
	[SerializeField]
	private UILabel _serverName;

	[SerializeField]
	private UISprite _statePic;

	[SerializeField]
	private GameObject _chosenFrame;

	public string serverName
	{
		get
		{
			return this._serverName.text;
		}
		set
		{
			this._serverName.text = value;
		}
	}

	private void Awake()
	{
		UIEventListener.Get(base.transform.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickServerBtn);
	}

	public void OnClickServerBtn(GameObject obj = null)
	{
		this.SetChosen(!this._chosenFrame.activeInHierarchy);
	}

	public void SetState(AreaState state)
	{
		switch (state)
		{
		case AreaState.Healthy:
			this._statePic.spriteName = "Select_server_bar_green";
			break;
		case AreaState.Crowded:
			this._statePic.spriteName = "Select_server_bar_orange";
			break;
		case AreaState.Full:
			this._statePic.spriteName = "Select_server_bar_red";
			break;
		default:
			this._statePic.spriteName = "Select_server_bar_gray";
			break;
		}
	}

	public void SetChosen(bool isChosen)
	{
		if (isChosen == this._chosenFrame.activeInHierarchy)
		{
			return;
		}
		this._chosenFrame.SetActive(isChosen);
		this._serverName.color = ((!isChosen) ? new Color32(156, 242, 253, 255) : new Color32(255, 248, 89, 255));
		base.transform.GetComponent<UIButton>().isEnabled = !isChosen;
		if (isChosen)
		{
			object serverName = this.serverName;
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21002, serverName, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}
	}

	public void BindingAreaInfo(AreaInfo info)
	{
		this.serverName = info.areaName;
		this.SetState((AreaState)info.areaState);
	}
}
