using System;
using System.Collections.Generic;
using UnityEngine;

public class UIWrapGrid : MonoBehaviour
{
	private Transform mTrans;

	private UIPanel mPanel;

	private UIScrollView mScroll;

	private bool mHorizontal;

	private List<Transform> mChildren = new List<Transform>();

	[SerializeField]
	private int boundsOffset = 150;

	protected virtual void Start()
	{
		this.InitGrid();
	}

	public void InitGrid()
	{
		this.mTrans = base.transform;
		this.mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
		this.mScroll = this.mPanel.GetComponent<UIScrollView>();
		if (this.mScroll != null)
		{
			this.mScroll.GetComponent<UIPanel>().onClipMove = new UIPanel.OnClippingMoved(this.OnMove);
		}
		this.mChildren.Clear();
		for (int i = 0; i < this.mTrans.childCount; i++)
		{
			this.mChildren.Add(this.mTrans.GetChild(i));
		}
		this.mChildren.Sort(new Comparison<Transform>(UIGrid.SortByName));
		if (this.mScroll == null)
		{
			return;
		}
		if (this.mScroll.movement == UIScrollView.Movement.Horizontal)
		{
			this.mHorizontal = true;
		}
		else if (this.mScroll.movement == UIScrollView.Movement.Vertical)
		{
			this.mHorizontal = false;
		}
		this.WrapContent();
	}

	protected virtual void OnMove(UIPanel panel)
	{
		this.WrapContent();
	}

	private void WrapContent()
	{
		Vector3[] worldCorners = this.mPanel.worldCorners;
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector = worldCorners[i];
			vector = this.mTrans.InverseTransformPoint(vector);
			worldCorners[i] = vector;
		}
		Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
		if (this.mHorizontal)
		{
			int j = 0;
			int count = this.mChildren.Count;
			while (j < count)
			{
				Transform transform = this.mChildren[j];
				float num = transform.localPosition.x - vector2.x;
				float num2 = worldCorners[0].x - (float)this.boundsOffset;
				float num3 = worldCorners[2].x + (float)this.boundsOffset;
				num += this.mPanel.clipOffset.x - this.mTrans.localPosition.x;
				if (!UICamera.IsPressed(transform.gameObject))
				{
					NGUITools.SetActive(transform.gameObject, num > num2 && num < num3, false);
				}
				j++;
			}
		}
		else
		{
			int k = 0;
			int count2 = this.mChildren.Count;
			while (k < count2)
			{
				Transform transform2 = this.mChildren[k];
				float num4 = transform2.localPosition.y - vector2.y;
				float num5 = worldCorners[0].y - (float)this.boundsOffset;
				float num6 = worldCorners[2].y + (float)this.boundsOffset;
				num4 += this.mPanel.clipOffset.y - this.mTrans.localPosition.y;
				if (!UICamera.IsPressed(transform2.gameObject))
				{
					bool activeSelf = transform2.gameObject.activeSelf;
					bool flag = num4 > num5 && num4 < num6;
					if (activeSelf != flag)
					{
						NGUITools.SetActive(transform2.gameObject, flag, false);
					}
				}
				k++;
			}
		}
	}
}
