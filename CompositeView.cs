using MobaTools.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeView<T> : ICompositeView<T>
{
	private Transform m_parent;

	private Transform m_transform;

	private GameObject m_gameObject;

	private List<T> m_childs;

	private string m_name;

	private UIPanel m_panel;

	protected UIScrollView m_scrollView;

	protected UIGrid m_gridView;

	protected UIListView listView;

	public List<string> m_data;

	protected Callback<T, int, CompositeView<T>> OnListViewChanged;

	protected Callback<T, int> OnChildViewChanged;

	public Transform parent
	{
		get
		{
			return this.m_parent;
		}
		set
		{
			this.m_parent = value;
		}
	}

	public Transform transform
	{
		get
		{
			return this.m_transform;
		}
		set
		{
			this.m_transform = value;
		}
	}

	public GameObject gameObject
	{
		get
		{
			if (this.m_gameObject == null && this.transform != null)
			{
				this.m_gameObject = this.transform.gameObject;
			}
			return this.m_gameObject;
		}
		set
		{
			this.m_gameObject = value;
		}
	}

	public List<T> childList
	{
		get
		{
			if (this.m_childs == null)
			{
				this.m_childs = new List<T>();
			}
			return this.m_childs;
		}
		set
		{
			this.m_childs = value;
		}
	}

	public string name
	{
		get
		{
			if (this.m_name == null)
			{
				this.m_name = this.transform.name;
			}
			return this.m_name;
		}
		set
		{
			this.m_name = value;
			this.transform.name = this.m_name;
		}
	}

	public int childCount
	{
		get
		{
			if (this.childList != null)
			{
				return this.childList.Count;
			}
			return 0;
		}
	}

	public UIPanel mPanel
	{
		get
		{
			if (this.m_panel == null)
			{
				this.m_panel = this.transform.GetComponent<UIPanel>();
			}
			return this.m_panel;
		}
	}

	protected UIScrollView scrollView
	{
		get
		{
			if (this.m_scrollView == null && this.listView != null)
			{
				this.m_scrollView = this.listView.mScroll;
			}
			return this.m_scrollView;
		}
		set
		{
			this.m_scrollView = value;
		}
	}

	protected UIGrid gridView
	{
		get
		{
			if (this.m_gridView == null && this.listView != null)
			{
				this.m_gridView = this.listView.mGrid;
			}
			return this.m_gridView;
		}
		set
		{
			this.m_gridView = value;
		}
	}

	public List<string> childData
	{
		get
		{
			return this.m_data;
		}
		set
		{
			this.m_data = value;
		}
	}

	public float cellWidth
	{
		get
		{
			if (this.gridView != null)
			{
				return this.gridView.cellWidth;
			}
			return 0f;
		}
		set
		{
			if (this.gridView != null)
			{
				this.gridView.cellWidth = value;
			}
		}
	}

	public float cellHeight
	{
		get
		{
			if (this.gridView != null)
			{
				return this.gridView.cellHeight;
			}
			return 0f;
		}
		set
		{
			if (this.gridView != null)
			{
				this.gridView.cellHeight = value;
			}
		}
	}

	public CompositeView()
	{
	}

	public CompositeView(Transform root)
	{
		this.transform = root;
		this.m_gridView = this.transform.GetComponent<UIGrid>();
		this.m_scrollView = this.transform.GetComponentInParent<UIScrollView>();
	}

	public T CreateInstance<T>(Transform root)
	{
		return (T)((object)Activator.CreateInstance(typeof(T), new object[]
		{
			root
		}));
	}

	public List<T> GetChildren()
	{
		return this.childList;
	}

	public T GetChild(int index)
	{
		if (index >= 0 && index < this.childCount)
		{
			return this.childList[index];
		}
		return default(T);
	}

	public void AddChild(T item)
	{
		if (item != null)
		{
			this.childList.Add(item);
		}
	}

	public void RemoveChild(T item)
	{
		if (item != null && this.childList.Contains(item))
		{
			this.childList.Remove(item);
		}
	}

	public void Clear()
	{
		this.childList.Clear();
	}

	public abstract T GetChild(Transform root);

	public abstract T GetChild(string name);

	public abstract void CreateChildView(int dataCount, int itemCount, GameObject itemPrefab, Transform changeRoot = null);

	public void SetActive(bool active)
	{
		this.gameObject.SetActive(active);
	}

	public void ResetScrollView()
	{
		this.scrollView.ResetPosition();
	}

	public void OnListItemChanged(Transform go, int dataIndex)
	{
		T child = this.GetChild(go);
		if (child != null)
		{
			this.OnListViewChanged(child, dataIndex, this);
		}
	}

	public void ResetListView(int dataCount, int seekIndex = 0)
	{
		if (this.listView != null)
		{
			this.listView.InitListView(dataCount, seekIndex, 0);
		}
	}

	public void SeekTo(int seekIndex)
	{
		if (this.listView != null)
		{
			this.listView.Seek(seekIndex);
		}
	}

	public void SetListViewChangedCallback(Callback<T, int, CompositeView<T>> OnItemChanged)
	{
		if (this.listView != null)
		{
			this.listView.SetDelegate(new UIListView.OnItemChange(this.OnListItemChanged), null);
		}
		this.OnListViewChanged = OnItemChanged;
	}

	public void SetChildViewCallback(Callback<T, int> callback)
	{
		this.OnChildViewChanged = callback;
	}

	protected void OnCreateChildView(int dataCount, int itemCount)
	{
		if (this.listView != null)
		{
			this.listView.InitListView(dataCount, itemCount, 0);
		}
		else
		{
			for (int i = 0; i < itemCount; i++)
			{
				if (this.OnListViewChanged != null)
				{
					this.OnListViewChanged(this.childList[i], i, this);
				}
				if (this.OnChildViewChanged != null)
				{
					this.OnChildViewChanged(this.childList[i], i);
				}
			}
		}
		this.Reposition();
	}

	public virtual void UpdateViews()
	{
		if (this.listView != null)
		{
			this.listView.OnUpdate();
		}
	}

	public virtual void Reposition()
	{
		if (this.listView != null)
		{
			this.listView.ResetChildPositions();
		}
		else
		{
			if (this.gridView != null)
			{
				this.gridView.Reposition();
			}
			if (this.scrollView != null)
			{
				this.scrollView.ResetPosition();
			}
		}
	}

	public virtual void ResetPosition()
	{
	}

	public void ChangeRoot(Transform changeRoot)
	{
		if (changeRoot != null && this.transform != changeRoot)
		{
			if (this.transform.childCount > 0)
			{
				foreach (Transform transform in this.transform)
				{
					transform.parent = changeRoot;
				}
			}
			else
			{
				this.transform = changeRoot;
			}
		}
	}
}
