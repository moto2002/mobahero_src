using Assets.Scripts.Model;
using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class SelectView : BaseView<SelectView>
	{
		private Transform AllButton;

		private Transform All;

		private Transform Power;

		private Transform Agile;

		private Transform IQ;

		private Transform Employ;

		private UISprite BG;

		private Transform PlayGame;

		private Transform BackButton;

		private UILabel Number;

		private HeroView favouredHeroView;

		private SelectHeroView selectedHeroView;

		private GameObject m_HeroItem;

		private TweenAlpha m_AlphaController;

		private BattleType battle_type;

		public int selectIndex;

		private List<string> select_heros = new List<string>();

		private List<string> all_heros;

		public SelectView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "SelectView");
		}

		public override void Init()
		{
			base.Init();
			this.m_HeroItem = base.LoadPrefabCache("HeroItem");
			this.AllButton = this.transform.Find("Anchor/AllButton");
			this.Employ = this.AllButton.Find("Employ");
			this.All = this.AllButton.Find("All");
			this.Power = this.AllButton.Find("Power");
			this.Agile = this.AllButton.Find("Agile");
			this.IQ = this.AllButton.Find("IQ");
			this.BG = this.transform.Find("Bottom/BG").GetComponent<UISprite>();
			this.BackButton = this.transform.Find("Anchor/BackButton");
			Transform root = this.transform.Find("Anchor/HeroPanel/HeroView");
			this.favouredHeroView = new HeroView(root);
			this.Number = this.transform.Find("Bottom/BattleNum/Number").GetComponent<UILabel>();
			this.PlayGame = this.transform.Find("Bottom/PlayGame");
			Transform root2 = this.transform.Find("Bottom/HeroPanel/HeroView");
			this.selectedHeroView = new SelectHeroView(root2);
			UIEventListener.Get(this.BackButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBackBtnEvent);
			UIEventListener.Get(this.All.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowTypeHero);
			UIEventListener.Get(this.Power.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowTypeHero);
			UIEventListener.Get(this.Agile.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowTypeHero);
			UIEventListener.Get(this.IQ.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowTypeHero);
			UIEventListener.Get(this.Employ.gameObject).onClick = new UIEventListener.VoidDelegate(this.ShowTypeHero);
			UIEventListener.Get(this.PlayGame.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickGoToGame);
			this.m_AlphaController = this.transform.GetComponent<TweenAlpha>();
		}

		public override void HandleAfterOpenView()
		{
			this.m_AlphaController.Begin();
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
			this.UpdateBtnState();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
			this.UpdateHeroData();
			this.UpdateFavouredHeroView();
			this.UpdateSelectedHeroView();
		}

		public override void Destroy()
		{
			this.selectIndex = 0;
			this.favouredHeroView.Clear();
			this.selectedHeroView.Clear();
			this.AllButton = null;
			this.All = null;
			this.Power = null;
			this.Agile = null;
			this.IQ = null;
			this.Employ = null;
			this.BG = null;
			this.PlayGame = null;
			this.BackButton = null;
			this.Number = null;
			this.favouredHeroView = null;
			this.selectedHeroView = null;
			this.m_HeroItem = null;
			base.Destroy();
		}

		private void UpdateHeroData()
		{
			CharacterDataMgr.instance.UpdateHerosData();
			this.all_heros = CharacterDataMgr.instance.OwenHeros;
			int selectedHeroCountByBattle = this.GetSelectedHeroCountByBattle();
			this.select_heros = new List<string>();
			for (int i = 0; i < selectedHeroCountByBattle; i++)
			{
				this.select_heros.Add(string.Empty);
			}
			CharacterDataMgr arg_6C_0 = CharacterDataMgr.instance;
			CharacterDataMgr arg_67_0 = CharacterDataMgr.instance;
			int num = (int)this.battle_type;
			List<string> selectedHeros = arg_6C_0.GetSelectedHeros(arg_67_0.ChangeStrUserKey(num.ToString()));
			if (selectedHeros != null)
			{
				for (int j = selectedHeroCountByBattle - 1; j >= 0; j--)
				{
					int num2 = selectedHeroCountByBattle - 1 - j;
					if (num2 < selectedHeros.Count && this.TBCCheckLive(selectedHeros[num2], null))
					{
						this.select_heros[j] = selectedHeros[num2];
					}
					else
					{
						this.select_heros[j] = string.Empty;
					}
				}
			}
		}

		private void UpdateBtnState()
		{
			foreach (Transform transform in this.AllButton)
			{
				transform.GetComponent<UIToggle>().enabled = true;
				transform.GetComponent<UIToggle>().startsActive = false;
			}
			this.All.GetComponent<UIToggle>().startsActive = true;
			this.All.GetComponent<UIToggle>().value = true;
			int num = ModelManager.Instance.Get_userData_filed_X("UnionId");
			if (num > 0)
			{
				this.Employ.GetComponent<UIToggle>().enabled = false;
			}
		}

		private void UpdateFavouredHeroView()
		{
			int count = this.all_heros.Count;
			this.favouredHeroView.SetListViewChangedCallback(new Callback<HeroItem, int, CompositeView<HeroItem>>(this.OnHeroItemChanged));
			this.favouredHeroView.CreateChildView(count, count, this.m_HeroItem, null);
		}

		private void OnHeroItemChanged(HeroItem item, int dataIndex, CompositeView<HeroItem> parent)
		{
			string text = this.all_heros[dataIndex];
			bool isMaster = this.select_heros.LastIndexOf(text) == this.select_heros.Count - 1;
			bool isSelected = this.select_heros.Contains(text);
			item.name = text;
			item.battletype = this.battle_type;
			item.ShowHero(text, false);
			if (this.battle_type == BattleType.YZ)
			{
				if (this.TBCCheckLive(text, item))
				{
					item.OnChangeHeroCallback = new Callback<HeroItem, bool>(this.OnChangeFavouredHero);
				}
			}
			else
			{
				item.OnChangeHeroCallback = new Callback<HeroItem, bool>(this.OnChangeFavouredHero);
			}
			item.UpdateSelect(isSelected, isMaster);
		}

		private void OnChangeFavouredHero(HeroItem item, bool isSelected)
		{
			string name = item.gameObject.name;
			if (isSelected)
			{
				if (!this.JudgeHeroSaturation())
				{
					if (!this.select_heros.Contains(name))
					{
						this.UpdateSelectState(item, true);
					}
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText("英雄人数达到上限", 1f);
				}
			}
			else if (this.ReturnIndex(this.select_heros) >= 0 && this.select_heros.Contains(name))
			{
				this.UpdateSelectState(item, false);
			}
		}

		private int GetSelectedHeroCountByBattle()
		{
			switch (this.battle_type)
			{
			case BattleType.None:
			case BattleType.ZY:
			case BattleType.JYZY:
			case BattleType.SGZX:
			case BattleType.JJC:
			case BattleType.YZ:
				return 3;
			case BattleType.JJC_DE:
				return 3;
			case BattleType.DLD:
				return 5;
			default:
				return 0;
			}
		}

		private void UpdateSelectedHeroView()
		{
			int selectedHeroCountByBattle = this.GetSelectedHeroCountByBattle();
			if (selectedHeroCountByBattle > 0)
			{
				this.CreateSelectHeroView(selectedHeroCountByBattle, 0);
				this.Reposition(selectedHeroCountByBattle);
			}
		}

		private void CreateSelectHeroView(int hero_count, int battleArray = 0)
		{
			this.selectedHeroView.SetListViewChangedCallback(new Callback<SelectHeroItem, int, CompositeView<SelectHeroItem>>(this.OnSelectedHeroItemChanged));
			this.selectedHeroView.CreateChildView(hero_count, hero_count, this.m_HeroItem, null);
		}

		private void OnSelectedHeroItemChanged(SelectHeroItem item, int dataIndex, CompositeView<SelectHeroItem> parent)
		{
			bool flag = this.select_heros != null && dataIndex >= 0 && dataIndex < this.select_heros.Count;
			bool isMaster = dataIndex == this.select_heros.Count - 1;
			if (flag)
			{
				string text = this.select_heros[dataIndex];
				item.SetActive(true);
				item.ShowHero(text, true);
				item.name = text;
				item.UpdateSelect(true, isMaster);
			}
			else
			{
				item.ShowHero(string.Empty, true);
				item.name = string.Empty;
				item.UpdateSelect(false, isMaster);
			}
			item.OnChangeHeroCallback = new Callback<HeroItem, bool>(this.OnChangeSelectedHero);
		}

		private void OnChangeSelectedHero(HeroItem item, bool isSelected)
		{
			this.UpdateSelectState(item, isSelected);
		}

		private void Reposition(int type = 3)
		{
			if (type == 3)
			{
				this.BG.width = 1280;
				this.selectedHeroView.cellWidth = 280f;
			}
			else if (type == 5)
			{
				this.BG.width = 1480;
				this.selectedHeroView.cellWidth = 230f;
			}
			this.selectedHeroView.Reposition();
		}

		private void UpdateSelectState(HeroItem item, bool select)
		{
			string name = item.name;
			if (name == string.Empty)
			{
				return;
			}
			bool flag = this.select_heros.Contains(name);
			if (this.battle_type == BattleType.YZ && !this.TBCCheckLive(name, null))
			{
				select = false;
				flag = false;
			}
			if (select)
			{
				if (flag)
				{
					return;
				}
				int num = this.select_heros.LastIndexOf(string.Empty);
				this.select_heros[num] = name;
				bool isMaster = this.select_heros.IndexOf(name) == this.select_heros.Count - 1;
				SelectHeroItem child = this.selectedHeroView.GetChild(num);
				child.ShowHero(name, true);
				child.UpdateSelect(true, isMaster);
				child.name = name;
				HeroItem child2 = this.favouredHeroView.GetChild(name);
				child2.UpdateSelect(true, num == this.select_heros.Count - 1);
			}
			else
			{
				if (!flag)
				{
					return;
				}
				int index = this.select_heros.IndexOf(name);
				this.select_heros[index] = string.Empty;
				SelectHeroItem child3 = this.selectedHeroView.GetChild(index);
				child3.ShowHero(string.Empty, true);
				child3.UpdateSelect(true, false);
				child3.name = string.Empty;
				HeroItem child4 = this.favouredHeroView.GetChild(name);
				child4.UpdateSelect(false, false);
			}
		}

		private void Type_Hero(List<string> HeroName)
		{
			int num = 0;
			foreach (Transform transform in this.favouredHeroView.transform)
			{
				if (HeroName.Contains(transform.name))
				{
					transform.gameObject.SetActive(true);
					num++;
				}
				else
				{
					transform.gameObject.SetActive(false);
					transform.transform.localPosition = this.favouredHeroView.GetChild(0).transform.localPosition;
				}
			}
		}

		private void ClickBackBtnEvent(GameObject object_1 = null)
		{
			CtrlManager.CloseWindow(WindowID.SelectView);
		}

		private void ClickGoToGame(GameObject object_1 = null)
		{
		}

		private void GetRewardSuccess()
		{
		}

		private void GetRewardFail()
		{
			Singleton<TipView>.Instance.ShowViewSetText("获取奖励失败", 1f);
		}

		private void ShowTypeHero(GameObject objct_1 = null)
		{
			string name = objct_1.gameObject.name;
			if (name == "Power")
			{
				this.Type_Hero(CharacterDataMgr.instance.OwenPowerHeros);
			}
			else if (name == "Agile")
			{
				this.Type_Hero(CharacterDataMgr.instance.OwenAgileHeros);
			}
			else if (name == "IQ")
			{
				this.Type_Hero(CharacterDataMgr.instance.OwenIQHeros);
			}
			else
			{
				this.Type_Hero(CharacterDataMgr.instance.OwenHeros);
			}
			this.favouredHeroView.Reposition();
		}

		private int ReturnIndex(List<string> str)
		{
			int num = 0;
			for (int i = 0; i < str.Count; i++)
			{
				if (str[i] != string.Empty)
				{
					num++;
				}
			}
			return num;
		}

		private int JudgeBattleType()
		{
			int result = 0;
			switch (this.battle_type)
			{
			case BattleType.None:
			case BattleType.ZY:
			case BattleType.JYZY:
			case BattleType.SGZX:
			case BattleType.JJC:
				result = 0;
				break;
			case BattleType.JJC_DE:
			case BattleType.DLD:
				result = 1;
				break;
			}
			return result;
		}

		public void SetBattleType(BattleType type)
		{
			this.battle_type = type;
		}

		public bool TBCCheckLive(string NPCId, HeroItem heroItem = null)
		{
			return true;
		}

		public bool JudgeHeroSaturation()
		{
			bool result = false;
			int num = 0;
			for (int i = 0; i < this.select_heros.Count; i++)
			{
				if (this.select_heros[i] != string.Empty && !string.IsNullOrEmpty(this.select_heros[i]))
				{
					num++;
				}
			}
			BattleType battleType = this.battle_type;
			if (battleType != BattleType.DLD)
			{
				if (num >= 3)
				{
					result = true;
				}
			}
			else if (num >= 5)
			{
				result = true;
			}
			return result;
		}

		public Dictionary<int, string> ReturnHeroDict(List<string> heroList)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			int num = 0;
			for (int i = 0; i < heroList.Count; i++)
			{
				if (heroList[i] != null && heroList[i] != string.Empty)
				{
					dictionary.Add(num, heroList[i]);
					num++;
				}
			}
			return dictionary;
		}

		public bool isMasterHero(int heroIndex)
		{
			return this.select_heros != null && this.select_heros.Count > 0 && heroIndex == this.select_heros.Count - 1;
		}
	}
}
