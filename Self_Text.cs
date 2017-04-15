using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Self_Text : MonoBehaviour
{
	public GameObject mTemplate;

	public UIGrid mGrid;

	public int mMaxShowCount = 6;

	public List<object> mDateList = new List<object>();

	private void Awake()
	{
		this.mGrid = base.transform.Find("indow Panel/Scroll View/UIGrid").GetComponent<UIGrid>();
		for (int i = 0; i < 500; i++)
		{
			this.mDateList.Add(i);
		}
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateView());
	}

	private void ResetList()
	{
		this.mDateList.Clear();
		for (int i = 100; i < 200; i++)
		{
			this.mDateList.Add(i);
		}
		this.UpdateListView();
	}

	[DebuggerHidden]
	private IEnumerator UpdateView()
	{
		Self_Text.<UpdateView>c__Iterator15A <UpdateView>c__Iterator15A = new Self_Text.<UpdateView>c__Iterator15A();
		<UpdateView>c__Iterator15A.<>f__this = this;
		return <UpdateView>c__Iterator15A;
	}

	[DebuggerHidden]
	private IEnumerator InitList()
	{
		Self_Text.<InitList>c__Iterator15B <InitList>c__Iterator15B = new Self_Text.<InitList>c__Iterator15B();
		<InitList>c__Iterator15B.<>f__this = this;
		return <InitList>c__Iterator15B;
	}

	private void UpdateListView()
	{
		ReuseScrollView component = this.mGrid.GetComponent<ReuseScrollView>();
		component.mScrollView.ResetPosition();
		component.UpdateList(this.mDateList.Count);
		List<UIWidget> list = component.GetList();
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i].gameObject;
			if (i < this.mDateList.Count)
			{
				gameObject.GetComponentInChildren<UILabel>().text = this.mDateList[i].ToString();
			}
		}
	}

	private void OnChangeItem(GameObject go)
	{
		int num = int.Parse(go.name);
		if (num >= this.mDateList.Count)
		{
			return;
		}
		go.GetComponentInChildren<UILabel>().text = this.mDateList[num].ToString();
	}
}
