using System;
using UnityEngine;

public class AwardItem : MonoBehaviour
{
	public UISprite icon;

	public UILabel itemName;

	public UILabel itemCount;

	public UITexture texture;

	public void SetInfo(string iconName, string name, int num, bool isEquip = false)
	{
		if (iconName != null)
		{
			this.texture.mainTexture = ResourceManager.Load<Texture>(iconName, true, true, null, 0, false);
			this.texture.enabled = true;
			this.icon.enabled = false;
		}
		if (name != null)
		{
			this.itemName.text = name;
		}
		this.itemCount.text = "x" + num.ToString();
	}
}
