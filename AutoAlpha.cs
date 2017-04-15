using System;
using UnityEngine;

public class AutoAlpha : MonoBehaviour
{
	public ChangeAlphaType Type;

	public float time = 4f;

	private float curTime;

	private bool isActive;

	public float startAlpha;

	public float endAlpha;

	private MeshFilter[] meshFilters;

	private Renderer[] rens;

	private void Awake()
	{
		this.rens = base.transform.GetComponentsInChildren<Renderer>(true);
		this.meshFilters = base.transform.GetComponentsInChildren<MeshFilter>(true);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!this.isActive)
		{
			return;
		}
		if (this.curTime > this.time)
		{
			this.isActive = false;
			this.ChangeAlpha(this.endAlpha);
		}
		this.curTime += Time.deltaTime;
	}

	private void OnEnable()
	{
		this.isActive = true;
		this.curTime = 0f;
		this.ChangeAlpha(this.startAlpha);
	}

	private void ChangeAlpha(float alpha)
	{
		if (this.Type == ChangeAlphaType.Material)
		{
			for (int i = 0; i < this.rens.Length; i++)
			{
				Renderer renderer = this.rens[i];
				string materialColorName = AutoAlpha.GetMaterialColorName(renderer.sharedMaterial);
				if (materialColorName != null)
				{
					Color color = renderer.material.GetColor(materialColorName);
					color.a = alpha;
					renderer.material.SetColor(materialColorName, color);
				}
			}
		}
		else
		{
			for (int j = 0; j < this.meshFilters.Length; j++)
			{
				Color[] array = this.meshFilters[j].mesh.colors;
				if (array.Length == 0)
				{
					if (this.meshFilters[j].mesh.vertices.Length == 0)
					{
						NcSpriteFactory.CreateEmptyMesh(this.meshFilters[j]);
					}
					array = new Color[this.meshFilters[j].mesh.vertices.Length];
					for (int k = 0; k < array.Length; k++)
					{
						array[k] = Color.white;
					}
				}
				for (int l = 0; l < array.Length; l++)
				{
					Color color2 = array[l];
					color2.a = alpha;
					array[l] = color2;
				}
				this.meshFilters[j].mesh.colors = array;
			}
		}
	}

	private void OnDisable()
	{
		this.isActive = false;
		this.curTime = 0f;
	}

	public static string GetMaterialColorName(Material mat)
	{
		string[] array = new string[]
		{
			"_Color",
			"_TintColor",
			"_EmisColor"
		};
		if (mat != null)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}
}
