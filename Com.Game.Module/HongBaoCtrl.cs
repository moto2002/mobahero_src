using Assets.Scripts.Model;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class HongBaoCtrl : MonoBehaviour
	{
		public Transform TestForHuaping;

		public Transform TuhaoPacket;

		public Transform ZaixianPacket;

		public UILabel TuhaoNum;

		public UILabel CDTime;

		public int TuhaoCount;

		private TimeSpan time_last_one;

		private DateTime lastone = new DateTime(2000, 1, 1);

		private bool isClick;

		private Vector3 targetPos = default(Vector3);

		private Vector3 posRange = default(Vector3);

		private Vector3 mLastPos = default(Vector3);

		private DateTime localTime = new DateTime(2000, 1, 1);

		private DateTime temp = new DateTime(2000, 1, 1);

		private int packetType;

		private int packetId;

		private object[] msg;

		private Task CouldDown;

		private Task TimeChecker;

		private CoroutineManager cor = new CoroutineManager();

		private Dictionary<Transform, Vector3> parentsPosRec = new Dictionary<Transform, Vector3>();

		public RedPacketsData hongbaoData;

		private void Awake()
		{
			this.msg = new object[]
			{
				MobaGameCode.GetRedPackets,
				MobaGameCode.ClientReportOnlineTime
			};
		}

		private void OnEnable()
		{
			this.Reegister();
		}

		private void Start()
		{
			UIEventListener.Get(this.TuhaoPacket.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnPressPacket);
			UIEventListener.Get(this.ZaixianPacket.gameObject).onPress = new UIEventListener.BoolDelegate(this.OnPressPacket);
			UIEventListener.Get(this.TuhaoPacket.gameObject).onDrag = new UIEventListener.VectorDelegate(this.OnDragPacket);
			UIEventListener.Get(this.ZaixianPacket.gameObject).onDrag = new UIEventListener.VectorDelegate(this.OnDragPacket);
			UIEventListener.Get(this.TuhaoPacket.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenTuhaoHongbao);
			UIEventListener.Get(this.ZaixianPacket.gameObject).onClick = new UIEventListener.VoidDelegate(this.OpenZaixianHongbao);
		}

		private void OnDisable()
		{
			this.Unregister();
			this.lastone = new DateTime(2000, 1, 1);
		}

		private void Reegister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msg, true, "OnMsg_");
			MVC_MessageManager.AddListener_view(MobaGameCode.RichManGiftMgr, new MobaMessageFunc(this.OnMsg_RichManGiftMgr));
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msg, false, "OnMsg_");
			MVC_MessageManager.RemoveListener_view(MobaGameCode.RichManGiftMgr, new MobaMessageFunc(this.OnMsg_RichManGiftMgr));
		}

		public void OnPvpStartGame()
		{
			this.cor.StopAllCoroutine();
			this.TimeChecker = null;
			this.CouldDown = null;
		}

		private void OnMsg_GetRedPackets(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					if (mobaErrorCode == MobaErrorCode.NotInRedPacketTime)
					{
						this.ZaixianPacket.gameObject.SetActive(false);
					}
				}
				else
				{
					byte[] buffer = (byte[])operationResponse.Parameters[84];
					byte[] buffer2 = (byte[])operationResponse.Parameters[202];
					byte[] buffer3 = (byte[])operationResponse.Parameters[88];
					byte[] buffer4 = (byte[])operationResponse.Parameters[246];
					byte[] buffer5 = (byte[])operationResponse.Parameters[146];
					RedPacketsData redPacketsData = SerializeHelper.Deserialize<RedPacketsData>(buffer);
					List<EquipmentInfoData> list = SerializeHelper.Deserialize<List<EquipmentInfoData>>(buffer2);
					List<HeroInfoData> list2 = SerializeHelper.Deserialize<List<HeroInfoData>>(buffer3);
					List<DropItemData> list3 = SerializeHelper.Deserialize<List<DropItemData>>(buffer4);
					List<DropItemData> listRepeatItem = SerializeHelper.Deserialize<List<DropItemData>>(buffer5);
					ToolsFacade.Instance.GetRewards_WriteInModels_WithoutShowEffect(list, list2, list3, listRepeatItem, null);
					CtrlManager.OpenWindow(WindowID.RedPacketView, null);
					if (list3 != null && list3.Count > 0)
					{
						Singleton<RedPacketView>.Instance.UpdateGiftData(list3[0]);
					}
					else if (list != null && list.Count > 0)
					{
						Singleton<RedPacketView>.Instance.UpdateGiftData(list[0]);
					}
					else if (list2 != null && list2.Count > 0)
					{
						Singleton<RedPacketView>.Instance.UpdateGiftData(list2[0]);
					}
					this.cor.StopCoroutine(this.TimeChecker);
					if (redPacketsData.timeleft != 0)
					{
						this.StopTimeChecker();
						this.TimeChecker = this.cor.StartCoroutine(this.CheckTime(), true);
					}
					this.PacketsData(redPacketsData);
					this.isClick = false;
					this.lastone = Tools_TimeCheck.ServerCurrentTime;
				}
			}
		}

		private void OnMsg_ClientReportOnlineTime(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
				if (mobaErrorCode != MobaErrorCode.Ok)
				{
					if (mobaErrorCode == MobaErrorCode.LargeTimeDiff)
					{
						DateTime dateTime = new DateTime((long)operationResponse.Parameters[11]);
						this.localTime = dateTime;
					}
				}
			}
		}

		public void OnMsg_RichManGiftMgr(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				bool flag = (bool)operationResponse.Parameters[1];
				if (flag)
				{
					int num = (int)operationResponse.Parameters[30];
					if (num == 1)
					{
						this.TuhaoCount = (int)operationResponse.Parameters[55];
						this.UpdateTuhaoHongBao();
					}
					else if (num == 2)
					{
						int type = (int)operationResponse.Parameters[234];
						int count = (int)operationResponse.Parameters[101];
						string name = (string)operationResponse.Parameters[59];
						bool flag2 = (bool)operationResponse.Parameters[176];
						CtrlManager.OpenWindow(WindowID.RedPacketView, null);
						Singleton<RedPacketView>.Instance.UpdateGiftData(type, count, name);
						this.TuhaoCount--;
						this.UpdateTuhaoHongBao();
						if (flag2)
						{
							this.SendTrumpet(type, count, name);
						}
					}
					else
					{
						this.TuhaoPacket.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				this.TuhaoPacket.gameObject.SetActive(false);
			}
		}

		public void PacketsData(RedPacketsData _data)
		{
			if (_data.timeleft != 0)
			{
				this.StopTimeChecker();
				this.TimeChecker = this.cor.StartCoroutine(this.CheckTime(), true);
			}
			this.hongbaoData = _data;
			this.packetType = _data.type;
			this.packetId = _data.id;
			this.UpdateZaixianHongBao(_data);
		}

		public void UpdateTuhaoHongBao()
		{
			if (this.TuhaoCount < 1)
			{
				this.TuhaoPacket.gameObject.SetActive(false);
				return;
			}
			this.TuhaoPacket.gameObject.SetActive(true);
			this.TuhaoNum.text = "X" + this.TuhaoCount.ToString();
		}

		private void UpdateZaixianHongBao(RedPacketsData _data)
		{
			if (_data.id == 0)
			{
				this.ZaixianPacket.gameObject.SetActive(false);
				return;
			}
			if (!ToolsFacade.Instance.IsInTimeInterval(_data.start_time, _data.end_time))
			{
				this.ZaixianPacket.gameObject.SetActive(false);
				return;
			}
			bool flag = ToolsFacade.Instance.IsInTimeInterval(_data.start_time2, _data.end_time2);
			this.ZaixianPacket.gameObject.SetActive(flag);
			if (!flag)
			{
				return;
			}
			if (_data.timeleft != 0)
			{
				this.CDTime.gameObject.SetActive(true);
			}
			this.ZaixianPacket.gameObject.SetActive(true);
			if (this.CouldDown != null)
			{
				this.cor.StopCoroutine(this.CouldDown);
			}
			this.CouldDown = this.cor.StartCoroutine(this.CountDown(_data.timeleft), true);
		}

		private void StopTimeChecker()
		{
			if (this.TimeChecker != null)
			{
				this.cor.StopCoroutine(this.TimeChecker);
				this.TimeChecker = null;
			}
		}

		private void OnPressPacket(GameObject go, bool state)
		{
			if (!state)
			{
				if (go.transform.localPosition.x > -150f)
				{
					this.targetPos = new Vector3(730f, go.transform.localPosition.y, 0f);
				}
				else
				{
					this.targetPos = new Vector3(-961f, go.transform.localPosition.y, 0f);
				}
				go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, this.targetPos, 1f);
			}
		}

		private Vector3 GetParentsPos(Transform trans)
		{
			if (trans == null)
			{
				return Vector3.zero;
			}
			if (this.parentsPosRec.ContainsKey(trans))
			{
				return this.parentsPosRec[trans];
			}
			Transform transform = trans;
			Vector3 vector = transform.localPosition;
			while (transform.parent != null)
			{
				transform = transform.parent;
				vector += transform.localPosition;
			}
			this.parentsPosRec.Add(trans, vector);
			return vector;
		}

		protected Vector3 ScreenPos_to_NGUIPos(Vector3 screenPos)
		{
			Vector3 position = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
			position = UICamera.currentCamera.transform.InverseTransformPoint(position);
			Vector3 vector = this.GetParentsPos(base.transform) - this.GetParentsPos(UICamera.currentCamera.transform);
			return new Vector3(position.x - vector.x, position.y - vector.y, 0f);
		}

		private void OnDragPacket(GameObject go, Vector2 delta)
		{
			this.mLastPos = UICamera.lastTouchPosition;
			this.mLastPos = this.ScreenPos_to_NGUIPos(this.mLastPos);
			this.mLastPos.y = Mathf.Clamp(this.mLastPos.y, -395f, 118f);
			this.mLastPos.x = Mathf.Clamp(this.mLastPos.x, -961f, 730f);
			go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, this.mLastPos, 1f);
		}

		private void OpenTuhaoHongbao(GameObject go)
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.RichManGiftMgr, null, new object[]
			{
				2
			});
		}

		private void OpenZaixianHongbao(GameObject go)
		{
			if (this.isClick)
			{
				return;
			}
			if (this.CDTime.gameObject.activeInHierarchy)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_gift_newyear"), 1f);
				return;
			}
			this.isClick = true;
			if (!this.lastone.Equals(this.temp))
			{
				this.time_last_one = Tools_TimeCheck.ServerCurrentTime - this.lastone;
			}
			else
			{
				this.time_last_one = ModelManager.Instance.Get_loginTime_diff_X();
			}
			if (this.localTime.Equals(this.temp))
			{
				this.localTime = Tools_TimeCheck.ServerCurrentTime;
			}
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetRedPackets, null, new object[]
			{
				this.packetId,
				this.packetType,
				this.localTime.Ticks,
				(int)this.time_last_one.TotalSeconds
			});
			this.localTime = new DateTime(2000, 1, 1);
		}

		[DebuggerHidden]
		private IEnumerator CountDown(int _time)
		{
			HongBaoCtrl.<CountDown>c__Iterator136 <CountDown>c__Iterator = new HongBaoCtrl.<CountDown>c__Iterator136();
			<CountDown>c__Iterator._time = _time;
			<CountDown>c__Iterator.<$>_time = _time;
			<CountDown>c__Iterator.<>f__this = this;
			return <CountDown>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator CheckTime()
		{
			return new HongBaoCtrl.<CheckTime>c__Iterator137();
		}

		[DebuggerHidden]
		public IEnumerator CheckHongBaoVisi()
		{
			HongBaoCtrl.<CheckHongBaoVisi>c__Iterator138 <CheckHongBaoVisi>c__Iterator = new HongBaoCtrl.<CheckHongBaoVisi>c__Iterator138();
			<CheckHongBaoVisi>c__Iterator.<>f__this = this;
			return <CheckHongBaoVisi>c__Iterator;
		}

		private void SendTrumpet(int _type, int _count, string _name)
		{
			string nickName = ModelManager.Instance.Get_userData_X().NickName;
			long userId = long.Parse(ModelManager.Instance.Get_userData_X().UserId);
			int userLevel = CharacterDataMgr.instance.GetUserLevel(ModelManager.Instance.Get_userData_X().Exp);
			int ladder = ToolsFacade.Instance.ToInt32(ModelManager.Instance.Get_userData_X().LadderScore);
			int botLevel = ModelManager.Instance.Get_BottleData_Level();
			int head = ModelManager.Instance.Get_userData_filed_X("Avatar");
			int headFrame = ModelManager.Instance.Get_userData_filed_X("PictureFrame");
			int charmRankvalue = ModelManager.Instance.Get_userData_filed_X("CharmRankValue");
			string message = string.Concat(new string[]
			{
				"打开了",
				_name,
				"的红包获得了",
				_count.ToString(),
				"个",
				(_type != 1) ? "钻石" : "金币"
			});
			AgentBaseInfo client = new AgentBaseInfo
			{
				NickName = nickName,
				UserId = userId,
				head = head,
				headFrame = headFrame,
				Level = userLevel,
				Ladder = ladder,
				BotLevel = botLevel,
				CharmRankvalue = charmRankvalue
			};
			ChatMessageNew data = new ChatMessageNew
			{
				Client = client,
				ChatType = 8,
				Message = message,
				TargetId = null,
				TimeTick = ToolsFacade.ServerCurrentTime.Ticks
			};
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(100, SerializeHelper.Serialize<ChatMessageNew>(data));
			SendMsgManager.SendMsgParam sendMsgParam = new SendMsgManager.SendMsgParam(true, "正在发送...", false, 15f);
			NetWorkHelper.Instance.client.SendSessionChannelMessage(2, MobaChannel.Chat, dictionary);
			if (this.TuhaoCount == 0)
			{
				this.TuhaoPacket.gameObject.SetActive(false);
			}
		}
	}
}
