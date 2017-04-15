using MobaTools.Prefab;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DartsAction : BulletAction
	{
		public float DartsFlyForwardTime = 2f;

		public BoneAnchorType boneAnchorType = BoneAnchorType.RightHand;

		private float sinValue;

		private float engalValue;

		private Vector3 prePosition;

		private SphereCollider collider;

		[DebuggerHidden]
		protected override IEnumerator Bullet_Coroutine()
		{
			DartsAction.<Bullet_Coroutine>c__Iterator58 <Bullet_Coroutine>c__Iterator = new DartsAction.<Bullet_Coroutine>c__Iterator58();
			<Bullet_Coroutine>c__Iterator.<>f__this = this;
			return <Bullet_Coroutine>c__Iterator;
		}

		protected bool CheckSelfExist()
		{
			return !(base.unit == null) && base.unit.isLive;
		}

		protected bool BackMoveDelta(float moveDelta)
		{
			this.prePosition = base.transform.position;
			base.transform.rotation = Quaternion.LookRotation(base.unit.GetBonePos(this.boneAnchorType) - base.transform.position);
			base.transform.position += base.transform.forward * moveDelta;
			return Vector3.Distance(base.transform.position, base.unit.GetBonePos(this.boneAnchorType)) < 0.1f || Vector3.Dot((base.unit.GetBonePos(this.boneAnchorType) - this.prePosition).normalized, (base.unit.GetBonePos(this.boneAnchorType) - base.transform.position).normalized) < 0f;
		}

		protected override void OnDestroy()
		{
			this.TryStopAffectWeapon();
			base.OnDestroy();
		}

		protected override void TryShowWeapon()
		{
			if (!base.IsAffectWeapon())
			{
				return;
			}
			this.DoShowWeapon();
		}

		private void DoShowWeapon()
		{
			if (this.data.weaponPosType == 1)
			{
				if (this._weaponFirTrans == null)
				{
					base.GetWeaponPosFromBoneWeapon(out this._weaponFirTrans);
				}
				if (this._weaponFirTrans != null)
				{
					this._weaponFirTrans.gameObject.SetActive(true);
				}
			}
		}

		protected override void TryHideWeapon()
		{
			if (!base.IsAffectWeapon())
			{
				return;
			}
			this.DoHideWeapon();
		}

		private void DoHideWeapon()
		{
			if (this.data.weaponPosType == 1)
			{
				if (this._weaponFirTrans == null)
				{
					base.GetWeaponPosFromBoneWeapon(out this._weaponFirTrans);
				}
				if (this._weaponFirTrans != null)
				{
					this._weaponFirTrans.gameObject.SetActive(false);
				}
			}
		}

		private void TryStopAffectWeapon()
		{
			if (!base.IsAffectWeapon())
			{
				return;
			}
			this.DoShowWeapon();
			this._weaponFirTrans = null;
			this._weaponSecTrans = null;
		}
	}
}
