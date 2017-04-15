using System;
using UnityEngine;

public class NcParticleSpiral : NcEffectBehaviour
{
	public struct SpiralSettings
	{
		public int numArms;

		public int numPPA;

		public float partSep;

		public float turnDist;

		public float vertDist;

		public float originOffset;

		public float turnSpeed;

		public float fade;

		public float size;
	}

	protected const int Min_numArms = 1;

	protected const int Max_numArms = 10;

	protected const int Min_numPPA = 20;

	protected const int Max_numPPA = 60;

	protected const float Min_partSep = -0.3f;

	protected const float Max_partSep = 0.3f;

	protected const float Min_turnDist = -1.5f;

	protected const float Max_turnDist = 1.5f;

	protected const float Min_vertDist = 0f;

	protected const float Max_vertDist = 0.5f;

	protected const float Min_originOffset = -3f;

	protected const float Max_originOffset = 3f;

	protected const float Min_turnSpeed = -180f;

	protected const float Max_turnSpeed = 180f;

	protected const float Min_fade = -1f;

	protected const float Max_fade = 1f;

	protected const float Min_size = -2f;

	protected const float Max_size = 2f;

	public float m_fDelayTime;

	protected float m_fStartTime;

	public GameObject m_ParticlePrefab;

	public int m_nNumberOfArms = 2;

	public int m_nParticlesPerArm = 100;

	public float m_fParticleSeparation = 0.05f;

	public float m_fTurnDistance = 0.5f;

	public float m_fVerticalTurnDistance;

	public float m_fOriginOffset;

	public float m_fTurnSpeed;

	public float m_fFadeValue;

	public float m_fSizeValue;

	public int m_nNumberOfSpawns = 9999999;

	public float m_fSpawnRate = 5f;

	private float timeOfLastSpawn = -1000f;

	private int spawnCount;

	private int totParticles;

	private NcParticleSpiral.SpiralSettings defaultSettings;

