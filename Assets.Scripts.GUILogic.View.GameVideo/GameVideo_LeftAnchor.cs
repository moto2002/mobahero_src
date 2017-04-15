using Com.Game.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_LeftAnchor : MonoBehaviour
	{
		public UIToggle ToggleTemplate;

		public UIGrid ToggleGrid;

		private List<GameVideo_Toggle> toggleDataList = new List<GameVideo_Toggle>();

		private void Awake()
		{
			this.SetDefaultToggleData();
		}

		private void SetDefaultToggleData()
		{
			if (this.toggleDataList == null)
			{
				this.toggleDataList = new List<GameVideo_Toggle>();
			}
			this.toggleDataList.Clear();
			this.toggleDataList.Add(new GameVideo_Toggle
			{
				Priority = 1,
				IconSpriteName = "Live_icons_01",
				TabName = LanguageManager.Instance.GetStringById("LiveorPlayback_Paging001", "战斗回放"),
				ClickMsg = (ClientMsg)26028
			});
			this.toggleDataList.Add(new GameVideo_Toggle
			{
				Priority = 2,
				IconSpriteName = "Live_icons_02",
				TabName = LanguageManager.Instance.GetStringById("LiveorPlayback_Paging002", "游戏直播"),
				ClickMsg = (ClientMsg)26029
			});
		}

		[DebuggerHidden]
		private IEnumerator SetDefaultToggle()
		{
			GameVideo_LeftAnchor.<SetDefaultToggle>c__Iterator12B <SetDefaultToggle>c__Iterator12B = new GameVideo_LeftAnchor.<SetDefaultToggle>c__Iterator12B();
			<SetDefaultToggle>c__Iterator12B.<>f__this = this;
			return <SetDefaultToggle>c__Iterator12B;
		}

		public void OnClick_Toggle(GameObject _obj)
		{
			for (int i = 0; i < this.toggleDataList.Count; i++)
			{
				if (this.toggleDataList[i].CheckGameObj(_obj))
				{
					this.toggleDataList[i].SendMsg();
				}
			}
		}

		public void InitToggles()
		{
			GridHelper.FillGrid<UIToggle>(this.ToggleGrid, this.ToggleTemplate, 0, delegate(int idx, UIToggle comp)
			{
			});
			this.RefreshToggles();
		}

		public void RefreshToggles()
		{
			this.SetDefaultToggleData();
			this.toggleDataList.Sort();
			GridHelper.FillGrid<UIToggle>(this.ToggleGrid, this.ToggleTemplate, this.toggleDataList.Count, delegate(int idx, UIToggle toggle)
			{
				this.toggleDataList[idx].ComponentRef = toggle;
				toggle.gameObject.SetActive(true);
				toggle.transform.FindChild("Label").GetComponent<UILabel>().text = this.toggleDataList[idx].TabName;
				toggle.transform.FindChild("Pic").GetComponent<UISprite>().spriteName = this.toggleDataList[idx].IconSpriteName;
				UIEventListener.Get(toggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Toggle);
			});
			this.ToggleGrid.Reposition();
			base.StartCoroutine(this.SetDefaultToggle());
		}
	}
}
