using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchTypeItem : MonoBehaviour
{
	[SerializeField]
	private UIGrid _grid;

	[SerializeField]
	private UILabel _title;

	[NonSerialized]
	public MatchTeamType MatchTeamType;

	[NonSerialized]
	public List<MatchItem> TeamObjList = new List<MatchItem>();

	public Callback<GameObject> OnDelete;

	public Callback<GameObject> OnAddFriend;

	private MatchItem _matchItem;

	private bool _isRefreshPosition;

	private void OnDisable()
	{
		this._isRefreshPosition = false;
	}

	public void Init(List<RoomData> friends, int num, MatchTeamType type = MatchTeamType.Blue, bool isOwner = false)
	{
		MatchTypeItem.<Init>c__AnonStorey2A1 <Init>c__AnonStorey2A = new MatchTypeItem.<Init>c__AnonStorey2A1();
		<Init>c__AnonStorey2A.isOwner = isOwner;
		<Init>c__AnonStorey2A.friends = friends;
		<Init>c__AnonStorey2A.<>f__this = this;
		if (<Init>c__AnonStorey2A.friends == null)
		{
			return;
		}
		if (type == MatchTeamType.Blue)
		{
			this._title.text = LanguageManager.Instance.GetStringById("GangUpUI_BlueTeam");
			this._title.color = new Color32(0, 203, 211, 255);
			this._matchItem = Resources.Load<MatchItem>("Prefab/UI/Team/MatchItem");
		}
		else if (type == MatchTeamType.Red)
		{
			this._title.text = LanguageManager.Instance.GetStringById("GangUpUI_RedTeam");
			this._title.color = new Color32(201, 8, 8, 255);
			this._matchItem = Resources.Load<MatchItem>("Prefab/UI/Team/MatchItemRed");
		}
		else if (type == MatchTeamType.Gray)
		{
			this._title.text = LanguageManager.Instance.GetStringById("GangUpUI_Spectator");
			this._title.color = new Color32(128, 128, 128, 255);
			this._matchItem = Resources.Load<MatchItem>("Prefab/UI/Team/MatchItemGray");
		}
		if (!this._isRefreshPosition)
		{
			this.TeamObjList.Clear();
			GridHelper.FillGrid<MatchItem>(this._grid, this._matchItem, num, delegate(int idx, MatchItem comp)
			{
				comp.gameObject.SetActive(true);
				comp.deleteCallBack = new Callback<GameObject>(<Init>c__AnonStorey2A.<>f__this.DeleteItem);
				if (!<Init>c__AnonStorey2A.isOwner)
				{
					comp.addFriendCallBack = new Callback<GameObject>(<Init>c__AnonStorey2A.<>f__this.AddFriendCallback);
				}
				else
				{
					comp.addFriendCallBack = null;
				}
				<Init>c__AnonStorey2A.<>f__this.TeamObjList.Add(comp);
			});
			this._grid.Reposition();
			this._isRefreshPosition = true;
		}
		int j;
		for (j = 0; j < <Init>c__AnonStorey2A.friends.Count; j++)
		{
			MatchItem matchItem = this.TeamObjList.FirstOrDefault((MatchItem obj) => obj.recordRoomData != null && obj.recordRoomData.UserId == <Init>c__AnonStorey2A.friends[j].UserId);
			if (matchItem == null)
			{
				matchItem = this.GetChangeMatchItem(<Init>c__AnonStorey2A.friends);
				if (matchItem)
				{
					matchItem.Init(<Init>c__AnonStorey2A.friends[j], InviteType.AcceptInvite, <Init>c__AnonStorey2A.isOwner, type);
				}
				else
				{
					ClientLogger.Error("MatchTypeItem.Init failed");
				}
			}
			else
			{
				matchItem.Init(<Init>c__AnonStorey2A.friends[j], InviteType.AcceptInvite, <Init>c__AnonStorey2A.isOwner, type);
			}
		}
		int i;
		for (i = 0; i < this.TeamObjList.Count; i++)
		{
			if (<Init>c__AnonStorey2A.friends.FirstOrDefault((RoomData obj) => this.TeamObjList[i].recordRoomData != null && obj.UserId == this.TeamObjList[i].recordRoomData.UserId) == null)
			{
				this.TeamObjList[i].Init(null, InviteType.Null, <Init>c__AnonStorey2A.isOwner, type);
			}
		}
	}

	private MatchItem GetChangeMatchItem(List<RoomData> friendList)
	{
		MatchItem matchItem = this.TeamObjList.FirstOrDefault((MatchItem obj) => obj.recordRoomData == null);
		if (matchItem == null)
		{
			for (int i = 0; i < this.TeamObjList.Count; i++)
			{
				string usedId = this.TeamObjList[i].recordRoomData.UserId;
				if (friendList.Find((RoomData obj) => obj.UserId == usedId) == null)
				{
					return this.TeamObjList[i];
				}
			}
		}
		return matchItem;
	}

	public UIGrid GetGrid()
	{
		return this._grid;
	}

	public void ClearCurData()
	{
		if (this.TeamObjList == null || this.TeamObjList.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.TeamObjList.Count; i++)
		{
			this.TeamObjList[i].Init(null, InviteType.Null, false, this.MatchTeamType);
		}
	}

	private void DeleteItem(GameObject obj = null)
	{
		if (this.OnDelete != null)
		{
			this.OnDelete(obj);
		}
	}

	private void AddFriendCallback(GameObject obj = null)
	{
		if (this.OnAddFriend != null)
		{
			this.OnAddFriend(obj);
		}
	}
}
