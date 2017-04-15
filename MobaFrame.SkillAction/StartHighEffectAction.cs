using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class StartHighEffectAction : BaseHighEffAction
	{
		public new float rotateY;

		protected override void OnSendStart()
		{
			HighEffInfo highEffInfo = new HighEffInfo();
			if (this.targetUnits != null)
			{
				highEffInfo.unitIds = new List<short>();
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					if (this.targetUnits[i] != null)
					{
						highEffInfo.unitIds.Add((short)this.targetUnits[i].unique_id);
					}
				}
			}
			highEffInfo.ownerUnitId = ((!(this.owner != null)) ? 0 : this.owner.unique_id);
			highEffInfo.casterUnitId = ((!(base.unit != null)) ? 0 : base.unit.unique_id);
			highEffInfo.rotatoY = this.rotateY;
			highEffInfo.highEffId = this.data.higheffId;
			highEffInfo.skillId = this.skillId;
			if (this.skillPosition.HasValue)
			{
				highEffInfo.skillPosition = MoveController.Vector3ToSVector3(this.skillPosition.Value);
			}
			else
			{
				highEffInfo.skillPosition = MoveController.Vector3ToSVector3(Vector3.zero);
			}
			PvpEvent.SendDoHighEffEvent(highEffInfo);
		}

		protected override void OnRecordStart()
		{
			if (base.unit == null)
			{
				return;
			}
		}

		protected void doStartHighEffect_Action()
		{
			if (this.data == null)
			{
				ClientLogger.Error("高级效果没找到:" + this.higheffId);
				return;
			}
			switch (this.data.higheffType)
			{
			case HighEffType.Normal:
				this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.JiFei:
				this.AddAction(ActionManager.JiFei(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.WeiYi:
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				}
				else
				{
					this.AddAction(ActionManager.Weiyi(this.higheffId, this.skillId, this.targetUnits, base.unit, this.rotateY));
				}
				break;
			case HighEffType.YunXuan:
				this.AddAction(ActionManager.YunXuan(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.WuDi:
				this.AddAction(ActionManager.WuDi(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.MoFaMianchu:
				this.AddAction(ActionManager.MoFaMianYi(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.DingShen:
				this.AddAction(ActionManager.DingShen(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ChengMo:
				this.AddAction(ActionManager.ChengMo(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.BaoZha:
				this.AddAction(ActionManager.BaoZha(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.GuangHuang:
				this.AddAction(ActionManager.Guanghuan(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.AddBuff:
				this.AddAction(ActionManager.AttachBuff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.JianShe:
			case HighEffType.ZhousiJianshe:
				this.AddAction(ActionManager.Sputtering(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ClearAllBuff:
				this.AddAction(ActionManager.ClearAllBuff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ClearHalfBuff:
				this.AddAction(ActionManager.ClearHalfBuff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.BornUnit:
				if (this.targetUnits != null)
				{
					for (int i = 0; i < this.targetUnits.Count; i++)
					{
						this.AddAction(ActionManager.BornUnit(this.higheffId, this.skillId, this.targetUnits[i], this.skillPosition));
					}
				}
				break;
			case HighEffType.MoFaHuDun:
				this.AddAction(ActionManager.MoFaHuDun(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Repel:
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				}
				else
				{
					this.AddAction(ActionManager.Reple(this.higheffId, this.skillId, this.targetUnits, base.unit, this.rotateY));
				}
				break;
			case HighEffType.Petrifaction:
				this.AddAction(ActionManager.Petrifaction(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Tow:
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				}
				else
				{
					this.AddAction(ActionManager.Tow(this.higheffId, this.skillId, this.targetUnits, base.unit));
				}
				break;
			case HighEffType.ShakeCamera:
				if (base.unit != null && base.unit.isPlayer && base.unit.isLive)
				{
					ActionManager.ShakeCamera(this.higheffId, this.skillId, base.unit);
				}
				break;
			case HighEffType.ReBirth:
				this.AddAction(ActionManager.Rebirth(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Occoecatio:
				this.AddAction(ActionManager.Occoecatio(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.AttackExtraDamage:
				this.AddAction(ActionManager.AttackExtraDamage(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Frozen:
				this.AddAction(ActionManager.Frozen(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Charm:
				this.AddAction(ActionManager.Charm(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.AttachHighEff:
				Debug.LogError("此高级效果已近过时！AttachHighEff");
				break;
			case HighEffType.Taunt:
				this.AddAction(ActionManager.Taunt(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ClearAllDebuff:
				this.AddAction(ActionManager.ClearAllDeBuff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Growth:
				this.AddAction(ActionManager.Growth(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.AttackForTargetBuff:
				this.AddAction(ActionManager.AttackForTargetBuff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Morph:
				this.AddAction(ActionManager.Morph(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Chaos:
				this.AddAction(ActionManager.Chaos(this.higheffId, this.skillId, this.targetUnits, base.unit, this.skillPosition));
				break;
			case HighEffType.Jump:
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				}
				else
				{
					for (int j = 0; j < this.targetUnits.Count; j++)
					{
						this.AddAction(ActionManager.Jump(this.higheffId, this.skillId, this.targetUnits[j], this.skillPosition));
					}
				}
				break;
			case HighEffType.PerformReplace:
				this.AddAction(ActionManager.PerformReplace(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.HookBack:
				this.AddAction(ActionManager.HookBack(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.PlayEffectPerform:
				this.AddAction(ActionManager.PlayEffectPerform(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Invisible:
				this.AddAction(ActionManager.Invisible(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ScreenEffect:
				ActionManager.ScreenEffect(this.higheffId, this.skillId);
				break;
			case HighEffType.HuiGuangFanZhao:
				this.AddAction(ActionManager.HuiGuangFanZhao(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.BackHome:
				for (int k = 0; k < this.targetUnits.Count; k++)
				{
					BackHomeAction action = ActionManager.BackHome(this.higheffId, this.skillId, this.targetUnits[k]);
					this.AddAction(action);
				}
				break;
			case HighEffType.Funeral:
				this.AddAction(ActionManager.Funeral(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Fear:
				this.AddAction(ActionManager.Fear(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.DoSkillEffAgain:
				for (int l = 0; l < this.targetUnits.Count; l++)
				{
					this.AddAction(ActionManager.DoSkillEffAgain(this.higheffId, this.skillId, this.targetUnits[l]));
				}
				break;
			case HighEffType.SpawnerMonster:
				if (this.targetUnits != null)
				{
					for (int m = 0; m < this.targetUnits.Count; m++)
					{
						this.AddAction(ActionManager.SpawnMonster(this.higheffId, this.skillId, this.targetUnits[m]));
					}
				}
				break;
			case HighEffType.Switch:
				for (int n = 0; n < this.targetUnits.Count; n++)
				{
					this.AddAction(ActionManager.Switch(this.higheffId, this.skillId, this.targetUnits[n]));
				}
				break;
			case HighEffType.ResidentProp:
				for (int num = 0; num < this.targetUnits.Count; num++)
				{
					this.AddAction(ActionManager.ResidentProp(this.higheffId, this.skillId, this.targetUnits[num]));
				}
				break;
			case HighEffType.Flyingkick:
				if (Singleton<PvpManager>.Instance.IsInPvp)
				{
					this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				}
				else
				{
					Vector3 position = Vector3.zero;
					if (this.skillPosition.HasValue)
					{
						position = this.skillPosition.Value;
					}
					this.AddAction(ActionManager.Flyingkick(this.higheffId, this.skillId, base.unit, this.targetUnits, position));
				}
				break;
			case HighEffType.AddDataBag:
				this.AddAction(ActionManager.AddDataBag(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Temptation:
				this.AddAction(ActionManager.Temptation(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ReplaceMaterial:
				this.AddAction(ActionManager.ReplaceMaterial(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.FengBaoZhiYan:
				this.AddAction(ActionManager.FengBaoZhiYan(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.ConjureSkill:
				this.AddAction(ActionManager.ConjureSkill(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Imprisonment:
				this.AddAction(ActionManager.Imprisonment(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.RemoveHighEff:
				this.AddAction(ActionManager.DestroyHighEff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.BetweenSputter:
				this.AddAction(ActionManager.BetweenSputter(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.BloodBallGuangHuan:
				this.AddAction(ActionManager.BloodBallGuanghuan(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Sprint:
				this.AddAction(ActionManager.Sprint(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.AddGold:
				this.AddAction(ActionManager.AddGold(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.PhysicCritProp:
				this.AddAction(ActionManager.AddPhysicCritPropAction(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.PassiveJianShe:
				this.AddAction(ActionManager.Sputtering(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.AssistAddGold:
				this.AddAction(ActionManager.AssistAddGold(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.BuffLayerEffect:
				this.AddAction(ActionManager.BuffLayerEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.RemoveBuff:
				this.AddAction(ActionManager.RemoveBuffHigheff(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.TengYunTuJi:
				this.AddAction(ActionManager.TengYunTuJi(this.higheffId, this.skillId, base.unit, this.targetUnits, (!this.skillPosition.HasValue) ? Vector3.zero : this.skillPosition.Value));
				break;
			case HighEffType.GaoJiYingSheng:
				this.AddAction(ActionManager.GaoJiYingSheng(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Blow:
				this.AddAction(ActionManager.SimpleHighEffect(this.higheffId, this.skillId, this.targetUnits, base.unit));
				break;
			case HighEffType.Parabola:
				this.AddAction(ActionManager.Parabola(this.higheffId, this.skillId, this.targetUnits, base.unit, this.skillPosition));
				break;
			}
		}

		protected override void StartHighEff()
		{
			this.doStartHighEffect_Action();
		}

		protected override void StopHighEff()
		{
		}
	}
}
