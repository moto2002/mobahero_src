using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Module;
using System;
using UnityEngine;

public class BarrageEmitItemCtrl : MonoBehaviour
{
	private SysBulletScreenVo _cfgDataVo;

	[SerializeField]
	private UILabel _content;

	[SerializeField]
	private GameObject _line;

	[SerializeField]
	private UIButton _selectObj;

	public string content
	{
		get
		{
			if (this._content != null)
			{
				return this._content.text;
			}
			return string.Empty;
		}
		private set
		{
			if (this._content != null)
			{
				this._content.text = value;
			}
		}
	}

	private void Awake()
	{
	}

	private void OnDestroy()
	{
	}

	private void OnEnable()
	{
	}

	public void SetData(SysBulletScreenVo _param)
	{
		if (_param == null)
		{
			return;
		}
		base.gameObject.name = _param.id.ToString();
		this._cfgDataVo = _param;
		this.content = LanguageManager.Instance.GetStringById(_param.content);
		BarrageItem component = NGUITools.AddChild(base.transform.FindChild("select").gameObject, Singleton<BarrageEmitterView>.Instance.mItemCache).GetComponent<BarrageItem>();
		BarrageEmitterView.SetFormat(_param.format.ToString(), component);
		component.transform.GetComponent<TweenPosition>().enabled = false;
		component.transform.localPosition = new Vector3(130f, 0f, 0f);
		component.fontSize = 60;
		component.ComposeText(Singleton<BarrageEmitterView>.Instance.barragePrefix, base.transform.Find("label").GetComponent<UILabel>().text);
	}

	public void HideSeparator(bool isHide)
	{
		this._line.SetActive(!isHide);
	}

	public void Preview(bool isShow)
	{
		this._selectObj.state = ((!isShow) ? UIButtonColor.State.Normal : UIButtonColor.State.Pressed);
	}

	public void Send()
	{
		if (base.gameObject == null || base.transform.GetComponentInChildren<BarrageItem>() == null)
		{
			return;
		}
		if (base.gameObject.transform.FindChild("lockIcon") != null)
		{
			return;
		}
		string text = ModelManager.Instance.Get_BarrageCfgDataById_X(base.gameObject.name).format.ToString();
		if (text == null)
		{
			text = "1";
		}
		string msg = BarrageEmitterView.DataPacking(base.gameObject.transform.GetComponentInChildren<BarrageItem>().text, text);
		if (Singleton<BarrageEmitterView>.Instance.sceneType == BarrageSceneType.SelectHero || Singleton<BarrageEmitterView>.Instance.sceneType == BarrageSceneType.WatcherMode_SelectHero)
		{
			ModelManager.Instance.Send_C2PCaption_2GameServer(msg);
		}
		else
		{
			ModelManager.Instance.Send_C2PCaption(msg);
		}
		Singleton<BarrageEmitterView>.Instance.RecordEmitting();
		AudioMgr.PlayUI("Play_Menu_click", null, false, false);
		MobaMessageManagerTools.SendClientMsg(ClientV2V.BattleController_Open, null, false);
	}
}
