using Com.Game.Module;
using MobaClient;
using MobaHeros.Pvp;
using System;
using UnityEngine;

public class TestPvpPeer : MonoBehaviour
{
	public bool Enable;

	public int IncomingLag;

	public int IncmingJitter = 1;

	public int IncomingLossPercentage = 1;

	public int OutgogingLag;

	public int OutgoingJitter;

	public int OutgoingLossPercentage;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void OnGUI()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp && GUILayout.Button("根据脚本变量应用参数", new GUILayoutOption[0]))
		{
			this.test();
		}
	}

	public void test()
	{
		MobaPvpServerClientPeer pvpserver_peer = NetWorkHelper.Instance.client.pvpserver_peer;
		if (pvpserver_peer == null)
		{
			return;
		}
		if (this.Enable)
		{
			pvpserver_peer.IsSimulationEnabled = true;
			pvpserver_peer.NetworkSimulationSettings.IncomingLag = this.IncomingLag;
			pvpserver_peer.NetworkSimulationSettings.IncomingJitter = this.IncmingJitter;
			pvpserver_peer.NetworkSimulationSettings.IncomingLossPercentage = this.IncomingLossPercentage;
			pvpserver_peer.NetworkSimulationSettings.OutgoingLag = this.OutgogingLag;
			pvpserver_peer.NetworkSimulationSettings.OutgoingJitter = this.OutgoingJitter;
			pvpserver_peer.NetworkSimulationSettings.OutgoingLossPercentage = this.OutgoingLossPercentage;
		}
		else
		{
			pvpserver_peer.IsSimulationEnabled = false;
		}
	}
}
