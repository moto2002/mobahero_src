using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIGrid))]
public class LoopScrollView_ZX : MonoBehaviour
{
	public delegate void OnItemChange(GameObject go);

	public delegate void OnClickItem(GameObject go, int i);

	public delegate void OnPressItem(GameObject go, bool state);

	public List<UIWidget> m_itemList = new List<UIWidget>();

	private Vector4 m_posParam;

	private Transform m_cachedTransform;

	private int m_startIndex;

	private int m_MaxCount;

	private LoopScrollView_ZX.OnItemChange m_pItemChangeCallBack;

	private LoopScrollView_ZX.OnClickItem m_pOnClickItemCallBack;

	private LoopScrollView_ZX.OnPressItem m_pOnPressItemCallBack;

	public UIScrollView m_scrollView;

	private int listItemsMax = 100;

	private float lastpozition;

	private bool firstVislable;

	private bool lastVisiable;

	public int ListItemsMax
	{
		get
		{
			return this.listItemsMax;
		}
		set
		{
			this.listItemsMax = value;
			this.UpdateListItem(this.listItemsMax);
		}
	}

	private void OnEnable()
	{
		if (this.m_scrollView != null)
		{
			this.m_scrollView.ResetPosition();
		}
	}

	private void Awake()
	{
		this.m_cachedTransform = base.transform;
		this.m_scrollView = this.m_cachedTransform.parent.GetComponent<UIScrollView>();
		this.m_scrollView.RestritMove = true;
		UIScrollView expr_34 = this.m_scrollView;
		expr_34.moveevent = (UIScrollView.MoveEvent)Delegate.Combine(expr_34.moveevent, new UIScrollView.MoveEvent(this._LateUpdate));
		this.m_scrollView.GetComponent<UIPanel>().cullWhileDragging = true;
		UIGrid component = base.GetComponent<UIGrid>();
		float cellWidth = component.cellWidth;
		float cellHeight = component.cellHeight;
		this.m_posParam = new Vector4(cellWidth, cellHeight, (float)((component.arrangement != UIGrid.Arrangement.Horizontal) ? 0 : 1), (float)((component.arrangement != UIGrid.Arrangement.Vertical) ? 0 : 1));
		this.Init(true);
	}

	private void Start()
	{
		this.UpdateListItem(this.listItemsMax);
		this.m_scrollView.ResetPosition();
	}

	public void ResetPosition()
	{
		if (this.m_scrollView != null)
		{
			this.m_scrollView.ResetPosition();
		}
	}

	public void Init(bool _clickItem)
	{
		this.m_itemList.Clear();
		for (int i = 0; i < this.m_cachedTransform.childCount; i++)
		{
			Transform child = this.m_cachedTransform.GetChild(i);
			UIWidget uIWidget = child.GetComponent<UIWidget>();
			if (uIWidget == null)
			{
				uIWidget = child.gameObject.AddComponent<UIWidget>();
			}
			uIWidget.name = this.m_itemList.Count.ToString();
			this.m_itemList.Add(uIWidget);
			if (_clickItem)
			{
				BoxCollider x = child.GetComponent<BoxCollider>();
				if (x == null)
				{
					x = child.gameObject.AddComponent<BoxCollider>();
					uIWidget.autoResizeBoxCollider = true;
				}
				UIEventListener uIEventListener = child.GetComponent<UIEventListener>();
				if (uIEventListener == null)
				{
					uIEventListener = child.gameObject.AddComponent<UIEventListener>();
				}
				uIEventListener.onClick = new UIEventListener.VoidDelegate(this.OnClickListItem);
				uIEventListener.onPress = new UIEventListener.BoolDelegate(this.OnPressListItem);
			}
		}
	}

	public void UpdateListItem(int _count)
	{
		this.m_startIndex = 0;
		this.m_MaxCount = _count;
		for (int i = 0; i < this.m_itemList.Count; i++)
		{
			UIWidget uIWidget = this.m_itemList[i];
			uIWidget.name = i.ToString();
			uIWidget.Invalidate(true);
			NGUITools.SetActive(uIWidget.gameObject, i < _count);
		}
	}

