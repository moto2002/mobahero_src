using MobaHeros;
using System;
using UnityEngine;

public class BuffItem : MonoBehaviour
{
	[SerializeField]
	private UISprite m_BuffSprite;

	[SerializeField]
	private UILabel m_BuffLabel;

	[SerializeField]
	private UISprite m_BuffBg;

	[SerializeField]
	private UISprite m_mask;

	[SerializeField]
	private string _name = string.Empty;

	private void Awake()
	{
	}

	public string GetName()
	{
		if (this._name == string.Empty)
		{
			this._name = base.gameObject.name;
		}
		return this._name;
	}

	public void SetActive(bool isActive)
	{
		base.gameObject.SetActive(isActive);
	}

	public void SetTexture(string name)
	{
		if (this.m_BuffSprite == null)
		{
			this.m_BuffSprite = base.gameObject.GetComponent<UISprite>();
		}
		this.m_BuffSprite.spriteName = name;
		this._name = string.Empty;
	}

	public void SetLayer(string layer)
	{
		if (int.Parse(layer) == 1)
		{
			this.m_BuffLabel.text = string.Empty;
		}
		else
		{
			this.m_BuffLabel.text = layer;
		}
	}

	public void SetType(EffectGainType type)
	{
		if (type == EffectGainType.positive)
		{
			this.m_BuffBg.color = Color.green;
		}
		else if (type == EffectGainType.negative)
		{
			this.m_BuffBg.color = Color.red;
		}
	}

	public void SetMaskAmount(float amount)
	{
		this.m_mask.fillAmount = amount;
	}

	public void SetMaskActive(bool isEffective)
	{
		this.m_mask.gameObject.SetActive(isEffective);
	}
}
