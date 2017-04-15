using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class SacrificialView : BaseView<SacrificialView>
	{
		private Transform CenterAnchor;

		private ScrificialCenter sacrificialCenter;

		private Transform Type1;

		private UIGrid Type1_Grid;

		private Transform Type2;

		private UIGrid Type2_Grid;

		private Transform Type2_Label;

		private UIScrollView HeroPanel;

		private Transform Mask;

		private CoroutineManager cor = new CoroutineManager();

		private GameObject heroID;

		private GameObject heroPrefab;

		private SacrificialItem sacrificialItem;

		public List<string> HeroinOrder = new List<string>();

		public string RecordName;

		private heroTypeBtn currentHeroBtn;

		public heroTypeBtn CurrentHeroBtn
		{
			get
			{
				return this.currentHeroBtn;
			}
			set
			{
				this.currentHeroBtn = value;
				this.InitBtn();
			}
		}

		public SacrificialView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "SacrificialView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("HeroAltar_Title_HeroAltar");
		}

		public override void Init()
		{
			base.Init();
			this.CenterAnchor = this.transform.Find("CenterAnchor");
			this.sacrificialItem = Resources.Load<SacrificialItem>("Prefab/UI/Sacrificial/SacrificialItem");
			this.Type1 = this.transform.Find("CenterAnchor/Scroll View/Type1");
			this.Type1_Grid = this.Type1.Find("Grid").GetComponent<UIGrid>();
			this.Type2 = this.transform.Find("CenterAnchor/Scroll View/Type2");
			this.Type2_Grid = this.Type2.Find("Grid").GetComponent<UIGrid>();
			this.Type2_Label = this.Type2.Find("Label");
			this.HeroPanel = this.transform.Find("CenterAnchor/Scroll View").GetComponent<UIScrollView>();
			this.Mask = this.transform.Find("CenterAnchor/Mask");
			this.sacrificialCenter = this.CenterAnchor.GetComponent<ScrificialCenter>();
		}

		public override void HandleAfterOpenView()
		{
			this.UpdateHeroView_(heroTypeBtn.All);
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearResources();
		}

		private void ClearResources()
		{
			if (this.sacrificialItem != null)
			{
				this.sacrificialItem.ClearResources();
			}
			if (this.Type1_Grid != null)
			{
				Transform transform = this.Type1_Grid.transform;
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Transform child = transform.GetChild(i);
					SacrificialItem component = child.gameObject.GetComponent<SacrificialItem>();
					if (component != null)
					{
						component.ClearResources();
					}
				}
			}
		}

		public override void RegisterUpdateHandler()
		{
			CtrlManager.CloseWindow(WindowID.MenuBottomBarView);
			Singleton<MenuTopBarView>.Instance.DoOpenOtherViewAction(0.25f);
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
			this.CurrentHeroBtn = this.currentHeroBtn;
		}

		public override void Destroy()
		{
			this.heroID = null;
			base.Destroy();
		}

		private void InitBtn()
		{
		}

		private void UpdateHeroView_(heroTypeBtn type)
		{
			this.HeroinOrder.Clear();
			CharacterDataMgr.instance.UpdateHerosData();
			List<string> allData = new List<string>();
			if (type == heroTypeBtn.All)
			{
				allData = CharacterDataMgr.instance.AllHeros;
			}
			allData = CharacterDataMgr.instance.AllHeros;
			this.HeroinOrder.AddRange(this.GetData(allData, heroDataType.Owned));
			this.HeroinOrder.AddRange(this.GetData(allData, heroDataType.Free));
			this.HeroinOrder.AddRange(this.GetData(allData, heroDataType.Non_Owned));
			this.sacrificialCenter.UpdateView(this.GetData(allData, heroDataType.Owned), this.GetData(allData, heroDataType.Free), this.GetData(allData, heroDataType.Non_Owned));
			SummSkinData summSkinData = new SummSkinData();
			summSkinData.SkinId = 0;
			summSkinData.NpcId = string.Empty;
			summSkinData.SummId = 0L;
		}

		private List<string> GetData(List<string> allData, heroDataType type)
		{
			List<string> list = new List<string>();
			foreach (string current in Singleton<PvpManager>.Instance.freeHeros)
			{
				try
				{
					list.Add(current.Split(new char[]
					{
						','
					})[0]);
				}
				catch (Exception ex)
				{
					CtrlManager.ShowMsgBox(ex.ToString(), this.ToString() + "解析限免英雄数组时报错，检查代码", delegate
					{
					}, PopViewType.PopOneButton, "确定", "取消", null);
				}
			}
			if (allData == null || allData.Count == 0)
			{
				return new List<string>();
			}
			List<string> list2 = new List<string>();
			switch (type)
			{
			case heroDataType.Owned:
				list2 = this.GetTypeData(CharacterDataMgr.instance.OwenHeros, allData);
				break;
			case heroDataType.Non_Owned:
				if (CharacterDataMgr.instance.AllNoHaveHeros != null && CharacterDataMgr.instance.AllCanCallHeros != null)
				{
					list2 = this.GetTypeData(CharacterDataMgr.instance.AllNoHaveHeros, allData).Except(this.GetTypeData(list, allData)).ToList<string>();
				}
				break;
			case heroDataType.Free:
				list2 = this.GetTypeData(list, allData).Except(this.GetTypeData(CharacterDataMgr.instance.OwenHeros, allData)).ToList<string>();
				break;
			}
			List<string> list3 = new List<string>();
			for (int i = 0; i < list2.Count; i++)
			{
				list3.Add(list2[list2.Count - i - 1]);
			}
			return list3;
		}

		private List<string> GetTypeData(List<string> list1, List<string> list2)
		{
			List<string> list3 = new List<string>();
			foreach (string current in list1)
			{
				if (list2 != null && list2.Contains(current))
				{
					list3.Add(current);
				}
			}
			return list3;
		}
	}
}
