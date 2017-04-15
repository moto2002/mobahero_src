using System;
using UnityEngine;

public class NcChangeAlpha : NcEffectBehaviour
{
	public enum TARGET_TYPE
	{
		MeshColor,
		MaterialColor
	}

	public enum CHANGE_MODE
	{
		FromTo
	}

	public NcChangeAlpha.TARGET_TYPE m_TargetType;

	public float m_fDelayTime = 2f;

	public float m_fChangeTime = 1f;

	public bool m_bRecursively = true;

	public NcChangeAlpha.CHANGE_MODE m_ChangeMode;

	public float m_fFromAlphaValue = 1f;

	public float m_fToMeshValue;

	public bool m_bAutoDeactive = true;

	protected float m_fStartTime;

	protected float m_fStartChangeTime;

	public static NcChangeAlpha SetChangeTime(GameObject baseGameObject, float fLifeTime, float fChangeTime, float fFromMeshAlphaValue, float fToMeshAlphaValue)
	{
		NcChangeAlpha ncChangeAlpha = baseGameObject.AddComponent<NcChangeAlpha>();
		ncChangeAlpha.SetChangeTime(fLifeTime, fChangeTime, fFromMeshAlphaValue, fToMeshAlphaValue);
		return ncChangeAlpha;
	}

	public void SetChangeTime(float fDelayTime, float fChangeTime, float fFromAlphaValue, float fToAlphaValue)
	{
		this.m_fDelayTime = fDelayTime;
		this.m_fChangeTime = fChangeTime;
		this.m_fFromAlphaValue = fFromAlphaValue;
		this.m_fToMeshValue = fToAlphaValue;
		if (NcEffectBehaviour.IsActive(base.gameObject))
		{
			this.Start();
			this.Update();
		}
	}

	public void Restart()
	{
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_fStartChangeTime = 0f;
		this.ChangeToAlpha(0f);
	}

	private void Awake()
	{
		this.m_fStartTime = 0f;
		this.m_fStartChangeTime = 0f;
	}

	private void Start()
	{
		this.Restart();
	}

	private void Update()
	{
		if (0f < this.m_fStartChangeTime)
		{
			if (0f < this.m_fChangeTime)
			{
				float num = (NcEffectBehaviour.GetEngineTime() - this.m_fStartChangeTime) / this.m_fChangeTime;
				if (1f < num)
				{
					num = 1f;
					if (this.m_bAutoDeactive && this.m_fToMeshValue <= 0f)
					{
						NcEffectBehaviour.SetActiveRecursively(base.gameObject, false);
					}
				}
				this.ChangeToAlpha(num);
			}
			else
			{
				this.ChangeToAlpha(1f);
			}
		}
		else if (0f < this.m_fStartTime && this.m_fStartTime + this.m_fDelayTime <= NcEffectBehaviour.GetEngineTime())
		{
			this.StartChange();
		}
	}

	private void StartChange()
	{
		this.m_fStartChangeTime = NcEffectBehaviour.GetEngineTime();
	}

	private void ChangeToAlpha(float fElapsedRate)
	{
		float num = Mathf.Lerp(this.m_fFromAlphaValue, this.m_fToMeshValue, fElapsedRate);
		if (this.m_TargetType == NcChangeAlpha.TARGET_TYPE.MeshColor)
		{
			MeshFilter[] array;
			if (this.m_bRecursively)
			{
				array = base.transform.GetComponentsInChildren<MeshFilter>(true);
			}
			else
			{
				array = base.transform.GetComponents<MeshFilter>();
			}
			for (int i = 0; i < array.Length; i++)
			{
				Color[] array2 = array[i].mesh.colors;
				if (array2.Length == 0)
				{
					if (array[i].mesh.vertices.Length == 0)
					{
						NcSpriteFactory.CreateEmptyMesh(array[i]);
					}
					array2 = new Color[array[i].mesh.vertices.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = Color.white;
					}
				}
				for (int k = 0; k < array2.Length; k++)
				{
					Color color = array2[k];
					color.a = num;
					array2[k] = color;
				}
				array[i].mesh.colors = array2;
			}
		}
		else
		{
			Renderer[] array3;
			if (this.m_bRecursively)
			{
				array3 = base.transform.GetComponentsInChildren<Renderer>(true);
			}
			else
			{
				array3 = base.transform.GetComponents<Renderer>();
			}
			for (int l = 0; l < array3.Length; l++)
			{
				Renderer renderer = array3[l];
				string materialColorName = NcEffectBehaviour.GetMaterialColorName(renderer.sharedMaterial);
				if (materialColorName != null)
				{
					Color color2 = renderer.material.GetColor(materialColorName);
					color2.a = num;
					renderer.material.SetColor(materialColorName, color2);
				}
			}
		}
		if (fElapsedRate == 1f && num == 0f)
		{
			NcEffectBehaviour.SetActiveRecursively(base.gameObject, false);
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fChangeTime /= fSpeedRate;
	}

	public override void OnSetReplayState()
	{
		base.OnSetReplayState();
		this.m_NcEffectInitBackup = new NcEffectInitBackup();
		if (this.m_TargetType == NcChangeAlpha.TARGET_TYPE.MeshColor)
		{
			this.m_NcEffectInitBackup.BackupMeshColor(base.gameObject, this.m_bRecursively);
		}
		else
		{
			this.m_NcEffectInitBackup.BackupMaterialColor(base.gameObject, this.m_bRecursively);
		}
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_fStartChangeTime = 0f;
		if (this.m_NcEffectInitBackup != null)
		{
			if (this.m_TargetType == NcChangeAlpha.TARGET_TYPE.MeshColor)
			{
				this.m_NcEffectInitBackup.RestoreMeshColor();
			}
			else
			{
				this.m_NcEffectInitBackup.RestoreMaterialColor();
			}
		}
	}
}
