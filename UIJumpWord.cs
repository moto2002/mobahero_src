using System;
using UnityEngine;

public class UIJumpWord : MonoBehaviour
{
	public UILabel label;

	public UILabel sprite;

	public UILabel textMeshPro;

	public UILabel Gold;

	[NonSerialized]
	public string strValue;

	public void ShowGold()
	{
		this.label.gameObject.SetActive(false);
		this.Gold.text = "l";
		this.textMeshPro.gameObject.SetActive(true);
	}

	public void ShowDamage(bool isCriticle = true)
	{
		this.textMeshPro.gameObject.SetActive(false);
		this.label.gameObject.SetActive(true);
		this.sprite.SetActive(isCriticle);
	}
}