	public override int GetAnimationState()
	{
		if (!base.enabled || !NcEffectBehaviour.IsActive(base.gameObject))
		{
			return -1;
		}
		if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime + 0.1f)
		{
			return 1;
		}
		return -1;
	}

	public void RandomizeEditor()
	{
		this.m_nNumberOfArms = UnityEngine.Random.Range(1, 10);
		this.m_nParticlesPerArm = UnityEngine.Random.Range(20, 60);
		this.m_fParticleSeparation = UnityEngine.Random.Range(-0.3f, 0.3f);
		this.m_fTurnDistance = UnityEngine.Random.Range(-1.5f, 1.5f);
		this.m_fVerticalTurnDistance = UnityEngine.Random.Range(0f, 0.5f);
		this.m_fOriginOffset = UnityEngine.Random.Range(-3f, 3f);
		this.m_fTurnSpeed = UnityEngine.Random.Range(-180f, 180f);
		this.m_fFadeValue = UnityEngine.Random.Range(-1f, 1f);
		this.m_fSizeValue = UnityEngine.Random.Range(-2f, 2f);
	}

	private void Start()
	{
		this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
		if (this.m_ParticlePrefab == null)
		{
			ParticleEmitter component = base.GetComponent<ParticleEmitter>();
			if (component == null)
			{
				return;
			}
			component.emit = false;
		}
		this.defaultSettings = this.getSettings();
	}

	private void SpawnEffect()
	{
		GameObject gameObject;
		if (this.m_ParticlePrefab != null)
		{
			gameObject = base.CreateGameObject(this.m_ParticlePrefab);
			if (gameObject == null)
			{
				return;
			}
			base.ChangeParent(base.transform, gameObject.transform, true, null);
		}
		else
		{
			gameObject = base.gameObject;
		}
		ParticleEmitter component = gameObject.GetComponent<ParticleEmitter>();
		if (component == null)
		{
			return;
		}
		component.emit = false;
		component.useWorldSpace = false;
		ParticleAnimator component2 = component.transform.GetComponent<ParticleAnimator>();
		if (component2 != null)
		{
			component2.autodestruct = true;
		}
		component.Emit(this.m_nNumberOfArms * this.m_nParticlesPerArm);
		Particle[] particles = component.particles;
		float num = 6.28318548f / (float)this.m_nNumberOfArms;
		for (int i = 0; i < this.m_nNumberOfArms; i++)
		{
			float num2 = 0f;
			float f = (float)i * num;
			for (int j = 0; j < this.m_nParticlesPerArm; j++)
			{
				int num3 = i * this.m_nParticlesPerArm + j;
				float num4 = this.m_fOriginOffset + this.m_fTurnDistance * num2;
				Vector3 position = gameObject.transform.localPosition;
				position.x += num4 * Mathf.Cos(num2);
				position.z += num4 * Mathf.Sin(num2);
				float x = position.x * Mathf.Cos(f) + position.z * Mathf.Sin(f);
				float z = -position.x * Mathf.Sin(f) + position.z * Mathf.Cos(f);
				position.x = x;
				position.z = z;
				position.y += (float)j * this.m_fVerticalTurnDistance;
				if (component.useWorldSpace)
				{
					position = base.transform.TransformPoint(position);
				}
				particles[num3].position = position;
				num2 += this.m_fParticleSeparation;
				if (this.m_fFadeValue != 0f)
				{
					particles[num3].energy = particles[num3].energy * (1f - Mathf.Abs(this.m_fFadeValue)) + particles[num3].energy * Mathf.Abs(this.m_fFadeValue) * (float)((this.m_fFadeValue >= 0f) ? (j + 1) : (this.m_nParticlesPerArm - j)) / (float)this.m_nParticlesPerArm;
				}
				if (this.m_fSizeValue != 0f)
				{
					Particle[] expr_276_cp_0 = particles;
					int expr_276_cp_1 = num3;
					expr_276_cp_0[expr_276_cp_1].size = expr_276_cp_0[expr_276_cp_1].size + Mathf.Abs(this.m_fSizeValue) * (float)((this.m_fSizeValue >= 0f) ? (j + 1) : (this.m_nParticlesPerArm - j)) / (float)this.m_nParticlesPerArm;
				}
			}
		}
		component.particles = particles;
	}

	private void Update()
	{
		if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)
		{
			return;
		}
		if (this.m_fTurnSpeed != 0f)
		{
			base.transform.Rotate(base.transform.up * NcEffectBehaviour.GetEngineDeltaTime() * this.m_fTurnSpeed, Space.World);
		}
	}

	private void LateUpdate()
	{
		if (NcEffectBehaviour.GetEngineTime() < this.m_fStartTime + this.m_fDelayTime)
		{
			return;
		}
		float num = NcEffectBehaviour.GetEngineTime() - this.timeOfLastSpawn;
		if (this.m_fSpawnRate <= num && this.spawnCount < this.m_nNumberOfSpawns)
		{
			this.SpawnEffect();
			this.timeOfLastSpawn = NcEffectBehaviour.GetEngineTime();
			this.spawnCount++;
		}
	}

	public NcParticleSpiral.SpiralSettings getSettings()
	{
		NcParticleSpiral.SpiralSettings result;
		result.numArms = this.m_nNumberOfArms;
		result.numPPA = this.m_nParticlesPerArm;
		result.partSep = this.m_fParticleSeparation;
		result.turnDist = this.m_fTurnDistance;
		result.vertDist = this.m_fVerticalTurnDistance;
		result.originOffset = this.m_fOriginOffset;
		result.turnSpeed = this.m_fTurnSpeed;
		result.fade = this.m_fFadeValue;
		result.size = this.m_fSizeValue;
		return result;
	}

	public NcParticleSpiral.SpiralSettings resetEffect(bool killCurrent, NcParticleSpiral.SpiralSettings settings)
	{
		if (killCurrent)
		{
			this.killCurrentEffects();
		}
		this.m_nNumberOfArms = settings.numArms;
		this.m_nParticlesPerArm = settings.numPPA;
		this.m_fParticleSeparation = settings.partSep;
		this.m_fTurnDistance = settings.turnDist;
		this.m_fVerticalTurnDistance = settings.vertDist;
		this.m_fOriginOffset = settings.originOffset;
		this.m_fTurnSpeed = settings.turnSpeed;
		this.m_fFadeValue = settings.fade;
		this.m_fSizeValue = settings.size;
		this.SpawnEffect();
		this.timeOfLastSpawn = NcEffectBehaviour.GetEngineTime();
		this.spawnCount++;
		return this.getSettings();
	}

	public NcParticleSpiral.SpiralSettings resetEffectToDefaults(bool killCurrent)
	{
		return this.resetEffect(killCurrent, this.defaultSettings);
	}

	public NcParticleSpiral.SpiralSettings randomizeEffect(bool killCurrent)
	{
		if (killCurrent)
		{
			this.killCurrentEffects();
		}
		this.RandomizeEditor();
		this.SpawnEffect();
		this.timeOfLastSpawn = NcEffectBehaviour.GetEngineTime();
		this.spawnCount++;
		return this.getSettings();
	}

	private void killCurrentEffects()
	{
		ParticleEmitter[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleEmitter>();
		ParticleEmitter[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleEmitter particleEmitter = array[i];
			Debug.Log("resetEffect killing: " + particleEmitter.name);
			ParticleAnimator component = particleEmitter.transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = true;
			}
			Particle[] particles = particleEmitter.particles;
			for (int j = 0; j < particles.Length; j++)
			{
				particles[j].energy = 0.1f;
			}
			particleEmitter.particles = particles;
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fTurnSpeed *= fSpeedRate;
	}
}
