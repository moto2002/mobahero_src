using Assets.Scripts.Model;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class IAPManager : MonoBehaviour
{
	public List<string> productInfo = new List<string>();

	private CoroutineManager corMgr = new CoroutineManager();

	private Task buyTask;

	private bool isGetProductListEnd;

	private string oldSaveDate = string.Empty;

	public int price;

	private string order = string.Empty;

	private bool back;

	[DllImport("__Internal")]
	private static extern void TestMsg();

	[DllImport("__Internal")]
	private static extern void TestSendString(string s);

	[DllImport("__Internal")]
	private static extern void TestGetString();

	[DllImport("__Internal")]
	private static extern void InitIAPManager();

	[DllImport("__Internal")]
	private static extern bool IsProductAvailable();

	[DllImport("__Internal")]
	private static extern void RequstProductInfo(string s);

	[DllImport("__Internal")]
	private static extern void BuyProduct(string s);

	private void IOSToU(string s)
	{
		UnityEngine.Debug.Log("[MsgFrom ios]" + s);
	}

	private void ShowProductList(string s)
	{
		this.productInfo.Add(s);
	}

	public void GetProductListEnd(string s)
	{
		this.isGetProductListEnd = true;
	}

	private void FailedTransaction(string s)
	{
		UnityEngine.Debug.Log("[MsgFrom ios]FailedTransaction : " + s);
		if (this.productInfo.Count == 0)
		{
			return;
		}
		MobaMessageManagerTools.EndWaiting_manual("IAPInBuy");
		CtrlManager.ShowMsgBox("提示", "支付不成功，请重试！", new Action<bool>(this.ReComfirm), PopViewType.PopOneButton, "确定", "取消", null);
	}

	private void CancelledTransaction(string s)
	{
		UnityEngine.Debug.Log("[MsgFrom ios]CancelledTransaction : " + s);
		MobaMessageManagerTools.EndWaiting_manual("IAPInBuy");
	}

	private void RestoreTransaction(string s)
	{
		UnityEngine.Debug.Log("[MsgFrom ios]RestoreTransaction : " + s);
		MobaMessageManagerTools.EndWaiting_manual("IAPInBuy");
	}

	private void CompleteTransaction(string s)
	{
		UnityEngine.Debug.Log("[MsgFrom ios]CompleteTransaction : " + s);
		this.order = s;
	}

	private void ProvideContent(string s)
	{
		UnityEngine.Debug.Log("[MsgFrom ios]proivideContent : " + s);
		this.back = true;
		string text = string.Empty;
		AnalyticsToolManager.SetChargeRequest(ModelManager.Instance.Get_userData_X().UserId.ToString() + "_" + this.order, this.price, "IAP");
		if (PlayerPrefs.HasKey("ProductSaveData"))
		{
			text = PlayerPrefs.GetString("ProductSaveData");
		}
		this.oldSaveDate = text;
		if (text != string.Empty)
		{
			text = text + "|" + s;
		}
		else
		{
			text = s;
		}
		PlayerPrefs.SetString("ProductSaveData", text);
		PlayerPrefs.Save();
		this.corMgr.StartCoroutine(this.ShowProductTask(s), true);
	}

	public void CheckSaveProvideContent()
	{
		if (!PlayerPrefs.HasKey("ProductSaveData"))
		{
			return;
		}
		string text = string.Empty;
		text = PlayerPrefs.GetString("ProductSaveData");
		if (text == string.Empty)
		{
			PlayerPrefs.DeleteKey("ProductSaveData");
			PlayerPrefs.Save();
			return;
		}
		this.oldSaveDate = string.Empty;
		string[] array = text.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			this.corMgr.StartCoroutine(this.ShowProductTask(array[i]), true);
		}
	}

	[DebuggerHidden]
	private IEnumerator ShowProductTask(string s)
	{
		return new IAPManager.<ShowProductTask>c__Iterator199();
	}

	private void Awake()
	{
		MVC_MessageManager.AddListener_view(MobaGameCode.AppStoreCharge, new MobaMessageFunc(this.OnGetAppStoreCharge));
	}

	public void InitIAP()
	{
		this.isGetProductListEnd = false;
		IAPManager.InitIAPManager();
		this.corMgr.StartCoroutine(this.WaitingInitIAP(), true);
	}

	private void Destory()
	{
	}

	private void OnGetAppStoreCharge(MobaMessage msg = null)
	{
		MobaMessageManagerTools.EndWaiting_manual("IAPInBuy");
		if (msg != null)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode == MobaErrorCode.Ok)
			{
				InitSDK.instance.orderBack("0");
				AnalyticsToolManager.SetChargeSuccess(ModelManager.Instance.Get_userData_X().UserId.ToString() + "_" + this.order);
				if (this.oldSaveDate == string.Empty)
				{
					PlayerPrefs.DeleteKey("ProductSaveData");
				}
				else
				{
					PlayerPrefs.SetString("ProductSaveData", this.oldSaveDate);
				}
				PlayerPrefs.Save();
			}
			this.order = string.Empty;
		}
	}

	[DebuggerHidden]
	private IEnumerator WaitingInitIAP()
	{
		IAPManager.<WaitingInitIAP>c__Iterator19A <WaitingInitIAP>c__Iterator19A = new IAPManager.<WaitingInitIAP>c__Iterator19A();
		<WaitingInitIAP>c__Iterator19A.<>f__this = this;
		return <WaitingInitIAP>c__Iterator19A;
	}

	public void TryBuy(string id)
	{
		if (!this.isGetProductListEnd)
		{
			this.InitIAP();
		}
		this.back = false;
		if (this.buyTask != null)
		{
			this.buyTask.Stop();
		}
		MobaMessageManagerTools.BeginWaiting_manual("IAPTryBuy", string.Empty, true, 30f, true);
		int index = int.Parse(id);
		this.buyTask = this.corMgr.StartCoroutine(this.BuyProductTask(index), true);
	}

	[DebuggerHidden]
	private IEnumerator BuyProductTask(int index)
	{
		IAPManager.<BuyProductTask>c__Iterator19B <BuyProductTask>c__Iterator19B = new IAPManager.<BuyProductTask>c__Iterator19B();
		<BuyProductTask>c__Iterator19B.index = index;
		<BuyProductTask>c__Iterator19B.<$>index = index;
		<BuyProductTask>c__Iterator19B.<>f__this = this;
		return <BuyProductTask>c__Iterator19B;
	}

	private void ReComfirm(bool obj)
	{
	}
}
