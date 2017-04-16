using System;
using System.Collections.Generic;
using UnityEngine;

namespace GUIFramework
{
	public class UIUtils
	{
        /// <summary>
        /// 对比UIPanel深度
        /// </summary>
		private class CompareSubPanels : IComparer<UIPanel>
		{
			public int Compare(UIPanel left, UIPanel right)
			{
				return left.depth - right.depth;
			}
		}
        /// <summary>
        /// 根据最低深度值设置指定对象相关的UIPanel的深度
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="depth">最小深度</param>
		public static void SetTargetMinPanel(GameObject obj, int depth)
		{
			List<UIPanel> panelSorted = UIUtils.GetPanelSorted(obj, true);
			if (panelSorted != null)
			{
				for (int i = 0; i < panelSorted.Count; i++)
				{
					panelSorted[i].depth = depth + i;
				}
			}
		}
        /// <summary>
        /// 获取指定对象相关的UIPanel中的最大深度
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeInactive">是否包含没有激活的</param>
        /// <returns></returns>
		public static int GetMaxTargetDepth(GameObject obj, bool includeInactive = false)
		{
			int result = -1;
			List<UIPanel> panelSorted = UIUtils.GetPanelSorted(obj, includeInactive);
			if (panelSorted != null)
			{
				return panelSorted[panelSorted.Count - 1].depth;
			}
			return result;
		}
        /// <summary>
        /// 获取指定对象相关的UIPanel中的最大深度或者最小深度
        /// </summary>
        /// <param name="target"></param>
        /// <param name="maxDepth">最大深度还是最小深度</param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
		public static GameObject GetPanelDepthMaxMin(GameObject target, bool maxDepth, bool includeInactive)
		{
			List<UIPanel> panelSorted = UIUtils.GetPanelSorted(target, includeInactive);
			if (panelSorted == null)
			{
				return null;
			}
			if (maxDepth)
			{
				return panelSorted[panelSorted.Count - 1].gameObject;
			}
			return panelSorted[0].gameObject;
		}
        /// <summary>
        /// 获取指定对象层级中的UIPanel排序列表
        /// </summary>
        /// <param name="target"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
		private static List<UIPanel> GetPanelSorted(GameObject target, bool includeInactive = false)
		{
			UIPanel[] componentsInChildren = target.transform.GetComponentsInChildren<UIPanel>(includeInactive);
			if (componentsInChildren.Length > 0)
			{
				List<UIPanel> list = new List<UIPanel>(componentsInChildren);
				list.Sort(new UIUtils.CompareSubPanels());
				return list;
			}
			return null;
		}
	}
}
