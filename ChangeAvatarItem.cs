using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Module;
using MobaProtocol;
using System;
using UnityEngine;

public class ChangeAvatarItem : MonoBehaviour
{
	public UISprite BG;

	public UISprite Avatar;

	public UISprite Frame;

	public UISprite NEW;

	public UISprite NewPoint;

	public PriceTagController PriceTag;

	public UILabel Name;

	public UILabel Describe;

	public UILabel State;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void ClickAvatarItem(GameObject go)
	{
		Singleton<ChangeAvatarView>.Instance.UpdateSample(go.name);
		if (this.NewPoint.gameObject.activeInHierarchy)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ShowIconFrame, param, new object[]
			{
				int.Parse(go.name),
				1
			});
			this.NewPoint.gameObject.SetActive(false);
		}
	}

	private void ClickFrameItem(GameObject go)
	{
		Singleton<ChangeAvatarView>.Instance.UpdateSample(go.name);
		if (this.NewPoint.gameObject.activeInHierarchy)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.ShowIconFrame, param, new object[]
			{
				int.Parse(go.name),
				2
			});
			this.NewPoint.gameObject.SetActive(false);
		}
	}

	public void UpdateAvatarItem(SysSummonersHeadportraitVo vo, bool _isOwn, bool _isNewGet)
	{
		UIEventListener.Get(base.transform.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAvatarItem);
		this.Avatar.gameObject.SetActive(true);
		this.Frame.gameObject.SetActive(false);
		this.BG.spriteName = "Change_head_frame_0" + vo.headportrait_quality;
		this.Avatar.spriteName = vo.headportrait_icon;
		this.Name.text = LanguageManager.Instance.GetStringById(vo.headportrait_name);
		if (vo.headportrait_new == 1)
		{
			this.NEW.gameObject.SetActive(true);
		}
		else
		{
			this.NEW.gameObject.SetActive(false);
		}
		if (_isNewGet)
		{
			this.NewPoint.gameObject.SetActive(true);
			this.NEW.gameObject.SetActive(false);
		}
		else
		{
			this.NewPoint.gameObject.SetActive(false);
		}
		if (_isOwn)
		{
			this.PriceTag.gameObject.SetActive(false);
			this.Describe.gameObject.SetActive(false);
			this.State.gameObject.SetActive(true);
			if (vo.headportrait_id == Singleton<ChangeAvatarView>.Instance.Avatar)
			{
				this.State.text = "使用中";
				this.State.color = new Color32(255, 246, 10, 255);
			}
			else
			{
				this.State.text = "已拥有";
				this.State.color = new Color32(11, 233, 0, 255);
			}
		}
		else
		{
			this.State.gameObject.SetActive(false);
			this.SetDescribe(vo.headportrait_output, vo.headportrait_id);
			if (vo.headportrait_output == 1)
			{
				this.PriceTag.SetData(GoodsSubject.HeadPortrait, vo.headportrait_id.ToString(), 1, null, null, null);
			}
		}
	}

	private void SetDescribe(int type, int id)
	{
		this.Describe.gameObject.SetActive(true);
		this.PriceTag.gameObject.SetActive(false);
		switch (type)
		{
		case 1:
			this.Describe.gameObject.SetActive(false);
			this.PriceTag.gameObject.SetActive(true);
			break;
		case 2:
			this.Describe.text = "签到获得";
			break;
		case 3:
			this.Describe.text = "活动获得";
			break;
		case 4:
			this.Describe.text = "成就获得";
			break;
		case 5:
			this.Describe.text = "小魔瓶获得";
			break;
		case 6:
			this.Describe.text = "排行榜获得";
			break;
		}
	}

	public void UpdateAvatarItem(SysSummonersPictureframeVo vo, bool _isOwn, bool _isNewGet)
	{
		UIEventListener.Get(base.transform.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickFrameItem);
		this.Avatar.gameObject.SetActive(false);
		this.Frame.gameObject.SetActive(true);
		this.BG.spriteName = "Change_head_frame_0" + vo.pictureframe_quality;
		this.Frame.spriteName = vo.pictureframe_icon;
		this.Name.text = LanguageManager.Instance.GetStringById(vo.pictureframe_name);
		if (vo.pictureframe_new == 1)
		{
			this.NEW.gameObject.SetActive(true);
		}
		else
		{
			this.NEW.gameObject.SetActive(false);
		}
		if (_isNewGet)
		{
			this.NewPoint.gameObject.SetActive(true);
			this.NEW.gameObject.SetActive(false);
		}
		else
		{
			this.NewPoint.gameObject.SetActive(false);
		}
		if (_isOwn)
		{
			this.PriceTag.gameObject.SetActive(false);
			this.Describe.gameObject.SetActive(false);
			this.State.gameObject.SetActive(true);
			if (vo.pictureframe_id == Singleton<ChangeAvatarView>.Instance.PictureFrame)
			{
				this.State.text = "使用中";
				this.State.color = new Color32(255, 246, 10, 255);
			}
			else
			{
				this.State.text = "已拥有";
				this.State.color = new Color32(11, 233, 0, 255);
			}
		}
		else
		{
			this.State.gameObject.SetActive(false);
			this.SetDescribe(vo.pictureframe_output, int.Parse(vo.pictureframe_id));
			if (vo.pictureframe_output == 1)
			{
				this.PriceTag.SetData(GoodsSubject.HeadPortraitFrame, vo.pictureframe_id.ToString(), 1, null, null, null);
			}
		}
	}
}
