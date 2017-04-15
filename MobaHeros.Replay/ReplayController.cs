using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaHeros.Pvp;
using MobaHeros.Pvp.State;
using MobaProtocol;
using MobaProtocol.Data;
using Newbie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Replay
{
	public class ReplayController : BaseGameModule
	{
		private enum State
		{
			None,
			Replaying,
			Recording
		}

		private ReplayMessageIO _replayMessageIO;

		private ReplayMetaInfo.ReplayMetaEntry _curRecordMetaEntry;

		private ReplayMetaInfo _cachedMetaInfo;

		private bool _isMetaInfoInitSuc;

		private float _curLoadReplayFileRatio = 1f;

		private float _replayStartTime;

		private float _replayOffsetTime;

		private ReplayController.State _state;

		private bool _inGame;

		private Task _dispatchTask;

		private static int _nextReplayId;

		private int _curReplayId;

		private ReplayMessage _savedReconnectSyncMsg;

		private bool _isSaveReconnectSyncMsgSuc;

		public bool IsInitMetaInfoSuc
		{
			get
			{
				return this._isMetaInfoInitSuc;
			}
		}

		public bool IsRecording
		{
			get
			{
				return this._state == ReplayController.State.Recording && this._inGame;
			}
		}

		public bool IsReplaying
		{
			get
			{
				return this._state == ReplayController.State.Replaying && this._inGame;
			}
		}

		public bool IsReplayStart
		{
			get
			{
				return this._state == ReplayController.State.Replaying;
			}
		}

		public string CurRecordExtra
		{
			get
			{
				return (this._curRecordMetaEntry == null) ? null : this._curRecordMetaEntry.Extra;
			}
			set
			{
				if (this._curRecordMetaEntry == null)
				{
					throw new InvalidOperationException(string.Empty);
				}
				this._curRecordMetaEntry.Extra = value;
			}
		}

		public float CurLoadReplayFileRatio
		{
			get
			{
				return this._curLoadReplayFileRatio;
			}
		}

		public static string ReplayStoreDir
		{
			get
			{
				return Application.persistentDataPath + "/replay";
			}
		}

		public static string MetaFilePath
		{
			get
			{
				return ReplayController.ReplayStoreDir + "/replay.meta";
			}
		}

		public void DoInition()
		{
			if (!Directory.Exists(ReplayController.ReplayStoreDir))
			{
				Directory.CreateDirectory(ReplayController.ReplayStoreDir);
			}
			ReplayController._nextReplayId = PlayerPrefs.GetInt("nextreplayid", 1);
			this._isMetaInfoInitSuc = this.InitReplayMetaInfo();
		}

		private bool InitReplayMetaInfo()
		{
			return this.InitReplayMetaFile() && this.CreateMetaInfoFromFile();
		}

		private bool InitReplayMetaFile()
		{
			bool result;
			try
			{
				if (!File.Exists(ReplayController.MetaFilePath))
				{
					using (File.Create(ReplayController.MetaFilePath))
					{
					}
				}
				result = true;
			}
			catch (Exception var_1_38)
			{
				result = false;
			}
			return result;
		}

		private bool CreateMetaInfoFromFile()
		{
			bool result;
			try
			{
				using (FileStream fileStream = File.OpenRead(ReplayController.MetaFilePath))
				{
					int num = (int)fileStream.Length;
					if (num > 0)
					{
						byte[] array = new byte[num];
						int num2 = fileStream.Read(array, 0, num);
						if (num2 != num)
						{
							result = false;
						}
						else
						{
							this._cachedMetaInfo = (SerializerUtils.binaryDeserialize(array) as ReplayMetaInfo);
							if (this._cachedMetaInfo == null)
							{
								result = false;
							}
							else
							{
								this._cachedMetaInfo.ReplayEntryList.ForEach(delegate(ReplayMetaInfo.ReplayMetaEntry x)
								{
									x.Decode();
								});
								result = true;
							}
						}
					}
					else
					{
						this._cachedMetaInfo = new ReplayMetaInfo();
						result = true;
					}
				}
			}
			catch (Exception var_4_BD)
			{
				result = false;
			}
			return result;
		}

		public void ResetCtrlState()
		{
			this._state = ReplayController.State.None;
			this._isSaveReconnectSyncMsgSuc = false;
			this._savedReconnectSyncMsg.code = PvpCode.C2P_QueryInFightInfo;
			this._savedReconnectSyncMsg.time = 0.001f;
			this._savedReconnectSyncMsg.param = null;
		}

		private bool IsSavedReconnectSyncMsgValid()
		{
			return this._isSaveReconnectSyncMsgSuc;
		}

		private ReplayMessage GetSavedReconnectSyncMsg()
		{
			return this._savedReconnectSyncMsg;
		}

		public void SaveReconnectSyncMsgForRecord(object inParam)
		{
			this._isSaveReconnectSyncMsgSuc = false;
			this._savedReconnectSyncMsg.code = PvpCode.C2P_QueryInFightInfo;
			this._savedReconnectSyncMsg.time = 0.001f;
			this._savedReconnectSyncMsg.param = null;
			if (inParam == null)
			{
				return;
			}
			byte[] array = null;
			if (inParam is byte[])
			{
				array = (inParam as byte[]);
			}
			else
			{
				OperationResponse operationResponse = inParam as OperationResponse;
				if (operationResponse != null && operationResponse.Parameters != null && operationResponse.Parameters.ContainsKey(0))
				{
					array = (operationResponse.Parameters[0] as byte[]);
				}
			}
			if (array == null)
			{
				return;
			}
			this._savedReconnectSyncMsg.code = PvpCode.C2P_QueryInFightInfo;
			this._savedReconnectSyncMsg.time = 0.001f;
			this._savedReconnectSyncMsg.param = array;
			this._isSaveReconnectSyncMsgSuc = true;
		}

		public void StartRecord()
		{
			this._state = ReplayController.State.Recording;
			this._curReplayId = ReplayController.GetReplayId();
			this._curRecordMetaEntry = new ReplayMetaInfo.ReplayMetaEntry
			{
				ReplayId = this._curReplayId,
				ReplayFile = this._curReplayId + ".rep"
			};
			this._replayMessageIO = new ReplayMessageIO(ReplayController.ReplayStoreDir + "/" + this._curRecordMetaEntry.ReplayFile);
			if (this.IsSavedReconnectSyncMsgValid())
			{
				this._replayMessageIO.Push(this.GetSavedReconnectSyncMsg());
			}
		}

		public void EndRecord(Action<Exception> callback)
		{
			this.Save(callback);
			this._state = ReplayController.State.None;
		}

		public void StartReplay(int replayId)
		{
			this._state = ReplayController.State.None;
			this._curLoadReplayFileRatio = 0f;
			new Task(this.LoadByReplayId(replayId), true);
		}

		public void EndReplay()
		{
			this._state = ReplayController.State.None;
			this._inGame = false;
			Task.Clear(ref this._dispatchTask);
			PvpUtils.GoHome();
		}

		public float GetReplayTime()
		{
			return Time.time - this._replayStartTime;
		}

		public bool HandlePvpMsg(PvpCode msgID, object msgParam)
		{
			if (this.IsReplaying)
			{
				return true;
			}
			if (this.IsRecording && msgID != PvpCode.C2P_Ping && msgID != PvpCode.P2C_TipMessage)
			{
				OperationResponse operationResponse = msgParam as OperationResponse;
				ReplayMessage msg;
				if (operationResponse != null)
				{
					msg = new ReplayMessage
					{
						time = ReplayController.GetRecordTimeOffset(),
						code = msgID,
						param = operationResponse.Parameters[0] as byte[]
					};
				}
				else
				{
					msg = new ReplayMessage
					{
						time = ReplayController.GetRecordTimeOffset(),
						code = msgID,
						param = msgParam as byte[]
					};
				}
				this._replayMessageIO.Push(msg);
			}
			return false;
		}

		public override void OnGameStateChange(GameState oldState, GameState newState)
		{
			if (newState == GameState.Game_Playing)
			{
				this.OnGameStart();
			}
			else if (newState == GameState.Game_Over)
			{
				this.OnGameEnd();
			}
		}

		private void OnGameStart()
		{
			this._inGame = true;
			if (this.IsReplaying)
			{
				Task.Clear(ref this._dispatchTask);
				this._dispatchTask = new Task(this.DispatchLoop(), true);
				this._replayStartTime = Time.time;
				this._replayOffsetTime = this.GetReplayOffsetTime();
			}
			else if (this.IsRecording)
			{
				this.UpdateCurMetaInfo();
			}
			else if (!NewbieManager.Instance.IsInNewbieGuide())
			{
				this.StartRecord();
				this.UpdateCurMetaInfo();
			}
		}

		private void OnGameEnd()
		{
			if (this._state == ReplayController.State.Replaying)
			{
				this._state = ReplayController.State.None;
			}
			this._inGame = false;
			Task.Clear(ref this._dispatchTask);
		}

		private static float GetRecordTimeOffset()
		{
			DateTime? gameStartTime = Singleton<PvpManager>.Instance.GameStartTime;
			if (!gameStartTime.HasValue)
			{
				return 0f;
			}
			return (float)(DateTime.Now - gameStartTime.Value).TotalSeconds;
		}

		private void BeginLoad()
		{
			this.InitReplayContext();
			PvpStateManager.Instance.ChangeState(new PvpStateLoad());
			Singleton<PvpManager>.Instance.LoadPvpSceneBegin();
		}

		private void InitReplayContext()
		{
			Singleton<PvpManager>.Instance.SetBattleInfo(this._curRecordMetaEntry.BattleId, this._curRecordMetaEntry.JoinType);
			Singleton<PvpManager>.Instance.SetRoomInfo(this._curRecordMetaEntry.RoomId, this._curRecordMetaEntry.MyNewId, this._curRecordMetaEntry.AllPlayers, null);
		}

		private float GetReplayOffsetTime()
		{
			if (this._replayMessageIO != null && this._replayMessageIO.CurLoadMsgs != null && this._replayMessageIO.CurLoadMsgs.Count > 0)
			{
				for (int i = 0; i < this._replayMessageIO.CurLoadMsgs.Count; i++)
				{
					LoadReplayMessage loadReplayMessage = this._replayMessageIO.CurLoadMsgs[i];
					if (loadReplayMessage.code != PvpCode.C2P_QueryInFightInfo)
					{
						return loadReplayMessage.time;
					}
				}
			}
			return 0f;
		}

		[DebuggerHidden]
		private IEnumerator DispatchLoop()
		{
			ReplayController.<DispatchLoop>c__Iterator196 <DispatchLoop>c__Iterator = new ReplayController.<DispatchLoop>c__Iterator196();
			<DispatchLoop>c__Iterator.<>f__this = this;
			return <DispatchLoop>c__Iterator;
		}

		private void UpdateCurMetaInfo()
		{
			this._curRecordMetaEntry.RoomId = Singleton<PvpManager>.Instance.RoomInfo.RoomId;
			this._curRecordMetaEntry.MyNewId = Singleton<PvpManager>.Instance.MyLobbyUserId;
			this._curRecordMetaEntry.AllPlayers = Singleton<PvpManager>.Instance.RoomInfo.PvpPlayers.ToArray<ReadyPlayerSampleInfo>();
			int battleId = 0;
			if (int.TryParse(LevelManager.CurLevelId, out battleId))
			{
				this._curRecordMetaEntry.BattleId = battleId;
			}
			else
			{
				this._curRecordMetaEntry.BattleId = 80001;
			}
			this._curRecordMetaEntry.JoinType = PvpJoinType.Single;
		}

		private void SaveCurMetaEntry()
		{
			if (this._curRecordMetaEntry != null)
			{
				DateTime? gameStartTime = Singleton<PvpManager>.Instance.GameStartTime;
				if (gameStartTime.HasValue)
				{
					this._curRecordMetaEntry.Time = gameStartTime.Value;
					this._curRecordMetaEntry.GameDuration = DateTime.Now - gameStartTime.Value;
				}
				this._curRecordMetaEntry.Encode();
				this.GetReplayMetaInfo().ReplayEntryList.Add(this._curRecordMetaEntry);
				this._curRecordMetaEntry = null;
			}
			this.FlushMetaInfoToDisk();
		}

		private void FlushMetaInfoToDisk()
		{
			ReplayMetaInfo replayMetaInfo = this.GetReplayMetaInfo();
			SerializerUtils.binarySerialize(ReplayController.MetaFilePath, replayMetaInfo);
		}

		private void Save(Action<Exception> callback)
		{
			this._replayMessageIO.Save(delegate(Exception exception)
			{
				if (exception == null)
				{
					this.SaveCurMetaEntry();
				}
				if (callback != null)
				{
					callback(exception);
				}
			});
		}

		[DebuggerHidden]
		private IEnumerator LoadByReplayId(int replayid)
		{
			ReplayController.<LoadByReplayId>c__Iterator197 <LoadByReplayId>c__Iterator = new ReplayController.<LoadByReplayId>c__Iterator197();
			<LoadByReplayId>c__Iterator.replayid = replayid;
			<LoadByReplayId>c__Iterator.<$>replayid = replayid;
			<LoadByReplayId>c__Iterator.<>f__this = this;
			return <LoadByReplayId>c__Iterator;
		}

		private void LoadReplayFileFailHint()
		{
			this._curLoadReplayFileRatio = 1f;
			CtrlManager.ShowMsgBox("加载失败", "本地文件错误，尝试重新登录或清除软件缓存", delegate
			{
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}

		private T GetProbufMsg<T>(object param) where T : class
		{
			byte[] array = param as byte[];
			if (array == null)
			{
				return (T)((object)null);
			}
			return SerializeHelper.Deserialize<T>(array);
		}

		public ReplayMetaInfo.ReplayMetaEntry FindByReplayId(int replayid)
		{
			return this.GetReplayMetaInfo().ReplayEntryList.Find((ReplayMetaInfo.ReplayMetaEntry x) => x.ReplayId == replayid);
		}

		public bool DeleteFirstRecord()
		{
			if (!this._isMetaInfoInitSuc)
			{
				return false;
			}
			List<ReplayMetaInfo.ReplayMetaEntry> replayEntryList = this.GetReplayMetaInfo().ReplayEntryList;
			if (replayEntryList == null || replayEntryList.Count < 1)
			{
				return false;
			}
			ReplayMetaInfo.ReplayMetaEntry replayMetaEntry = replayEntryList[0];
			for (int i = 1; i < replayEntryList.Count; i++)
			{
				if (replayEntryList[i].Time < replayMetaEntry.Time)
				{
					replayMetaEntry = replayEntryList[i];
				}
			}
			return this.DeleteFileByReplayId(replayMetaEntry.ReplayId);
		}

		public bool DeleteFileByReplayId(int replayid)
		{
			if (!this._isMetaInfoInitSuc)
			{
				return false;
			}
			ReplayMetaInfo.ReplayMetaEntry replayMetaEntry = this.FindByReplayId(replayid);
			if (replayMetaEntry != null)
			{
				try
				{
					string path = ReplayController.ReplayStoreDir + "/" + replayMetaEntry.ReplayFile;
					if (File.Exists(path))
					{
						File.Delete(path);
					}
					this.GetReplayMetaInfo().ReplayEntryList.RemoveAll((ReplayMetaInfo.ReplayMetaEntry x) => x.ReplayId == replayid);
					this.FlushMetaInfoToDisk();
					bool result = true;
					return result;
				}
				catch (Exception var_2_84)
				{
					bool result = false;
					return result;
				}
				return false;
			}
			return false;
		}

		public ReplayMetaInfo GetReplayMetaInfo()
		{
			return this._cachedMetaInfo;
		}

		private static int GetReplayId()
		{
			int nextReplayId = ReplayController._nextReplayId;
			PlayerPrefs.SetInt("nextreplayid", ++ReplayController._nextReplayId);
			PlayerPrefs.Save();
			return nextReplayId;
		}
	}
}
