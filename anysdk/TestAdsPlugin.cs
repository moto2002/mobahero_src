using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestAdsPlugin : MonoBehaviour
	{
		private AnySDKAds _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKAds.getInstance();
			this._instance.setListener(this, "AdsExternalCall");
		}

		private void AdsExternalCall(string msg)
		{
			Debug.Log("AdsExternalCall(" + msg + ")");
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
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "showAds"))
			{
				this.showAds();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "hideAds"))
			{
				this.hideAds();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "preloadAds"))
			{
				this.preloadAds();
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestAdsPlugin"));
				base.gameObject.AddComponent<Test>();
			}
		}

		private void showAds()
		{
			if (this._instance.isAdTypeSupported(AdsType.AD_TYPE_BANNER))
			{
				this._instance.showAds(AdsType.AD_TYPE_BANNER, 1);
			}
		}

		private void hideAds()
		{
			if (this._instance.isAdTypeSupported(AdsType.AD_TYPE_BANNER))
			{
				this._instance.hideAds(AdsType.AD_TYPE_BANNER, 1);
			}
		}

		private void preloadAds()
		{
			if (this._instance.isAdTypeSupported(AdsType.AD_TYPE_BANNER))
			{
				this._instance.preloadAds(AdsType.AD_TYPE_BANNER, 1);
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
	}
}
