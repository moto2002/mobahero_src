using System;
using UnityEngine;

public class NcSpriteTexture : NcEffectBehaviour
{
	public GameObject m_NcSpriteFactoryPrefab;

	protected NcSpriteFactory m_NcSpriteFactoryCom;

	public NcSpriteFactory.NcFrameInfo[] m_NcSpriteFrameInfos;

	public float m_fUvScale = 1f;

	public int m_nSpriteFactoryIndex;

	public int m_nFrameIndex;

	public NcSpriteFactory.MESH_TYPE m_MeshType;

	public NcSpriteFactory.ALIGN_TYPE m_AlignType = NcSpriteFactory.ALIGN_TYPE.CENTER;

	public float m_fShowRate = 1f;

	protected GameObject m_EffectObject;

	private void Awake()
	{
		if (this.m_MeshFilter == null)
		{
			this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
			if (this.m_MeshFilter == null)
			{
				this.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
		}
		if (this.m_NcSpriteFactoryPrefab == null && base.gameObject.GetComponent<NcSpriteFactory>() != null)
		{
			this.m_NcSpriteFactoryPrefab = base.gameObject;
		}
		this.UpdateFactoryInfo(this.m_nSpriteFactoryIndex);
	}

	private void Start()
	{
		this.UpdateSpriteTexture(true);
	}

	public void SetSpriteFactoryIndex(string spriteName, int nFrameIndex, bool bRunImmediate)
	{
		if (this.m_NcSpriteFactoryCom == null)
		{
			if (!this.m_NcSpriteFactoryPrefab || !(this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return;
			}
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		this.m_nSpriteFactoryIndex = this.m_NcSpriteFactoryCom.GetSpriteNodeIndex(spriteName);
		this.SetSpriteFactoryIndex(this.m_nSpriteFactoryIndex, nFrameIndex, bRunImmediate);
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, int nFrameIndex, bool bRunImmediate)
	{
		if (!this.UpdateFactoryInfo(nSpriteFactoryIndex))
		{
			return;
		}
		this.SetFrameIndex(nFrameIndex);
		if (bRunImmediate)
		{
			this.UpdateSpriteTexture(bRunImmediate);
		}
	}

	public void SetFrameIndex(int nFrameIndex)
	{
		this.m_nFrameIndex = ((0 > nFrameIndex) ? this.m_nFrameIndex : nFrameIndex);
		if (this.m_NcSpriteFrameInfos == null)
		{
			return;
		}
		this.m_nFrameIndex = ((this.m_NcSpriteFrameInfos.Length != 0) ? ((this.m_NcSpriteFrameInfos.Length > this.m_nFrameIndex) ? this.m_nFrameIndex : (this.m_NcSpriteFrameInfos.Length - 1)) : 0);
	}

	public void SetShowRate(float fShowRate)
	{
		this.m_fShowRate = fShowRate;
		this.UpdateSpriteTexture(true);
	}

	private bool UpdateFactoryInfo(int nSpriteFactoryIndex)
	{
		this.m_nSpriteFactoryIndex = nSpriteFactoryIndex;
		if (this.m_NcSpriteFactoryCom == null)
		{
			if (!this.m_NcSpriteFactoryPrefab || !(this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null))
			{
				return false;
			}
			this.m_NcSpriteFactoryCom = this.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
		}
		if (!this.m_NcSpriteFactoryCom.IsValidFactory())
		{
			return false;
		}
		this.m_NcSpriteFrameInfos = this.m_NcSpriteFactoryCom.GetSpriteNode(this.m_nSpriteFactoryIndex).m_FrameInfos;
		this.m_fUvScale = this.m_NcSpriteFactoryCom.m_fUvScale;
		return true;
	}

	private void UpdateSpriteTexture(bool bShowEffect)
	{
		if (!this.UpdateSpriteMaterial())
		{
			return;
		}
		if (!this.m_NcSpriteFactoryCom.IsValidFactory())
		{
			return;
		}
		if (this.m_NcSpriteFrameInfos.Length == 0)
		{
			this.SetSpriteFactoryIndex(this.m_nSpriteFactoryIndex, this.m_nFrameIndex, false);
		}
		if (this.m_MeshFilter == null)
		{
			if (base.gameObject.GetComponent<MeshFilter>() != null)
			{
				this.m_MeshFilter = base.gameObject.GetComponent<MeshFilter>();
			}
			else
			{
				this.m_MeshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
		}
		NcSpriteFactory.CreatePlane(this.m_MeshFilter, this.m_fUvScale, this.m_NcSpriteFrameInfos[this.m_nFrameIndex], false, this.m_AlignType, this.m_MeshType, this.m_fShowRate);
		NcSpriteFactory.UpdateMeshUVs(this.m_MeshFilter, this.m_NcSpriteFrameInfos[this.m_nFrameIndex].m_TextureUvOffset, this.m_AlignType, this.m_fShowRate);
		if (bShowEffect)
		{
			this.m_EffectObject = this.m_NcSpriteFactoryCom.CreateSpriteEffect(this.m_nSpriteFactoryIndex, base.transform);
		}
	}

	public bool UpdateSpriteMaterial()
	{
		if (this.m_NcSpriteFactoryPrefab == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryPrefab.renderer == null || this.m_NcSpriteFactoryPrefab.renderer.sharedMaterial == null || this.m_NcSpriteFactoryPrefab.renderer.sharedMaterial.mainTexture == null)
		{
			return false;
		}
		if (base.renderer == null)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryCom == null)
		{
			return false;
		}
		if (this.m_nSpriteFactoryIndex < 0 || this.m_NcSpriteFactoryCom.GetSpriteNodeCount() <= this.m_nSpriteFactoryIndex)
		{
			return false;
		}
		if (this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture && this.m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.Auto)
		{
			return false;
		}
		base.renderer.sharedMaterial = this.m_NcSpriteFactoryPrefab.renderer.sharedMaterial;
		return true;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public override void OnUpdateToolData()
	{
	}
}
