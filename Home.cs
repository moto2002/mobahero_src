using Com.Game.Module;
using MobaFrame.SkillAction;
using MobaHeros.Pvp;
using System;
using System.Collections.Generic;

public class Home : Building
{
	private float fWarningTimer = 2f;

	private TowerAttackIndicator _towerIndicator;

	private PlayEffectAction m_effectFangtouta;

	protected override void OnCreate()
	{
		this.data = base.AddUnitComponent<MonsterDataManager>(!Singleton<PvpManager>.Instance.IsInPvp);
		this._towerIndicator = TowerAttackIndicator.TryAddIndicator(this);
		base.OnCreate();
	}

	public override void Wound(Units attacker, float damage)
	{
		if (this.fWarningTimer < 0f && attacker != null && attacker.isHero)
		{
			this.fWarningTimer = 15f;
			bool flag = false;
			IList<Units> mapUnits = MapManager.Instance.GetMapUnits(attacker.TeamType, TargetTag.Monster);
			if (mapUnits != null)
			{
				for (int i = 0; i < mapUnits.Count; i++)
				{
					if (mapUnits[i] != null && (mapUnits[i].transform.position - base.transform.position).sqrMagnitude < 100f && TagManager.CheckTag(mapUnits[i], TargetTag.CreepsAndMinions))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				if (base.isLocalUnit)
				{
					UIMessageBox.ShowTowerWoundWarn(this);
					Singleton<MiniMapView>.Instance.ShowTowerMapWarn(this);
				}
				if (this.m_effectFangtouta != null && !this.m_effectFangtouta.isDestroyed)
				{
					this.m_effectFangtouta.Destroy();
				}
				if (base.TeamType == TeamType.LM)
				{
					this.m_effectFangtouta = ActionManager.PlayEffect("Fx_Fangtouta_LM", this, null, null, true, string.Empty, null);
				}
				else
				{
					this.m_effectFangtouta = ActionManager.PlayEffect("Fx_Fangtouta_BL", this, null, null, true, string.Empty, null);
				}
			}
		}
		base.Wound(attacker, damage);
	}

	protected override void OnUpdate(float delta)
	{
		base.OnUpdate(delta);
		if (this.fWarningTimer >= 0f)
		{
			this.fWarningTimer -= delta;
		}
	}
}
