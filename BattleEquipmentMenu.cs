using Assets.Scripts.Model;
using Com.Game.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleEquipmentMenu : MonoBehaviour
{
	private UIGrid grid;

	private string[] strList;

	private BattleEquipType[] typeList;

	private LeftButtonItem item;

	private Callback<BattleEquipType> callBack;

	private Dictionary<BattleEquipType, LeftButtonItem> dict = new Dictionary<BattleEquipType, LeftButtonItem>();

	private BattleEquipType currentType = BattleEquipType.none;

	public BattleEquipType CurrentType
	{
		get
		{
			return this.currentType;
		}
		set
		{
			if (this.currentType != value)
			{
				this.currentType = value;
				this.UpdateState(this.currentType);
				if (this.callBack != null)
				{
					this.callBack(this.currentType);
				}
			}
		}
	}

	public BattleEquipmentMenu(UIGrid _grid, LeftButtonItem _item, Callback<BattleEquipType> _callBack, string[] _strList, BattleEquipType[] _typeList)
	{
		this.grid = _grid;
		this.strList = _strList;
		this.typeList = _typeList;
		this.item = _item;
		this.callBack = _callBack;
		this.Init();
	}

	public void Init()
	{
		GridHelper.FillGrid<LeftButtonItem>(this.grid, this.item, this.strList.Length, delegate(int idx, LeftButtonItem comp)
		{
			comp.gameObject.SetActive(true);
			comp.SetName(this.strList[idx]);
			comp.Type = this.typeList[idx];
			comp.SetIcon(comp.Type);
			UIEventListener.Get(comp.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickButton);
			this.dict[this.typeList[idx]] = comp;
		});
		if (this.CurrentType == BattleEquipType.none)
		{
			this.CurrentType = BattleEquipType.recommend;
		}
	}

	private void ClickButton(GameObject obj)
	{
		LeftButtonItem component = obj.GetComponent<LeftButtonItem>();
		if (obj == null)
		{
			return;
		}
		this.CurrentType = component.Type;
	}

	private void UpdateState(BattleEquipType type)
	{
		foreach (KeyValuePair<BattleEquipType, LeftButtonItem> current in this.dict)
		{
			current.Value.ChoseState(current.Key == type);
		}
	}
}
