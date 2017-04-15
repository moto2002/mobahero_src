using Com.Game.Data;
using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
	public static BattleManager Instance;

	public SysBattleConfigVo battle;

	public SysBattleSceneVo scene;

	private string scene_id;

	public bool isUsingOld;

	public bool isTesting;
}
