using Com.Game.Module;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public class UnitsSnapReporter
	{
		private const int SYNC_SERVER_TIME_COUNT_MAX = 3;

		private const float SYNC_SERVER_TIME_INTERAL = 3f;

		private const long SYNC_SERVER_TIME_RETRY_TICKS = 5000000L;

		private const long SYNC_SERVER_TIME_DELAY_MAX = 1000000000L;

		private static UnitsSnapReporter _instance;

		private CoroutineManager _coroutineManager;

		private float _lastSnapTimestamp;

		private float _lastTickTimestamp;

		private float _snapSendIntervalInSecond;

		private readonly List<UnitSnapInfo> _pool = new List<UnitSnapInfo>();

		private long _lastSnapTick;

		private PhotonClient _client;

		private int _dropTime = 2;

		private bool mIsSyncServerTime;

		private int mSyncServerTimeCount;

		private long mSyncServerDelayTicks = 1000000000L;

		private long mClientTicksWhenRecieveSync;

		private long mServerTicksWhenRecieveSync;

		private long networkDelayTicks = 1000000L;

		public static UnitsSnapReporter Instance
		{
			get
			{
				if (UnitsSnapReporter._instance == null)
				{
					UnitsSnapReporter._instance = new UnitsSnapReporter();
				}
				return UnitsSnapReporter._instance;
			}
		}

		public long NetworkDelayInMs
		{
			get
			{
				return this.networkDelayTicks / 10000L;
			}
		}

		public long time
		{
			get
			{
				return this.SyncTicks;
			}
		}

		public DateTime ServerTime
		{
			get
			{
				return new DateTime(this.SyncTicks);
			}
		}

		public bool IsSyncServerTime
		{
			get
			{
				return this.mIsSyncServerTime;
			}
		}

		public long ClientTicksWhenRecieveSync
		{
			get
			{
				return this.mClientTicksWhenRecieveSync;
			}
		}

		public long ServerTicksWhenRecieveSync
		{
			get
			{
				return this.mServerTicksWhenRecieveSync;
			}
		}

		public long SyncTicks
		{
			get
			{
				return this.ServerTicksWhenRecieveSync + (DateTime.Now.Ticks - this.ClientTicksWhenRecieveSync);
			}
		}

		public float SnapSendIntervalInSecond
		{
			get
			{
				return this._snapSendIntervalInSecond;
			}
		}

		private UnitsSnapReporter()
		{
			this._client = NetWorkHelper.Instance.client;
			this._coroutineManager = new CoroutineManager();
			this._lastSnapTimestamp = 0f;
			this._snapSendIntervalInSecond = 0.1f;
			MobaMessageManager.RegistMessage(PvpCode.C2P_Ping, new MobaMessageFunc(this.OnP2C_Ping));
			MobaMessageManager.RegistMessage(PvpCode.P2C_CheckPing, new MobaMessageFunc(this.OnP2C_CheckPing));
			MobaMessageManager.RegistMessage((ClientMsg)20008, delegate(MobaMessage msg)
			{
				this.OnDisconnect();
			});
		}

		private void OnConnect()
		{
			Singleton<NetStatistic>.Instance.StartLog();
			this.ClearSyncServerTimeState();
			this._lastTickTimestamp = Time.realtimeSinceStartup + 3f;
			this._coroutineManager.StartCoroutine(this.Run(), true);
		}

		public void StartCheckServerTime()
		{
			this.OnDisconnect();
			this.OnConnect();
		}

		private void OnDisconnect()
		{
			Singleton<NetStatistic>.Instance.StopLog();
			this._coroutineManager.StopAllCoroutine();
			this.ClearSyncServerTimeState();
		}

		public void StateClear()
		{
			this._coroutineManager.StopAllCoroutine();
			this.ClearSyncServerTimeState();
		}

		[DebuggerHidden]
		private IEnumerator Run()
		{
			UnitsSnapReporter.<Run>c__Iterator1A0 <Run>c__Iterator1A = new UnitsSnapReporter.<Run>c__Iterator1A0();
			<Run>c__Iterator1A.<>f__this = this;
			return <Run>c__Iterator1A;
		}

		public bool SendUnitsSnap(int unitId, Vector3 pos, float rotate, byte isMoving, Vector3 speed)
		{
			return true;
		}

		private void OnP2C_Ping(MobaMessage msg)
		{
			P2CPing probufMsg = msg.GetProbufMsg<P2CPing>();
			long clientTime = probufMsg.clientTime;
			long serverTime = probufMsg.serverTime;
			this.P2C_Ping(clientTime, serverTime);
		}

		private void OnP2C_CheckPing(MobaMessage msg)
		{
			P2CCheckPing probufMsg = msg.GetProbufMsg<P2CCheckPing>();
			long serverTime = probufMsg.serverTime;
			this._client.SendPvpServerMsg(PvpCode.P2C_CheckPing, new object[]
			{
				SerializeHelper.Serialize<C2PPingInfo>(new C2PPingInfo
				{
					clientTime = TimeSpan.FromSeconds((double)Time.time).Ticks
				})
			});
		}

		private void P2C_Ping(long clientTicks, long serverTicks)
		{
			this.mIsSyncServerTime = true;
			long ticks = DateTime.Now.Ticks;
			this.networkDelayTicks = ticks - clientTicks;
			long num = serverTicks + this.networkDelayTicks / 2L;
			if (this.networkDelayTicks < this.mSyncServerDelayTicks)
			{
				this.mSyncServerDelayTicks = this.networkDelayTicks;
				this.mClientTicksWhenRecieveSync = ticks;
				this.mServerTicksWhenRecieveSync = num;
			}
			if (this.networkDelayTicks > 5000000L)
			{
				this.mSyncServerTimeCount--;
				if (this.mSyncServerTimeCount < 0)
				{
					this.mSyncServerTimeCount = 0;
				}
			}
		}

		private void ClearSyncServerTimeState()
		{
			this.mIsSyncServerTime = false;
			this.mSyncServerTimeCount = 0;
			this.mSyncServerDelayTicks = 1000000000L;
			this.mClientTicksWhenRecieveSync = 0L;
			this.mServerTicksWhenRecieveSync = 0L;
		}
	}
}
