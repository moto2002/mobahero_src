using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaHeros;
using MobaProtocol;
using MobaServer;
using Newbie;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class SummonerRegisterView : BaseView<SummonerRegisterView>
	{
		private Transform dice;

		private Transform goToGame;

		private UIInput name_Input;

		private UIGrid headportrait;

		private string Name = string.Empty;

		private List<string> callBack = new List<string>();

		public SummonerRegisterView()
		{
			this.WinResCfg = new WinResurceCfg(true, false, "Prefab/UI/Home/SummonerRegisterView");
		}

		public override void Init()
		{
			this.dice = this.transform.Find("Dice");
			this.goToGame = this.transform.Find("GameBtn");
			this.name_Input = this.transform.Find("InputFrame/Label").GetComponent<UIInput>();
			this.headportrait = this.transform.Find("Grid").GetComponent<UIGrid>();
			UIEventListener.Get(this.dice.gameObject).onClick = new UIEventListener.VoidDelegate(this.Dice_Event);
			EventDelegate item = new EventDelegate(new EventDelegate.Callback(this.ChangeLabelText));
			this.name_Input.GetComponent<UIInput>().onChange = new List<EventDelegate>();
			this.name_Input.GetComponent<UIInput>().onChange.Add(item);
			UIEventListener.Get(this.goToGame.gameObject).onClick = new UIEventListener.VoidDelegate(this.CreatName_Event);
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.TestCreateName();
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearResources();
		}

		public void ClearResources()
		{
			if (this.headportrait != null)
			{
				this.ClearUITextureResources(this.headportrait.gameObject);
			}
		}

		private void ClearUITextureResources(GameObject inGo)
		{
			if (inGo == null)
			{
				return;
			}
			UITexture[] componentsInChildren = inGo.GetComponentsInChildren<UITexture>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UITexture uITexture = componentsInChildren[i];
					if (uITexture != null && uITexture.mainTexture != null)
					{
						uITexture.mainTexture = null;
					}
				}
			}
		}

		private void TestCreateName()
		{
			AutoTestController.InvokeTestLogic(AutoTestTag.Login, delegate
			{
				this.Dice_Event(null);
				AutoTestController.InvokeTestLogic(AutoTestTag.Login, delegate
				{
					this.CreatName_Event(null);
				}, 1f);
			}, 1f);
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaGameCode.MagicBottleItem, new MobaMessageFunc(this.OnGetMsg_ModfiyNickName));
			MVC_MessageManager.AddListener_view(MobaGameCode.ModfiyAvatar, new MobaMessageFunc(this.OnGetMsg_ModfiyAvatar));
			MobaMessageManager.RegistMessage((ClientMsg)20005, new MobaMessageFunc(this.OnGetMsg_Connected_game));
			MobaMessageManager.RegistMessage((ClientMsg)20006, new MobaMessageFunc(this.OnGetMsg_Disconnected_game));
			this.UpdatePortrait();
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaGameCode.MagicBottleItem, new MobaMessageFunc(this.OnGetMsg_ModfiyNickName));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.ModfiyAvatar, new MobaMessageFunc(this.OnGetMsg_ModfiyAvatar));
			MobaMessageManager.UnRegistMessage((ClientMsg)20005, new MobaMessageFunc(this.OnGetMsg_Connected_game));
			MobaMessageManager.UnRegistMessage((ClientMsg)20006, new MobaMessageFunc(this.OnGetMsg_Disconnected_game));
			this.callBack.Clear();
			this.Name = string.Empty;
		}

		public override void RefreshUI()
		{
			this.name_Input.value = string.Empty;
			this.name_Input.gameObject.GetComponent<UILabel>().text = string.Empty;
		}

		protected void OnGetMsg_Connected_game(MobaMessage msg)
		{
		}

		protected void OnGetMsg_Disconnected_game(MobaMessage msg)
		{
		}

		private void Dice_Event(GameObject object_1 = null)
		{
			string[] array = GetRandomStr.Name(1);
			string value = array[0];
			this.name_Input.value = value;
			this.name_Input.GetComponent<UIInput>().value = value;
		}

		private void ChangeLabelText()
		{
			if (this.transform == null)
			{
				return;
			}
			if (this.name_Input == null)
			{
				this.name_Input = this.transform.Find("InputFrame/Label").GetComponent<UIInput>();
			}
			UIFont component = this.name_Input.transform.GetComponent<UIFont>();
			component.dynamicFont.RequestCharactersInTexture(this.name_Input.value, 50);
			char[] array = this.name_Input.value.Replace('\u3000', '\0').ToCharArray();
			if (array == null)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!this.IsValid(component.dynamicFont, array[i]))
				{
					array[i] = '\0';
				}
			}
			string text = new string(array);
			text = text.Replace("\n", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\r", string.Empty);
			text = FilterWorder.Instance.ReplaceKeyword(text);
			this.name_Input.GetComponent<UIInput>().value = text;
		}

		private bool IsValid(Font font, char c)
		{
			CharacterInfo characterInfo;
			return font.GetCharacterInfo(c, out characterInfo, 50) && characterInfo.width > 0f;
		}

		private void CreatName_Event(GameObject object_1)
		{
			string value = this.name_Input.value;
			if (string.IsNullOrEmpty(value))
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameCannotBeEmpty"), 1f);
				return;
			}
			if (value.Length > 7)
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameOverLength"), 1f);
			}
			else if (string.IsNullOrEmpty(value))
			{
				Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameCannotBeEmpty"), 1f);
			}
			else
			{
				if (value.Contains("*") || FilterWorder.Instance.CheckKeyword(value))
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_IllegalCharacter"), 1f);
					return;
				}
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_RefreshData"), true, 15f);
				MobaMessageManagerTools.BeginWaiting_manual("SummonerRegister", "刷新数据...", true, 20f, true);
				SendMsgManager.Instance.SendMsg(MobaGameCode.ModfiyAvatar, null, new object[]
				{
					int.Parse(this.Name),
					1
				});
				if (!this.callBack.Contains("Name"))
				{
					SendMsgManager.Instance.SendMsg(MobaGameCode.MagicBottleItem, param, new object[]
					{
						value
					});
				}
			}
		}

		private void OnGetMsg_ModfiyNickName(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			MobaMessageManagerTools.EndWaiting_manual("SummonerRegister");
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				if (mobaErrorCode != MobaErrorCode.NickNameExist)
				{
					this.ClickChangeName(num, operationResponse.DebugMessage, string.Empty);
				}
				else
				{
					this.ClickChangeName(num, operationResponse.DebugMessage, string.Empty);
				}
			}
			else
			{
				this.ClickChangeName(num, operationResponse.DebugMessage, string.Empty);
			}
		}

		private void ClickChangeName(int i = 0, string j = null, string k = "")
		{
			if (i != 0)
			{
				if (i != 10106)
				{
					if (i != 70003)
					{
						Singleton<TipView>.Instance.ShowViewSetText("未知错误", 1f);
						this.RefreshUI();
					}
					else
					{
						Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_MissingNameCard"), 1f);
					}
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("SummonerUI_Passport_Tips_NameHasBeenUsed"), 1f);
					this.TestCreateName();
				}
			}
			else
			{
				this.Account("Name");
			}
		}

		private void OnGetMsg_ModfiyAvatar(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			MobaMessageManagerTools.EndWaiting_manual("SummonerRegister");
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.ClickHeroHeadCallBack(num, operationResponse.DebugMessage);
			}
			else
			{
				this.ClickHeroHeadCallBack(num, operationResponse.DebugMessage);
			}
		}

		private void ClickHeroHeadCallBack(int i = 0, string str = null)
		{
			if (i != 0)
			{
				Singleton<TipView>.Instance.ShowViewSetText("未知错误", 1f);
				Singleton<MenuTopBarView>.Instance.RefreshUI();
			}
			else
			{
				this.Account("Avatar");
			}
		}

		private void Account(string type)
		{
			if (this.callBack.Contains(type))
			{
				return;
			}
			this.callBack.Add(type);
			if (this.callBack.Count >= 2)
			{
				this.SucceedInCreatingCount();
			}
		}

		private void SucceedInCreatingCount()
		{
			ModelManager.Instance.Get_userData_X().LoginCount++;
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.SetExtData("3");
				AnalyticsToolManager.Register(ModelManager.Instance.Get_accountData_X().AccountId);
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SetAnySDKExtData("2");
			}
			Singleton<MenuTopBarView>.Instance.RefreshUI();
			CtrlManager.CloseWindow(WindowID.SummonerRegisterView);
			NewbieManager.Instance.TryProcessNewbieGuide();
		}

		private void UpdatePortrait()
		{
			Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersHeadportraitVo>();
			List<string> list = new List<string>(dicByType.Keys);
			this.Name = list[0];
			for (int i = 0; i < 16; i++)
			{
				SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(list[i].ToString());
				if (dataById.portrait_type == 1)
				{
					this.headportrait.transform.GetChild(i).name = dataById.headportrait_id.ToString();
					this.headportrait.transform.GetChild(i).GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.headportrait_icon, true, true, null, 0, false);
					UIEventListener expr_B5 = UIEventListener.Get(this.headportrait.transform.GetChild(i).gameObject);
					expr_B5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_B5.onClick, new UIEventListener.VoidDelegate(this.CreatePor));
				}
			}
		}

		private void CreatePor(GameObject obj)
		{
			this.Name = obj.name;
			for (int i = 0; i < this.Name.ToCharArray().Length; i++)
			{
				if (this.Name.ToCharArray()[i] < '\0' || this.Name.ToCharArray()[i] > 'c')
				{
					return;
				}
			}
		}
	}
}
