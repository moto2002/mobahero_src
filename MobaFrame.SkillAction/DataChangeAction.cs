using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class DataChangeAction : BaseDataAction
	{
		public Units targetUnit;

		public bool isReverse;

		public string buffId;

		private bool isDebugData;

		private DataChangeInfo changeInfo = new DataChangeInfo();

		private BuffData data;

		protected override void OnSendStart()
		{
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.data = Singleton<BuffDataManager>.Instance.GetVo(this.buffId);
		}

		protected override bool doAction()
		{
			if (GlobalSettings.NoDamage || Singleton<PvpManager>.Instance.IsInPvp)
			{
				this.Destroy();
				return false;
			}
			if (this.data == null || this.data.damage_ids == null)
			{
				return false;
			}
			if (this.targetUnit == null)
			{
				return false;
			}
			List<DataInfo> list = new List<DataInfo>();
			if (this.isReverse)
			{
				list = this.targetUnit.buffManager.GetBuffDataInfo(this.buffId);
				this.targetUnit.buffManager.RemBuffDataInfo(this.buffId);
			}
			else
			{
				list = this.GetDataInfos();
				this.targetUnit.buffManager.AddBuffDataInfo(this.buffId, list);
			}
			if (list == null)
			{
				this.Destroy();
				return false;
			}
			List<DataInfo> list2 = new List<DataInfo>(list);
			if (this.isReverse)
			{
				this.DoReverse(list2);
			}
			else
			{
				base.DoHuiGuangFanZhao(this.targetUnit, list2);
			}
			float hp = this.targetUnit.hp;
			float mp = this.targetUnit.mp;
			this.targetUnit.ChangeAttr(list2);
			float num = (hp - this.targetUnit.hp) * -1f;
			if (Mathf.Abs(num) > 0f)
			{
				this.targetUnit.jumpFontValue(AttrType.Hp, num, base.unit, false, this.targetUnit.isPlayer, 0, 0);
			}
			float num2 = (mp - this.targetUnit.mp) * -1f;
			if (Mathf.Abs(num2) > 0f)
			{
				this.targetUnit.jumpFontValue(AttrType.Mp, num2, base.unit, false, this.targetUnit.isPlayer, 0, 0);
			}
			if (num < 0f || this.targetUnit.hp <= 0f || float.IsNaN(this.targetUnit.hp))
			{
				this.targetUnit.Wound(base.unit, num);
				this.targetUnit.TryDeath(base.unit);
			}
			return true;
		}

		private void DoReverse(List<DataInfo> infos)
		{
			if (infos == null)
			{
				return;
			}
			for (int i = 0; i < infos.Count; i++)
			{
				infos[i].value = -infos[i].value;
			}
		}

		private List<DataInfo> GetDataInfos()
		{
			List<DataInfo> list = new List<DataInfo>();
			for (int i = 0; i < this.data.damage_ids.Length; i++)
			{
				DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(this.data.damage_ids[i]);
				if (vo == null)
				{
					Debug.LogError("没有找到伤害包：错误id" + this.data.damage_ids[i]);
				}
				else if (SkillUtility.DoMoFaMianYi(this.targetUnit, vo))
				{
					this.targetUnit.jumpFont(JumpFontType.MoFaMianYi, string.Empty, null, true);
				}
				else if (SkillUtility.DoWuDi(this.targetUnit, vo))
				{
					this.targetUnit.jumpFont(JumpFontType.WuDi, string.Empty, null, true);
				}
				else
				{
					DataInfo dataInfo = new DataInfo();
					dataInfo.reverse = this.isReverse;
					dataInfo.damageID = this.data.damage_ids[i];
					if (vo.IsPropertyValue)
					{
						dataInfo.key = vo.property_key;
						dataInfo.value = vo.property_value;
						dataInfo.percent = vo.property_percent;
						dataInfo.formula = false;
						list.Add(dataInfo);
					}
					else if (vo.IsPropertyFormula)
					{
						dataInfo.key = vo.property_key;
						dataInfo.value = FormulaTool.GetFormualValue(vo.property_formula, this.targetUnit);
						dataInfo.percent = vo.property_percent;
						dataInfo.formula = true;
						list.Add(dataInfo);
					}
					else
					{
						Debug.LogError("DataChange 公式配置错误：既不是IsPropertyValue也不是IsPropertyFormula：错误id" + this.data.damage_ids[i]);
					}
				}
			}
			return list;
		}
	}
}
