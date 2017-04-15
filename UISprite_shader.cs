using System;
using UnityEngine;

public class UISprite_shader : UISprite
{
	protected UIPanel panelObj;

	protected Material GrayMaterial;

	public override Material material
	{
		get
		{
			Material material = base.material;
			if (material == null)
			{
				material = ((!(base.atlas != null)) ? null : base.atlas.spriteMaterial);
			}
			if (this.GrayMaterial != null)
			{
				return this.GrayMaterial;
			}
			return material;
		}
	}

	public void SetGray()
	{
		this.GrayMaterial = new Material(Shader.Find("Custom/MaskGray2"))
		{
			mainTexture = this.material.mainTexture
		};
		this.RefreshPanel(base.gameObject);
	}

	public void SetVisible(bool isVisible)
	{
		if (isVisible)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			base.transform.localScale = new Vector3(0f, 0f, 0f);
		}
	}

	public void SetEnabled(bool isEnabled)
	{
		if (isEnabled)
		{
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			if (component)
			{
				component.enabled = true;
			}
			this.SetNormal();
		}
		else
		{
			BoxCollider component2 = base.gameObject.GetComponent<BoxCollider>();
			if (component2)
			{
				component2.enabled = false;
			}
			this.SetGray();
		}
	}

	public void SetNormal()
	{
		this.GrayMaterial = null;
		this.RefreshPanel(base.gameObject);
	}

	private void RefreshPanel(GameObject go)
	{
		if (this.panelObj == null)
		{
			this.panelObj = NGUITools.FindInParents<UIPanel>(go);
		}
		if (this.panelObj != null)
		{
			this.panelObj.enabled = false;
			this.panelObj.enabled = true;
		}
	}
}
