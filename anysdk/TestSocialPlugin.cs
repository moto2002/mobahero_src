using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestSocialPlugin : MonoBehaviour
	{
		private AnySDKSocial _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKSocial.getInstance();
			this._instance.setListener(this, "SocialExternalCall");
		}

		private void SocialExternalCall(string msg)
		{
			Debug.Log("SocialExternalCall(" + msg + ")");
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
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "submitScore"))
			{
				this.submitScore();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "showLeaderboard"))
			{
				this.showLeaderboard();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "showAchievements"))
			{
				this.showAchievements();
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "unlockAchievement"))
			{
				this.unlockAchievement();
			}
			if (GUI.Button(new Rect(5f, 305f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestSocialPlugin"));
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

		private void submitScore()
		{
			AnySDKSocial.getInstance().submitScore("1", 100L);
		}

		private void showLeaderboard()
		{
			AnySDKSocial.getInstance().showLeaderboard("1");
		}

		private void showAchievements()
		{
			AnySDKSocial.getInstance().showAchievements();
		}

		private void unlockAchievement()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["xx1"] = "xx1";
			dictionary["xx2"] = "xx2";
			AnySDKSocial.getInstance().unlockAchievement(dictionary);
		}
	}
}
