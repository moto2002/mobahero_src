using System;
using UnityEngine;

public class NcEffectInitBackup
{
	protected enum SAVE_TYPE
	{
		NONE,
		ONE,
		RECURSIVELY
	}

	protected NcEffectInitBackup.SAVE_TYPE m_SavedMaterialColor;

	protected Renderer m_MaterialColorRenderer;

	protected string m_MaterialColorColorName;

	protected Vector4 m_MaterialColorSaveValue;

	protected Renderer[] m_MaterialColorRenderers;

	protected string[] m_MaterialColorColorNames;

	protected Vector4[] m_MaterialColorSaveValues;

	protected NcEffectInitBackup.SAVE_TYPE m_SavedMeshColor;

	protected MeshFilter m_MeshColorMeshFilter;

	protected Vector4 m_MeshColorSaveValue;

	protected MeshFilter[] m_MeshColorMeshFilters;

	protected Vector4[] m_MeshColorSaveValues;

	protected NcUvAnimation m_NcUvAnimation;

	protected Vector2 m_UvAniSaveValue;

	protected Transform m_Transform;

	protected NcTransformTool m_NcTansform;

	public void BackupTransform(Transform targetTrans)
	{
		this.m_Transform = targetTrans;
		this.m_NcTansform = new NcTransformTool(this.m_Transform);
	}

	public void RestoreTransform()
	{
		this.m_NcTansform.CopyToLocalTransform(this.m_Transform);
	}

	public void BackupMaterialColor(GameObject targetObj, bool bRecursively)
	{
		if (targetObj == null)
		{
			return;
		}
		if (bRecursively)
		{
			this.m_SavedMaterialColor = NcEffectInitBackup.SAVE_TYPE.RECURSIVELY;
		}
		else
		{
			this.m_SavedMaterialColor = NcEffectInitBackup.SAVE_TYPE.ONE;
		}
		Transform transform = targetObj.transform;
		if (this.m_SavedMaterialColor == NcEffectInitBackup.SAVE_TYPE.RECURSIVELY)
		{
			this.m_MaterialColorRenderers = transform.GetComponentsInChildren<Renderer>(true);
			this.m_MaterialColorColorNames = new string[this.m_MaterialColorRenderers.Length];
			this.m_MaterialColorSaveValues = new Vector4[this.m_MaterialColorRenderers.Length];
			for (int i = 0; i < this.m_MaterialColorRenderers.Length; i++)
			{
				Renderer renderer = this.m_MaterialColorRenderers[i];
				this.m_MaterialColorColorNames[i] = NcEffectInitBackup.GetMaterialColorName(renderer.sharedMaterial);
				if (this.m_MaterialColorColorNames[i] != null)
				{
					this.m_MaterialColorSaveValues[i] = renderer.material.GetColor(this.m_MaterialColorColorNames[i]);
				}
			}
		}
		else
		{
			this.m_MaterialColorRenderer = transform.GetComponent<Renderer>();
			if (this.m_MaterialColorRenderer != null)
			{
				this.m_MaterialColorColorName = NcEffectInitBackup.GetMaterialColorName(this.m_MaterialColorRenderer.sharedMaterial);
				if (this.m_MaterialColorColorName != null)
				{
					this.m_MaterialColorSaveValue = this.m_MaterialColorRenderer.material.GetColor(this.m_MaterialColorColorName);
				}
			}
		}
	}

	public void RestoreMaterialColor()
	{
		if (this.m_SavedMaterialColor == NcEffectInitBackup.SAVE_TYPE.NONE)
		{
			return;
		}
		if (this.m_SavedMaterialColor == NcEffectInitBackup.SAVE_TYPE.RECURSIVELY)
		{
			for (int i = 0; i < this.m_MaterialColorRenderers.Length; i++)
			{
				if (this.m_MaterialColorRenderers[i] != null && this.m_MaterialColorColorNames[i] != null)
				{
					this.m_MaterialColorRenderers[i].material.SetColor(this.m_MaterialColorColorNames[i], this.m_MaterialColorSaveValues[i]);
				}
			}
		}
		else if (this.m_MaterialColorRenderers != null)
		{
			this.m_MaterialColorColorName = NcEffectInitBackup.GetMaterialColorName(this.m_MaterialColorRenderer.sharedMaterial);
			if (this.m_MaterialColorColorName != null)
			{
				this.m_MaterialColorRenderer.material.SetColor(this.m_MaterialColorColorName, this.m_MaterialColorSaveValue);
			}
		}
	}

