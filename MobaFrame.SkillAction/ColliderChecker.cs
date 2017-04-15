using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class ColliderChecker : MonoBehaviour
	{
		public enum DamageColliderType
		{
			CapsuleCollider,
			SphereCollider,
			BoxCollider,
			SectorCollider,
			SelfCollider
		}

		[SerializeField]
		public List<GameObject> hitColliders = new List<GameObject>();

		public EffectiveRangeType effectRangeType;

		public float effectRange1;

		public float effectRange2;

		public float effectRange3;

		public Callback<GameObject> OnColliderEnterCallback;

		public Callback<GameObject> OnColliderExitCallback;

		public bool isActiveCollider;

		public bool isEffectOver;

		private void OnDestroy()
		{
			this.OnColliderEnterCallback = null;
			this.OnColliderEnterCallback = null;
			this.hitColliders.Clear();
			this.isActiveCollider = false;
		}

		public void CreateCollider(EffectiveRangeType effectRangeType, float effectRange1, float effectRange2, float effectRange3)
		{
			this.effectRangeType = effectRangeType;
			this.effectRange1 = effectRange1;
			this.effectRange2 = effectRange2;
			this.effectRange3 = effectRange3;
			this.hitColliders.Clear();
			switch (effectRangeType)
			{
			case EffectiveRangeType.JuXing:
				this.AddCollider(ColliderChecker.DamageColliderType.BoxCollider, effectRange1, effectRange2, effectRange3);
				break;
			case EffectiveRangeType.YuanXing:
				this.AddCollider(ColliderChecker.DamageColliderType.SphereCollider, effectRange1, effectRange2, effectRange3);
				break;
			case EffectiveRangeType.ShanXing:
				this.AddCollider(ColliderChecker.DamageColliderType.SectorCollider, effectRange1, effectRange2, effectRange3);
				break;
			case EffectiveRangeType.Single:
				this.AddCollider(ColliderChecker.DamageColliderType.SphereCollider, effectRange1, effectRange2, effectRange1);
				break;
			case EffectiveRangeType.SelfCollider:
				this.AddCollider(ColliderChecker.DamageColliderType.SelfCollider, 0f, 0f, 0f);
				break;
			}
		}

		public void OnTriggerEnter(Collider collider)
		{
			if (!this.isActiveCollider)
			{
				return;
			}
			if (collider == null)
			{
				return;
			}
			if (!TagManager.IsAttackTarget(collider.gameObject))
			{
				return;
			}
			if (!this.hitColliders.Contains(collider.gameObject))
			{
				this.hitColliders.Add(collider.gameObject);
			}
			if (this.OnColliderEnterCallback != null)
			{
				this.OnColliderEnterCallback(collider.gameObject);
			}
		}

		public void OnTriggerExit(Collider collider)
		{
			if (collider == null)
			{
				return;
			}
			if (!this.isActiveCollider)
			{
				return;
			}
			if (this.hitColliders == null)
			{
				return;
			}
			if (this.hitColliders.Contains(collider.gameObject))
			{
				this.hitColliders.Remove(collider.gameObject);
			}
			if (this.OnColliderExitCallback != null)
			{
				this.OnColliderExitCallback(collider.gameObject);
			}
		}

		private void AddCollider(ColliderChecker.DamageColliderType collider_type, float param_1 = 0f, float param_2 = 0f, float param_3 = 0f)
		{
			switch (collider_type)
			{
			case ColliderChecker.DamageColliderType.CapsuleCollider:
			{
				CapsuleCollider capsuleCollider = base.gameObject.GetComponent<CapsuleCollider>();
				if (capsuleCollider == null)
				{
					capsuleCollider = base.gameObject.AddComponent<CapsuleCollider>();
				}
				capsuleCollider.radius = param_1;
				capsuleCollider.height = param_2;
				capsuleCollider.isTrigger = true;
				break;
			}
			case ColliderChecker.DamageColliderType.SphereCollider:
			{
				SphereCollider sphereCollider = base.gameObject.GetComponent<SphereCollider>();
				if (sphereCollider == null)
				{
					sphereCollider = base.gameObject.AddComponent<SphereCollider>();
				}
				sphereCollider.radius = param_1;
				sphereCollider.center = new Vector3(0f, 0f, param_2);
				sphereCollider.isTrigger = true;
				break;
			}
			case ColliderChecker.DamageColliderType.BoxCollider:
			{
				BoxCollider boxCollider = base.gameObject.GetComponent<BoxCollider>();
				if (boxCollider == null)
				{
					boxCollider = base.gameObject.AddComponent<BoxCollider>();
				}
				boxCollider.center = new Vector3(0f, param_3 / 2f, param_1 / 2f);
				boxCollider.size = new Vector3(param_2, param_3, param_1);
				boxCollider.isTrigger = true;
				break;
			}
			case ColliderChecker.DamageColliderType.SectorCollider:
			{
				GameObject original = null;
				if (param_2 == 20f)
				{
					original = ResourceManager.Load<GameObject>("Sector20", true, true, null, 0, false);
				}
				else if (param_2 == 30f)
				{
					original = ResourceManager.Load<GameObject>("Sector30", true, true, null, 0, false);
				}
				else if (param_2 == 45f)
				{
					original = ResourceManager.Load<GameObject>("Sector45", true, true, null, 0, false);
				}
				else if (param_2 == 60f)
				{
					original = ResourceManager.Load<GameObject>("Sector60", true, true, null, 0, false);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localScale = new Vector3(param_1, param_1, 5f);
				gameObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
				break;
			}
			}
		}
	}
}
