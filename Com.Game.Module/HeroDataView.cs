using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class HeroDataView : BaseView<HeroDataView>
	{
		public Dictionary<int, Dictionary<int, float>> _allHeroValues = new Dictionary<int, Dictionary<int, float>>();

		private int _curHeroId = -1;

		private List<int> _heroIds = new List<int>();

		private HeroDataMonitor mediator;

		public HeroDataMonitor Mediator
		{
			get
			{
				return this.mediator;
			}
		}

		public HeroDataView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/HeroDetailView");
		}

		public void SetData(Dictionary<int, float> _data, int hero_id)
		{
			if (_data != null)
			{
				if (!this._allHeroValues.ContainsKey(hero_id))
				{
					this._allHeroValues.Add(hero_id, _data);
				}
				else
				{
					this._allHeroValues[hero_id] = _data;
				}
			}
			if (!this._heroIds.Contains(hero_id))
			{
				this._heroIds.Add(hero_id);
			}
		}

		public void OnAttrChange(int type, object newVal, object changeVal, int heroId)
		{
			if (this._allHeroValues.ContainsKey(heroId))
			{
				Dictionary<int, float> dictionary = this._allHeroValues[heroId];
				if (dictionary.ContainsKey(type))
				{
					dictionary[type] = (float)newVal;
				}
			}
			if (this.mediator != null)
			{
				this.mediator.OnAttrChange(type, newVal, changeVal, heroId);
			}
		}

		public override void Init()
		{
			base.Init();
			this.mediator = this.gameObject.GetComponent<HeroDataMonitor>();
			this.mediator.InitData(this._allHeroValues, this._heroIds);
		}

		public void Reset()
		{
		}

		public override void Destroy()
		{
			if (this.mediator != null)
			{
				UnityEngine.Object.Destroy(this.mediator);
			}
			base.Destroy();
		}
	}
}
