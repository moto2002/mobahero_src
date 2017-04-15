using Com.Game.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class HudBarRecycler
	{
		public const string MainHeroSlider = "MainHeroSlider";

		public const string MonsterSlider = "MonsterSlider";

		public const string SummonMonsterSlider = "SummonerMonsterSlider";

		public const string TowerSlider = "TowerSlider";

		public const string CreepSlider = "CreepSlider";

		public const string HeroSlider = "HeroSlider";

		public const string EyeSlider = "EyeSlider";

		private readonly Dictionary<string, ObjectRecycler<UIBloodBar>> _sliderPools = new Dictionary<string, ObjectRecycler<UIBloodBar>>();

		public UIBloodBar Create(string name, Transform parent, Units unit)
		{
			if (!this._sliderPools.ContainsKey(name))
			{
				this._sliderPools[name] = new ObjectRecycler<UIBloodBar>(() => this.DoCreate(name), null);
			}
			return this._sliderPools[name].Create(parent);
		}

		private UIBloodBar DoCreate(string name)
		{
			Transform transform = MapManager.Instance.SpawnUI(name, null);
			bool flag = name == "MainHeroSlider" || name == "CreepSlider";
			if (transform == null && flag)
			{
				GameObject gameObject = Resources.Load("Prefab/UI/Battle/HP/" + name) as GameObject;
				if (gameObject)
				{
					transform = gameObject.transform;
				}
			}
			if (transform)
			{
				UIBloodBar component = transform.GetComponent<UIBloodBar>();
				if (component)
				{
					component.BloodType = name;
				}
				return component;
			}
			return null;
		}

		public void Release(UIBloodBar bar)
		{
			if (!bar)
			{
				return;
			}
			if (this._sliderPools.ContainsKey(bar.BloodType))
			{
				this._sliderPools[bar.BloodType].Release(bar);
			}
			else
			{
				ClientLogger.Error("cannot find pool for #" + bar.BloodType);
			}
		}

		public void ReleaseAll()
		{
			foreach (KeyValuePair<string, ObjectRecycler<UIBloodBar>> current in this._sliderPools)
			{
				current.Value.DestroyPool();
			}
			this._sliderPools.Clear();
		}

		[Conditional("DEBUG_MODE")]
		public static void Log(string msg, UnityEngine.Object context = null)
		{
			if (context)
			{
				UnityEngine.Debug.Log("====" + msg, context);
			}
			else
			{
				UnityEngine.Debug.Log("====" + msg);
			}
		}
	}
}
