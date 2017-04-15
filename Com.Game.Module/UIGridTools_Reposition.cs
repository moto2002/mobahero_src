using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class UIGridTools_Reposition : UIGrid
	{
		protected override void Sort(BetterList<Transform> list)
		{
			base.Sort(list);
		}

		[ContextMenu("Execute")]
		public override void Reposition()
		{
			base.Reposition();
			if (Application.isPlaying && !this.mInitDone && NGUITools.GetActive(this))
			{
				this.mReposition = true;
				return;
			}
			if (!this.mInitDone)
			{
				this.Init();
			}
			this.mReposition = false;
			Transform transform = base.transform;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (this.sorting != UIGrid.Sorting.None)
			{
				BetterList<Transform> betterList = new BetterList<Transform>();
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (child && (!this.hideInactive || NGUITools.GetActive(child.gameObject)))
					{
						betterList.Add(child);
					}
				}
				if (this.sorting == UIGrid.Sorting.Custom)
				{
					betterList.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortByName));
				}
				else if (this.sorting == UIGrid.Sorting.Horizontal)
				{
					betterList.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortHorizontal));
				}
				else if (this.sorting == UIGrid.Sorting.Vertical)
				{
					betterList.Sort(new BetterList<Transform>.CompareFunc(UIGrid.SortVertical));
				}
				else
				{
					this.Sort(betterList);
				}
				int j = 0;
				int size = betterList.size;
				while (j < size)
				{
					Transform transform2 = betterList[j];
					if (NGUITools.GetActive(transform2.gameObject) || !this.hideInactive)
					{
						float z = transform2.localPosition.z;
						Vector3 vector = (this.arrangement != UIGrid.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z);
						if (this.animateSmoothly && Application.isPlaying)
						{
							SpringPosition.Begin(transform2.gameObject, vector, 15f).updateScrollView = true;
						}
						else
						{
							transform2.localPosition = vector;
						}
						num3 = Mathf.Max(num3, num);
						num4 = Mathf.Max(num4, num2);
						if (++num >= this.maxPerLine && this.maxPerLine > 0)
						{
							num = 0;
							num2++;
						}
					}
					j++;
				}
			}
			else
			{
				for (int k = 0; k < transform.childCount; k++)
				{
					Transform child2 = transform.GetChild(k);
					if (NGUITools.GetActive(child2.gameObject) || !this.hideInactive)
					{
						float z2 = child2.localPosition.z;
						Vector3 vector2 = (this.arrangement != UIGrid.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z2) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z2);
						if (this.animateSmoothly && Application.isPlaying)
						{
							SpringPosition.Begin(child2.gameObject, vector2, 15f).updateScrollView = true;
						}
						else
						{
							child2.localPosition = vector2;
						}
						num3 = Mathf.Max(num3, num);
						num4 = Mathf.Max(num4, num2);
						if (++num >= this.maxPerLine && this.maxPerLine > 0)
						{
							num = 0;
							num2++;
						}
					}
				}
			}
			if (this.pivot != UIWidget.Pivot.TopLeft)
			{
				Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.pivot);
				float num5;
				float num6;
				if (this.arrangement == UIGrid.Arrangement.Horizontal)
				{
					num5 = Mathf.Lerp(0f, (float)num3 * this.cellWidth, pivotOffset.x);
					num6 = Mathf.Lerp((float)(-(float)num4) * this.cellHeight, 0f, pivotOffset.y);
				}
				else
				{
					num5 = Mathf.Lerp(0f, (float)num4 * this.cellWidth, pivotOffset.x);
					num6 = Mathf.Lerp((float)(-(float)num3) * this.cellHeight, 0f, pivotOffset.y);
				}
				for (int l = 0; l < transform.childCount; l++)
				{
					Transform child3 = transform.GetChild(l);
					if (NGUITools.GetActive(child3.gameObject) || !this.hideInactive)
					{
						SpringPosition component = child3.GetComponent<SpringPosition>();
						if (component != null)
						{
							SpringPosition expr_445_cp_0 = component;
							expr_445_cp_0.target.x = expr_445_cp_0.target.x - num5;
							SpringPosition expr_45A_cp_0 = component;
							expr_45A_cp_0.target.y = expr_45A_cp_0.target.y - num6;
						}
						else
						{
							Vector3 localPosition = child3.localPosition;
							localPosition.x -= num5;
							localPosition.y -= num6;
							child3.localPosition = localPosition;
						}
					}
				}
			}
			if (this.keepWithinPanel && this.mPanel != null)
			{
				this.mPanel.ConstrainTargetToBounds(transform, true);
			}
			if (this.onReposition != null)
			{
				this.onReposition();
			}
		}
	}
}
