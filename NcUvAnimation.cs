using System;
using UnityEngine;

public class NcUvAnimation : NcEffectAniBehaviour
{
	public float m_fScrollSpeedX = 1f;

	public float m_fScrollSpeedY;

	public float m_fTilingX = 1f;

	public float m_fTilingY = 1f;

	public float m_fOffsetX;

	public float m_fOffsetY;

	public bool m_bUseSmoothDeltaTime;

	public bool m_bFixedTileSize;

	public bool m_bRepeat = true;

	public bool m_bAutoDestruct;

	protected Vector3 m_OriginalScale = default(Vector3);

	protected Vector2 m_OriginalTiling = default(Vector2);

	protected Vector2 m_EndOffset = default(Vector2);

	protected Vector2 m_RepeatOffset = default(Vector2);

	protected Renderer m_Renderer;

	public void SetFixedTileSize(bool bFixedTileSize)
	{
		this.m_bFixedTileSize = bFixedTileSize;
	}

	public override int GetAnimationState()
	{
		if (!this.m_bRepeat)
		{
			if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject) || !base.IsEndAnimation())
			{
			}
		}
		return -1;
	}

	public override void ResetAnimation()
	{
		if (!base.enabled)
		{
			base.enabled = true;
		}
		this.Start();
	}

	private void Start()
	{
		this.m_Renderer = base.renderer;
		if (this.m_Renderer == null || this.m_Renderer.sharedMaterial == null || this.m_Renderer.sharedMaterial.mainTexture == null)
		{
			base.enabled = false;
		}
		else
		{
			base.renderer.material.mainTextureScale = new Vector2(this.m_fTilingX, this.m_fTilingY);
			base.AddRuntimeMaterial(base.renderer.material);
			float num = this.m_fOffsetX + this.m_fTilingX;
			this.m_RepeatOffset.x = num - (float)((int)num);
			if (this.m_RepeatOffset.x < 0f)
			{
				this.m_RepeatOffset.x = this.m_RepeatOffset.x + 1f;
			}
			num = this.m_fOffsetY + this.m_fTilingY;
			this.m_RepeatOffset.y = num - (float)((int)num);
			if (this.m_RepeatOffset.y < 0f)
			{
				this.m_RepeatOffset.y = this.m_RepeatOffset.y + 1f;
			}
			this.m_EndOffset.x = 1f - (this.m_fTilingX - (float)((int)this.m_fTilingX) + (float)((this.m_fTilingX - (float)((int)this.m_fTilingX) >= 0f) ? 0 : 1));
			this.m_EndOffset.y = 1f - (this.m_fTilingY - (float)((int)this.m_fTilingY) + (float)((this.m_fTilingY - (float)((int)this.m_fTilingY) >= 0f) ? 0 : 1));
			base.InitAnimationTimer();
		}
	}

	private void Update()
	{
		if (this.m_Renderer == null || this.m_Renderer.sharedMaterial == null || this.m_Renderer.sharedMaterial.mainTexture == null)
		{
			return;
		}
		if (this.m_bFixedTileSize)
		{
			if (this.m_fScrollSpeedX != 0f && this.m_OriginalScale.x != 0f)
			{
				this.m_fTilingX = this.m_OriginalTiling.x * (base.transform.lossyScale.x / this.m_OriginalScale.x);
			}
			if (this.m_fScrollSpeedY != 0f && this.m_OriginalScale.y != 0f)
			{
				this.m_fTilingY = this.m_OriginalTiling.y * (base.transform.lossyScale.y / this.m_OriginalScale.y);
			}
			base.renderer.material.mainTextureScale = new Vector2(this.m_fTilingX, this.m_fTilingY);
		}
		if (this.m_bUseSmoothDeltaTime)
		{
			this.m_fOffsetX += this.m_Timer.GetSmoothDeltaTime() * this.m_fScrollSpeedX;
			this.m_fOffsetY += this.m_Timer.GetSmoothDeltaTime() * this.m_fScrollSpeedY;
		}
		else
		{
			this.m_fOffsetX += this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedX;
			this.m_fOffsetY += this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedY;
		}
		bool flag = false;
		if (!this.m_bRepeat)
		{
			this.m_RepeatOffset.x = this.m_RepeatOffset.x + this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedX;
			if (this.m_RepeatOffset.x < 0f || 1f < this.m_RepeatOffset.x)
			{
				this.m_fOffsetX = this.m_EndOffset.x;
				base.enabled = false;
				flag = true;
			}
			this.m_RepeatOffset.y = this.m_RepeatOffset.y + this.m_Timer.GetDeltaTime() * this.m_fScrollSpeedY;
			if (this.m_RepeatOffset.y < 0f || 1f < this.m_RepeatOffset.y)
			{
				this.m_fOffsetY = this.m_EndOffset.y;
				base.enabled = false;
				flag = true;
			}
		}
		this.m_Renderer.material.mainTextureOffset = new Vector2(this.m_fOffsetX - (float)((int)this.m_fOffsetX), this.m_fOffsetY - (float)((int)this.m_fOffsetY));
		if (flag)
		{
			base.OnEndAnimation();
			if (this.m_bAutoDestruct)
			{
				if (this.m_bReplayState)
				{
					NcEffectBehaviour.SetActiveRecursively(base.gameObject, false);
				}
				else
				{
					UnityEngine.Object.DestroyObject(base.gameObject);
				}
			}
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fScrollSpeedX *= fSpeedRate;
		this.m_fScrollSpeedY *= fSpeedRate;
	}

	public override void OnUpdateToolData()
	{
		this.m_OriginalScale = base.transform.lossyScale;
		this.m_OriginalTiling.x = this.m_fTilingX;
		this.m_OriginalTiling.y = this.m_fTilingY;
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		this.ResetAnimation();
	}
}
