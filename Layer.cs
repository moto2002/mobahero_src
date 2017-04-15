using System;
using UnityEngine;

public class Layer
{
	public static int DefaultLayer = LayerMask.NameToLayer("Default");

	public static int DefaultMask = 1 << Layer.DefaultLayer;

	public static int IgnoreLayer = LayerMask.NameToLayer("Ignore Raycast");

	public static int IgnoreMask = 1 << Layer.IgnoreLayer;

	public static int TransparentFxLayer = LayerMask.NameToLayer("TransparentFX");

	public static int TransparentFxMask = 1 << Layer.TransparentFxLayer;

	public static int UILayer = LayerMask.NameToLayer("UI");

	public static int UIMask = 1 << Layer.UILayer;

	public static int NGUILayer = LayerMask.NameToLayer("NGUI");

	public static int NGUIMask = 1 << Layer.NGUILayer;

	public static int UIEffectLayer = LayerMask.NameToLayer("UIEffect");

	public static int UIEffectMask = 1 << Layer.UIEffectLayer;

	public static int UnitLayer = LayerMask.NameToLayer("Unit");

	public static int UnitMask = 1 << Layer.UnitLayer;

	public static int MonsterLayer = LayerMask.NameToLayer("Monster");

	public static int MonsterMask = 1 << Layer.MonsterLayer;

	public static int ShopLayer = LayerMask.NameToLayer("Shop");

	public static int ShopMask = 1 << Layer.ShopLayer;

	public static int ItemLayer = LayerMask.NameToLayer("Item");

	public static int ItemMask = 1 << Layer.ItemLayer;

	public static int GroundLayer = LayerMask.NameToLayer("Ground");

	public static int GroundMask = 1 << Layer.GroundLayer;

	public static int GrassLayer = LayerMask.NameToLayer("Grass");

	public static int GrassMask = 1 << Layer.GrassLayer;

	public static int DynamicObstacleLayer = LayerMask.NameToLayer("DynamicObstacle");

	public static int DynamicObstacleMask = 1 << Layer.DynamicObstacleLayer;

	public static int BuildingObstacleLayer = LayerMask.NameToLayer("BuildingObstacle");

	public static int BuildingObstacleMask = 1 << Layer.BuildingObstacleLayer;

	public static int StaticObstacleLayer = LayerMask.NameToLayer("StaticObstacle");

	public static int StaticObstacleMask = 1 << Layer.StaticObstacleLayer;
}
