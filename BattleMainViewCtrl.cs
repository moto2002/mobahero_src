using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleMainViewCtrl : BaseViewCtrl
{
	public Button[] skillBnt = new Button[4];

	private void Awake()
	{
	}

	public void OnSkillButton(int index)
	{
		EditorGamePlay.Ins.ScreenController.Player.SkillManager.Skill(index, EditorGamePlay.Ins.SkillTargetUnit, new Vector3?(EditorGamePlay.Ins.SkillPosition.position));
	}
}
