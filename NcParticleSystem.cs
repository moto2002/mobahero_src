using System;
using System.Reflection;
using UnityEngine;

public class NcParticleSystem : NcEffectBehaviour
{
	public enum ParticleDestruct
	{
		NONE,
		COLLISION,
		WORLD_Y
	}

	protected const int m_nAllocBufCount = 50;

	protected bool m_bDisabledEmit;

	public float m_fStartDelayTime;

	public bool m_bBurst;

	public float m_fBurstRepeatTime = 0.5f;

	public int m_nBurstRepeatCount;

	public int m_fBurstEmissionCount = 10;

	public float m_fEmitTime;

	public float m_fSleepTime;

	public bool m_bScaleWithTransform;

	public bool m_bWorldSpace = true;

	public float m_fStartSizeRate = 1f;

	public float m_fStartLifeTimeRate = 1f;

	public float m_fStartEmissionRate = 1f;

	public float m_fStartSpeedRate = 1f;

	public float m_fRenderLengthRate = 1f;

	public float m_fLegacyMinMeshNormalVelocity = 10f;

	public float m_fLegacyMaxMeshNormalVelocity = 10f;

	public float m_fShurikenSpeedRate = 1f;

	protected bool m_bStart;

	protected Vector3 m_OldPos = Vector3.zero;

	protected bool m_bLegacyRuntimeScale = true;

	public NcParticleSystem.ParticleDestruct m_ParticleDestruct;

	public LayerMask m_CollisionLayer = -1;

	public float m_fCollisionRadius = 0.3f;

	public float m_fDestructPosY = 0.2f;

	public GameObject m_AttachPrefab;

	public float m_fPrefabScale = 1f;

	public float m_fPrefabSpeed = 1f;

	public float m_fPrefabLifeTime = 2f;

	protected bool m_bSleep;

	protected float m_fStartTime;

	protected float m_fDurationStartTime;

	protected float m_fEmitStartTime;

	protected int m_nCreateCount;

	protected bool m_bScalePreRender;

	protected bool m_bMeshParticleEmitter;

	protected ParticleSystem m_ps;

	protected ParticleEmitter m_pe;

	protected ParticleAnimator m_pa;

	protected ParticleRenderer m_pr;

	protected ParticleSystem.Particle[] m_BufPsParts;

	protected ParticleSystem.Particle[] m_BufColliderOriParts;

	protected ParticleSystem.Particle[] m_BufColliderConParts;

	public void SetDisableEmit()
	{
		this.m_bDisabledEmit = true;
	}

	public bool IsShuriken()
	{
		return base.particleSystem != null;
	}

