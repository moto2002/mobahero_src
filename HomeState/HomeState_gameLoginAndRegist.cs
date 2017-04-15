using anysdk;
using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using MobaClient;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace HomeState
{
	internal class HomeState_gameLoginAndRegist : HomeStateBase
	{
		private bool bFinish;

		public HomeState_gameLoginAndRegist() : base(HomeStateCode.HomeState_gameLoginAndRegist, MobaPeerType.C2GateServer)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.Login();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
			MVC_MessageManager.AddListener_view(MobaGameCode.Login, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.AddListener_view(MobaGameCode.Register, new MobaMessageFunc(this.OnRegister));
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
			MVC_MessageManager.RemoveListener_view(MobaGameCode.Login, new MobaMessageFunc(this.OnLogin));
			MVC_MessageManager.RemoveListener_view(MobaGameCode.Register, new MobaMessageFunc(this.OnRegister));
		}

		private void Regist()
		{
			SendMsgManager.Instance.SendMsg(MobaGameCode.Register, null, new object[]
			{
				"default"
			});
		}

		private void Login()
		{
			if (!SendMsgManager.Instance.SendMsg(MobaGameCode.Login, null, new object[0]))
			{
				Debug.LogError("Login send failed");
			}
		}

		private void OnLogin(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				ClientLogger.Error("游戏登陆失败...ret=null");
				string content = "游戏登录失败：系统错误,是否重试？";
				this.ShowBox2("提示", content, new Action(this.Login), new Action(this.QuitGame));
			}
			else
			{
				int num = (int)operationResponse.Parameters[1];
				int num2 = num;
				if (num2 != -1)
				{
					if (num2 != 0)
					{
						if (num2 != 10)
						{
							if (num2 != 10103)
							{
								if (num2 != 50101)
								{
									ClientLogger.Error(string.Format("登陆失败...不明的ReturnCode : {0}", num));
									Singleton<TipView>.Instance.ShowViewSetText("游戏登录失败：系统错误", 1f);
								}
								else
								{
									this.ShowBox1("提示", "认证错误， 需要重新登录", new Action(this.RestartGame));
								}
							}
							else
							{
								this.Regist();
							}
						}
						else
						{
							Singleton<TipView>.Instance.ShowViewSetText("游戏版验证失败！！！", 1f);
							this.Regist();
						}
					}
					else
					{
						UserData data = ModelManager.Instance.Get_userData_X();
						this.DumpUserInfo(data);
						this.bFinish = true;
						HomeManager.Instance.ChangeState(HomeStateCode.HomeState_requestData);
					}
				}
				else
				{
					this.ShowBox1("提示", "资源版本错误， 需要重新登录", new Action(this.RestartGame));
				}
			}
		}

		private void OnRegister(MobaMessage msg)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null)
			{
				int num = (int)operationResponse.Parameters[1];
				int num2 = num;
				switch (num2)
				{
				case 10102:
					Singleton<TipView>.Instance.ShowViewSetText("登录游戏失败:用户已存在", 1f);
					this.Regist();
					return;
				case 10103:
					this.ShowBox1("提示", "认证错误， 需要重新登录", new Action(this.RestartGame));
					return;
				case 10104:
				case 10105:
					IL_65:
					if (num2 == -1)
					{
						this.ShowBox1("提示", "资源版本错误， 需要重新登录", new Action(this.RestartGame));
						return;
					}
					if (num2 == 0)
					{
						this.Login();
						return;
					}
					if (num2 == 10)
					{
						Singleton<TipView>.Instance.ShowViewSetText("游戏版验证失败！！！", 1f);
						this.Regist();
						return;
					}
					if (num2 == 10110)
					{
						Singleton<TipView>.Instance.ShowViewSetText("登录游戏失败:昵称为空", 1f);
						this.Regist();
						return;
					}
					if (num2 != 50101)
					{
						Singleton<TipView>.Instance.ShowViewSetText("登录游戏失败:系统错误", 1f);
						this.Regist();
						return;
					}
					this.ShowBox1("提示", "认证错误， 需要重新登录", new Action(this.RestartGame));
					return;
				case 10106:
					Singleton<TipView>.Instance.ShowViewSetText("登录游戏失败:该昵称已存在", 1f);
					this.Regist();
					return;
				}
				goto IL_65;
			}
			Singleton<TipView>.Instance.ShowViewSetText("游戏注册失败：系统错误", 1f);
			this.Regist();
		}

		private void QuitGame()
		{
			GlobalObject.QuitApp();
		}

		private void RestartGame()
		{
			if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SetAnySDKExtData("4");
				AnySDK.getInstance().release();
			}
			GlobalObject.ReStartGame();
		}

		private void ShowBox1(string title, string content, Action callback)
		{
			CtrlManager.ShowMsgBox("提示", content, delegate
			{
				if (callback != null)
				{
					callback();
				}
			}, PopViewType.PopOneButton, "确定", "取消", null);
		}

		private void ShowBox2(string title, string content, Action callback, Action callback2)
		{
			CtrlManager.ShowMsgBox("提示", content, delegate(bool ret)
			{
				if (ret)
				{
					if (callback != null)
					{
						callback();
					}
				}
				else if (callback2 != null)
				{
					callback2();
				}
			}, PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void DumpUserInfo(UserData data)
		{
			if (data != null)
			{
				ModelManager.Instance.Get_userData_X().Money = data.Money;
				ModelManager.Instance.Get_userData_X().Diamonds = data.Diamonds;
				ModelManager.Instance.Get_userData_X().SmallCap = data.SmallCap;
				if (Singleton<MenuTopBarView>.Instance.gameObject)
				{
					Singleton<MenuTopBarView>.Instance.RefreshUI();
				}
			}
		}

		protected override void OnConnected_game(MobaMessage msg)
		{
			if (!this.bFinish)
			{
				this.Login();
			}
		}

		protected override void OnDisconnected_game(MobaMessage msg)
		{
		}
	}
}
