using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestSharePlugin : MonoBehaviour
	{
		private AnySDKShare _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKShare.getInstance();
			this._instance.setListener(this, "ShareExternalCall");
		}

		private void ShareExternalCall(string msg)
		{
			Debug.Log("ShareExternalCall(" + msg + ")");
			Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
			int num = Convert.ToInt32(dictionary["code"]);
			string text = dictionary["msg"];
			switch (num)
			{
			}
		}

		private void OnDestroy()
		{
		}

		private void OnGUI()
		{
			GUI.color = Color.white;
			GUI.skin.button.fontSize = 30;
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "Share"))
			{
				this.share();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestSharePlugin"));
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

		private void share()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["title"] = "AnySDK是一个神奇的SDK";
			dictionary["titleUrl"] = "http://www.AnySDK.cn";
			dictionary["site"] = "AnySDK";
			dictionary["siteUrl"] = "http://www.AnySDK.cn";
			dictionary["text"] = "一次集成，多渠道发布";
			dictionary["comment"] = "无";
			this._instance.share(dictionary);
		}
	}
}
