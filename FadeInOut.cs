using System;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
	private Material[] childMaterials;

	private float currentAlpha = 1f;

	private float fading;

	private float timeStep = 0.1f;

	public float blendTime = 2f;

	private float blend;

	private string colorName = "_Color";

	private string tinyColorName = "_TintColor";

	public float LightIntensityMult = -0.5f;

	public float LifeTime = 1.5f;

	public bool RandomRotation;

	public Vector3 PositionOffset;

	private void Start()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		this.childMaterials = new Material[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = componentsInChildren[i];
			this.childMaterials[i] = renderer.material;
		}
	}

	public void FadeIn()
	{
		base.CancelInvoke();
		this.fading = 1f;
		this.blend = 0f;
		this.currentAlpha = 0f;
		base.InvokeRepeating("CustomUpdate", 0f, this.timeStep);
	}

	public void FadeOut()
	{
		base.CancelInvoke();
		this.fading = -1f;
		this.blend = 0f;
		this.currentAlpha = 1f;
		base.InvokeRepeating("CustomUpdate", 0f, this.timeStep);
	}

	private void CustomUpdate()
	{
		this.blend += this.timeStep;
		if (this.fading > 0f)
		{
			this.currentAlpha += this.timeStep / this.blendTime;
		}
		else
		{
			this.currentAlpha -= this.timeStep / this.blendTime;
		}
		this.currentAlpha = Mathf.Clamp(this.currentAlpha, 0f, 1f);
		for (int i = 0; i < this.childMaterials.Length; i++)
		{
			Material material = this.childMaterials[i];
			if (material && material.HasProperty(this.colorName))
			{
				Color color = material.GetColor(this.colorName);
				color.a = this.currentAlpha;
				material.SetColor(this.colorName, color);
			}
			if (material && material.HasProperty(this.tinyColorName))
			{
				Color color2 = material.GetColor(this.tinyColorName);
				color2.a = this.currentAlpha / 2f;
				material.SetColor(this.tinyColorName, color2);
			}
		}
		if (this.blend >= this.blendTime)
		{
			base.CancelInvoke();
		}
	}

	public void setAlpah(float a)
	{
		for (int i = 0; i < this.childMaterials.Length; i++)
		{
			Material material = this.childMaterials[i];
			Color color = material.GetColor(this.colorName);
			color.a = a;
			material.SetColor(this.colorName, color);
		}
	}
}
