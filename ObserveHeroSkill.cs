using System;
using UnityEngine;

public class ObserveHeroSkill : MonoBehaviour
{
	public UITexture Texture;

	public UILabel Level;

	public UISprite Mask;

	public void SetCdPercent(float percent)
	{
		this.Mask.fillAmount = percent;
	}
}
