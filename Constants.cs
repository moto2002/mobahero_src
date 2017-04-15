using System;
using UnityEngine;

public static class Constants
{
	public const bool DEBUG = false;

	public const bool DEBUG_CONTROL = false;

	public const bool DEBUG_SKILL = false;

	public const bool DEBUG_MAP = false;

	public const bool DEBUG_BUILDING = false;

	public const bool DEBUG_OPEN = true;

	public const bool DEBUG_AI = true;

	public const string TAG_MONSTER = "Monster";

	public const string TAG_PLAYER = "Player";

	public const string TAG_HERO = "Hero";

	public const string TAG_TOWER = "Building";

	public const string TAG_HOME = "Home";

	public const string TAG_MAP = "Map";

	public const string TAG_ITEM = "Item";

	public const string TAG_BUFF = "BuffItem";

	public const string TAG_DangBan = "DangBan";

	public const string TAG_SPAWNPOINT = "SpawnPoint";

	public const string OBJ_SCREENPOINT = "Target";

	public const int DEFAULT_TARGET_FPS = 30;

	public const int BATTLE_TARGET_FPS = 30;

	public const int BATTLE_TARGET_HIGHFPS = 60;

	public static int LAYER_HM = LayerMask.GetMask(new string[]
	{
		"Monster",
		"Unit"
	});

	public static int LAYER_UnitSelectObj = LayerMask.GetMask(new string[]
	{
		"UnitSelectObj"
	});
}
