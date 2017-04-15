using MobaHeros;
using System;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class PVE_DataRevertAction : BaseDataAction
	{
		private float hpUpdateInterval = 1f;

		private float mpUpdateInterval = 1f;

		private float hpUpdateTime;

		private float mpUpdateTime;

		private float dataUpdateTime;

		private bool isDataUpdate;

		private float preHpValue;

		private float preMpValue;

		protected override void OnDestroy()
		{
			this.preHpValue = 0f;
			this.preMpValue = 0f;
			base.OnDestroy();
		}

		protected override void OnRecordStart()
		{
			if (!this.isDataUpdate)
			{
				return;
			}
			base.OnRecordStart();
		}

		protected override void OnSendStart()
		{
		}

		protected override bool doAction()
		{
			if (base.unit == null)
			{
				return false;
			}
			this.isDataUpdate = false;
			this.preHpValue = base.unit.hp;
			this.preMpValue = base.unit.mp;
			float deltaTime = Time.deltaTime;
			this.hpUpdateTime += deltaTime;
			if (this.hpUpdateTime >= this.hpUpdateInterval && base.unit.hp < base.unit.hp_max)
			{
				this.hpUpdateTime = 0f;
				float value = base.unit.hp_restore + base.unit.GetAttr(AttrType.Power) * base.unit.GetData<float>(DataType.HpRestoreParam);
				base.unit.ChangeAttr(AttrType.Hp, OpType.Add, value);
				if (this.preHpValue == base.unit.hp)
				{
					this.isDataUpdate = false;
				}
				else
				{
					this.isDataUpdate = true;
				}
			}
			if (base.unit.mp < base.unit.mp_max)
			{
				this.mpUpdateTime += deltaTime;
			}
			if (this.mpUpdateTime >= this.mpUpdateInterval && base.unit.mp < base.unit.mp_max)
			{
				this.mpUpdateTime = 0f;
				float value2 = base.unit.mp_restore + base.unit.GetAttr(AttrType.Intelligence) * base.unit.GetData<float>(DataType.MpRestoreParam);
				base.unit.ChangeAttr(AttrType.Mp, OpType.Add, value2);
				if (this.preMpValue == base.unit.mp)
				{
					this.isDataUpdate = false;
				}
				else
				{
					this.isDataUpdate = true;
				}
			}
			return true;
		}
	}
}
