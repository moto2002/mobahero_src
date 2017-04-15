using System;
using UnityEngine;

public class NcParticleEmit : NcEffectBehaviour
{
	public enum AttachType
	{
		Active,
		Destroy
	}

	public NcParticleEmit.AttachType m_AttachType;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public GameObject m_ParticlePrefab;

	public int m_EmitCount = 10;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected GameObject m_CreateGameObject;

	protected bool m_bEnabled;

	protected ParticleSystem m_ps;

	public override int GetAnimationState()
	{
		if (this.m_bEnabled && base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && this.m_ParticlePrefab != null)
		{
			if (this.m_AttachType == NcParticleEmit.AttachType.Active && ((this.m_nRepeatCount == 0 && this.m_nCreateCount < 1) || (0f < this.m_fRepeatTime && this.m_nRepeatCount == 0) || (0 < this.m_nRepeatCount && this.m_nCreateCount < this.m_nRepeatCount)))
			{
				return 1;
			}
			if (this.m_AttachType == NcParticleEmit.AttachType.Destroy)
			{
				return 1;
			}
		}
		return 0;
	}

	public void UpdateImmediately()
	{
		this.Update();
	}

	public GameObject EmitSharedParticle()
	{
		return this.CreateAttachSharedParticle();
	}

	public GameObject GetInstanceObject()
	{
		if (this.m_CreateGameObject == null)
		{
			this.UpdateImmediately();
		}
		return this.m_CreateGameObject;
	}

	public void SetEnable(bool bEnable)
	{
		this.m_bEnabled = bEnable;
	}

	private void Awake()
	{
		this.m_bEnabled = (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && base.GetComponent<NcDontActive>() == null);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.m_ParticlePrefab == null)
		{
			return;
		}
		if (this.m_AttachType == NcParticleEmit.AttachType.Active)
		{
			if (!this.m_bStartAttach)
			{
				this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
				this.m_bStartAttach = true;
			}
			if (this.m_fStartTime + this.m_fDelayTime <= NcEffectBehaviour.GetEngineTime())
			{
				this.CreateAttachPrefab();
				if ((0f < this.m_fRepeatTime && this.m_nRepeatCount == 0) || this.m_nCreateCount < this.m_nRepeatCount)
				{
					this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
					this.m_fDelayTime = this.m_fRepeatTime;
				}
				else
				{
					base.enabled = false;
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		if (this.m_bEnabled && NcEffectBehaviour.IsSafe() && this.m_AttachType == NcParticleEmit.AttachType.Destroy && this.m_ParticlePrefab != null)
		{
			this.CreateAttachPrefab();
		}
		base.OnDestroy();
	}

	private void CreateAttachPrefab()
	{
		this.m_nCreateCount++;
		this.CreateAttachSharedParticle();
		if ((this.m_fRepeatTime == 0f || this.m_AttachType == NcParticleEmit.AttachType.Destroy) && 0 < this.m_nRepeatCount && this.m_nCreateCount < this.m_nRepeatCount)
		{
			this.CreateAttachPrefab();
		}
	}

	private GameObject CreateAttachSharedParticle()
	{
		if (this.m_CreateGameObject == null)
		{
			this.m_CreateGameObject = NsSharedManager.inst.GetSharedParticleGameObject(this.m_ParticlePrefab);
		}
		if (this.m_CreateGameObject == null)
		{
			return null;
		}
		Vector3 vector = base.transform.position + this.m_AddStartPos + this.m_ParticlePrefab.transform.position;
		this.m_CreateGameObject.transform.position = new Vector3(UnityEngine.Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + vector.x, UnityEngine.Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + vector.y, UnityEngine.Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + vector.z);
		if (this.m_CreateGameObject.particleEmitter != null)
		{
			this.m_CreateGameObject.particleEmitter.Emit(this.m_EmitCount);
		}
		else
		{
			if (this.m_ps == null)
			{
				this.m_ps = this.m_CreateGameObject.GetComponent<ParticleSystem>();
			}
			if (this.m_ps != null)
			{
				this.m_ps.Emit(this.m_EmitCount);
			}
		}
		return this.m_CreateGameObject;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fRepeatTime /= fSpeedRate;
	}
}
