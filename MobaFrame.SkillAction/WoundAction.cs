using Com.Game.Module;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class WoundAction : BaseDataAction
	{
		public Units targetUnit;

		public new List<short> dataKeys;

		public new List<float> dataValues;

		public bool isCrit;

		public int damageType;

		private WoundInfo woundInfo = new WoundInfo();

		private bool isDebugData;

		private float damage;

		protected override void OnSendStart()
		{
		}

		protected override void OnRecordStart()
		{
		}

		protected override bool doAction()
		{
			if (GlobalSettings.NoDamage)
			{
				return false;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				this.GetDataInfo(out num, out num2, out num3);
			}
			else
			{
				base.DoHuiGuangFanZhao(this.targetUnit, this.dataKeys, ref this.dataValues);
				this.targetUnit.ChangeAttr(this.GetDataInfo(out num, out num2, out num3));
			}
			if (Mathf.Abs(num) >= 1f)
			{
				if (num <= 0f || !(base.unit != null) || !base.unit.isPlayer || !(this.targetUnit != null) || !this.targetUnit.isLive || this.targetUnit.isPlayer)
				{
					this.targetUnit.jumpFontValue(AttrType.Hp, num, base.unit, this.isCrit, this.targetUnit.isPlayer, 0, this.damageType);
				}
			}
			if (Mathf.Abs(num2) >= 1f && this.targetUnit != base.unit)
			{
				this.targetUnit.jumpFontValue(AttrType.Mp, num2, base.unit, this.isCrit, this.targetUnit.isPlayer, 0, this.damageType);
			}
			if (Mathf.Abs(num3) > 1f)
			{
				if (num3 <= 0f || !(base.unit != null) || !base.unit.isPlayer || !(this.targetUnit != null) || !this.targetUnit.isLive || this.targetUnit.isPlayer)
				{
					this.targetUnit.jumpFontValue(AttrType.Shield, num3, base.unit, this.isCrit, this.targetUnit.isPlayer, 0, this.damageType);
				}
			}
			else if (this.targetUnit == base.unit && this.targetUnit.isPlayer && num2 > 1f)
			{
				this.targetUnit.jumpFontValue(AttrType.Mp, num2, base.unit, this.isCrit, this.targetUnit.isPlayer, 0, this.damageType);
			}
			if (num < 0f || this.targetUnit.hp <= 0f || float.IsNaN(this.targetUnit.hp))
			{
				this.targetUnit.Wound(base.unit, num);
				this.targetUnit.TryDeath(base.unit);
			}
			return true;
		}

		private List<DataInfo> GetDataInfo(out float damage, out float mpDamage, out float shieldDamage)
		{
			damage = 0f;
			mpDamage = 0f;
			shieldDamage = 0f;
			List<DataInfo> list = new List<DataInfo>();
			for (int i = 0; i < this.dataKeys.Count; i++)
			{
				DataInfo dataInfo = new DataInfo();
				dataInfo.key = (AttrType)this.dataKeys[i];
				dataInfo.value = this.dataValues[i];
				list.Add(dataInfo);
				if (dataInfo.key == AttrType.Hp)
				{
					damage = dataInfo.value;
				}
				if (dataInfo.key == AttrType.Mp)
				{
					mpDamage = dataInfo.value;
				}
				if (dataInfo.key == AttrType.Shield)
				{
					shieldDamage = dataInfo.value;
				}
			}
			return list;
		}
	}
}