	public List<UIWidget> GetItemList()
	{
		return this.m_itemList;
	}

	public List<T> GetItemList<T>() where T : Component
	{
		List<T> list = new List<T>();
		foreach (UIWidget current in this.m_itemList)
		{
			list.Add(current.GetComponent<T>());
		}
		return list;
	}

	public List<T> GetItemListInChildren<T>() where T : Component
	{
		List<T> list = new List<T>();
		foreach (UIWidget current in this.m_itemList)
		{
			list.Add(current.GetComponentInChildren<T>());
		}
		return list;
	}

	private void _LateUpdate()
	{
		if (this.m_itemList.Count <= 1)
		{
			return;
		}
		int index = -1;
		int index2 = -1;
		int num = 0;
		this.firstVislable = this.m_itemList[0].isVisible;
		this.lastVisiable = this.m_itemList[this.m_itemList.Count - 1].isVisible;
		if (this.firstVislable == this.lastVisiable)
		{
			return;
		}
		if (Math.Abs(this.lastpozition - this.m_scrollView.transform.position.y) < 0.01f)
		{
			return;
		}
		if (this.m_scrollView.transform.position.y - this.lastpozition > 0f)
		{
			if (this.firstVislable)
			{
				return;
			}
			index = 0;
			index2 = this.m_itemList.Count - 1;
			num = -1;
		}
		if (this.m_scrollView.transform.position.y - this.lastpozition < 0f)
		{
			if (this.lastVisiable)
			{
				return;
			}
			index = this.m_itemList.Count - 1;
			index2 = 0;
			num = 1;
		}
		this.lastpozition = this.m_scrollView.transform.position.y;
		int num2 = int.Parse(this.m_itemList[index].gameObject.name);
		int num3 = int.Parse(this.m_itemList[index2].gameObject.name);
		if (num3 <= this.m_startIndex || num3 >= this.m_MaxCount - 1)
		{
			this.m_scrollView.restrictWithinPanel = true;
			return;
		}
		this.m_scrollView.restrictWithinPanel = false;
		UIWidget uIWidget = this.m_itemList[index];
		Vector3 b = new Vector2((float)num * this.m_posParam.x * this.m_posParam.z, (float)num * this.m_posParam.y * this.m_posParam.w);
		uIWidget.cachedTransform.localPosition = this.m_itemList[index2].cachedTransform.localPosition + b;
		this.m_itemList.RemoveAt(index);
		this.m_itemList.Insert(index2, uIWidget);
		uIWidget.name = ((num2 <= num3) ? (num3 + 1) : (num3 - 1)).ToString();
		this.OnItemChangeMsg(uIWidget.gameObject);
	}

	public void SetDelegate(LoopScrollView_ZX.OnItemChange _onItemChange, LoopScrollView_ZX.OnClickItem _onClickItem, LoopScrollView_ZX.OnPressItem _onPressItem = null)
	{
		this.m_pItemChangeCallBack = _onItemChange;
		if (_onClickItem != null)
		{
			this.m_pOnClickItemCallBack = _onClickItem;
		}
		if (_onPressItem != null)
		{
			this.m_pOnPressItemCallBack = _onPressItem;
		}
	}

	private void OnItemChangeMsg(GameObject go)
	{
		if (this.m_pItemChangeCallBack != null)
		{
			this.m_pItemChangeCallBack(go);
		}
	}

	private void OnClickListItem(GameObject go)
	{
		int i = int.Parse(go.name);
		if (this.m_pOnClickItemCallBack != null)
		{
			this.m_pOnClickItemCallBack(go, i);
		}
	}

	private void OnPressListItem(GameObject go, bool state)
	{
		if (this.m_pOnPressItemCallBack != null)
		{
			this.m_pOnPressItemCallBack(go, state);
		}
	}
}
