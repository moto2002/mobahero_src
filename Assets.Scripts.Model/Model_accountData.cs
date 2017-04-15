using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
	internal class Model_accountData : ModelBase<AccountModelData>
	{
		public Model_accountData()
		{
			base.Init(EModelType.Model_accountData);
			base.Data = new AccountModelData();
			AccountModelData accountModelData = base.Data as AccountModelData;
			accountModelData.loginDataList = this.GetLoginList();
		}

		public override void RegisterMsgHandler()
		{
			MVC_MessageManager.AddListener_model(MobaMasterCode.Login, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaMasterCode.GuestUpgrade, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaMasterCode.Register, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaMasterCode.LoginByPlatformUid, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.AddListener_model(MobaMasterCode.LoginByChannelId, new MobaMessageFunc(this.OnGetMsg));
		}

		public override void UnRegisterMsgHandler()
		{
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.Login, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.GuestUpgrade, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.Register, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.LoginByPlatformUid, new MobaMessageFunc(this.OnGetMsg));
			MVC_MessageManager.RemoveListener_model(MobaMasterCode.LoginByChannelId, new MobaMessageFunc(this.OnGetMsg));
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			MobaMasterCode mobaMasterCode = MVC_MessageManager.NotifyModel_to_Master((ClientMsg)msg.ID);
			MobaMasterCode mobaMasterCode2 = mobaMasterCode;
			switch (mobaMasterCode2)
			{
			case MobaMasterCode.Login:
				this.OnGetMsg_MasterCode_Login(msg);
				goto IL_82;
			case MobaMasterCode.HulaiLogin:
			case MobaMasterCode.LoginOut:
			case MobaMasterCode.GuestUpgradeToOfficial:
				IL_2E:
				if (mobaMasterCode2 == MobaMasterCode.Register)
				{
					this.OnGetMsg_MasterCode_Register(msg);
					goto IL_82;
				}
				if (mobaMasterCode2 != MobaMasterCode.GuestUpgrade)
				{
					goto IL_82;
				}
				this.OnGetMsg_MasterCode_GuestUpgrade(msg);
				goto IL_82;
			case MobaMasterCode.LoginByPlatformUid:
				this.OnGetMsg_MasterCode_LoginByPlatformUid(msg);
				goto IL_82;
			case MobaMasterCode.LoginByChannelId:
				this.OnGetMsg_MasterCode_LoginByPlatformUid(msg);
				goto IL_82;
			}
			goto IL_2E;
			IL_82:
			base.TriggerListners();
		}

		private void OnGetMsg_MasterCode_Register(MobaMessage msg)
		{
			base.LastError = 505;
			AccountModelData accountModelData = base.Data as AccountModelData;
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					base.LastError = (int)operationResponse.Parameters[1];
					base.Valid = true;
					MobaErrorCode lastError = (MobaErrorCode)base.LastError;
					if (lastError == MobaErrorCode.Ok)
					{
						AccountData accountData = SerializeHelper.Deserialize<AccountData>(operationResponse.Parameters[85] as byte[]);
						if (accountData != null)
						{
							accountModelData.accountData = accountData;
							base.DebugMessage = "注册成功!";
							this.SaveLoginList(accountModelData.accountData.UserName, accountModelData.accountData.Password);
						}
						else
						{
							base.DebugMessage = "注册成功，但数据获取失败";
							base.Valid = false;
						}
					}
				}
				else
				{
					base.Valid = false;
				}
			}
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private void OnGetMsg_MasterCode_Login(MobaMessage msg)
		{
			base.LastError = 505;
			base.Valid = true;
			if (msg != null)
			{
				AccountModelData accountModelData = base.Data as AccountModelData;
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					base.LastError = (int)operationResponse.Parameters[1];
					if (base.LastError == 0)
					{
						Dictionary<byte, object> dictionary = operationResponse.Parameters[85] as Dictionary<byte, object>;
						if (dictionary == null)
						{
							accountModelData.accountData = SerializeHelper.Deserialize<AccountData>(operationResponse.Parameters[85] as byte[]);
						}
						else
						{
							accountModelData.accountData = this.ToAccountData(dictionary);
						}
						this.SaveLoginList(accountModelData.accountData.UserName, accountModelData.accountData.Password);
					}
				}
				else
				{
					base.Valid = false;
				}
			}
		}

		private void OnGetMsg_MasterCode_LoginByPlatformUid(MobaMessage msg)
		{
			base.LastError = 505;
			base.Valid = true;
			if (msg != null)
			{
				AccountModelData accountModelData = base.Data as AccountModelData;
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					base.LastError = (int)operationResponse.Parameters[1];
					if (base.LastError == 0)
					{
						AccountData accountData = SerializeHelper.Deserialize<AccountData>(operationResponse.Parameters[85] as byte[]);
						accountModelData.accountData = accountData;
						this.SaveLoginList(accountModelData.accountData.UserName, accountModelData.accountData.Password);
					}
				}
				else
				{
					base.Valid = false;
				}
			}
		}

		private void OnGetMsg_MasterCode_GuestUpgrade(MobaMessage msg)
		{
			this.OnGetMsg_MasterCode_Register(msg);
		}

		private AccountData ToAccountData(object serObj)
		{
			if (serObj is IDictionary)
			{
				Dictionary<byte, object> dictionary = (Dictionary<byte, object>)serObj;
				AccountData accountData = new AccountData();
				accountData.AccountId = (string)dictionary[71];
				accountData.UserName = (string)dictionary[72];
				accountData.Password = (string)dictionary[74];
				accountData.Mail = (string)dictionary[73];
				accountData.UserType = (int)dictionary[77];
				accountData.DeviceType = (int)dictionary[75];
				accountData.DeviceToken = (string)dictionary[76];
				accountData.ServerName = (int)dictionary[53];
				accountData.ChannelId = (string)dictionary[78];
				if (dictionary.ContainsKey(225))
				{
					accountData.AccessToken = (string)dictionary[225];
				}
				if (dictionary.ContainsKey(222))
				{
					accountData.PlatformUid = (string)dictionary[222];
				}
				if (dictionary.ContainsKey(223))
				{
					accountData.Channel = (string)dictionary[223];
				}
				if (dictionary.ContainsKey(224))
				{
					accountData.ChannelUid = (string)dictionary[224];
				}
				if (dictionary.ContainsKey(227))
				{
					accountData.IsBindPhone = (bool)dictionary[227];
				}
				if (dictionary.ContainsKey(228))
				{
					accountData.IsBindEmail = (bool)dictionary[228];
				}
				return accountData;
			}
			return null;
		}

		public void SetData(AccountData newData)
		{
			AccountModelData accountModelData = base.Data as AccountModelData;
			accountModelData.accountData = newData;
			base.LastError = 0;
			base.Valid = (base.LastError == 0 && null != base.Data);
		}

		private List<string[]> GetLoginList()
		{
			List<string[]> list = new List<string[]>();
			if (PlayerPrefs.HasKey("UserName"))
			{
				list.Add(new string[]
				{
					PlayerPrefs.GetString("UserName"),
					PlayerPrefs.GetString("PassWord")
				});
			}
			if (PlayerPrefs.HasKey("UserName2"))
			{
				list.Add(new string[]
				{
					PlayerPrefs.GetString("UserName2"),
					PlayerPrefs.GetString("PassWord2")
				});
			}
			return list;
		}

		private void SaveLoginList(string userName, string passWord)
		{
			string[] array = new string[]
			{
				userName,
				passWord
			};
			List<string[]> loginList = this.GetLoginList();
			if (loginList != null)
			{
				if (userName != string.Empty)
				{
					for (int i = 0; i < loginList.Count; i++)
					{
						if (loginList[i][0] == array[0])
						{
							loginList.Remove(loginList[i]);
							break;
						}
					}
					loginList.Insert(0, array);
				}
				if (loginList.Count == 1)
				{
					PlayerPrefs.SetString("UserName", loginList[0][0]);
					PlayerPrefs.SetString("PassWord", loginList[0][1]);
					PlayerPrefs.DeleteKey("UserName2");
					PlayerPrefs.DeleteKey("PassWord2");
				}
				else if (loginList.Count >= 2)
				{
					PlayerPrefs.SetString("UserName", loginList[0][0]);
					PlayerPrefs.SetString("PassWord", loginList[0][1]);
					PlayerPrefs.SetString("UserName2", loginList[1][0]);
					PlayerPrefs.SetString("PassWord2", loginList[1][1]);
				}
			}
			else
			{
				PlayerPrefs.SetString("UserName", array[0]);
				PlayerPrefs.SetString("PassWord", array[1]);
			}
			PlayerPrefs.Save();
			this.GetLoginList();
		}
	}
}
