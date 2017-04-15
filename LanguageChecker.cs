using Com.Game.Utils;
using System;
using UnityEngine;

public class LanguageChecker : MonoBehaviour
{
	public LanguageCheckerType languageCheckerType = LanguageCheckerType.UILabel;

	public string languageId = string.Empty;

	private UILabel _label;

	private UISprite _sprite;

	private UITexture _texture;

	private void OnEnable()
	{
		if (!LanguageManager.Instance.getDataReady || !this.TryGetComponentReady())
		{
			return;
		}
		switch (this.languageCheckerType)
		{
		case LanguageCheckerType.UILabel:
			this.UpdateLabel();
			break;
		case LanguageCheckerType.UISprite:
			this.UpdateSprite();
			break;
		case LanguageCheckerType.UITexture:
			this.UpdateTexture();
			break;
		}
	}

	private void OnDestroy()
	{
		this._label = null;
		this._sprite = null;
		this._texture = null;
	}

	private bool TryGetComponentReady()
	{
		bool flag;
		switch (this.languageCheckerType)
		{
		case LanguageCheckerType.UILabel:
			this._label = base.gameObject.GetComponent<UILabel>();
			flag = (this._label != null);
			break;
		case LanguageCheckerType.UISprite:
			this._sprite = base.gameObject.GetComponent<UISprite>();
			flag = (this._sprite != null);
			break;
		case LanguageCheckerType.UITexture:
			this._texture = base.gameObject.GetComponent<UITexture>();
			flag = (this._texture != null);
			break;
		default:
			ClientLogger.Error("Undefined Language Checker Type");
			return false;
		}
		if (!flag)
		{
			ClientLogger.Error("LanguageChecker 无法获得对应脚本 gameObject.name=" + this.GetObjectPath(base.transform));
		}
		return flag;
	}

	private string GetObjectPath(Transform trans)
	{
		Transform parent = trans.parent;
		string text = trans.gameObject.name;
		while (parent != null)
		{
			text = string.Format("{0}/{1}", parent.gameObject.name, text);
			parent = parent.parent;
		}
		return text;
	}

	private void UpdateLabel()
	{
		if (LanguageManager.Instance.GetStringById(this.languageId) != null)
		{
			this._label.text = LanguageManager.Instance.GetStringById(this.languageId);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void UpdateSprite()
	{
		if (LanguageManager.Instance.GetStringById(this.languageId) != null)
		{
			this._sprite.spriteName = LanguageManager.Instance.GetStringById(this.languageId);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void UpdateTexture()
	{
		if (LanguageManager.Instance.GetStringById(this.languageId) != null)
		{
			this._texture.mainTexture = ResourceManager.Load<Texture>(LanguageManager.Instance.GetStringById(this.languageId), true, true, null, 0, false);
		}
		else
		{
			base.enabled = false;
		}
	}
}
