using System;
using System.Collections.Generic;
using UnityEngine;

namespace anysdk
{
	public class TestPushPlugin : MonoBehaviour
	{
		private AnySDKPush _instance;

		private void Awake()
		{
		}

		private void Start()
		{
			this._instance = AnySDKPush.getInstance();
			this._instance.setListener(this, "PushExternalCall");
		}

		private void PushExternalCall(string msg)
		{
			Debug.Log("PushExternalCall(" + msg + ")");
			Dictionary<string, string> dictionary = AnySDKUtil.stringToDictionary(msg);
			int num = Convert.ToInt32(dictionary["code"]);
			string text = dictionary["msg"];
			int num2 = num;
			if (num2 != 0)
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
			if (GUI.Button(new Rect(5f, 5f, (float)(Screen.width - 10), 70f), "startPush"))
			{
				this.startPush();
			}
			if (GUI.Button(new Rect(5f, 80f, (float)(Screen.width - 10), 70f), "closePush"))
			{
				this.closePush();
			}
			if (GUI.Button(new Rect(5f, 155f, (float)(Screen.width - 10), 70f), "setAlias"))
			{
				this.setAlias("AnySDK");
			}
			if (GUI.Button(new Rect(5f, 230f, (float)(Screen.width - 10), 70f), "delAlias"))
			{
				this.delAlias("AnySDK");
			}
			if (GUI.Button(new Rect(5f, 305f, (float)(Screen.width - 10), 70f), "setTags"))
			{
				this.setTags(new List<string>
				{
					"easy",
					"fast"
				});
			}
			if (GUI.Button(new Rect(5f, 380f, (float)(Screen.width - 10), 70f), "delTags"))
			{
				this.delTags(new List<string>
				{
					"easy",
					"fast"
				});
			}
			if (GUI.Button(new Rect(5f, 455f, (float)(Screen.width - 10), 70f), "return"))
			{
				UnityEngine.Object.Destroy(base.GetComponent("TestPushPlugin"));
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

		private void startPush()
		{
			AnySDKPush.getInstance().startPush();
		}

		private void closePush()
		{
			AnySDKPush.getInstance().closePush();
		}

		private void setAlias(string alias)
		{
			AnySDKPush.getInstance().setAlias(alias);
		}

		private void delAlias(string alias)
		{
			AnySDKPush.getInstance().delAlias(alias);
		}

		private void setTags(List<string> tags)
		{
			AnySDKPush.getInstance().setTags(tags);
		}

		private void delTags(List<string> tags)
		{
			AnySDKPush.getInstance().delTags(tags);
		}
	}
}
