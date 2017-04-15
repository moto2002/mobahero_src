using Assets.Scripts.Model;
using System;
using UnityEngine;

public class ReYun : MonoBehaviour
{
	public enum Gender
	{
		reyun_m,
		reyun_f,
		reyun_o
	}

	public enum QuestStatus
	{
		reyun_start,
		reyun_done,
		reyun_fail
	}

	public struct strutDict
	{
		public string key;

		public string value;
	}

	private static ReYun _instance;

	public static ReYun instance
	{
		get
		{
			if (ReYun._instance == null)
			{
				ReYun._instance = InitSDK.instance.gameObject.AddComponent<ReYun>();
			}
			return ReYun._instance;
		}
	}

	private void DemoStart()
	{
	}

	public void Init()
	{
	}

	public void Login(string accountId, int level)
	{
		this.Game_Login(accountId, ReYun.Gender.reyun_o, "0", ModelManager.Instance.Get_curLoginServerInfo().serverip, level);
	}

	protected void Game_Login(string account, ReYun.Gender genders, string age, string serverId, int level)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.reyun.sdk.ReYun");
		androidJavaClass.CallStatic("setLoginWithAccountID", new object[]
		{
			account,
			level,
			serverId
		});
	}

	public void Register(string accountId)
	{
		this.Game_Register(accountId, ReYun.Gender.reyun_o.ToString(), "0", ModelManager.Instance.Get_curLoginServerInfo().serverip, "Hoolai");
	}

	public void Quest(string taskId, int level)
	{
		this.Game_Quest(taskId, ReYun.QuestStatus.reyun_done.ToString(), "main", level);
	}

	protected void Game_Register(string account, string gender, string age, string serverId, string accountType)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.reyun.sdk.ReYun");
		androidJavaClass.CallStatic("setNRegisterWithAccountID", new object[]
		{
			account,
			accountType,
			gender,
			age,
			serverId
		});
	}

	public void SetPayment(string orderId, int price, string paymentType)
	{
		this.Game_SetPayment(orderId, paymentType, "CH", 12.1f, 12.1f, "xxx", 2L, 4);
	}

	protected void Game_SetPayment(string transactionId, string paymentType, string currencyType, float currencyAmount, float virtualCoinAmount, string iapName, long iapAmount, int level)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.reyun.sdk.ReYun");
		androidJavaClass.CallStatic("setPayment", new object[]
		{
			transactionId,
			paymentType,
			currencyType,
			currencyAmount,
			virtualCoinAmount,
			iapName,
			iapAmount,
			level
		});
	}

	protected void Game_SetEconomy(string itemName, long itemAmount, float itemTotalPrice, int level)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.reyun.sdk.ReYun");
		androidJavaClass.CallStatic("setEconomy", new object[]
		{
			itemName,
			itemAmount,
			itemTotalPrice,
			level
		});
	}

	protected void Game_Quest(string questId, string status, string questType, int level)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.reyun.sdk.ReYun");
		androidJavaClass.CallStatic("setNQuest", new object[]
		{
			questId,
			status,
			questType,
			level
		});
	}

	protected void Game_SetEvent(string eventName, string key, string value)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.reyun.sdk.ReYun");
		androidJavaClass.CallStatic("setNEvent", new object[]
		{
			eventName,
			key,
			value
		});
	}
}
