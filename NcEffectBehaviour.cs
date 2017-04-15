using System;
using System.Collections.Generic;
using UnityEngine;

public class NcEffectBehaviour : MonoBehaviour
{
	public class _RuntimeIntance
	{
		public GameObject m_ParentGameObject;

		public GameObject m_ChildGameObject;

		public _RuntimeIntance(GameObject parentGameObject, GameObject childGameObject)
		{
			this.m_ParentGameObject = parentGameObject;
			this.m_ChildGameObject = childGameObject;
		}
	}

	private static bool m_bShuttingDown;

	private static GameObject m_RootInstance;

	public float m_fUserTag;

	protected MeshFilter m_MeshFilter;

	protected List<Material> m_RuntimeMaterials;

	protected bool m_bReplayState;

	protected NcEffectInitBackup m_NcEffectInitBackup;

	public NcEffectBehaviour()
	{
		this.m_MeshFilter = null;
	}

	public static float GetEngineTime()
	{
		if (Time.time == 0f)
		{
			return 1E-06f;
		}
		return Time.time;
	}

	public static float GetEngineDeltaTime()
	{
		return Time.deltaTime;
	}

	public virtual int GetAnimationState()
	{
		return -1;
	}

	public static GameObject GetRootInstanceEffect()
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		if (NcEffectBehaviour.m_RootInstance == null)
		{
			NcEffectBehaviour.m_RootInstance = GameObject.Find("_InstanceObject");
			if (NcEffectBehaviour.m_RootInstance == null)
			{
				NcEffectBehaviour.m_RootInstance = new GameObject("_InstanceObject");
			}
		}
		return NcEffectBehaviour.m_RootInstance;
	}

	public static Texture[] PreloadTexture(GameObject tarObj)
	{
		return NsEffectManager.PreloadResource(tarObj);
	}

	protected static void SetActive(GameObject target, bool bActive)
	{
		target.SetActive(bActive);
	}

	protected static void SetActiveRecursively(GameObject target, bool bActive)
	{
		int num = target.transform.childCount - 1;
		while (0 <= num)
		{
			if (num < target.transform.childCount)
			{
				NcEffectBehaviour.SetActiveRecursively(target.transform.GetChild(num).gameObject, bActive);
			}
			num--;
		}
		target.SetActive(bActive);
	}

	protected static bool IsActive(GameObject target)
	{
		return target.activeInHierarchy && target.activeSelf;
	}

	protected static void RemoveAllChildObject(GameObject parent, bool bImmediate)
	{
		int num = parent.transform.childCount - 1;
		while (0 <= num)
		{
			if (num < parent.transform.childCount)
			{
				Transform child = parent.transform.GetChild(num);
				if (bImmediate)
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
				else
				{
					UnityEngine.Object.Destroy(child.gameObject);
				}
			}
			num--;
		}
	}

	public static void HideNcDelayActive(GameObject tarObj)
	{
		NcEffectBehaviour.SetActiveRecursively(tarObj, false);
	}

	protected void AddRuntimeMaterial(Material addMaterial)
	{
		if (this.m_RuntimeMaterials == null)
		{
			this.m_RuntimeMaterials = new List<Material>();
		}
		if (!this.m_RuntimeMaterials.Contains(addMaterial))
		{
			this.m_RuntimeMaterials.Add(addMaterial);
		}
	}

	public static string GetMaterialColorName(Material mat)
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

	protected void DisableEmit()
	{
		NcParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<NcParticleSystem>(true);
		NcParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			NcParticleSystem ncParticleSystem = array[i];
			if (ncParticleSystem != null)
			{
				ncParticleSystem.SetDisableEmit();
			}
		}
		NcAttachPrefab[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<NcAttachPrefab>(true);
		NcAttachPrefab[] array2 = componentsInChildren2;
		for (int j = 0; j < array2.Length; j++)
		{
			NcAttachPrefab ncAttachPrefab = array2[j];
			if (ncAttachPrefab != null)
			{
				ncAttachPrefab.enabled = false;
			}
		}
		ParticleSystem[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array3 = componentsInChildren3;
		for (int k = 0; k < array3.Length; k++)
		{
			ParticleSystem particleSystem = array3[k];
			if (particleSystem != null)
			{
				particleSystem.enableEmission = false;
			}
		}
		ParticleEmitter[] componentsInChildren4 = base.gameObject.GetComponentsInChildren<ParticleEmitter>(true);
		ParticleEmitter[] array4 = componentsInChildren4;
		for (int l = 0; l < array4.Length; l++)
		{
			ParticleEmitter particleEmitter = array4[l];
			if (particleEmitter != null)
			{
				particleEmitter.emit = false;
			}
		}
	}

	public static bool IsSafe()
	{
		return !NcEffectBehaviour.m_bShuttingDown && !Application.isLoadingLevel;
	}

	protected GameObject CreateEditorGameObject(GameObject srcGameObj)
	{
		if (srcGameObj.name.Contains("flare 24"))
		{
			return srcGameObj;
		}
		return srcGameObj;
	}

	public GameObject CreateGameObject(string name)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		return this.CreateEditorGameObject(new GameObject(name));
	}

	public GameObject CreateGameObject(GameObject original)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		return this.CreateEditorGameObject((GameObject)UnityEngine.Object.Instantiate(original));
	}

	public GameObject CreateGameObject(GameObject prefabObj, Vector3 position, Quaternion rotation)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		return this.CreateEditorGameObject((GameObject)UnityEngine.Object.Instantiate(prefabObj, position, rotation));
	}

	public GameObject CreateGameObject(GameObject parentObj, GameObject prefabObj)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		GameObject gameObject = this.CreateGameObject(prefabObj);
		if (parentObj != null)
		{
			this.ChangeParent(parentObj.transform, gameObject.transform, true, null);
		}
		return gameObject;
	}

	public GameObject CreateGameObject(GameObject parentObj, Transform parentTrans, GameObject prefabObj)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		GameObject gameObject = this.CreateGameObject(prefabObj);
		if (parentObj != null)
		{
			this.ChangeParent(parentObj.transform, gameObject.transform, true, parentTrans);
		}
		return gameObject;
	}

	protected TT AddNcComponentToObject<TT>(GameObject toObj) where TT : NcEffectBehaviour
	{
		NcEffectBehaviour ncEffectBehaviour = toObj.AddComponent<TT>();
		if (this.m_bReplayState)
		{
			ncEffectBehaviour.OnSetReplayState();
		}
		return (TT)((object)ncEffectBehaviour);
	}

	protected void ChangeParent(Transform newParent, Transform child, bool bKeepingLocalTransform, Transform addTransform)
	{
		NcTransformTool ncTransformTool = null;
		if (bKeepingLocalTransform)
		{
			ncTransformTool = new NcTransformTool(child.transform);
			if (addTransform != null)
			{
				ncTransformTool.AddTransform(addTransform);
			}
		}
		child.parent = newParent;
		if (bKeepingLocalTransform)
		{
			ncTransformTool.CopyToLocalTransform(child.transform);
		}
		if (bKeepingLocalTransform)
		{
			NcBillboard[] componentsInChildren = child.GetComponentsInChildren<NcBillboard>();
			NcBillboard[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				NcBillboard ncBillboard = array[i];
				ncBillboard.UpdateBillboard();
			}
		}
	}

	protected void UpdateMeshColors(Color color)
	{
		if (this.m_MeshFilter == null)
		{
			this.m_MeshFilter = (MeshFilter)base.gameObject.GetComponent(typeof(MeshFilter));
		}
		if (this.m_MeshFilter == null || this.m_MeshFilter.sharedMesh == null || this.m_MeshFilter.mesh == null)
		{
			return;
		}
		Color[] array = new Color[this.m_MeshFilter.mesh.vertexCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		this.m_MeshFilter.mesh.colors = array;
	}

	protected virtual void OnDestroy()
	{
		if (this.m_RuntimeMaterials != null)
		{
			foreach (Material current in this.m_RuntimeMaterials)
			{
				UnityEngine.Object.Destroy(current);
			}
			this.m_RuntimeMaterials = null;
		}
	}

	public void OnApplicationQuit()
	{
		NcEffectBehaviour.m_bShuttingDown = true;
	}

	public virtual void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public virtual void OnSetActiveRecursively(bool bActive)
	{
	}

	public virtual void OnUpdateToolData()
	{
	}

	public virtual void OnSetReplayState()
	{
		this.m_bReplayState = true;
	}

	public virtual void OnResetReplayStage(bool bClearOldParticle)
	{
	}
}
