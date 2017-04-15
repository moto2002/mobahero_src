using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaFrame.SkillAction
{
	public class StartSkillAction : BaseSkillAction
	{
		protected override void OnDestroy()
		{
			this.RemoveActionFromSkill(this);
			base.OnDestroy();
		}

		protected override void OnSendStart()
		{
			if (Singleton<PvpManager>.Instance.IsInPvp && PvpServerStartSkillHeroList.IsStartByServer(base.unit.npc_id))
			{
				return;
			}
			StartSkillInfo startSkillInfo = new StartSkillInfo();
			startSkillInfo.unitId = base.unit.unique_id;
			startSkillInfo.skillId = this.skillKey.SkillID;
			Vector3? targetPosition = this.targetPosition;
			if (targetPosition.HasValue)
			{
				startSkillInfo.targetPosition = MoveController.Vector3ToSVector3(this.targetPosition.Value);
			}
			List<short> list = null;
			if (this.targetUnits != null)
			{
				list = new List<short>();
				for (int i = 0; i < this.targetUnits.Count; i++)
				{
					list.Add((short)this.targetUnits[i].unique_id);
				}
			}
			startSkillInfo.targetUnits = list;
			PvpEvent.SendStartSkillEvent(startSkillInfo);
		}

		protected override bool doAction()
		{
			Singleton<TriggerManager>.Instance.SendUnitSkillStateEvent(UnitEvent.UnitSkillCmdStart, base.unit, this.skill);
			base.DoRatate(true, 0f);
			this.skill.OnSkillStartBegin(this);
			this.AddActionToSkill(SkillCastPhase.Cast_In, this);
			this.Attack_Coroutine();
			base.unit.SetCurSkillOrAttack(this.skill);
			if (this.skill != null)
			{
				if (this.skill.IsAttack)
				{
					base.unit.CallAttacked();
				}
				else if (this.skill.IsSkill)
				{
					base.unit.CallSkilled();
				}
			}
			return true;
		}

		protected virtual void Attack_Coroutine()
		{
			if (this.skillData == null)
			{
				return;
			}
			switch (this.skillData.logicType)
			{
			case SkillLogicType.Attck:
			case SkillLogicType.Skill:
			case SkillLogicType.AnSha:
			case SkillLogicType.TouGuangJiDian:
			{
				SimpleSkillAction simpleSkillAction = ActionManager.SimpleSkill(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (simpleSkillAction != null)
				{
					simpleSkillAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					simpleSkillAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(simpleSkillAction);
				}
				break;
			}
			case SkillLogicType.TuXi:
			{
				TuxiAction tuxiAction = ActionManager.TuXi(this.skillKey, base.unit, this.targetUnits);
				if (tuxiAction != null)
				{
					tuxiAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					tuxiAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(tuxiAction);
				}
				break;
			}
			case SkillLogicType.SWCR:
			{
				MultiSkillAction multiSkillAction = ActionManager.MultiSkill(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (multiSkillAction != null)
				{
					multiSkillAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					multiSkillAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(multiSkillAction);
				}
				break;
			}
			case SkillLogicType.RenWu:
			{
				RenWuAction renWuAction = ActionManager.RenWu(this.skillKey, base.unit, this.targetUnits, this.targetPosition);
				if (renWuAction != null)
				{
					renWuAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					renWuAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(renWuAction);
				}
				break;
			}
			case SkillLogicType.SiLingMaiChong:
			{
				SiLingMaiChongAction siLingMaiChongAction = ActionManager.SiLingMaiChong(this.skillKey, base.unit, this.targetUnits);
				if (siLingMaiChongAction != null)
				{
					siLingMaiChongAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					siLingMaiChongAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(siLingMaiChongAction);
				}
				break;
			}
			case SkillLogicType.ShanShuo:
			{
				Vector3? targetPosition = this.targetPosition;
				if (!targetPosition.HasValue)
				{
					Debug.LogError(" ShanShuo Error : No skillPos !!");
					base.Stop();
				}
				else
				{
					ShanShuoAction shanShuoAction = ActionManager.ShanShuo(this.skillKey, base.unit, targetPosition.Value);
					if (shanShuoAction != null)
					{
						this.AddAction(shanShuoAction);
					}
				}
				break;
			}
			case SkillLogicType.FaLiSheQu:
			{
				FaLisheQuAction faLisheQuAction = ActionManager.FaLiSheQu(this.skillKey, base.unit, this.targetUnits);
				if (faLisheQuAction != null)
				{
					faLisheQuAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					faLisheQuAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(faLisheQuAction);
				}
				break;
			}
			case SkillLogicType.JianRengFengBao:
			{
				JianRengFengBaoAction jianRengFengBaoAction = ActionManager.JianRengFengBao(this.skillKey, base.unit);
				if (jianRengFengBaoAction != null)
				{
					jianRengFengBaoAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					jianRengFengBaoAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(jianRengFengBaoAction);
				}
				break;
			}
			case SkillLogicType.JinDianLianJie:
			{
				JinDianLianJieAction jinDianLianJieAction = ActionManager.JinDianLianJie(this.skillKey, base.unit, this.targetUnits);
				if (jinDianLianJieAction != null)
				{
					jinDianLianJieAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					jinDianLianJieAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(jinDianLianJieAction);
				}
				break;
			}
			case SkillLogicType.HuoYanBaoHong:
			{
				HuoYanBaoHongAction huoYanBaoHongAction = ActionManager.HuoYanBaoHong(this.skillKey, base.unit, this.targetUnits);
				if (huoYanBaoHongAction != null)
				{
					huoYanBaoHongAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					huoYanBaoHongAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(huoYanBaoHongAction);
				}
				break;
			}
			case SkillLogicType.SiWangLianHua:
			{
				SiWangLiHuanAction siWangLiHuanAction = ActionManager.SiWangLiHuan(this.skillKey, base.unit, this.targetUnits);
				if (siWangLiHuanAction != null)
				{
					siWangLiHuanAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					siWangLiHuanAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(siWangLiHuanAction);
				}
				break;
			}
			case SkillLogicType.Waltz:
			{
				WaltzAction waltzAction = ActionManager.Waltz(this.skillKey, base.unit, this.targetUnits);
				if (waltzAction != null)
				{
					waltzAction.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
					waltzAction.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
					this.AddAction(waltzAction);
				}
				break;
			}
			case SkillLogicType.ShanShuoWeiYi:
			{
				Vector3? targetPosition2 = this.targetPosition;
				if (!targetPosition2.HasValue)
				{
					Debug.LogError(" ShanShuoWeiYi Error : No skillPos !!");
					base.Stop();
				}
				else
				{
					ShanShuoAction shanShuoAction2 = ActionManager.ShanShuoWeiYi(this.skillKey, base.unit, targetPosition2.Value);
					if (shanShuoAction2 != null)
					{
						shanShuoAction2.OnSkillDamageCallback = new Callback<BaseSkillAction, List<Units>>(this.OnSkillDamage);
						shanShuoAction2.OnSkillEndCallback = new Callback<BaseSkillAction>(this.OnSkillEnd);
						this.AddAction(shanShuoAction2);
					}
				}
				break;
			}
			case SkillLogicType.FenShen:
			{
				FenShenAction fenShenAction = ActionManager.FenShen(this.skillKey, base.unit);
				break;
			}
			}
		}
	}
}
