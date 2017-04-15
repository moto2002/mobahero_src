using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameOverDirectorHelper
{
	[DebuggerHidden]
	public static IEnumerator Begin()
	{
		return new GameOverDirectorHelper.<Begin>c__Iterator1A4();
	}

	private static void SetAnimSpeed(float t, bool animatorLock)
	{
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.HeroAndMonster);
		foreach (Units current in mapUnits)
		{
			current.SetRawAnimSpeed(t, animatorLock);
		}
	}

	private static void ShowAtMainCamera()
	{
		List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.HeroAndMonster);
		foreach (Units current in mapUnits)
		{
			GameOverDirectorHelper.ShowAtMainCamera(current, GameOverDirectorHelper.IsWin(GameManager.FinalResult, (TeamType)current.teamType));
		}
	}

	private static bool IsWin(TeamType res, TeamType team)
	{
		if (res == TeamType.LM)
		{
			return team == TeamType.LM;
		}
		return res == TeamType.BL && team == TeamType.BL;
	}

	private static void ShowAtMainCamera(Units unit, bool win)
	{
		if (!unit.isLive)
		{
			return;
		}
		if (Camera.main && win)
		{
			Vector3 position = Camera.main.transform.position;
			position.y = unit.transform.position.y;
			unit.transform.LookAt(position);
		}
		if (unit.isHero)
		{
			unit.PlayAnim((!win) ? AnimationType.Failure : AnimationType.Victory, true, 0, true, false);
		}
	}

	private static float GetTowerExploderTime()
	{
		return 4.5f;
	}
}
