using Assets.Scripts.Server;
using GameLogin.State;
using MobaMessageData;
using System;
using UnityEngine;

public class PlayerMng : IGlobalComServer
{
	private IVedioController player;

	private static PlayerMng m_instance;

	private bool enable;

	public static PlayerMng Instance
	{
		get
		{
			if (PlayerMng.m_instance == null)
			{
			}
			return PlayerMng.m_instance;
		}
	}

	public void OnAwake()
	{
		PlayerMng.m_instance = this;
	}

	public void OnStart()
	{
	}

	public void OnUpdate()
	{
	}

	public void OnDestroy()
	{
	}

	public void Enable(bool b)
	{
		if (this.enable != b)
		{
			this.enable = b;
			if (b)
			{
				this.Load();
			}
			else
			{
				this.Unload();
			}
		}
	}

	public void OnRestart()
	{
		if (this.enable)
		{
			this.Unload();
		}
	}

	public void OnApplicationQuit()
	{
		if (this.enable)
		{
			this.Unload();
		}
	}

	public void OnApplicationFocus(bool isFocus)
	{
	}

	public void OnApplicationPause(bool isPause)
	{
	}

	private void Load()
	{
		MobaMessageManager.RegistMessage((ClientMsg)25022, new MobaMessageFunc(this.OnMsg_SetActive));
		MobaMessageManager.RegistMessage((ClientMsg)25021, new MobaMessageFunc(this.OnMsg_Create));
		MobaMessageManager.RegistMessage((ClientMsg)25020, new MobaMessageFunc(this.OnMsg_SetResource));
		MobaMessageManager.RegistMessage((ClientMsg)25018, new MobaMessageFunc(this.OnMsg_Stop));
		MobaMessageManager.RegistMessage((ClientMsg)25019, new MobaMessageFunc(this.OnMsg_Loop));
		MobaMessageManager.RegistMessage((ClientMsg)25017, new MobaMessageFunc(this.OnMsg_Play));
	}

	private void Unload()
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)25022, new MobaMessageFunc(this.OnMsg_SetActive));
		MobaMessageManager.UnRegistMessage((ClientMsg)25021, new MobaMessageFunc(this.OnMsg_Create));
		MobaMessageManager.UnRegistMessage((ClientMsg)25020, new MobaMessageFunc(this.OnMsg_SetResource));
		MobaMessageManager.UnRegistMessage((ClientMsg)25018, new MobaMessageFunc(this.OnMsg_Stop));
		MobaMessageManager.UnRegistMessage((ClientMsg)25019, new MobaMessageFunc(this.OnMsg_Loop));
		MobaMessageManager.UnRegistMessage((ClientMsg)25017, new MobaMessageFunc(this.OnMsg_Play));
		if (this.player != null)
		{
			this.player.Unload();
			UnityEngine.Object.DestroyImmediate(this.player.Obj);
		}
	}

	private void OnMsg_SetActive(MobaMessage msg)
	{
		MsgData_Vedio_setActive msgData_Vedio_setActive = msg.Param as MsgData_Vedio_setActive;
		if (msgData_Vedio_setActive != null)
		{
			this.player.Enable = msgData_Vedio_setActive._active;
		}
	}

	private void OnMsg_Create(MobaMessage msg)
	{
		MsgData_Vedio_creatPlayer msgData_Vedio_creatPlayer = msg.Param as MsgData_Vedio_creatPlayer;
		if (msgData_Vedio_creatPlayer != null && this.player == null && msgData_Vedio_creatPlayer._create)
		{
			this.player = this.CreatePlayer(msgData_Vedio_creatPlayer._playerID);
		}
	}

	private void OnMsg_SetResource(MobaMessage msg)
	{
		MsgData_Vedio_setName msgData_Vedio_setName = msg.Param as MsgData_Vedio_setName;
		if (msgData_Vedio_setName != null)
		{
			this.player.Resource = msgData_Vedio_setName._name;
		}
	}

	private void OnMsg_Play(MobaMessage msg)
	{
		MsgData_Vedio_play msgData_Vedio_play = msg.Param as MsgData_Vedio_play;
		if (msgData_Vedio_play != null)
		{
			this.player.Play = true;
		}
	}

	private void OnMsg_Loop(MobaMessage msg)
	{
		MsgData_Vedio_loop msgData_Vedio_loop = msg.Param as MsgData_Vedio_loop;
		if (msgData_Vedio_loop != null)
		{
			this.player.Loop = msgData_Vedio_loop._loop;
		}
	}

	private void OnMsg_Stop(MobaMessage msg)
	{
		MsgData_Vedio_stop msgData_Vedio_stop = msg.Param as MsgData_Vedio_stop;
		if (msgData_Vedio_stop != null)
		{
			this.player.Play = false;
		}
	}

	private void callback_Start(IVedioController player)
	{
		MsgData_VedioCallback msgParam = new MsgData_VedioCallback(player.ID, player.Resource);
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25015, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private void callback_Stop(IVedioController player)
	{
		MsgData_VedioCallback msgParam = new MsgData_VedioCallback(player.ID, player.Resource);
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25016, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private void callback_Ready(IVedioController player)
	{
		MsgData_VedioCallback msgParam = new MsgData_VedioCallback(player.ID, player.Resource);
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)25014, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
	}

	private IVedioController CreatePlayer(int id)
	{
		IVedioController vedioController = this.CreatePlayer_mobile(id);
		vedioController.ID = id;
		vedioController.Callback_Ready = new Action<IVedioController>(this.callback_Ready);
		vedioController.Callback_Start = new Action<IVedioController>(this.callback_Start);
		vedioController.Callback_End = new Action<IVedioController>(this.callback_Stop);
		return vedioController;
	}

	private VedioController CreatePlayer_pc(int id)
	{
		UnityEngine.Object original = Resources.Load("Prefab/UI/Login/VedioController");
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.position = new Vector3(1000f, 0f, 0f);
		VedioController component = gameObject.GetComponent<VedioController>();
		gameObject.name = "VedioPlayer_" + id.ToString();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return component;
	}

	private VedioController2 CreatePlayer_mobile(int id)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefab/UI/Login/VedioController2")) as GameObject;
		VedioController2 component = gameObject.GetComponent<VedioController2>();
		gameObject.name = "VedioPlayer_" + id.ToString();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return component;
	}
}
