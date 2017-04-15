using Com.Game.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitVisibilityManager : Singleton<UnitVisibilityManager>
{
	private Dictionary<string, List<int>> LmGrassInfo = new Dictionary<string, List<int>>();

	private Dictionary<string, List<int>> BlGrassInfo = new Dictionary<string, List<int>>();

	public void Init()
	{
		MobaMessageManager.RegistMessage((ClientMsg)25039, new MobaMessageFunc(this.BattleEnd));
		Units.OnUnitsDead += new Action<Units, Units>(this.OnUnitsDead);
	}

	private void OnUnitsDead(Units obj, Units attacker)
	{
		if (Singleton<UnitVisibilityManager>.Instance != null)
		{
			Singleton<UnitVisibilityManager>.Instance.ClearUnitGrassInfo(obj.unique_id, (TeamType)obj.teamType);
		}
	}

	private void BattleEnd(MobaMessage msg)
	{
		this.LmGrassInfo.Clear();
		this.BlGrassInfo.Clear();
		MobaMessageManager.UnRegistMessage((ClientMsg)25039, new MobaMessageFunc(this.BattleEnd));
		Units.OnUnitsDead += new Action<Units, Units>(this.OnUnitsDead);
	}

	public void SwitchGrassState(string grassId, int unitUid, TeamType unitTeam, bool enterOrExit)
	{
	}

	public void ClearUnitGrassInfo(int unitUid, TeamType unitTeam)
	{
		if (unitTeam == TeamType.LM)
		{
			foreach (string current in this.LmGrassInfo.Keys)
			{
				this.SwitchGrassState(current, unitUid, unitTeam, false);
			}
		}
		else if (unitTeam == TeamType.BL)
		{
			foreach (string current2 in this.BlGrassInfo.Keys)
			{
				this.SwitchGrassState(current2, unitUid, unitTeam, false);
			}
		}
	}

	public string GetGrassByUnit(int unitUid, TeamType unitTeam)
	{
		if (unitTeam == TeamType.LM)
		{
			foreach (string current in this.LmGrassInfo.Keys)
			{
				if (this.LmGrassInfo[current].Contains(unitUid))
				{
					string result = current;
					return result;
				}
			}
		}
		else if (unitTeam == TeamType.BL)
		{
			foreach (string current2 in this.BlGrassInfo.Keys)
			{
				if (this.BlGrassInfo[current2].Contains(unitUid))
				{
					string result = current2;
					return result;
				}
			}
		}
		return string.Empty;
	}

	public static void BecomeHalfVisible(Units unit)
	{
		if (unit != null)
		{
			unit.ShowAlpha(false, 0.5f, 0.02f, 0f);
			if (unit.mHpBar != null && !GlobalSettings.Instance.UIOpt)
			{
				unit.mHpBar.SetBarAlpha(0.5f);
			}
		}
	}

	public static void BecomeInvisible(Units unit)
	{
		if (unit != null)
		{
			if (unit.mHpBar != null && !GlobalSettings.Instance.UIOpt)
			{
				unit.mHpBar.SetBarAlpha(0f);
			}
			if (unit.mText)
			{
				unit.mText.ShowHudText(false);
			}
			unit.MarkAsTarget(false);
		}
	}

	public static void NewbieBecomeInvisible(Units inUnit)
	{
		if (inUnit == null)
		{
			return;
		}
		GameObject gameObject;
		if (inUnit.mHpBar != null)
		{
			gameObject = inUnit.mHpBar.gameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		if (inUnit.mText != null)
		{
			inUnit.mText.ShowHudText(false);
		}
		gameObject = inUnit.gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	public static void NewbieBecomeVisible(Units inUnit)
	{
		if (inUnit == null)
		{
			return;
		}
		GameObject gameObject;
		if (inUnit.mHpBar != null)
		{
			gameObject = inUnit.mHpBar.gameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
		if (inUnit.mText != null)
		{
			inUnit.mText.ShowHudText(true);
		}
		gameObject = inUnit.gameObject;
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
	}

	public static void BecomeFullVisible(Units unit)
	{
		if (unit != null)
		{
			if (unit.mHpBar != null && !GlobalSettings.Instance.UIOpt)
			{
				unit.mHpBar.SetDebuffIcon(false, string.Empty);
				unit.mHpBar.SetBarAlpha(1f);
			}
			if (unit.mText)
			{
				unit.mText.ShowHudText(true);
			}
			if (unit.mCharacterEffect != null)
			{
				unit.mCharacterEffect.CastShadows(true);
			}
		}
	}

	public static void SetUnitAlpha(Units unit, float alpha, bool hideBlood = true, bool hideShadow = true, bool hideMarkAsTarget = true, bool isHideHUDText = true)
	{
		if (unit != null)
		{
			if (unit.mText != null)
			{
				if ((double)alpha > 0.1)
				{
					unit.mText.ShowHudText(true);
				}
				else if (isHideHUDText)
				{
					unit.mText.ShowHudText(false);
				}
			}
			if (hideBlood && unit.mHpBar != null && !GlobalSettings.Instance.UIOpt)
			{
				unit.mHpBar.SetDebuffIcon(false, string.Empty);
				unit.mHpBar.SetBarAlpha(0f);
			}
			if (hideShadow && unit.mCharacterEffect != null)
			{
				unit.mCharacterEffect.CastShadows(false);
			}
			if (hideMarkAsTarget)
			{
				unit.MarkAsTarget(false);
			}
			if (alpha > 0.95f)
			{
				UnitVisibilityManager.BecomeFullVisible(unit);
			}
		}
	}

	public static void SetParticlesVisible(Units unit, bool isShow)
	{
		if (unit != null)
		{
			unit.SetParticlesVisible(isShow);
		}
	}

	public static void SetItemVisible(Units unit, bool b = false)
	{
		if (unit != null)
		{
			Renderer[] componentsInChildren = unit.gameObject.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = b;
				}
			}
		}
	}
}
