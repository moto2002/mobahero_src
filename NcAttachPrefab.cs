using System;
using UnityEngine;

public class NcAttachPrefab : NcEffectBehaviour
{
	public enum AttachType
	{
		Active,
		Destroy
	}

	public NcAttachPrefab.AttachType m_AttachType;

	public float m_fDelayTime;

	public float m_fRepeatTime;

	public int m_nRepeatCount;

	public GameObject m_AttachPrefab;

	public float m_fPrefabSpeed = 1f;

	public float m_fPrefabLifeTime;

	public bool m_bWorldSpace;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_AccumStartRot = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	public int m_nSpriteFactoryIndex = -1;

	[HideInInspector]
	public bool m_bDetachParent;

	protected float m_fStartTime;

	protected int m_nCreateCount;

	protected bool m_bStartAttach;

	protected GameObject[] m_CreateGameObjects;

	protected bool m_bEnabled;

	public override int GetAnimationState()
	{
		if (this.m_bEnabled && base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && this.m_AttachPrefab != null)
		{
			if (this.m_AttachType == NcAttachPrefab.AttachType.Active && ((this.m_nRepeatCount == 0 && this.m_nCreateCount < 1) || (0f < this.m_fRepeatTime && this.m_nRepeatCount == 0) || (0 < this.m_nRepeatCount && this.m_nCreateCount < this.m_nRepeatCount)))
			{
				return 1;
			}
			if (this.m_AttachType == NcAttachPrefab.AttachType.Destroy)
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

	public void CreateAttachInstance()
	{
		this.CreateAttachGameObject();
	}

	public GameObject GetInstanceObject()
	{
		if (this.m_CreateGameObjects == null)
		{
			this.UpdateImmediately();
		}
		return (this.m_CreateGameObjects != null && this.m_CreateGameObjects.Length >= 1) ? this.m_CreateGameObjects[0] : null;
	}

	public virtual GameObject[] GetInstanceObjects()
	{
		if (this.m_CreateGameObjects == null)
		{
			this.UpdateImmediately();
		}
		return this.m_CreateGameObjects;
	}

	public void SetEnable(bool bEnable)
	{
		this.m_bEnabled = bEnable;
	}

	protected virtual void Awake()
	{
		this.m_bEnabled = (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && base.GetComponent<NcDontActive>() == null);
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		if (this.m_AttachPrefab == null)
		{
			return;
		}
		if (this.m_AttachType == NcAttachPrefab.AttachType.Active)
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
		if (this.m_bEnabled && NcEffectBehaviour.IsSafe() && this.m_AttachType == NcAttachPrefab.AttachType.Destroy && this.m_AttachPrefab != null)
		{
			this.CreateAttachPrefab();
		}
		base.OnDestroy();
	}

	private void CreateAttachPrefab()
	{
		this.CreateAttachGameObject();
		if ((this.m_fRepeatTime == 0f || this.m_AttachType == NcAttachPrefab.AttachType.Destroy) && 0 < this.m_nRepeatCount && this.m_nCreateCount < this.m_nRepeatCount)
		{
			this.CreateAttachPrefab();
		}
	}

	private void CreateAttachGameObject()
	{
		GameObject gameObject = base.CreateGameObject(this.GetTargetGameObject(), (!(this.GetTargetGameObject() == base.gameObject)) ? base.transform : null, this.m_AttachPrefab);
		if (this.m_bReplayState)
		{
			NsEffectManager.SetReplayEffect(gameObject);
		}
		if (gameObject == null)
		{
			return;
		}
		if (this.m_AttachType == NcAttachPrefab.AttachType.Active)
		{
			if (this.m_CreateGameObjects == null)
			{
				this.m_CreateGameObjects = new GameObject[Mathf.Max(1, this.m_nRepeatCount)];
			}
			for (int i = 0; i < this.m_CreateGameObjects.Length; i++)
			{
				if (this.m_CreateGameObjects[i] == null)
				{
					this.m_CreateGameObjects[i] = gameObject;
					break;
				}
			}
		}
		this.m_nCreateCount++;
		Vector3 position = gameObject.transform.position;
		gameObject.transform.position = this.m_AddStartPos + new Vector3(UnityEngine.Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + position.x, UnityEngine.Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + position.y, UnityEngine.Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + position.z);
		gameObject.transform.localRotation *= Quaternion.Euler(this.m_AccumStartRot.x * (float)this.m_nCreateCount, this.m_AccumStartRot.y * (float)this.m_nCreateCount, this.m_AccumStartRot.z * (float)this.m_nCreateCount);
		GameObject expr_1B0 = gameObject;
		expr_1B0.name = expr_1B0.name + " " + this.m_nCreateCount;
		NcEffectBehaviour.SetActiveRecursively(gameObject, true);
		NsEffectManager.AdjustSpeedRuntime(gameObject, this.m_fPrefabSpeed);
		if (0f < this.m_fPrefabLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = gameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = base.AddNcComponentToObject<NcAutoDestruct>(gameObject);
			}
			ncAutoDestruct.m_fLifeTime = this.m_fPrefabLifeTime;
		}
		if (this.m_bDetachParent)
		{
			NcDetachParent x = gameObject.GetComponent<NcDetachParent>();
			if (x == null)
			{
				x = base.AddNcComponentToObject<NcDetachParent>(gameObject);
			}
		}
		if (0 <= this.m_nSpriteFactoryIndex)
		{
			NcSpriteFactory component = gameObject.GetComponent<NcSpriteFactory>();
			if (component)
			{
				component.SetSprite(this.m_nSpriteFactoryIndex, false);
			}
		}
		this.OnCreateAttachGameObject();
	}

	protected virtual void OnCreateAttachGameObject()
	{
	}

	private GameObject GetTargetGameObject()
	{
		if (this.m_bWorldSpace || this.m_AttachType == NcAttachPrefab.AttachType.Destroy)
		{
			return NcEffectBehaviour.GetRootInstanceEffect();
		}
		return base.gameObject;
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDelayTime /= fSpeedRate;
		this.m_fRepeatTime /= fSpeedRate;
		this.m_fPrefabLifeTime /= fSpeedRate;
		this.m_fPrefabSpeed *= fSpeedRate;
	}

	public override void OnSetActiveRecursively(bool bActive)
	{
		if (this.m_CreateGameObjects == null)
		{
			return;
		}
		for (int i = 0; i < this.m_CreateGameObjects.Length; i++)
		{
			if (this.m_CreateGameObjects[i] != null)
			{
				NsEffectManager.SetActiveRecursively(this.m_CreateGameObjects[i], bActive);
			}
		}
	}

	public static void Ng_ChangeLayerWithChild(GameObject rootObj, int nLayer)
	{
		if (rootObj == null)
		{
			return;
		}
		rootObj.layer = nLayer;
		for (int i = 0; i < rootObj.transform.childCount; i++)
		{
			NcAttachPrefab.Ng_ChangeLayerWithChild(rootObj.transform.GetChild(i).gameObject, nLayer);
		}
	}
}
