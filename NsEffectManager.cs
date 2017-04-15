using System;
using System.Collections.Generic;
using UnityEngine;

public class NsEffectManager : MonoBehaviour
{
	public static Texture[] PreloadResource(GameObject tarObj)
	{
		if (tarObj == null)
		{
			return new Texture[0];
		}
		return NsEffectManager.PreloadResource(tarObj, new List<GameObject>
		{
			tarObj
		});
	}

	public static Component GetComponentInChildren(GameObject tarObj, Type findType)
	{
		if (tarObj == null)
		{
			return null;
		}
		return NsEffectManager.GetComponentInChildren(tarObj, findType, new List<GameObject>
		{
			tarObj
		});
	}

	public static GameObject CreateReplayEffect(GameObject tarPrefab)
	{
		if (tarPrefab == null)
		{
			return null;
		}
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(tarPrefab);
		NsEffectManager.SetReplayEffect(gameObject);
		return gameObject;
	}

	public static void SetReplayEffect(GameObject instanceObj)
	{
		NsEffectManager.PreloadResource(instanceObj);
		NsEffectManager.SetActiveRecursively(instanceObj, false);
		NcEffectBehaviour[] componentsInChildren = instanceObj.GetComponentsInChildren<NcEffectBehaviour>(true);
		NcEffectBehaviour[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			NcEffectBehaviour ncEffectBehaviour = array[i];
			ncEffectBehaviour.OnSetReplayState();
		}
	}

	public static void RunReplayEffect(GameObject instanceObj, bool bClearOldParticle)
	{
		NsEffectManager.SetActiveRecursively(instanceObj, true);
		NcEffectBehaviour[] componentsInChildren = instanceObj.GetComponentsInChildren<NcEffectBehaviour>(true);
		NcEffectBehaviour[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			NcEffectBehaviour ncEffectBehaviour = array[i];
			ncEffectBehaviour.OnResetReplayStage(bClearOldParticle);
		}
	}

	public static void AdjustSpeedRuntime(GameObject target, float fSpeedRate)
	{
		NcEffectBehaviour[] componentsInChildren = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		NcEffectBehaviour[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			NcEffectBehaviour ncEffectBehaviour = array[i];
			ncEffectBehaviour.OnUpdateEffectSpeed(fSpeedRate, true);
		}
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
		int num = target.transform.childCount - 1;
		while (0 <= num)
		{
			if (num < target.transform.childCount)
			{
				NsEffectManager.SetActiveRecursively(target.transform.GetChild(num).gameObject, bActive);
			}
			num--;
		}
		target.SetActive(bActive);
	}

	public static bool IsActive(GameObject target)
	{
		return target.activeInHierarchy && target.activeSelf;
	}

	protected static void SetActiveRecursivelyEffect(GameObject target, bool bActive)
	{
		NcEffectBehaviour[] componentsInChildren = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		NcEffectBehaviour[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			NcEffectBehaviour ncEffectBehaviour = array[i];
			ncEffectBehaviour.OnSetActiveRecursively(bActive);
		}
	}

