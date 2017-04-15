using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EffectManagerUI : MonoBehaviour
{
	private GameObject self;

	private string heroId;

	public Dictionary<int, List<PerformVo>> character_effects = new Dictionary<int, List<PerformVo>>();

	protected Dictionary<int, Transform> bones = new Dictionary<int, Transform>();

	private CharacterController m_controller;

	public void Init(GameObject self, string heroId)
	{
		this.self = self;
		this.heroId = heroId;
		this.InitEffects();
		this.ReInitBoneAnchor();
		this.ShowBirthEffect();
	}

	private void InitEffects()
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.heroId);
		if (heroMainData == null)
		{
			return;
		}
		string effect_id = heroMainData.effect_id;
		if (effect_id == null)
		{
			return;
		}
		if (StringUtils.CheckValid(effect_id))
		{
			this.character_effects.Clear();
			string[] stringValue = StringUtils.GetStringValue(effect_id, ',');
			for (int i = 0; i < stringValue.Length; i++)
			{
				if (StringUtils.CheckValid(stringValue[i]))
				{
					string[] stringValue2 = StringUtils.GetStringValue(stringValue[i], '|');
					if (stringValue2 != null)
					{
						int key = int.Parse(stringValue2[0]);
						string text = stringValue2[1];
						if (StringUtils.CheckValid(text))
						{
							if (!this.character_effects.ContainsKey(key))
							{
								this.character_effects.Add(key, new List<PerformVo>());
							}
							this.character_effects[key].Add(PerformVo.Create(text, null));
						}
					}
				}
			}
		}
	}

	private void ShowBirthEffect()
	{
		int num = 5;
		if (this.character_effects != null && this.character_effects.ContainsKey(num))
		{
			this.ShowCharacterEffect(num, this.character_effects[num]);
		}
	}

	public void ShowCharacterEffect(int effect_trigger_type, List<PerformVo> perform_ids)
	{
		if (effect_trigger_type == 5)
		{
			if (perform_ids != null)
			{
				for (int i = 0; i < perform_ids.Count; i++)
				{
					this.StartEffect(perform_ids[i]);
				}
			}
		}
	}

	private void StartEffect(PerformVo perform)
	{
		if (perform == null)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		Transform parent;
		this.GetBone(perform.data.config.effect_anchor, out parent, out zero);
		GameObject gameObject = ResourceManager.Load<GameObject>(perform.data.config.effect_id, true, true, null, 0, false);
		Transform transform = UnityEngine.Object.Instantiate(gameObject.transform) as Transform;
		transform.parent = parent;
		transform.gameObject.AddComponent<RenderQueue>();
		if (perform.data.effect_pos_offset == null)
		{
			transform.localPosition = zero;
		}
		else
		{
			transform.localPosition = new Vector3(perform.data.effect_pos_offset[0], perform.data.effect_pos_offset[1], perform.data.effect_pos_offset[2]) + zero;
		}
		if (perform.data.effect_rotation_offset == null)
		{
			transform.localRotation = Quaternion.identity;
		}
		else
		{
			transform.localRotation = Quaternion.Euler(new Vector3(perform.data.effect_rotation_offset[0], perform.data.effect_rotation_offset[1], perform.data.effect_rotation_offset[2]));
		}
		base.StopCoroutine(this.ShowEffect(transform));
		base.StartCoroutine(this.ShowEffect(transform));
	}

	[DebuggerHidden]
	private IEnumerator ShowEffect(Transform effect)
	{
		EffectManagerUI.<ShowEffect>c__Iterator3C <ShowEffect>c__Iterator3C = new EffectManagerUI.<ShowEffect>c__Iterator3C();
		<ShowEffect>c__Iterator3C.effect = effect;
		<ShowEffect>c__Iterator3C.<$>effect = effect;
		return <ShowEffect>c__Iterator3C;
	}

	private void ReInitBoneAnchor()
	{
		Transform[] componentsInChildren = this.self.gameObject.GetComponentsInChildren<Transform>();
		this.bones.Clear();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name == "Bip01 Footsteps")
			{
				this.bones.Add(3, componentsInChildren[i]);
			}
			else if (componentsInChildren[i].name == "Bip01 Spine")
			{
				this.bones.Add(4, componentsInChildren[i]);
			}
			else if (componentsInChildren[i].name == "Bip01 Head")
			{
				this.bones.Add(5, componentsInChildren[i]);
			}
			else if (componentsInChildren[i].name == "Bip01 L Hand")
			{
				this.bones.Add(6, componentsInChildren[i]);
			}
			else if (componentsInChildren[i].name == "Bip01 R Hand")
			{
				this.bones.Add(7, componentsInChildren[i]);
			}
			else if (componentsInChildren[i].name == "weapon")
			{
				this.bones.Add(8, componentsInChildren[i]);
			}
		}
	}

	public void GetBone(int bone_type, out Transform bone, out Vector3 offset)
	{
		bone = base.transform;
		offset = new Vector3(0f, 0f, 0f);
		switch (bone_type)
		{
		case 0:
			bone = base.transform;
			offset = new Vector3(0f, 0f, 0f);
			break;
		case 1:
			bone = base.transform;
			offset = new Vector3(0f, this.GetHeight() / 2f, 0f);
			break;
		case 2:
			bone = base.transform;
			offset = new Vector3(0f, this.GetHeight(), 0f);
			break;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
			if (this.bones != null && this.bones.ContainsKey(bone_type))
			{
				bone = this.bones[bone_type];
				offset = new Vector3(0f, 0f, 0f);
			}
			break;
		}
	}

	public float GetHeight()
	{
		float result = 0f;
		if (this.m_controller == null)
		{
			this.m_controller = base.GetComponent<CharacterController>();
		}
		if (this.m_controller)
		{
			result = this.m_controller.height;
		}
		else if (base.collider)
		{
			string name = base.collider.GetType().Name;
			switch (name)
			{
			case "BoxCollider":
			{
				BoxCollider boxCollider = base.collider as BoxCollider;
				result = boxCollider.size.y;
				break;
			}
			case "SphereCollider":
			{
				SphereCollider sphereCollider = base.collider as SphereCollider;
				result = sphereCollider.radius * 2f;
				break;
			}
			case "CapsuleCollider":
			{
				CapsuleCollider capsuleCollider = base.collider as CapsuleCollider;
				result = capsuleCollider.height;
				break;
			}
			}
		}
		return result;
	}
}
