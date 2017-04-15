using Assets.Scripts.GUILogic.View.HomeChatView;
using System;
using UnityEngine;

namespace Com.Game.Utils
{
	public class GridHelper
	{
		public delegate void GridBinder<T>(int idx, T comp) where T : Component;

		public static void FillGrid<T>(UIGrid grid, T sample, int count, GridHelper.GridBinder<T> binder) where T : Component
		{
			if (!grid)
			{
				throw new ArgumentNullException("UIGrid is null");
			}
			int num = count - grid.transform.childCount;
			for (int i = 0; i < num; i++)
			{
				if (!sample)
				{
					throw new ArgumentNullException("FillGrid sample is null");
				}
				NGUITools.AddChild(grid.gameObject, sample.gameObject);
			}
			if (binder != null)
			{
				for (int j = 0; j < count; j++)
				{
					Transform child = grid.transform.GetChild(j);
					binder(j, child.GetComponent<T>());
				}
				for (int k = count; k < grid.transform.childCount; k++)
				{
					Transform child2 = grid.transform.GetChild(k);
					NGUITools.SetActive(child2.gameObject, false);
				}
			}
		}

		public static void AddToGrid<T>(UIGrid grid, T sample, bool isSelf, GridHelper.GridBinder<T> binder) where T : Component
		{
			if (!grid)
			{
				throw new ArgumentNullException("UIGrid is null");
			}
			GameObject gameObject = NGUITools.AddChild(grid.gameObject, sample.gameObject);
			if (binder != null)
			{
				int num = grid.transform.childCount - 1;
				if (num < 0)
				{
					return;
				}
				if (num == 0)
				{
					if (isSelf)
					{
						gameObject.transform.localPosition = new Vector3(719f, 2f, 0f);
					}
					else
					{
						gameObject.transform.localPosition = Vector3.zero + new Vector3(0f, 2f, 0f);
					}
				}
				else
				{
					Transform child = grid.transform.GetChild(num - 1);
					int line = child.GetComponent<ChatItem>().Line;
					int num2 = line;
					if (num2 != 1)
					{
						if (num2 == 2)
						{
							if (isSelf)
							{
								gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + 719f, child.localPosition.y - 187f + (float)((num >= 5) ? 0 : 2), child.transform.localPosition.z);
							}
							else
							{
								gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, child.localPosition.y - 187f + (float)((num >= 5) ? 0 : 2), child.transform.localPosition.z);
							}
						}
					}
					else if (isSelf)
					{
						gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + 719f, child.localPosition.y - 136f + (float)((num >= 5) ? 0 : 1), child.transform.localPosition.z);
					}
					else
					{
						gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, child.localPosition.y - 136f + (float)((num >= 5) ? 0 : 1), child.transform.localPosition.z);
					}
				}
				binder(num, gameObject.GetComponent<T>());
			}
		}

		public static void FillGridWithFirstChild<T>(UIGrid grid, int count, GridHelper.GridBinder<T> binder) where T : Component
		{
			if (!grid)
			{
				throw new ArgumentNullException("UIGrid is null");
			}
			T component = grid.transform.GetChild(0).GetComponent<T>();
			GridHelper.FillGrid<T>(grid, component, count, binder);
		}
	}
}