	public void BackupUvAnimation(NcUvAnimation uvAniCom)
	{
		if (uvAniCom == null)
		{
			return;
		}
		this.m_NcUvAnimation = uvAniCom;
		this.m_UvAniSaveValue = new Vector2(this.m_NcUvAnimation.m_fScrollSpeedX, this.m_NcUvAnimation.m_fScrollSpeedY);
	}

	public void RestoreUvAnimation()
	{
		if (this.m_NcUvAnimation == null)
		{
			return;
		}
		this.m_NcUvAnimation.m_fScrollSpeedX = this.m_UvAniSaveValue.x;
		this.m_NcUvAnimation.m_fScrollSpeedY = this.m_UvAniSaveValue.y;
	}

	public void BackupMeshColor(GameObject targetObj, bool bRecursively)
	{
		if (targetObj == null)
		{
			return;
		}
		if (bRecursively)
		{
			this.m_SavedMeshColor = NcEffectInitBackup.SAVE_TYPE.RECURSIVELY;
		}
		else
		{
			this.m_SavedMeshColor = NcEffectInitBackup.SAVE_TYPE.ONE;
		}
		if (this.m_SavedMeshColor == NcEffectInitBackup.SAVE_TYPE.RECURSIVELY)
		{
			this.m_MeshColorMeshFilters = targetObj.GetComponentsInChildren<MeshFilter>(true);
			this.m_MeshColorSaveValues = new Vector4[this.m_MeshColorMeshFilters.Length];
			if (this.m_MeshColorMeshFilters == null || this.m_MeshColorMeshFilters.Length < 0)
			{
				return;
			}
			for (int i = 0; i < this.m_MeshColorMeshFilters.Length; i++)
			{
				this.m_MeshColorSaveValues[i] = this.GetMeshColor(this.m_MeshColorMeshFilters[i]);
			}
		}
		else
		{
			this.m_MeshColorMeshFilter = targetObj.GetComponent<MeshFilter>();
			this.m_MeshColorSaveValue = this.GetMeshColor(this.m_MeshColorMeshFilter);
		}
	}

	public void RestoreMeshColor()
	{
		if (this.m_SavedMeshColor == NcEffectInitBackup.SAVE_TYPE.NONE)
		{
			return;
		}
		if (this.m_SavedMeshColor == NcEffectInitBackup.SAVE_TYPE.RECURSIVELY)
		{
			if (this.m_MeshColorMeshFilters == null || this.m_MeshColorMeshFilters.Length < 0)
			{
				return;
			}
			for (int i = 0; i < this.m_MeshColorMeshFilters.Length; i++)
			{
				this.SetMeshColor(this.m_MeshColorMeshFilters[i], this.m_MeshColorSaveValues[i]);
			}
		}
		else
		{
			this.SetMeshColor(this.m_MeshColorMeshFilter, this.m_MeshColorSaveValue);
		}
	}

	protected Color GetMeshColor(MeshFilter mFilter)
	{
		if (mFilter == null || mFilter.mesh == null)
		{
			return Color.white;
		}
		Color[] array = mFilter.mesh.colors;
		if (array.Length == 0)
		{
			array = new Color[mFilter.mesh.vertices.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Color.white;
			}
			mFilter.mesh.colors = array;
			return Color.white;
		}
		return array[0];
	}

	protected void SetMeshColor(MeshFilter mFilter, Color tarColor)
	{
		if (mFilter == null || mFilter.mesh == null)
		{
			return;
		}
		Color[] array = mFilter.mesh.colors;
		if (array.Length == 0)
		{
			array = new Color[mFilter.mesh.vertices.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Color.white;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = tarColor;
		}
		mFilter.mesh.colors = array;
	}

	protected static string GetMaterialColorName(Material mat)
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