	public bool IsLegacy()
	{
		return base.particleEmitter != null && base.particleEmitter.enabled;
	}

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (this.m_bBurst)
		{
			if (0 >= this.m_nBurstRepeatCount)
			{
				return 1;
			}
			if (this.m_nCreateCount < this.m_nBurstRepeatCount)
			{
				return 1;
			}
			return 0;
		}
		else
		{
			if (0f < this.m_fStartDelayTime)
			{
				return 1;
			}
			if (0f >= this.m_fEmitTime || this.m_fSleepTime > 0f)
			{
				return -1;
			}
			if (this.m_nCreateCount < 1)
			{
				return 1;
			}
			return 0;
		}
	}

	public bool IsMeshParticleEmitter()
	{
		return this.m_bMeshParticleEmitter;
	}

	public void ResetParticleEmit(bool bClearOldParticle)
	{
		this.m_bDisabledEmit = false;
		this.m_OldPos = base.transform.position;
		this.Init();
		if (bClearOldParticle && this.m_pe != null)
		{
			this.m_pe.ClearParticles();
		}
		if (this.m_bBurst || 0f < this.m_fStartDelayTime)
		{
			this.SetEnableParticle(false);
		}
		else
		{
			this.SetEnableParticle(true);
		}
	}

	private void Awake()
	{
		if (this.IsShuriken())
		{
			this.m_ps = base.particleSystem;
		}
		else
		{
			this.m_pe = base.GetComponent<ParticleEmitter>();
			this.m_pa = base.GetComponent<ParticleAnimator>();
			this.m_pr = base.GetComponent<ParticleRenderer>();
			if (this.m_pe != null)
			{
				this.m_bMeshParticleEmitter = this.m_pe.ToString().Contains("MeshParticleEmitter");
			}
		}
	}

	private void OnEnable()
	{
		if (this.m_bScaleWithTransform)
		{
			this.AddRenderEventCall();
		}
		this.m_OldPos = base.transform.position;
	}

	private void OnDisable()
	{
		if (this.m_bScaleWithTransform)
		{
			this.RemoveRenderEventCall();
		}
	}

	private void Init()
	{
		this.m_bSleep = false;
		this.m_nCreateCount = 0;
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		this.m_fEmitStartTime = 0f;
	}

	private void Start()
	{
		if (this.m_bDisabledEmit)
		{
			return;
		}
		this.m_bStart = true;
		this.Init();
		if (this.IsShuriken())
		{
			this.ShurikenInitParticle();
		}
		else
		{
			this.LegacyInitParticle();
		}
		if (this.m_bBurst || 0f < this.m_fStartDelayTime)
		{
			this.SetEnableParticle(false);
		}
	}

	private void Update()
	{
		if (this.m_bDisabledEmit)
		{
			return;
		}
		if (this.m_fEmitStartTime == 0f)
		{
			if (0f < this.m_fStartDelayTime)
			{
				if (this.m_fStartTime + this.m_fStartDelayTime <= NcEffectBehaviour.GetEngineTime())
				{
					this.m_fEmitStartTime = NcEffectBehaviour.GetEngineTime();
					this.m_fDurationStartTime = NcEffectBehaviour.GetEngineTime();
					this.SetEnableParticle(true);
				}
				return;
			}
			this.m_fEmitStartTime = NcEffectBehaviour.GetEngineTime();
			this.m_fDurationStartTime = NcEffectBehaviour.GetEngineTime();
		}
		if (this.m_bBurst)
		{
			if (this.m_fDurationStartTime <= NcEffectBehaviour.GetEngineTime())
			{
				if (this.m_nBurstRepeatCount == 0 || this.m_nCreateCount < this.m_nBurstRepeatCount)
				{
					this.m_fDurationStartTime = this.m_fBurstRepeatTime + NcEffectBehaviour.GetEngineTime();
					this.m_nCreateCount++;
					if (this.IsShuriken())
					{
						this.m_ps.Emit(this.m_fBurstEmissionCount);
					}
					else if (this.m_pe != null)
					{
						this.m_pe.Emit(this.m_fBurstEmissionCount);
					}
				}
			}
		}
		else if (this.m_bSleep)
		{
			if (this.m_fEmitStartTime + this.m_fEmitTime + this.m_fSleepTime < NcEffectBehaviour.GetEngineTime())
			{
				this.SetEnableParticle(true);
				this.m_fEmitStartTime = NcEffectBehaviour.GetEngineTime();
				this.m_bSleep = false;
			}
		}
		else if (0f < this.m_fEmitTime && this.m_fEmitStartTime + this.m_fEmitTime < NcEffectBehaviour.GetEngineTime())
		{
			this.m_nCreateCount++;
			this.SetEnableParticle(false);
			if (0f < this.m_fSleepTime)
			{
				this.m_bSleep = true;
			}
			else
			{
				this.SetDisableEmit();
			}
		}
	}

	private void FixedUpdate()
	{
		if (this.m_ParticleDestruct != NcParticleSystem.ParticleDestruct.NONE)
		{
			bool flag = false;
			if (this.IsShuriken())
			{
				if (this.m_ps != null)
				{
					this.AllocateParticleSystem(ref this.m_BufColliderOriParts);
					this.AllocateParticleSystem(ref this.m_BufColliderConParts);
					this.m_ps.GetParticles(this.m_BufColliderOriParts);
					this.m_ps.GetParticles(this.m_BufColliderConParts);
					this.ShurikenScaleParticle(this.m_BufColliderConParts, this.m_ps.particleCount, this.m_bScaleWithTransform, true);
					for (int i = 0; i < this.m_ps.particleCount; i++)
					{
						bool flag2 = false;
						Vector3 position;
						if (this.m_bWorldSpace)
						{
							position = this.m_BufColliderConParts[i].position;
						}
						else
						{
							position = base.transform.TransformPoint(this.m_BufColliderConParts[i].position);
						}
						if (this.m_ParticleDestruct == NcParticleSystem.ParticleDestruct.COLLISION)
						{
							if (Physics.CheckSphere(position, this.m_fCollisionRadius, this.m_CollisionLayer))
							{
								flag2 = true;
							}
						}
						else if (this.m_ParticleDestruct == NcParticleSystem.ParticleDestruct.WORLD_Y && position.y <= this.m_fDestructPosY)
						{
							flag2 = true;
						}
						if (flag2 && 0f < this.m_BufColliderOriParts[i].lifetime)
						{
							this.m_BufColliderOriParts[i].lifetime = 0f;
							flag = true;
							this.CreateAttachPrefab(position, this.m_BufColliderConParts[i].size * this.m_fPrefabScale);
						}
					}
					if (flag)
					{
						this.m_ps.SetParticles(this.m_BufColliderOriParts, this.m_ps.particleCount);
					}
				}
			}
			else if (this.m_pe != null)
			{
				Particle[] particles = this.m_pe.particles;
				Particle[] particles2 = this.m_pe.particles;
				this.LegacyScaleParticle(particles2, this.m_bScaleWithTransform, true);
				for (int j = 0; j < particles2.Length; j++)
				{
					bool flag3 = false;
					Vector3 position;
					if (this.m_bWorldSpace)
					{
						position = particles2[j].position;
					}
					else
					{
						position = base.transform.TransformPoint(particles2[j].position);
					}
					if (this.m_ParticleDestruct == NcParticleSystem.ParticleDestruct.COLLISION)
					{
						if (Physics.CheckSphere(position, this.m_fCollisionRadius, this.m_CollisionLayer))
						{
							flag3 = true;
						}
					}
					else if (this.m_ParticleDestruct == NcParticleSystem.ParticleDestruct.WORLD_Y && position.y <= this.m_fDestructPosY)
					{
						flag3 = true;
					}
					if (flag3 && 0f < particles[j].energy)
					{
						particles[j].energy = 0f;
						flag = true;
						this.CreateAttachPrefab(position, particles2[j].size * this.m_fPrefabScale);
					}
				}
				if (flag)
				{
					this.m_pe.particles = particles;
				}
			}
		}
	}

	private void OnPreRender()
	{
		if (!this.m_bStart)
		{
			return;
		}
		if (this.m_bScaleWithTransform)
		{
			this.m_bScalePreRender = true;
			if (this.IsShuriken())
			{
				this.ShurikenSetRuntimeParticleScale(true);
			}
			else
			{
				this.LegacySetRuntimeParticleScale(true);
			}
		}
	}

	private void OnPostRender()
	{
		if (!this.m_bStart)
		{
			return;
		}
		if (this.m_bScalePreRender)
		{
			if (this.IsShuriken())
			{
				this.ShurikenSetRuntimeParticleScale(false);
			}
			else
			{
				this.LegacySetRuntimeParticleScale(false);
			}
		}
		this.m_OldPos = base.transform.position;
		this.m_bScalePreRender = false;
	}

	private void CreateAttachPrefab(Vector3 position, float size)
	{
		if (this.m_AttachPrefab == null)
		{
			return;
		}
		GameObject gameObject = base.CreateGameObject(this.m_AttachPrefab, this.m_AttachPrefab.transform.position + position, this.m_AttachPrefab.transform.rotation);
		if (gameObject == null)
		{
			return;
		}
		base.ChangeParent(NcEffectBehaviour.GetRootInstanceEffect().transform, gameObject.transform, false, null);
		NcTransformTool.CopyLossyToLocalScale(gameObject.transform.lossyScale * size, gameObject.transform);
		NsEffectManager.AdjustSpeedRuntime(gameObject, this.m_fPrefabSpeed);
		if (0f < this.m_fPrefabLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = gameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = gameObject.AddComponent<NcAutoDestruct>();
			}
			ncAutoDestruct.m_fLifeTime = this.m_fPrefabLifeTime;
		}
	}

	private void AddRenderEventCall()
	{
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			Camera camera = allCameras[i];
			NsRenderManager nsRenderManager = camera.GetComponent<NsRenderManager>();
			if (nsRenderManager == null)
			{
				nsRenderManager = camera.gameObject.AddComponent<NsRenderManager>();
			}
			nsRenderManager.AddRenderEventCall(this);
		}
	}

	private void RemoveRenderEventCall()
	{
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			Camera camera = allCameras[i];
			NsRenderManager component = camera.GetComponent<NsRenderManager>();
			if (component != null)
			{
				component.RemoveRenderEventCall(this);
			}
		}
	}

	private void SetEnableParticle(bool bEnable)
	{
		if (this.m_ps != null)
		{
			this.m_ps.enableEmission = bEnable;
		}
		if (this.m_pe != null)
		{
			this.m_pe.emit = bEnable;
		}
	}

	private void ClearParticle()
	{
		if (this.m_ps != null)
		{
			this.m_ps.Clear(false);
		}
		if (this.m_pe != null)
		{
			this.m_pe.ClearParticles();
		}
	}

	public float GetScaleMinMeshNormalVelocity()
	{
		return this.m_fLegacyMinMeshNormalVelocity * ((!this.m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(base.transform));
	}

	public float GetScaleMaxMeshNormalVelocity()
	{
		return this.m_fLegacyMaxMeshNormalVelocity * ((!this.m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(base.transform));
	}

	private void LegacyInitParticle()
	{
		if (this.m_pe != null)
		{
			this.LegacySetParticle();
		}
	}

	private void LegacySetParticle()
	{
		ParticleEmitter pe = this.m_pe;
		ParticleAnimator pa = this.m_pa;
		ParticleRenderer pr = this.m_pr;
		if (pe == null || pr == null)
		{
			return;
		}
		if (this.m_bLegacyRuntimeScale)
		{
			Vector3 b = Vector3.one * this.m_fStartSpeedRate;
			float fStartSpeedRate = this.m_fStartSpeedRate;
			pe.minSize *= this.m_fStartSizeRate;
			pe.maxSize *= this.m_fStartSizeRate;
			pe.minEnergy *= this.m_fStartLifeTimeRate;
			pe.maxEnergy *= this.m_fStartLifeTimeRate;
			pe.minEmission *= this.m_fStartEmissionRate;
			pe.maxEmission *= this.m_fStartEmissionRate;
			pe.worldVelocity = Vector3.Scale(pe.worldVelocity, b);
			pe.localVelocity = Vector3.Scale(pe.localVelocity, b);
			pe.rndVelocity = Vector3.Scale(pe.rndVelocity, b);
			pe.angularVelocity *= fStartSpeedRate;
			pe.rndAngularVelocity *= fStartSpeedRate;
			pe.emitterVelocityScale *= fStartSpeedRate;
			if (pa != null)
			{
				pa.rndForce = Vector3.Scale(pa.rndForce, b);
				pa.force = Vector3.Scale(pa.force, b);
			}
			pr.lengthScale *= this.m_fRenderLengthRate;
		}
		else
		{
			Vector3 b2 = ((!this.m_bScaleWithTransform) ? Vector3.one : pe.transform.lossyScale) * this.m_fStartSpeedRate;
			float num = ((!this.m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(pe.transform)) * this.m_fStartSpeedRate;
			float num2 = ((!this.m_bScaleWithTransform) ? 1f : NcTransformTool.GetTransformScaleMeanValue(pe.transform)) * this.m_fStartSizeRate;
			pe.minSize *= num2;
			pe.maxSize *= num2;
			pe.minEnergy *= this.m_fStartLifeTimeRate;
			pe.maxEnergy *= this.m_fStartLifeTimeRate;
			pe.minEmission *= this.m_fStartEmissionRate;
			pe.maxEmission *= this.m_fStartEmissionRate;
			pe.worldVelocity = Vector3.Scale(pe.worldVelocity, b2);
			pe.localVelocity = Vector3.Scale(pe.localVelocity, b2);
			pe.rndVelocity = Vector3.Scale(pe.rndVelocity, b2);
			pe.angularVelocity *= num;
			pe.rndAngularVelocity *= num;
			pe.emitterVelocityScale *= num;
			if (pa != null)
			{
				pa.rndForce = Vector3.Scale(pa.rndForce, b2);
				pa.force = Vector3.Scale(pa.force, b2);
			}
			pr.lengthScale *= this.m_fRenderLengthRate;
		}
	}

	private void LegacyParticleSpeed(float fSpeed)
	{
		ParticleEmitter pe = this.m_pe;
		ParticleAnimator pa = this.m_pa;
		ParticleRenderer pr = this.m_pr;
		if (pe == null || pr == null)
		{
			return;
		}
		Vector3 b = Vector3.one * fSpeed;
		pe.minEnergy /= fSpeed;
		pe.maxEnergy /= fSpeed;
		pe.worldVelocity = Vector3.Scale(pe.worldVelocity, b);
		pe.localVelocity = Vector3.Scale(pe.localVelocity, b);
		pe.rndVelocity = Vector3.Scale(pe.rndVelocity, b);
		pe.angularVelocity *= fSpeed;
		pe.rndAngularVelocity *= fSpeed;
		pe.emitterVelocityScale *= fSpeed;
		if (pa != null)
		{
			pa.rndForce = Vector3.Scale(pa.rndForce, b);
			pa.force = Vector3.Scale(pa.force, b);
		}
	}

	private void LegacySetRuntimeParticleScale(bool bScale)
	{
		if (!this.m_bLegacyRuntimeScale)
		{
			return;
		}
		if (this.m_pe != null)
		{
			Particle[] particles = this.m_pe.particles;
			this.m_pe.particles = this.LegacyScaleParticle(particles, bScale, true);
		}
	}

	public Particle[] LegacyScaleParticle(Particle[] parts, bool bScale, bool bPosUpdate)
	{
		float num;
		if (bScale)
		{
			num = NcTransformTool.GetTransformScaleMeanValue(base.transform);
		}
		else
		{
			num = 1f / NcTransformTool.GetTransformScaleMeanValue(base.transform);
		}
		for (int i = 0; i < parts.Length; i++)
		{
			if (!this.IsMeshParticleEmitter())
			{
				if (this.m_bWorldSpace)
				{
					if (bPosUpdate)
					{
						Vector3 a = this.m_OldPos - base.transform.position;
						if (bScale)
						{
							int expr_70_cp_1 = i;
							parts[expr_70_cp_1].position = parts[expr_70_cp_1].position - a * (1f - 1f / num);
						}
					}
					int expr_9A_cp_1 = i;
					parts[expr_9A_cp_1].position = parts[expr_9A_cp_1].position - base.transform.position;
					int expr_BC_cp_1 = i;
					parts[expr_BC_cp_1].position = parts[expr_BC_cp_1].position * num;
					int expr_D4_cp_1 = i;
					parts[expr_D4_cp_1].position = parts[expr_D4_cp_1].position + base.transform.position;
				}
				else
				{
					int expr_FB_cp_1 = i;
					parts[expr_FB_cp_1].position = parts[expr_FB_cp_1].position * num;
				}
			}
			int expr_113_cp_1 = i;
			parts[expr_113_cp_1].angularVelocity = parts[expr_113_cp_1].angularVelocity * num;
			int expr_127_cp_1 = i;
			parts[expr_127_cp_1].velocity = parts[expr_127_cp_1].velocity * num;
			int expr_13F_cp_1 = i;
			parts[expr_13F_cp_1].size = parts[expr_13F_cp_1].size * num;
		}
		return parts;
	}

	private void ShurikenInitParticle()
	{
		if (this.m_ps != null)
		{
			this.m_ps.startSize *= this.m_fStartSizeRate;
			this.m_ps.startLifetime *= this.m_fStartLifeTimeRate;
			this.m_ps.emissionRate *= this.m_fStartEmissionRate;
			this.m_ps.startSpeed *= this.m_fStartSpeedRate;
		}
	}

	private void AllocateParticleSystem(ref ParticleSystem.Particle[] tmpPsParts)
	{
		if (tmpPsParts == null || tmpPsParts.Length < this.m_ps.particleCount)
		{
			tmpPsParts = new ParticleSystem.Particle[this.m_ps.particleCount + 50];
		}
	}

	private void ShurikenSetRuntimeParticleScale(bool bScale)
	{
		if (this.m_ps != null)
		{
			this.AllocateParticleSystem(ref this.m_BufPsParts);
			this.m_ps.GetParticles(this.m_BufPsParts);
			this.m_BufPsParts = this.ShurikenScaleParticle(this.m_BufPsParts, this.m_ps.particleCount, bScale, true);
			this.m_ps.SetParticles(this.m_BufPsParts, this.m_ps.particleCount);
		}
	}

	public ParticleSystem.Particle[] ShurikenScaleParticle(ParticleSystem.Particle[] parts, int nCount, bool bScale, bool bPosUpdate)
	{
		float num;
		if (bScale)
		{
			num = NcTransformTool.GetTransformScaleMeanValue(base.transform);
		}
		else
		{
			num = 1f / NcTransformTool.GetTransformScaleMeanValue(base.transform);
		}
		for (int i = 0; i < nCount; i++)
		{
			if (this.m_bWorldSpace)
			{
				if (bPosUpdate)
				{
					Vector3 a = this.m_OldPos - base.transform.position;
					if (bScale)
					{
						int expr_66_cp_1 = i;
						parts[expr_66_cp_1].position = parts[expr_66_cp_1].position - a * (1f - 1f / num);
					}
				}
				int expr_90_cp_1 = i;
				parts[expr_90_cp_1].position = parts[expr_90_cp_1].position - base.transform.position;
				int expr_B2_cp_1 = i;
				parts[expr_B2_cp_1].position = parts[expr_B2_cp_1].position * num;
				int expr_CA_cp_1 = i;
				parts[expr_CA_cp_1].position = parts[expr_CA_cp_1].position + base.transform.position;
			}
			else
			{
				int expr_F1_cp_1 = i;
				parts[expr_F1_cp_1].position = parts[expr_F1_cp_1].position * num;
			}
			int expr_109_cp_1 = i;
			parts[expr_109_cp_1].size = parts[expr_109_cp_1].size * num;
		}
		return parts;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fStartDelayTime /= fSpeedRate;
		this.m_fBurstRepeatTime /= fSpeedRate;
		this.m_fEmitTime /= fSpeedRate;
		this.m_fSleepTime /= fSpeedRate;
		this.m_fShurikenSpeedRate *= fSpeedRate;
		this.LegacyParticleSpeed(fSpeedRate);
		this.m_fPrefabLifeTime /= fSpeedRate;
		this.m_fPrefabSpeed *= fSpeedRate;
	}

	public override void OnSetReplayState()
	{
		base.OnSetReplayState();
		this.m_pe = base.GetComponent<ParticleEmitter>();
		this.m_pa = base.GetComponent<ParticleAnimator>();
		if (this.m_pa != null)
		{
			this.m_pa.autodestruct = false;
		}
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		this.ResetParticleEmit(bClearOldParticle);
	}

	public static void Ng_SetProperty(object srcObj, string fieldName, object newValue)
	{
		PropertyInfo property = srcObj.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && property.CanWrite)
		{
			property.SetValue(srcObj, newValue, null);
		}
		else
		{
			Debug.LogWarning(fieldName + " could not be write.");
		}
	}

	public static object Ng_GetProperty(object srcObj, string fieldName)
	{
		object result = null;
		PropertyInfo property = srcObj.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null && property.CanRead && property.GetIndexParameters().Length == 0)
		{
			result = property.GetValue(srcObj, null);
		}
		else
		{
			Debug.LogWarning(fieldName + " could not be read.");
		}
		return result;
	}
}
