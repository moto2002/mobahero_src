using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestIAPPlugin : MonoBehaviour
	{
		private AnySDKIAP _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKIAP.getInstance();
			this._instance.setListener(this, "IAPExternalCall");
		}

		private void IAPExternalCall(string msg)
		{
			Debug.Log("IAPExternalCall(" + msg + ")");
			Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
			int num = Convert.ToInt32(dictionary["code"]);
			string text = dictionary["msg"];
			switch (num)
			{
			case 0:
				Debug.Log("IAPExternalCall(" + this._instance.getOrderId(this._instance.getPluginId()[0]) + ")");
				break;
			}
		}

		private void OnDestroy()
		{
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUI.skin.button.fontSize = 30;
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "payForProduct"))
			{
				this.payForProduct();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "getOrderId"))
			{
				this.getOrderId();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestIAPPlugin"));
				base.gameObject.AddComponent<Test>();
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
			{
				Application.Quit();
				AnySDK.getInstance().release();
			}
		}

		private void getOrderId()
		{
			string orderId = this._instance.getOrderId(string.Empty);
			Debug.Log("AnySDK@ getOrder id " + orderId);
		}

		private void payForProduct()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Product_Id"] = "1";
			dictionary["Product_Name"] = "10元宝";
			dictionary["Product_Price"] = "1";
			dictionary["Product_Count"] = "1";
			dictionary["Product_Desc"] = "gold";
			dictionary["Coin_Name"] = "元宝";
			dictionary["Coin_Rate"] = "10";
			dictionary["Role_Id"] = "123456";
			dictionary["Role_Name"] = "test";
			dictionary["Role_Grade"] = "1";
			dictionary["Role_Balance"] = "1";
			dictionary["Vip_Level"] = "1";
			dictionary["Party_Name"] = "test";
			dictionary["Server_Id"] = "1";
			dictionary["Server_Name"] = "test";
			dictionary["EXT"] = "test";
			this._instance.payForProduct(dictionary, string.Empty);
		}
	}
}
