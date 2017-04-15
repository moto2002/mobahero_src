using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MatchItem : MonoBehaviour
{
	[SerializeField]
	private Transform M_StateLock;

	[SerializeField]
	private Transform M_StateActive;

	[SerializeField]
	private UILabel M_SummonerName;

	[SerializeField]
	private Transform M_Delete;

	[SerializeField]
	private Transform M_Drag;

	[SerializeField]
	private UILabel M_Number;

	[SerializeField]
	private Transform M_Frame;

	[SerializeField]
	private Transform M_AddFriend;

	private CoroutineManager M_CoroutineManager = new CoroutineManager();

	public Callback<GameObject> deleteCallBack;

	public Callback<GameObject> addFriendCallBack;

	public RoomData recordRoomData;

	public InviteType recordInviteType;

	public bool recordHoner;

	public string recordUseId = string.Empty;

	public MatchTeamType recordTeamType;

	private bool ClickAddFriend;

	public void Init(RoomData roomData, InviteType match = InviteType.Null, bool isHoner = false, MatchTeamType type = MatchTeamType.Blue)
	{
		this.recordTeamType = type;
		this.recordInviteType = match;
		this.recordHoner = isHoner;
		this.M_StateActive.gameObject.SetActive(match == InviteType.AcceptInvite);
		this.M_StateLock.gameObject.SetActive(match == InviteType.Null);
		this.recordRoomData = roomData;
		this.IsShowFrame(false);
		if (roomData == null)
		{
			string name = string.Empty;
			base.name = name;
			this.recordUseId = name;
		}
		else
		{
			string name = roomData.UserId;
			base.name = name;
			this.recordUseId = name;
		}
		if (isHoner)
		{
			if (!base.GetComponent<KHDragDropItem>())
			{
				base.gameObject.AddComponent<KHDragDropItem>();
			}
			else
			{
				base.GetComponent<KHDragDropItem>().enabled = true;
			}
			base.GetComponent<KHDragDropItem>().cloneOnDrag = true;
			if (roomData == null)
			{
				base.GetComponent<KHDragDropItem>().enabled = false;
			}
		}
		else if (base.GetComponent<KHDragDropItem>())
		{
			base.GetComponent<KHDragDropItem>().enabled = false;
		}
		this.M_Drag.gameObject.SetActive(isHoner);
		if (!isHoner && roomData != null)
		{
			this.M_AddFriend.gameObject.SetActive(Singleton<PvpRoomView>.Instance.IsFriend(roomData.AccountId));
		}
		if (roomData == null)
		{
			return;
		}
		if (isHoner)
		{
			this.M_Delete.gameObject.SetActive(!roomData.IsHomeMain);
		}
		else
		{
			this.M_Delete.gameObject.SetActive(false);
		}
		this.M_SummonerName.text = roomData.NickName + ((!roomData.IsHomeMain) ? string.Empty : LanguageManager.Instance.GetStringById("GangUpUI_HouseOwner"));
		if (roomData.NickName == ModelManager.Instance.Get_userData_X().NickName)
		{
			this.M_SummonerName.color = new Color(1f, 0.843137264f, 0.211764708f);
		}
		else
		{
			this.M_SummonerName.color = Color.white;
		}
		this.M_SummonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(roomData.CharmRankValue);
		UIEventListener.Get(this.M_Delete.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickM_Delete);
	}

	public void IsShowFrame(bool isShow)
	{
		this.M_Frame.gameObject.SetActive(isShow);
	}

	public void ClickAddFriendIcon()
	{
		if (this.ClickAddFriend)
		{
			return;
		}
		this.ClickAddFriend = true;
		if (this.M_AddFriend.gameObject.activeInHierarchy)
		{
			this.M_AddFriend.GetComponent<UIWidget>().color = new Color32(120, 120, 120, 255);
		}
		this.M_CoroutineManager.StopAllCoroutine();
		this.M_CoroutineManager.StartCoroutine(this.IEnumRecordTime(6f), true);
	}

	[DebuggerHidden]
	private IEnumerator IEnumRecordTime(float num)
	{
		MatchItem.<IEnumRecordTime>c__Iterator188 <IEnumRecordTime>c__Iterator = new MatchItem.<IEnumRecordTime>c__Iterator188();
		<IEnumRecordTime>c__Iterator.num = num;
		<IEnumRecordTime>c__Iterator.<$>num = num;
		<IEnumRecordTime>c__Iterator.<>f__this = this;
		return <IEnumRecordTime>c__Iterator;
	}

	private void ClickM_Delete(GameObject obj = null)
	{
		if (this.deleteCallBack != null)
		{
			this.deleteCallBack(obj);
		}
	}

	private void ClickM_AddFriend(GameObject obj = null)
	{
		if (this.addFriendCallBack != null)
		{
			this.addFriendCallBack(obj);
		}
	}
}
