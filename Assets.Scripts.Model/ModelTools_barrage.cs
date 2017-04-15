using Com.Game.Data;
using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
	public static class ModelTools_barrage
	{
		public static BarrageModelData Get_BarrageModelData(this ModelManager mmng)
		{
			BarrageModelData result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_barrage))
			{
				result = mmng.GetData<BarrageModelData>(EModelType.Model_barrage);
			}
			return result;
		}

		public static Dictionary<string, SysBulletScreenVo> Get_BarrageCfgData_X(this ModelManager mmng)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			return barrageModelData.dataCfg;
		}

		public static SysBulletScreenVo Get_BarrageCfgDataById_X(this ModelManager mmng, string id)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			if (barrageModelData.dataCfg.ContainsKey(id))
			{
				return barrageModelData.dataCfg[id];
			}
			return barrageModelData.dataCfg["1"];
		}

		public static List<SysBulletScreenVo> Get_ClassifiedBarrageCfgData_X(this ModelManager mmng, int index)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			return barrageModelData.dataClassify[index];
		}

		public static SysBulletScreenFormatVo Get_BarrageFormat_X(this ModelManager mmng, string id)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			return barrageModelData.GetBarrageFormatById(id);
		}

		public static Color32 Get_ColorByString_X(this ModelManager mmng, string str)
		{
			Color32 result;
			if (str == "[]")
			{
				result = new Color32(255, 255, 255, 200);
				return result;
			}
			string[] array = str.Split(new char[]
			{
				','
			});
			if (array.Length != 4)
			{
				result = new Color32(255, 255, 255, 200);
				return result;
			}
			byte[] array2 = Array.ConvertAll<string, byte>(array, (string s) => Convert.ToByte(s, 10));
			result = new Color32(array2[0], array2[1], array2[2], array2[3]);
			return result;
		}

		public static string Get_UserBarrageColor_X(this ModelManager mmng)
		{
			return "255,255,255,200";
		}

		public static bool Get_IsBarrageLocked_X(this ModelManager mmng, SysBulletScreenVo item)
		{
			if (item == null)
			{
				return false;
			}
			if (item.type == "1")
			{
				return false;
			}
			if (item.type == "2")
			{
				string[] array = ModelManager.Instance.Get_userData_X().captionIds.Split(new char[]
				{
					','
				});
				if (array == null || array.Length == 0)
				{
					return true;
				}
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string a = array2[i];
					if (a == item.id.ToString())
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				if (!item.type.StartsWith("3"))
				{
					return false;
				}
				string[] array3 = item.type.Split(new char[]
				{
					'|'
				});
				if (array3.Length < 2)
				{
					return false;
				}
				int num = int.Parse(array3[1]);
				return num > ModelManager.Instance.Get_userData_X().VIP;
			}
		}

		public static void Send_C2PCaption(this ModelManager mmng, string msg)
		{
			Singleton<BarragePlayerCtrl>.Instance.LaunchMyBarrage(msg);
			SendMsgManager.Instance.SendPvpMsgBase<C2PCaption>(PvpCode.C2P_Caption, new C2PCaption
			{
				captionStr = msg
			});
		}

		public static void Send_C2PCaption_2GameServer(this ModelManager mmng, string msg)
		{
			Singleton<BarragePlayerCtrl>.Instance.LaunchMyBarrage(msg);
			Singleton<PvpManager>.Instance.SendLobbyMsgEx(LobbyCode.C2L_CaptionLobby, null, new object[]
			{
				msg
			});
		}

		public static void BarrageQueue_Enqueue(this ModelManager mmng, string str)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			barrageModelData.mQueue.Enqueue(str);
		}

		public static string BarrageQueue_Dequeue(this ModelManager mmng)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			if (barrageModelData.mQueue.Count <= 0)
			{
				return string.Empty;
			}
			return barrageModelData.mQueue.Dequeue();
		}

		public static void BarrageQueue_Clear(this ModelManager mmng)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			barrageModelData.mQueue.Clear();
		}

		public static int Get_BarrageQueueCount_X(this ModelManager mmng)
		{
			BarrageModelData barrageModelData = mmng.Get_BarrageModelData();
			return barrageModelData.mQueue.Count;
		}
	}
}
