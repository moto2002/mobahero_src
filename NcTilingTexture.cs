using System;
using UnityEngine;

public class NcTilingTexture : NcEffectBehaviour
{
	public float m_fTilingX = 2f;

	public float m_fTilingY = 2f;

	public float m_fOffsetX;

	public float m_fOffsetY;

	public bool m_bFixedTileSize;

	protected Vector3 m_OriginalScale = default(Vector3);

	protected Vector2 m_OriginalTiling = default(Vector2);

	private void Start()
	{
		if (base.renderer != null && base.renderer.material != null)
		{
			base.renderer.material.mainTextureScale = new Vector2(this.m_fTilingX, this.m_fTilingY);
			base.renderer.material.mainTextureOffset = new Vector2(this.m_fOffsetX - (float)((int)this.m_fOffsetX), this.m_fOffsetY - (float)((int)this.m_fOffsetY));
			base.AddRuntimeMaterial(base.renderer.material);
		}
	}

	private void Update()
	{
		if (this.m_bFixedTileSize)
		{
			if (this.m_OriginalScale.x != 0f)
			{
				this.m_fTilingX = this.m_OriginalTiling.x * (base.transform.lossyScale.x / this.m_OriginalScale.x);
			}
			if (this.m_OriginalScale.y != 0f)
			{
				this.m_fTilingY = this.m_OriginalTiling.y * (base.transform.lossyScale.y / this.m_OriginalScale.y);
			}
			base.renderer.material.mainTextureScale = new Vector2(this.m_fTilingX, this.m_fTilingY);
		}
	}

	public override void OnUpdateToolData()
	{
		this.m_OriginalScale = base.transform.lossyScale;
		this.m_OriginalTiling.x = this.m_fTilingX;
		this.m_OriginalTiling.y = this.m_fTilingY;
	}
}
