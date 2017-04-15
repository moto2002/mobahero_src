using System;
using UnityEngine;

public class NcDuplicator : NcEffectBehaviour
{
	public float m_fDuplicateTime = 0.1f;

	public int m_nDuplicateCount = 3;

	public float m_fDuplicateLifeTime;

	public Vector3 m_AddStartPos = Vector3.zero;

	public Vector3 m_AccumStartRot = Vector3.zero;

	public Vector3 m_RandomRange = Vector3.zero;

	protected int m_nCreateCount;

	protected float m_fStartTime;

	protected GameObject m_ClonObject;

	protected bool m_bInvoke;

	public override int GetAnimationState()
	{
		if (base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && (this.m_nDuplicateCount == 0 || (this.m_nDuplicateCount != 0 && this.m_nCreateCount < this.m_nDuplicateCount)))
		{
			return 1;
		}
		return 0;
	}

	public GameObject GetCloneObject()
	{
		return this.m_ClonObject;
	}

	private void Awake()
	{
		this.m_nCreateCount = 0;
		this.m_fStartTime = -this.m_fDuplicateTime;
		this.m_ClonObject = null;
		this.m_bInvoke = false;
		if (!base.enabled)
		{
			return;
		}
		if (base.transform.parent != null && base.enabled && NcEffectBehaviour.IsActive(base.gameObject) && base.GetComponent<NcDontActive>() == null)
		{
			this.InitCloneObject();
		}
	}

	protected override void OnDestroy()
	{
		if (this.m_ClonObject != null)
		{
			UnityEngine.Object.Destroy(this.m_ClonObject);
		}
		base.OnDestroy();
	}

	private void Start()
	{
		if (this.m_bInvoke)
		{
			this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
			this.CreateCloneObject();
			base.InvokeRepeating("CreateCloneObject", this.m_fDuplicateTime, this.m_fDuplicateTime);
		}
	}

	private void Update()
	{
		if (this.m_bInvoke)
		{
			return;
		}
		if ((this.m_nDuplicateCount == 0 || this.m_nCreateCount < this.m_nDuplicateCount) && this.m_fStartTime + this.m_fDuplicateTime <= NcEffectBehaviour.GetEngineTime())
		{
			this.m_fStartTime = NcEffectBehaviour.GetEngineTime();
			this.CreateCloneObject();
		}
	}

	private void InitCloneObject()
	{
		if (this.m_ClonObject == null)
		{
			this.m_ClonObject = base.CreateGameObject(base.gameObject);
			NcEffectBehaviour.HideNcDelayActive(this.m_ClonObject);
			NcDuplicator component = this.m_ClonObject.GetComponent<NcDuplicator>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			NcDelayActive component2 = this.m_ClonObject.GetComponent<NcDelayActive>();
			if (component2 != null)
			{
				UnityEngine.Object.Destroy(component2);
			}
			Component[] components = base.transform.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				if (!(components[i] is Transform) && !(components[i] is NcDuplicator))
				{
					UnityEngine.Object.Destroy(components[i]);
				}
			}
			NcEffectBehaviour.RemoveAllChildObject(base.gameObject, false);
			return;
		}
	}

	private void CreateCloneObject()
	{
		if (this.m_ClonObject == null)
		{
			return;
		}
		GameObject gameObject;
		if (base.transform.parent == null)
		{
			gameObject = base.CreateGameObject(base.gameObject);
		}
		else
		{
			gameObject = base.CreateGameObject(base.transform.parent.gameObject, this.m_ClonObject);
		}
		NcEffectBehaviour.SetActiveRecursively(gameObject, true);
		if (0f < this.m_fDuplicateLifeTime)
		{
			NcAutoDestruct ncAutoDestruct = gameObject.GetComponent<NcAutoDestruct>();
			if (ncAutoDestruct == null)
			{
				ncAutoDestruct = gameObject.AddComponent<NcAutoDestruct>();
			}
			ncAutoDestruct.m_fLifeTime = this.m_fDuplicateLifeTime;
		}
		Vector3 position = gameObject.transform.position;
		gameObject.transform.position = new Vector3(UnityEngine.Random.Range(-this.m_RandomRange.x, this.m_RandomRange.x) + position.x, UnityEngine.Random.Range(-this.m_RandomRange.y, this.m_RandomRange.y) + position.y, UnityEngine.Random.Range(-this.m_RandomRange.z, this.m_RandomRange.z) + position.z);
		gameObject.transform.position += this.m_AddStartPos;
		gameObject.transform.localRotation *= Quaternion.Euler(this.m_AccumStartRot.x * (float)this.m_nCreateCount, this.m_AccumStartRot.y * (float)this.m_nCreateCount, this.m_AccumStartRot.z * (float)this.m_nCreateCount);
		GameObject expr_18D = gameObject;
		expr_18D.name = expr_18D.name + " " + this.m_nCreateCount;
		this.m_nCreateCount++;
		if (this.m_bInvoke && this.m_nDuplicateCount <= this.m_nCreateCount)
		{
			base.CancelInvoke("CreateCloneObject");
		}
	}

	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		this.m_fDuplicateTime /= fSpeedRate;
		this.m_fDuplicateLifeTime /= fSpeedRate;
		if (bRuntime && this.m_ClonObject != null)
		{
			NsEffectManager.AdjustSpeedRuntime(this.m_ClonObject, fSpeedRate);
		}
	}
}
