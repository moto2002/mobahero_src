using System;
using System.Collections.Generic;
using UnityEngine;

public class ReuseScrollView : MonoBehaviour
{
	public delegate void OnItemChange(GameObject go);

	public UIScrollView mScrollView;

	private List<UIWidget> mItemList = new List<UIWidget>();

	private Vector4 mPosParam;

	private Transform mCachedTransform;

	private int mStartIndex;

	private int mMaxCount;

	private ReuseScrollView.OnItemChange m_pItemChangeCallBack;

	public void Init(GameObject go, string functionName = "OnChangeItem")
	{
		this.mItemList.Clear();
		for (int i = 0; i < this.mCachedTransform.childCount; i++)
		{
			Transform child = this.mCachedTransform.GetChild(i);
			UIWidget uIWidget = child.GetComponent<UIWidget>();
			if (uIWidget == null)
			{
				uIWidget = child.gameObject.AddComponent<UIWidget>();
			}
			uIWidget.name = this.mItemList.Count.ToString();
			this.mItemList.Add(uIWidget);
		}
	}

	public void UpdateList(int count)
	{
		this.mStartIndex = 0;
		this.mMaxCount = count;
		for (int i = 0; i < this.mItemList.Count; i++)
		{
			UIWidget uIWidget = this.mItemList[i];
			uIWidget.name = i.ToString();
			uIWidget.Invalidate(true);
			uIWidget.gameObject.SetActive(i < count);
		}
	}

	public void UpdateDragList()
	{
		for (int i = this.mMaxCount - this.mItemList.Count; i < this.mMaxCount; i++)
		{
			int num = 0;
			UIWidget uIWidget = this.mItemList[num];
			uIWidget.name = i.ToString();
			uIWidget.Invalidate(true);
			uIWidget.gameObject.SetActive(i < this.mMaxCount);
			num++;
		}
	}

	public List<UIWidget> GetList()
	{
		return this.mItemList;
	}

	private void Awake()
	{
		this.mCachedTransform = base.transform;
		this.mScrollView = this.mCachedTransform.parent.GetComponent<UIScrollView>();
		UIGrid component = base.GetComponent<UIGrid>();
		float cellWidth = component.cellWidth;
		float cellHeight = component.cellHeight;
		this.mPosParam = new Vector4(cellWidth, cellHeight, (float)((component.arrangement != UIGrid.Arrangement.Horizontal) ? 0 : 1), (float)((component.arrangement != UIGrid.Arrangement.Vertical) ? 0 : 1));
	}

	private void LateUpdate()
	{
		if (this.mItemList.Count <= 1)
		{
			return;
		}
		int index = -1;
		int index2 = -1;
		int num = 0;
		bool isVisible = this.mItemList[0].isVisible;
		bool isVisible2 = this.mItemList[this.mItemList.Count - 1].isVisible;
		if (isVisible == isVisible2)
		{
			return;
		}
		if (isVisible)
		{
			index = this.mItemList.Count - 1;
			index2 = 0;
			num = 1;
		}
		else if (isVisible2)
		{
			index = 0;
			index2 = this.mItemList.Count - 1;
			num = -1;
		}
		int num2 = int.Parse(this.mItemList[index].gameObject.name);
		int num3 = int.Parse(this.mItemList[index2].gameObject.name);
		if (num3 <= this.mStartIndex || num3 >= this.mMaxCount - 1)
		{
			this.mScrollView.restrictWithinPanel = true;
			return;
		}
		this.mScrollView.restrictWithinPanel = false;
		UIWidget uIWidget = this.mItemList[index];
		Vector3 b = new Vector3((float)num * this.mPosParam.x * this.mPosParam.z, (float)num * this.mPosParam.y * this.mPosParam.w, 0f);
		uIWidget.cachedTransform.localPosition = this.mItemList[index2].cachedTransform.localPosition + b;
		this.mItemList.RemoveAt(index);
		this.mItemList.Insert(index2, uIWidget);
		uIWidget.name = ((num2 <= num3) ? (num3 + 1) : (num3 - 1)).ToString();
		this.ShowChangeItem(uIWidget.gameObject);
	}

	public void SetDelegate(ReuseScrollView.OnItemChange _onItemChange)
	{
		this.m_pItemChangeCallBack = _onItemChange;
	}

	private void ShowChangeItem(GameObject go)
	{
		if (this.m_pItemChangeCallBack != null)
		{
			this.m_pItemChangeCallBack(go);
		}
	}
}
