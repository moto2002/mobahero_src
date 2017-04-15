using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	internal class MiniMapGroup
	{
		private int m_groupId;

		public List<Units> m_group;

		private int timerNum;

		private Transform item;

		private UILabel label;

		private UISprite sp;

		private CoroutineManager CorManager;

		private Task task;

		private Vector3 birthPosition;

		private bool isTurnDeath;

		private int teamType = 2;

		public MiniMapGroup(int groupId, Transform newItem)
		{
			this.InitGroup(groupId, newItem);
		}

		public UISprite GetUISprite()
		{
			return this.sp;
		}

		public void SetUISprite(string _spName)
		{
			this.sp.spriteName = _spName;
		}

		public void RestorePos()
		{
			this.item.localPosition = Singleton<MiniMapView>.Instance.ChangePostion(this.birthPosition.x, this.birthPosition.z);
		}

		public int GetTeamType()
		{
			return this.teamType;
		}

		public void SetTeamType(int type)
		{
			this.teamType = type;
		}

		public Transform GetItem()
		{
			return this.item;
		}

		public Units GetFristUnits()
		{
			if (this.m_group != null && this.m_group.Count > 0)
			{
				return this.m_group[0];
			}
			return null;
		}

		private void InitGroup(int groupId, Transform newItem)
		{
			this.m_groupId = groupId;
			this.m_group = new List<Units>();
			this.timerNum = 0;
			this.item = newItem;
			this.label = this.item.GetComponent<UILabel>();
			this.sp = this.item.GetComponent<UISprite>();
			this.CorManager = new CoroutineManager();
		}

		public void SaveBirthPosition(Vector3 v3)
		{
			this.birthPosition = v3;
		}

		public Vector3 GetBirthPosition()
		{
			return this.birthPosition;
		}

		public void UpdateMember(Units member)
		{
			if (this.m_group == null)
			{
				this.InitGroup(this.m_groupId, this.item);
			}
			if (!this.m_group.Contains(member))
			{
				this.m_group.Add(member);
			}
		}

		public bool CheckMemberDeath(Units member)
		{
			this.isTurnDeath = false;
			if (this.m_group == null)
			{
				return this.isTurnDeath;
			}
			if (this.m_group.Count == 0)
			{
				return this.isTurnDeath;
			}
			if (!member.isLive && this.m_group.Exists((Units obj) => obj == member))
			{
				this.m_group.Remove(member);
				if (this.m_group.Count == 0)
				{
					this.isTurnDeath = true;
				}
			}
			return this.isTurnDeath;
		}

		public bool CheckShow()
		{
			return this.m_group != null && this.m_group.Count > 0;
		}

		public void DestroyGroup()
		{
			if (this.label != null)
			{
				this.label.text = string.Empty;
			}
			if (this.task != null)
			{
				this.task.Stop();
			}
			if (this.CorManager != null)
			{
				this.CorManager.StopCoroutine(this.task);
			}
			this.task = null;
			if (this.m_group != null)
			{
				this.m_group.Clear();
			}
			this.m_group = null;
			this.CorManager = null;
			if (this.item != null)
			{
				UnityEngine.Object.Destroy(this.item.gameObject);
			}
		}

		public void StartTimer(int timeNeed)
		{
			this.timerNum = timeNeed;
			this.item.localPosition = Singleton<MiniMapView>.Instance.ChangePostion(this.birthPosition.x, this.birthPosition.z);
			if (this.task != null)
			{
				this.CorManager.StopCoroutine(this.task);
			}
			this.task = this.CorManager.StartCoroutine(this.TimerSystem(), true);
		}

		public void ShowSp(bool isShow)
		{
			this.sp.enabled = isShow;
		}

		[DebuggerHidden]
		public IEnumerator TimerSystem()
		{
			MiniMapGroup.<TimerSystem>c__IteratorE0 <TimerSystem>c__IteratorE = new MiniMapGroup.<TimerSystem>c__IteratorE0();
			<TimerSystem>c__IteratorE.<>f__this = this;
			return <TimerSystem>c__IteratorE;
		}
	}
}
