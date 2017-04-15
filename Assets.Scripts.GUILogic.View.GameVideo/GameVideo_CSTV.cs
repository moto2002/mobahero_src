using Assets.Scripts.Model;
using Com.Game.Module;
using CsSdk;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_CSTV : MonoBehaviour
	{
		public class CsTvCallback : ICsTvListener
		{
			public GameUserInfo getUserInfo()
			{
				Debug.LogError("getUserInfo in");
				GameUserInfo result;
				result.userId = ModelManager.Instance.Get_accountData_X().AccountId;
				string accountId = ModelManager.Instance.Get_accountData_X().AccountId;
				string extraData = GameVideo_CSTV.MD5Encrypt("xiaomeng_" + accountId);
				result.userToken = accountId;
				result.phoneNumber = ModelManager.Instance.Get_accountData_X().UserName;
				result.extraData = extraData;
				return result;
			}

			public void startPay(CsGoodsInfo goodsInfo)
			{
			}

			public CpAccountBalance queryAccountBalance()
			{
				CpAccountBalance result;
				result.unitDesc = "钻石";
				result.amount = 9999;
				return result;
			}

			public void notifyGiftResult(int code, string message, int accountBalance)
			{
				Debug.LogError("notifyGiftResult in: code = " + code);
			}
		}

		public class CsHttpCallback : ICsHttpListener
		{
			public void onStart()
			{
				Debug.LogError("onStart in");
			}

			public void onSuccess(string jsonObject)
			{
				Debug.LogError("onSuccess in: data = " + jsonObject);
			}

			public void onFailure(int code, string message)
			{
				Debug.LogError("onFailure in: message = " + message);
			}
		}

		public class OnLiveCallBack : ICsRecListener
		{
			public void onSuccess()
			{
				Debug.LogError("onSuccess");
			}

			public void onFailure(string msg)
			{
				Debug.LogError("onFailure => " + msg);
			}

			public void offline()
			{
				Debug.LogError("offline");
			}
		}

		public class LocalCallBack : ICsLocalRecListener
		{
			public void onFailure(string msg)
			{
				Debug.LogError("onFailure = " + msg);
			}

			public void onRecordStart()
			{
				Debug.LogError("onRecordStart = ");
			}

			public void onRecordFinish()
			{
				Debug.LogError("onRecordFinish = ");
			}
		}

		public GameObject LiveBtn;

		public GameObject VideoBtn;

		public GameObject IWantYouBtn;

		public GameVideo_CSTVStartShow StartShowRoot;

		[HideInInspector]
		public bool IsActive;

		private static readonly string GAME_ID = "1320";

		private static readonly string GAME_NAME = "魔霸英雄";

		private string gameUid;

		private string gameToken;

		private string phone;

		private string nickName;

		private string extraData;

		private CsTvInterface cstv;

		private CsRecInterface rec;

		private int orentation = 1;

		private int resolution = 1;

		private void OnEnable()
		{
			this.gameUid = ModelManager.Instance.Get_accountData_X().AccountId;
			this.gameToken = ModelManager.Instance.Get_accountData_X().AccountId;
			this.phone = ModelManager.Instance.Get_accountData_X().UserName;
			this.nickName = ModelManager.Instance.Get_userData_X().NickName;
			this.extraData = GameVideo_CSTV.MD5Encrypt("xiaomeng_" + this.gameToken);
			UIEventListener.Get(this.LiveBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_Live);
			UIEventListener.Get(this.VideoBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_Video);
			UIEventListener.Get(this.IWantYouBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_IWantYou);
			MobaMessageManager.RegistMessage((ClientMsg)26033, new MobaMessageFunc(this.OnMsg_SubmitLiveSetting));
			if (!Application.isEditor)
			{
				if (this.cstv == null)
				{
					this.cstv = new CsTvInterface();
					this.cstv.initialize(new GameVideo_CSTV.CsTvCallback());
				}
				if (this.rec == null)
				{
					this.rec = new CsRecInterface();
					this.rec.init();
				}
			}
		}

		private void OnDisable()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)26033, new MobaMessageFunc(this.OnMsg_SubmitLiveSetting));
		}

		public void ResetPage()
		{
			this.StartShowRoot.Hide();
		}

		public static string MD5Encrypt(string strText)
		{
			byte[] bytes = Encoding.Default.GetBytes(strText);
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] value = mD.ComputeHash(bytes);
			return BitConverter.ToString(value).Replace("-", string.Empty);
		}

		private void OnMsg_SubmitLiveSetting(MobaMessage msg)
		{
			object[] array = msg.Param as object[];
			string title = (string)array[0];
			int num = (int)array[1];
			this.rec.startLive(GameVideo_CSTV.GAME_NAME, title, this.gameUid, this.gameToken, this.phone, this.nickName, this.extraData, this.orentation, num, new GameVideo_CSTV.OnLiveCallBack());
		}

		private void OnClick_Live(GameObject obj = null)
		{
			GlobalObject.Instance.forceLockReStart = true;
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			if (!Application.isMobilePlatform)
			{
				Singleton<TipView>.Instance.ShowViewSetText("请换手机来打开", 1f);
				return;
			}
			this.cstv.getOnlineRoomListWithUI(GameVideo_CSTV.GAME_ID);
		}

		private void OnClick_Video(GameObject obj = null)
		{
			GlobalObject.Instance.forceLockReStart = true;
			NetWorkHelper.Instance.GateReconnection.LeaveGame("leave");
			if (!Application.isMobilePlatform)
			{
				Singleton<TipView>.Instance.ShowViewSetText("请换手机来打开", 1f);
				return;
			}
			this.cstv.getGameVideoListWithUI(GameVideo_CSTV.GAME_ID);
		}

		private void OnClick_IWantYou(GameObject obj = null)
		{
			this.StartShowRoot.Show();
		}

		public void SetActive(bool _isActive)
		{
			this.IsActive = _isActive;
			base.gameObject.SetActive(_isActive);
		}
	}
}
