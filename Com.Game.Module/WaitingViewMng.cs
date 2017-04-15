using Assets.Scripts.Server;
using MobaProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class WaitingViewMng : IGlobalComServer
	{
		private Dictionary<string, WaitInfo> dicWaitMsg;

		private string curText = string.Empty;

		private bool delayShow = true;

		private bool enable;

		private bool normal = true;

		private List<string> removeKey;

		private static WaitingViewMng instance;

		public static WaitingViewMng Instance
		{
			get
			{
				if (WaitingViewMng.instance == null)
				{
					throw new Exception("WaitingViewMng 初始化顺序有问题");
				}
				return WaitingViewMng.instance;
			}
		}

		public void OnAwake()
		{
			WaitingViewMng.instance = this;
			this.removeKey = new List<string>();
			this.Regist();
			this.dicWaitMsg = new Dictionary<string, WaitInfo>();
		}

		public void OnStart()
		{
		}

		public void OnUpdate()
		{
			if (!this.enable)
			{
				return;
			}
			this.removeKey.Clear();
			Dictionary<string, WaitInfo>.Enumerator enumerator = this.dicWaitMsg.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, WaitInfo> current = enumerator.Current;
				if (current.Value)
				{
					KeyValuePair<string, WaitInfo> current2 = enumerator.Current;
					current2.Value.OnUpdate();
				}
				else
				{
					List<string> arg_70_0 = this.removeKey;
					KeyValuePair<string, WaitInfo> current3 = enumerator.Current;
					arg_70_0.Add(current3.Key);
				}
			}
			for (int i = 0; i < this.removeKey.Count; i++)
			{
				this.Remove(this.removeKey[i]);
			}
			if (this.dicWaitMsg.Count == 0 && Time.frameCount % 5 == 0)
			{
				this.OpenWaitingView(false);
			}
		}

		public void OnDestroy()
		{
			this.RemoveAll();
			this.Unregist();
			WaitingViewMng.instance = null;
		}

		public void Enable(bool b)
		{
			this.enable = b;
			if (!this.enable)
			{
				this.RemoveAll();
			}
		}

		public void OnRestart()
		{
			if (Singleton<NewWaitingView>.Instance != null)
			{
				Singleton<NewWaitingView>.Instance.IsOpened = false;
			}
			this.Enable(false);
		}

		public void OnApplicationQuit()
		{
		}

		public void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnApplicationPause(bool isPause)
		{
		}

		public void Waiting(string key, string text = "waiting...", bool noraml = true, float time = 15f, bool delayShow = true)
		{
			this.Add(key, MobaMessageType.Client, 23002, text, this.normal, time, delayShow);
		}

		public void Waiting(MobaMessageType type, int code, string text = "waiting server...", bool normal = true, float time = 15f, bool delayShow = true)
		{
			string key = this.FormatMsg(type, code);
			this.Add(key, type, code, text, normal, time, delayShow);
		}

		private void Regist()
		{
			MobaMessageManager.RegistMessage(ClientNet.Disconnected_game, new MobaMessageFunc(this.OnDisconnected_game));
			MobaMessageManager.RegistMessage(ClientNet.Disconnected_master, new MobaMessageFunc(this.OnDisconnected_master));
		}

		private void Unregist()
		{
			MobaMessageManager.UnRegistMessage(ClientNet.Disconnected_game, new MobaMessageFunc(this.OnDisconnected_game));
			MobaMessageManager.UnRegistMessage(ClientNet.Disconnected_master, new MobaMessageFunc(this.OnDisconnected_master));
		}

		private void Add(string key, MobaMessageType type, int code, string text, bool normal, float waitTime, bool delayShow)
		{
			if (!this.dicWaitMsg.ContainsKey(key))
			{
				this.dicWaitMsg.Add(key, new WaitInfo(type, code, key, waitTime));
			}
			else
			{
				Dictionary<string, WaitInfo> dictionary;
				Dictionary<string, WaitInfo> expr_32 = dictionary = this.dicWaitMsg;
				WaitInfo w = dictionary[key];
				expr_32[key] = ++w;
			}
			this.curText = text;
			this.delayShow = delayShow;
			this.normal = normal;
			this.OpenWaitingView(true);
		}

		private void Remove(string key)
		{
			if (this.dicWaitMsg.ContainsKey(key))
			{
				this.dicWaitMsg[key].OnDestroy();
				this.dicWaitMsg.Remove(key);
			}
		}

		private void RemoveAll()
		{
			foreach (KeyValuePair<string, WaitInfo> current in this.dicWaitMsg)
			{
				current.Value.OnDestroy();
			}
			this.dicWaitMsg.Clear();
		}

		private void OpenWaitingView(bool b)
		{
			if (b)
			{
				if (!Singleton<NewWaitingView>.Instance.IsOpened)
				{
					CtrlManager.OpenWindow(WindowID.NewWaitingView, null);
				}
				this.RefreshWaitingViewText();
			}
			else if (Singleton<NewWaitingView>.Instance.IsOpened)
			{
				CtrlManager.CloseWindow(WindowID.NewWaitingView);
			}
		}

		private void OnDisconnected_game(MobaMessage msg)
		{
			this.RemoveAll();
		}

		private void OnDisconnected_master(MobaMessage msg)
		{
			this.RemoveAll();
		}

		private void RefreshWaitingViewText()
		{
			MobaMessageManagerTools.SendClientMsg(ClientC2V.WaitingView_text, this.curText, false);
			MobaMessageManagerTools.SendClientMsg(ClientC2V.WaitingView_show, this.delayShow, false);
			MobaMessageManagerTools.SendClientMsg(ClientC2V.WaitingView_normal, this.normal, false);
		}

		private string FormatMsg(MobaMessageType type, int code)
		{
			switch (type)
			{
			case MobaMessageType.MasterCode:
				return type + "_" + (MobaMasterCode)code;
			case MobaMessageType.GameCode:
				return type + "_" + (MobaGameCode)code;
			case MobaMessageType.PvpCode:
				return type + "_" + (PvpCode)code;
			default:
				if (type != MobaMessageType.Client)
				{
					return type + "_" + code;
				}
				return type + "_" + (ClientMsg)code;
			}
		}
	}
}
