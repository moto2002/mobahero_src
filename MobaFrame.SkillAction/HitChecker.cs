using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class HitChecker : MonoBehaviour
	{
		public enum DamageColliderType
		{
			CapsuleCollider,
			SphereCollider,
			BoxCollider,
			SectorCollider
		}

		public int skillNodeId;

		public EffectiveRangeType mEffectRangeType;

		public float effectRange1;

		public float effectRange2;

		public float effectRange3;

		public Callback<Collider> OnColliderEnterCallback;

		public Callback<Collider> OnColliderExitCallback;

		[SerializeField]
		private bool isActive;

		private Collider mCollider;

		private void Awake()
		{
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		private void OnDestroy()
		{
			this.OnColliderEnterCallback = null;
			this.OnColliderExitCallback = null;
			this.isActive = false;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!this.isActive)
			{
				return;
			}
			if (this.OnColliderEnterCallback != null)
			{
				this.OnColliderEnterCallback(collider);
			}
		}

		private void OnTriggerExit(Collider collider)
		{
			if (!this.isActive)
			{
				return;
			}
			if (this.OnColliderExitCallback != null)
			{
				this.OnColliderExitCallback(collider);
			}
		}

		public void CreateCollider()
		{
			this.CreateCollider((int)this.mEffectRangeType, this.effectRange1, this.effectRange2, this.effectRange3);
		}

		public void CreateCollider(int type, float param1, float param2, float param3)
		{
			if (this.mCollider != null && type != (int)this.mEffectRangeType)
			{
				UnityEngine.Object.Destroy(this.mCollider);
				this.mCollider = null;
			}
			switch (type)
			{
			case 1:
				this.mCollider = this.CreateCollider(HitChecker.DamageColliderType.BoxCollider, param1, param2, param3);
				break;
			case 2:
				this.mCollider = this.CreateCollider(HitChecker.DamageColliderType.SphereCollider, param1, param2, param3);
				break;
			case 3:
				this.mCollider = this.CreateCollider(HitChecker.DamageColliderType.SectorCollider, param1, param2, param3);
				break;
			case 4:
				this.mCollider = this.CreateCollider(HitChecker.DamageColliderType.SphereCollider, param1, param2, param3);
				break;
			case 5:
				break;
			default:
				this.EnableCollider(false);
				break;
			}
		}

		public void EnableCollider(bool active)
		{
			this.isActive = base.enabled;
			base.enabled = active;
		}

		private Collider CreateCollider(HitChecker.DamageColliderType collider_type, float param_1 = 0f, float param_2 = 0f, float param_3 = 0f)
		{
			switch (collider_type)
			{
			case HitChecker.DamageColliderType.CapsuleCollider:
			{
				CapsuleCollider capsuleCollider = base.gameObject.GetComponent<CapsuleCollider>();
				if (capsuleCollider == null)
				{
					capsuleCollider = base.gameObject.AddComponent<CapsuleCollider>();
				}
				capsuleCollider.radius = param_1;
				capsuleCollider.height = param_2;
				capsuleCollider.isTrigger = true;
				return capsuleCollider;
			}
			case HitChecker.DamageColliderType.SphereCollider:
			{
				SphereCollider sphereCollider = base.gameObject.GetComponent<SphereCollider>();
				if (sphereCollider == null)
				{
					sphereCollider = base.gameObject.AddComponent<SphereCollider>();
				}
				sphereCollider.radius = param_1;
				sphereCollider.center = new Vector3(0f, 0f, param_2);
				sphereCollider.isTrigger = true;
				return sphereCollider;
			}
			case HitChecker.DamageColliderType.BoxCollider:
			{
				BoxCollider boxCollider = base.gameObject.GetComponent<BoxCollider>();
				if (boxCollider == null)
				{
					boxCollider = base.gameObject.AddComponent<BoxCollider>();
				}
				boxCollider.center = new Vector3(0f, param_3 / 2f, param_1 / 2f);
				boxCollider.size = new Vector3(param_2, param_3, param_1);
				boxCollider.isTrigger = true;
				return boxCollider;
			}
			case HitChecker.DamageColliderType.SectorCollider:
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
				return gameObject.GetComponent<Collider>();
			}
			default:
				return null;
			}
		}
	}
}
