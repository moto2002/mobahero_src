using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class UISceneEditor : BaseView<UISceneEditor>
	{
		private Dictionary<string, object> _heroesDatas;

		private Dictionary<string, GameObject> mHeroRoot = new Dictionary<string, GameObject>();

		public UIPopupList mHeroList;

		public UIInput mCount;

		public UIButton mAdd;

		public UIButton mClear;

		public UIButton mClearAll;

		private UIButton mCloseBtn;

		private GameObject mRootObj;

		public int unitsPerRow = 50;

		public int unitSpacing = 1;

		private int rowCount;

		private int rowIndex;

		private bool mBaseEntityRefresh = true;

		private bool mShadows = true;

		private bool mEnableDynamiceObstacle = true;

		private bool mUpdateBloodBar = true;

		private bool mUpdateMinimap = true;

		private bool mUpdateFog = true;

		public UISceneEditor()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/SceneEditor");
		}

		private new void BindObject()
		{
			this.mCloseBtn = this.transform.Find("BtnClose").GetComponent<UIButton>();
			this.mHeroList = this.transform.Find("Hero/LstHeroes").GetComponent<UIPopupList>();
			this.mCount = this.transform.Find("Hero/InputCount").GetComponent<UIInput>();
			this.mAdd = this.transform.Find("Hero/BtnAdd").GetComponent<UIButton>();
			this.mClear = this.transform.Find("Hero/BtnClear").GetComponent<UIButton>();
			this.mClearAll = this.transform.Find("Hero/BtnClearAll").GetComponent<UIButton>();
			UIEventListener.Get(this.mCloseBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseWindow);
			UIEventListener.Get(this.mAdd.gameObject).onClick = new UIEventListener.VoidDelegate(this.AddHero);
			UIEventListener.Get(this.mClear.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClearCurrent);
			UIEventListener.Get(this.mClearAll.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClearAll);
			UIButton component = this.transform.Find("Hero/GameLogic").GetComponent<UIButton>();
			UIButton component2 = this.transform.Find("Hero/Shadows").GetComponent<UIButton>();
			UIButton component3 = this.transform.Find("Hero/Fog").GetComponent<UIButton>();
			UIEventListener.Get(component2.gameObject).onClick = new UIEventListener.VoidDelegate(this.ToggleShadows);
			UIEventListener.Get(component.gameObject).onClick = new UIEventListener.VoidDelegate(this.ToggleGameLogic);
			UIEventListener.Get(component3.gameObject).onClick = new UIEventListener.VoidDelegate(this.ToggleFog);
		}

		public override void Init()
		{
			this.BindObject();
			this.getAllHeroes();
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
		}

		private void CloseWindow(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.SceneEditor);
		}

		private void AddHero(GameObject go)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.mHeroList.value);
			GameObject gameObject = ResourceManager.Load<GameObject>(heroMainData.model_id, true, true, null, 0, false);
			if (gameObject == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					" ++++ Load SkillObject Error : npc_id = ",
					this.mHeroList.value,
					" modelId = ",
					heroMainData.model_id,
					" 没有对应的美术资源"
				}));
				return;
			}
			int num;
			if (int.TryParse(this.mCount.value, out num))
			{
				if (!this.mHeroRoot.ContainsKey(this.mHeroList.value))
				{
					this.mHeroRoot.Add(this.mHeroList.value, NGUITools.AddChild(this.mRootObj));
					this.mHeroRoot[this.mHeroList.value].name = this.mHeroList.value;
				}
				for (int i = 0; i < num; i++)
				{
					this.CreateHero(gameObject);
				}
			}
		}

		private void CreateHero(GameObject unit)
		{
			if (this.rowCount == this.unitsPerRow)
			{
				this.rowIndex++;
				this.rowCount = 0;
			}
			Units player = MapManager.Instance.GetPlayer();
			Vector3 position = Vector3.zero;
			if (null != player)
			{
				position = player.transform.position;
			}
			else
			{
				position = this.mRootObj.transform.position + this.mRootObj.transform.right * (float)this.rowCount * (float)this.unitSpacing + this.mRootObj.transform.forward * (float)this.rowIndex * (float)this.unitSpacing;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(unit, position, this.mRootObj.transform.rotation) as GameObject;
			gameObject.transform.parent = this.mHeroRoot[this.mHeroList.value].transform;
			this.rowCount++;
		}

		private void ClearCurrent(GameObject go)
		{
			NGUITools.Destroy(this.mHeroRoot[this.mHeroList.value]);
			this.mHeroRoot.Remove(this.mHeroList.value);
		}

		private void ClearAll(GameObject go)
		{
			foreach (string current in this.mHeroRoot.Keys)
			{
				NGUITools.Destroy(this.mHeroRoot[current]);
			}
			this.mHeroRoot.Clear();
			this.rowIndex = 0;
			this.rowCount = 0;
		}

		private void getAllHeroes()
		{
			this.mRootObj = new GameObject("HeroRoot");
			this.mRootObj.transform.localPosition = new Vector3(-24f, 0.5f, -4.3f);
			this.mRootObj.transform.localRotation = Quaternion.identity;
			this.mRootObj.transform.localScale = Vector3.one;
			this.mRootObj.layer = Layer.UnitLayer;
			this._heroesDatas = BaseDataMgr.instance.getData(typeof(SysHeroMainVo));
			if (this._heroesDatas == null)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (string current in this._heroesDatas.Keys)
			{
				list.Add(current);
			}
			list.Sort(delegate(string l, string r)
			{
				char[] array = l.ToCharArray();
				char[] array2 = r.ToCharArray();
				int num = Mathf.Min(array.Length, array2.Length);
				for (int i = 0; i < num; i++)
				{
					if (array[i] != array2[i])
					{
						return (int)(array[i] - array2[i]);
					}
				}
				return 0;
			});
			this.mHeroList.items = list;
			this.mHeroList.value = list[0];
		}

		private void ToggleGameLogic(GameObject go)
		{
			this.mBaseEntityRefresh = !this.mBaseEntityRefresh;
			BaseEntity.IsRefresh = this.mBaseEntityRefresh;
		}

		private void ToggleFog(GameObject go)
		{
			this.mUpdateFog = !this.mUpdateFog;
			FOWSystem.Instance.enableFog(this.mUpdateFog);
		}

		private void ToggleShadows(GameObject go)
		{
			this.mShadows = !this.mShadows;
			QualitySettings.SetQualityLevel(this.mShadows ? 7 : 6);
		}
	}
}
