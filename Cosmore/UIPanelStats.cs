using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Cosmore
{
	public class UIPanelStats : MonoBehaviour
	{
		[Serializable]
		public class Entry
		{
			public UIPanel panel;

			public string name;

			public int maxVertsSize;

			public int curVertsSize;

			public int rebuildTotal;

			public int rebuildCountIgnoreMutiple;

			public int fillDrawCall;

			public int UpdateGeometryTrim;

			public int UpdateGeometryNoTrim;

			public bool debugLog;

			[NonSerialized]
			public float startTime;

			public string statStr;

			public float TotalTime
			{
				get
				{
					return Time.realtimeSinceStartup - this.startTime;
				}
			}

			public int UpdateGeometry
			{
				get
				{
					return this.UpdateGeometryTrim + this.UpdateGeometryNoTrim;
				}
			}

			public void Update()
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = (int)this.TotalTime;
				if (num == 0)
				{
					num = 1;
				}
				stringBuilder.AppendFormat("#time: {0} #update: {1} #fill:{2}", num, this.UpdateGeometry / num, this.fillDrawCall / num);
				this.statStr = stringBuilder.ToString();
			}
		}

		public bool showAll;

		public bool logAll;

		public bool logTrim = true;

		public static UIPanelStats instance;

		public List<UIPanelStats.Entry> panelList;

		private readonly List<UIPanelStats.Entry> _allPanelList = new List<UIPanelStats.Entry>();

		private readonly List<UIPanelStats.Entry> _activePanelList = new List<UIPanelStats.Entry>();

		private void Awake()
		{
			UIPanelStats.instance = this;
		}

		private void OnDestroy()
		{
			UIPanelStats.instance = null;
		}

		private void Update()
		{
			this.panelList = ((!this.showAll) ? this._activePanelList : this._allPanelList);
			foreach (UIPanelStats.Entry current in this.panelList)
			{
				current.Update();
			}
		}

		private UIPanelStats.Entry EnsureExists(List<UIPanelStats.Entry> list, UIPanel panel)
		{
			UIPanelStats.Entry entry = list.Find((UIPanelStats.Entry x) => x.panel == panel);
			if (entry == null)
			{
				entry = new UIPanelStats.Entry
				{
					panel = panel,
					name = UIPanelStats.GetFullName(panel.transform),
					startTime = Time.realtimeSinceStartup
				};
				list.Add(entry);
			}
			return entry;
		}

		public static string GetFullName(Transform c)
		{
			if (c == null)
			{
				return string.Empty;
			}
			Transform parent = c.parent;
			return UIPanelStats.GetFullName(parent) + "/" + c.name;
		}

		[Conditional("MOBA_NGUI_STAT")]
		public static void On_UpdateGeometry(UIPanel panel, UIDrawCall dc, bool trim)
		{
			if (UIPanelStats.instance == null)
			{
				return;
			}
			UIPanelStats.Entry entry = UIPanelStats.instance.EnsureExists(UIPanelStats.instance._allPanelList, panel);
			if (trim)
			{
				entry.UpdateGeometryTrim++;
			}
			else
			{
				entry.UpdateGeometryNoTrim++;
			}
			if (dc.verts.size > entry.maxVertsSize)
			{
				entry.maxVertsSize = dc.verts.size;
			}
			entry.curVertsSize = dc.verts.size;
			entry = UIPanelStats.instance.EnsureExists(UIPanelStats.instance._activePanelList, panel);
			if (trim)
			{
				entry.UpdateGeometryTrim++;
			}
			else
			{
				entry.UpdateGeometryNoTrim++;
			}
			if (dc.verts.size > entry.maxVertsSize)
			{
				entry.maxVertsSize = dc.verts.size;
			}
			entry.curVertsSize = dc.verts.size;
			UIPanelStats.instance._activePanelList.RemoveAll((UIPanelStats.Entry x) => x.panel == null);
			UIPanelStats.instance.panelList.Sort((UIPanelStats.Entry x, UIPanelStats.Entry y) => y.UpdateGeometry - x.UpdateGeometry);
			if (UIPanelStats.instance.logAll)
			{
				UIPanelStats.LogIt(string.Concat(new object[]
				{
					"On_UpdateGeometry ",
					entry.name,
					" trim=",
					trim
				}), false);
			}
			if (trim && UIPanelStats.instance.logTrim)
			{
				UIPanelStats.LogIt(string.Concat(new object[]
				{
					"On_UpdateGeometry ",
					entry.name,
					" trim (v,t,c)=",
					dc.verts.size,
					", ",
					dc.triangles,
					", ",
					dc.cols.size
				}), false);
			}
		}

		[Conditional("MOBA_NGUI_STAT")]
		public static void On_Rebuild(UIPanel panel, bool ignore)
		{
			if (UIPanelStats.instance == null)
			{
				return;
			}
			UIPanelStats.Entry entry = UIPanelStats.instance.EnsureExists(UIPanelStats.instance._allPanelList, panel);
			entry.rebuildTotal++;
			if (!ignore)
			{
				entry.rebuildCountIgnoreMutiple++;
			}
			if (UIPanelStats.instance.logAll || (UIPanelStats.instance.showAll && entry.debugLog))
			{
				UIPanelStats.LogIt(string.Format("mRebuild on {0}", entry.name), true);
			}
			entry = UIPanelStats.instance.EnsureExists(UIPanelStats.instance._activePanelList, panel);
			entry.rebuildTotal++;
			if (!ignore)
			{
				entry.rebuildCountIgnoreMutiple++;
			}
			if (UIPanelStats.instance.logAll || (!UIPanelStats.instance.showAll && entry.debugLog))
			{
				UIPanelStats.LogIt(string.Format("mRebuild on {0}", entry.name), true);
			}
			UIPanelStats.instance._activePanelList.RemoveAll((UIPanelStats.Entry x) => x.panel == null);
			UIPanelStats.instance.panelList.Sort((UIPanelStats.Entry x, UIPanelStats.Entry y) => y.UpdateGeometry - x.UpdateGeometry);
		}

		[Conditional("MOBA_NGUI_STAT")]
		public static void On_FillDrawCall(UIWidget uiWidget, UIDrawCall dc)
		{
			if (!UIPanelStats.instance)
			{
				return;
			}
			if (uiWidget.panel)
			{
				UIPanelStats.Entry entry = UIPanelStats.instance.EnsureExists(UIPanelStats.instance._allPanelList, uiWidget.panel);
				entry.fillDrawCall++;
				entry = UIPanelStats.instance.EnsureExists(UIPanelStats.instance._activePanelList, uiWidget.panel);
				entry.fillDrawCall++;
			}
			if (UIPanelStats.instance.logAll)
			{
				UIPanelStats.LogIt("FillDrawCall " + uiWidget.name, false);
			}
		}

		private static void LogIt(string msg, bool showFrame = false)
		{
			if (showFrame)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"@",
					Time.frameCount,
					" ",
					msg
				}));
			}
			else
			{
				UnityEngine.Debug.Log(msg);
			}
		}
	}
}
