using System;
using UnityEngine;

public class TestChaosFight : MonoBehaviour
{
	private readonly TeamType[] _teams = new TeamType[]
	{
		TeamType.BL,
		TeamType.LM,
		TeamType.Team_3
	};

	private readonly string[] _teamNames;

	private int _selected = -1;

	private TestChaosFight()
	{
		this._teamNames = new string[this._teams.Length];
		for (int i = 0; i < this._teamNames.Length; i++)
		{
			this._teamNames[i] = this._teams[i].ToString();
		}
	}

	private void OnGUI()
	{
		int num = GUILayout.Toolbar(this._selected, this._teamNames, new GUILayoutOption[0]);
		if (this._selected != num)
		{
			this._selected = num;
			GameManager.Instance.ChaosFightMgr.ChangeLeadingTeam(this._teams[this._selected]);
		}
	}
}
