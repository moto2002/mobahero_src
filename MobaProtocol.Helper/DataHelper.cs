using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MobaProtocol.Helper
{
	public static class DataHelper
	{
		public static Dictionary<byte, object> ToHashtable(this AccountData data)
		{
			return new Dictionary<byte, object>
			{
				{
					71,
					data.AccountId
				},
				{
					72,
					data.UserName
				},
				{
					74,
					data.Password
				},
				{
					73,
					data.Mail
				},
				{
					77,
					data.UserType
				},
				{
					75,
					data.DeviceType
				},
				{
					76,
					data.DeviceToken
				},
				{
					53,
					data.ServerName
				},
				{
					78,
					data.ChannelId
				}
			};
		}

		public static AccountData ToAccountData(object serObj)
		{
			AccountData result;
			if (serObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)serObj;
				result = new AccountData
				{
					AccountId = (string)dictionary[71],
					UserName = (string)dictionary[72],
					Password = (string)dictionary[74],
					Mail = (string)dictionary[73],
					UserType = (int)dictionary[77],
					DeviceType = (int)dictionary[75],
					DeviceToken = (string)dictionary[76],
					ServerName = (int)dictionary[53],
					ChannelId = (string)dictionary[78]
				};
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static Dictionary<byte, object> ToHashtable(this UserData data)
		{
			return new Dictionary<byte, object>
			{
				{
					57,
					data.UserId
				},
				{
					59,
					data.NickName
				},
				{
					58,
					data.Avatar
				},
				{
					65,
					data.Status
				},
				{
					61,
					data.Money
				},
				{
					62,
					data.Diamonds
				},
				{
					67,
					data.Exp
				},
				{
					68,
					data.RankId
				},
				{
					64,
					data.Achievements
				},
				{
					69,
					data.HasBuy
				},
				{
					70,
					data.VIP
				},
				{
					83,
					data.SummonerId
				},
				{
					50,
					data.ServerId
				}
			};
		}

		public static UserData ToUserData(object serObj)
		{
			UserData result;
			if (serObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)serObj;
				result = new UserData
				{
					UserId = (string)dictionary[57],
					NickName = (string)dictionary[59],
					Avatar = (int)dictionary[58],
					Status = (UserStatus)dictionary[65],
					Money = (long)dictionary[61],
					Diamonds = (long)dictionary[62],
					Exp = (long)dictionary[67],
					RankId = (long)dictionary[68],
					Achievements = (int)dictionary[64],
					HasBuy = (bool)dictionary[69],
					VIP = (int)dictionary[70],
					SummonerId = (long)dictionary[83],
					ServerId = (int)dictionary[50]
				};
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static Dictionary<byte, object> ToHashtable(ServerInfo data)
		{
			return new Dictionary<byte, object>
			{
				{
					50,
					data.serverid
				},
				{
					51,
					data.serverip
				},
				{
					52,
					data.serverport
				},
				{
					45,
					data.udpaddress
				},
				{
					46,
					data.tcpaddress
				},
				{
					53,
					data.servername
				},
				{
					54,
					data.loadlevel
				},
				{
					55,
					data.peercount
				},
				{
					56,
					data.usercount
				},
				{
					243,
					data.serverState
				},
				{
					244,
					data.serverImage
				},
				{
					245,
					data.areaId
				},
				{
					30,
					data.appVer
				},
				{
					31,
					data.bindataVer
				},
				{
					33,
					data.bindataURL
				},
				{
					32,
					data.appUpdateURLAndroid + "|" + data.appUpdateURLIOS
				},
				{
					229,
					data.bindataMD5
				},
				{
					47,
					data.bindataZipSize
				},
				{
					42,
					data.updateContentURL
				}
			};
		}

		public static ServerInfo ToServerInfo(object serObj)
		{
			ServerInfo result;
			if (serObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)serObj;
				ServerInfo serverInfo = new ServerInfo();
				serverInfo.serverid = (string)dictionary[50];
				serverInfo.serverip = (string)dictionary[51];
				serverInfo.serverport = (int)dictionary[52];
				serverInfo.udpaddress = (string)dictionary[45];
				serverInfo.tcpaddress = (string)dictionary[46];
				serverInfo.servername = (string)dictionary[53];
				if (dictionary.ContainsKey(54))
				{
					serverInfo.loadlevel = (long)dictionary[54];
				}
				if (dictionary.ContainsKey(55))
				{
					serverInfo.peercount = (long)dictionary[55];
				}
				if (dictionary.ContainsKey(56))
				{
					serverInfo.usercount = (long)dictionary[56];
				}
				serverInfo.serverState = (byte)dictionary[243];
				serverInfo.serverImage = (string)dictionary[244];
				serverInfo.areaId = (int)dictionary[245];
				serverInfo.appVer = (string)dictionary[30];
				serverInfo.bindataVer = (string)dictionary[31];
				serverInfo.bindataURL = (string)dictionary[33];
				serverInfo.bindataMD5 = (string)dictionary[229];
				serverInfo.bindataZipSize = (int)dictionary[47];
				serverInfo.updateContentURL = (string)dictionary[42];
				string text = (string)dictionary[32];
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						'|'
					});
					if (array.Length > 1)
					{
						serverInfo.appUpdateURLAndroid = array[0];
						serverInfo.appUpdateURLIOS = array[1];
					}
				}
				result = serverInfo;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static List<ServerInfo> ToServerList(object listObj)
		{
			List<ServerInfo> result;
			if (listObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = listObj as Dictionary<byte, object>;
				if (dictionary != null && dictionary.Count > 0)
				{
					List<ServerInfo> list = new List<ServerInfo>();
					for (int i = 0; i < dictionary.Count; i++)
					{
						Dictionary<byte, object> serObj = dictionary.ElementAt(i).Value as Dictionary<byte, object>;
						ServerInfo serverInfo = DataHelper.ToServerInfo(serObj);
						if (serverInfo != null)
						{
							list.Add(serverInfo);
						}
					}
					result = list;
				}
				else
				{
					result = null;
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static List<TValue> DicToValueList<TKey, TValue>(Dictionary<TKey, TValue> dic)
		{
			List<TValue> list = new List<TValue>();
			foreach (KeyValuePair<TKey, TValue> current in dic)
			{
				list.Add(current.Value);
			}
			return list;
		}

		public static List<TKey> DicToKeyList<TKey, TValue>(Dictionary<TKey, TValue> dic)
		{
			List<TKey> list = new List<TKey>();
			foreach (KeyValuePair<TKey, TValue> current in dic)
			{
				list.Add(current.Key);
			}
			return list;
		}

		public static Dictionary<byte, object> ToHashtable(this List<ServerInfo> listdata)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			for (int i = 0; i < listdata.Count; i++)
			{
				Dictionary<byte, object> value = DataHelper.ToHashtable(listdata[i]);
				dictionary.Add((byte)i, value);
			}
			return dictionary;
		}

		public static Dictionary<byte, object> ToHashtable(ClientData data)
		{
			return new Dictionary<byte, object>
			{
				{
					30,
					data.AppVersion
				},
				{
					31,
					data.AppResVersion
				},
				{
					32,
					data.AppUpgradeUrl
				},
				{
					75,
					data.DeviceType
				},
				{
					33,
					data.ResUpgradeList
				}
			};
		}

		public static ClientData ToClientData(object serObj)
		{
			ClientData result;
			if (serObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)serObj;
				result = new ClientData
				{
					AppVersion = (string)dictionary[30],
					AppResVersion = (string)dictionary[31],
					AppUpgradeUrl = (string)dictionary[32],
					DeviceType = (int)dictionary[75],
					ResUpgradeList = (List<ResourceData>)dictionary[33]
				};
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