	protected static Texture[] PreloadResource(GameObject tarObj, List<GameObject> parentPrefabList)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		Renderer[] componentsInChildren = tarObj.GetComponentsInChildren<Renderer>(true);
		List<Texture> list = new List<Texture>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length > 0)
			{
				Material[] sharedMaterials = renderer.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					Material material = sharedMaterials[j];
					if (material != null && material.mainTexture != null)
					{
						list.Add(material.mainTexture);
					}
				}
			}
		}
		NcAttachPrefab[] componentsInChildren2 = tarObj.GetComponentsInChildren<NcAttachPrefab>(true);
		NcAttachPrefab[] array2 = componentsInChildren2;
		for (int k = 0; k < array2.Length; k++)
		{
			NcAttachPrefab ncAttachPrefab = array2[k];
			if (ncAttachPrefab.m_AttachPrefab != null)
			{
				Texture[] array3 = NsEffectManager.PreloadPrefab(ncAttachPrefab.m_AttachPrefab, parentPrefabList, true);
				if (array3 == null)
				{
					ncAttachPrefab.m_AttachPrefab = null;
				}
				else
				{
					list.AddRange(array3);
				}
			}
		}
		NcParticleSystem[] componentsInChildren3 = tarObj.GetComponentsInChildren<NcParticleSystem>(true);
		NcParticleSystem[] array4 = componentsInChildren3;
		for (int l = 0; l < array4.Length; l++)
		{
			NcParticleSystem ncParticleSystem = array4[l];
			if (ncParticleSystem.m_AttachPrefab != null)
			{
				Texture[] array5 = NsEffectManager.PreloadPrefab(ncParticleSystem.m_AttachPrefab, parentPrefabList, true);
				if (array5 == null)
				{
					ncParticleSystem.m_AttachPrefab = null;
				}
				else
				{
					list.AddRange(array5);
				}
			}
		}
		NcSpriteTexture[] componentsInChildren4 = tarObj.GetComponentsInChildren<NcSpriteTexture>(true);
		NcSpriteTexture[] array6 = componentsInChildren4;
		for (int m = 0; m < array6.Length; m++)
		{
			NcSpriteTexture ncSpriteTexture = array6[m];
			if (ncSpriteTexture.m_NcSpriteFactoryPrefab != null)
			{
				Texture[] array7 = NsEffectManager.PreloadPrefab(ncSpriteTexture.m_NcSpriteFactoryPrefab, parentPrefabList, false);
				if (array7 != null)
				{
					list.AddRange(array7);
				}
			}
		}
		NcParticleSpiral[] componentsInChildren5 = tarObj.GetComponentsInChildren<NcParticleSpiral>(true);
		NcParticleSpiral[] array8 = componentsInChildren5;
		for (int n = 0; n < array8.Length; n++)
		{
			NcParticleSpiral ncParticleSpiral = array8[n];
			if (ncParticleSpiral.m_ParticlePrefab != null)
			{
				Texture[] array9 = NsEffectManager.PreloadPrefab(ncParticleSpiral.m_ParticlePrefab, parentPrefabList, false);
				if (array9 != null)
				{
					list.AddRange(array9);
				}
			}
		}
		NcParticleEmit[] componentsInChildren6 = tarObj.GetComponentsInChildren<NcParticleEmit>(true);
		NcParticleEmit[] array10 = componentsInChildren6;
		for (int num = 0; num < array10.Length; num++)
		{
			NcParticleEmit ncParticleEmit = array10[num];
			if (ncParticleEmit.m_ParticlePrefab != null)
			{
				Texture[] array11 = NsEffectManager.PreloadPrefab(ncParticleEmit.m_ParticlePrefab, parentPrefabList, false);
				if (array11 != null)
				{
					list.AddRange(array11);
				}
			}
		}
		NcAttachSound[] componentsInChildren7 = tarObj.GetComponentsInChildren<NcAttachSound>(true);
		NcAttachSound[] array12 = componentsInChildren7;
		for (int num2 = 0; num2 < array12.Length; num2++)
		{
			NcAttachSound ncAttachSound = array12[num2];
			if (ncAttachSound.m_AudioClip != null)
			{
			}
		}
		NcSpriteFactory[] componentsInChildren8 = tarObj.GetComponentsInChildren<NcSpriteFactory>(true);
		NcSpriteFactory[] array13 = componentsInChildren8;
		for (int num3 = 0; num3 < array13.Length; num3++)
		{
			NcSpriteFactory ncSpriteFactory = array13[num3];
			if (ncSpriteFactory.m_SpriteList != null)
			{
				for (int num4 = 0; num4 < ncSpriteFactory.m_SpriteList.Count; num4++)
				{
					if (ncSpriteFactory.m_SpriteList[num4].m_EffectPrefab != null)
					{
						Texture[] array14 = NsEffectManager.PreloadPrefab(ncSpriteFactory.m_SpriteList[num4].m_EffectPrefab, parentPrefabList, true);
						if (array14 == null)
						{
							ncSpriteFactory.m_SpriteList[num4].m_EffectPrefab = null;
						}
						else
						{
							list.AddRange(array14);
						}
						if (ncSpriteFactory.m_SpriteList[num4].m_AudioClip != null)
						{
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	protected static Texture[] PreloadPrefab(GameObject tarObj, List<GameObject> parentPrefabList, bool bCheckDup)
	{
		if (!parentPrefabList.Contains(tarObj))
		{
			parentPrefabList.Add(tarObj);
			Texture[] result = NsEffectManager.PreloadResource(tarObj, parentPrefabList);
			parentPrefabList.Remove(tarObj);
			return result;
		}
		if (bCheckDup)
		{
			string text = string.Empty;
			for (int i = 0; i < parentPrefabList.Count; i++)
			{
				text = text + parentPrefabList[i].name + "/";
			}
			Debug.LogWarning("LoadError : Recursive Prefab - " + text + tarObj.name);
			return null;
		}
		return null;
	}

	protected static Component GetComponentInChildren(GameObject tarObj, Type findType, List<GameObject> parentPrefabList)
	{
		Component[] componentsInChildren = tarObj.GetComponentsInChildren(findType, true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Component component = array[i];
			if (component.GetComponent<NcDontActive>() == null)
			{
				return component;
			}
		}
		NcAttachPrefab[] componentsInChildren2 = tarObj.GetComponentsInChildren<NcAttachPrefab>(true);
		NcAttachPrefab[] array2 = componentsInChildren2;
		for (int j = 0; j < array2.Length; j++)
		{
			NcAttachPrefab ncAttachPrefab = array2[j];
			if (ncAttachPrefab.m_AttachPrefab != null)
			{
				Component validComponentInChildren = NsEffectManager.GetValidComponentInChildren(ncAttachPrefab.m_AttachPrefab, findType, parentPrefabList, true);
				if (validComponentInChildren != null)
				{
					return validComponentInChildren;
				}
			}
		}
		NcParticleSystem[] componentsInChildren3 = tarObj.GetComponentsInChildren<NcParticleSystem>(true);
		NcParticleSystem[] array3 = componentsInChildren3;
		for (int k = 0; k < array3.Length; k++)
		{
			NcParticleSystem ncParticleSystem = array3[k];
			if (ncParticleSystem.m_AttachPrefab != null)
			{
				Component validComponentInChildren = NsEffectManager.GetValidComponentInChildren(ncParticleSystem.m_AttachPrefab, findType, parentPrefabList, true);
				if (validComponentInChildren != null)
				{
					return validComponentInChildren;
				}
			}
		}
		NcSpriteTexture[] componentsInChildren4 = tarObj.GetComponentsInChildren<NcSpriteTexture>(true);
		NcSpriteTexture[] array4 = componentsInChildren4;
		for (int l = 0; l < array4.Length; l++)
		{
			NcSpriteTexture ncSpriteTexture = array4[l];
			if (ncSpriteTexture.m_NcSpriteFactoryPrefab != null)
			{
				Component validComponentInChildren = NsEffectManager.GetValidComponentInChildren(ncSpriteTexture.m_NcSpriteFactoryPrefab, findType, parentPrefabList, false);
				if (validComponentInChildren != null)
				{
					return validComponentInChildren;
				}
			}
		}
		NcParticleSpiral[] componentsInChildren5 = tarObj.GetComponentsInChildren<NcParticleSpiral>(true);
		NcParticleSpiral[] array5 = componentsInChildren5;
		for (int m = 0; m < array5.Length; m++)
		{
			NcParticleSpiral ncParticleSpiral = array5[m];
			if (ncParticleSpiral.m_ParticlePrefab != null)
			{
				Component validComponentInChildren = NsEffectManager.GetValidComponentInChildren(ncParticleSpiral.m_ParticlePrefab, findType, parentPrefabList, false);
				if (validComponentInChildren != null)
				{
					return validComponentInChildren;
				}
			}
		}
		NcParticleEmit[] componentsInChildren6 = tarObj.GetComponentsInChildren<NcParticleEmit>(true);
		NcParticleEmit[] array6 = componentsInChildren6;
		for (int n = 0; n < array6.Length; n++)
		{
			NcParticleEmit ncParticleEmit = array6[n];
			if (ncParticleEmit.m_ParticlePrefab != null)
			{
				Component validComponentInChildren = NsEffectManager.GetValidComponentInChildren(ncParticleEmit.m_ParticlePrefab, findType, parentPrefabList, false);
				if (validComponentInChildren != null)
				{
					return validComponentInChildren;
				}
			}
		}
		NcSpriteFactory[] componentsInChildren7 = tarObj.GetComponentsInChildren<NcSpriteFactory>(true);
		NcSpriteFactory[] array7 = componentsInChildren7;
		for (int num = 0; num < array7.Length; num++)
		{
			NcSpriteFactory ncSpriteFactory = array7[num];
			if (ncSpriteFactory.m_SpriteList != null)
			{
				for (int num2 = 0; num2 < ncSpriteFactory.m_SpriteList.Count; num2++)
				{
					if (ncSpriteFactory.m_SpriteList[num2].m_EffectPrefab != null)
					{
						Component validComponentInChildren = NsEffectManager.GetValidComponentInChildren(ncSpriteFactory.m_SpriteList[num2].m_EffectPrefab, findType, parentPrefabList, true);
						if (validComponentInChildren != null)
						{
							return validComponentInChildren;
						}
					}
				}
			}
		}
		return null;
	}

	protected static Component GetValidComponentInChildren(GameObject tarObj, Type findType, List<GameObject> parentPrefabList, bool bCheckDup)
	{
		if (!parentPrefabList.Contains(tarObj))
		{
			parentPrefabList.Add(tarObj);
			Component componentInChildren = NsEffectManager.GetComponentInChildren(tarObj, findType, parentPrefabList);
			parentPrefabList.Remove(tarObj);
			return componentInChildren;
		}
		if (bCheckDup)
		{
			string text = string.Empty;
			for (int i = 0; i < parentPrefabList.Count; i++)
			{
				text = text + parentPrefabList[i].name + "/";
			}
			Debug.LogWarning("LoadError : Recursive Prefab - " + text + tarObj.name);
			return null;
		}
		return null;
	}
}
