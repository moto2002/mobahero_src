using CsSdk;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoRec : MonoBehaviour
{
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
		private DemoRec outer;

		public LocalCallBack(DemoRec demo)
		{
			this.outer = demo;
		}

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

	private InputField mTitle;

	private InputField mTag;

	private Button mStartLive;

	private Button mGetLiveList;

	private string gameUid = "20000000797";

	private string gameToken = "96e79218965eb72c92a549dd5a330112";

	private string phone = "15068822229";

	private new string name = "Zhusong";

	private string extraData = "android_unity_rec_test";

	private int orentation = 1;

	private int resolution = 1;

	private CsRecInterface rec = new CsRecInterface();

	private CustomUIOpHelper helper;

	private void Start()
	{
		this.mTitle = GameObject.Find("title").gameObject.GetComponent<InputField>();
		this.mTag = GameObject.Find("tag").gameObject.GetComponent<InputField>();
		foreach (string current in new List<string>
		{
			"landscape",
			"portrait",
			"sd",
			"hd",
			"uhd"
		})
		{
			GameObject toggleObj = GameObject.Find(current);
			Toggle component = toggleObj.GetComponent<Toggle>();
			component.onValueChanged.AddListener(delegate(bool check)
			{
				this.onValueChanged(toggleObj, check);
			});
		}
		GameObject startLiveObj = GameObject.Find("startLive");
		this.mStartLive = startLiveObj.GetComponent<Button>();
		this.mStartLive.onClick.AddListener(delegate
		{
			this.OnClick(startLiveObj);
		});
		GameObject recordObj = GameObject.Find("recordfile");
		Button component2 = recordObj.GetComponent<Button>();
		component2.onClick.AddListener(delegate
		{
			this.OnClick(recordObj);
		});
		GameObject videomanagerObj = GameObject.Find("videomanager");
		Button component3 = videomanagerObj.GetComponent<Button>();
		component3.onClick.AddListener(delegate
		{
			this.OnClick(videomanagerObj);
		});
		GameObject stopObj = GameObject.Find("stop");
		Button component4 = stopObj.GetComponent<Button>();
		component4.onClick.AddListener(delegate
		{
			this.OnClick(stopObj);
		});
		this.rec.init();
		this.helper = this.rec.getCustomUIOpHelper();
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
	}

	public void onValueChanged(GameObject sender, bool check)
	{
		if (!check)
		{
			return;
		}
		string text = sender.name;
		switch (text)
		{
		case "landscape":
			this.orentation = 1;
			break;
		case "portrait":
			this.orentation = 0;
			break;
		case "sd":
			this.resolution = 0;
			break;
		case "hd":
			this.resolution = 1;
			break;
		case "uhd":
			this.resolution = 2;
			break;
		}
	}

	public void OnClick(GameObject sender)
	{
		string text = sender.name;
		switch (text)
		{
		case "startLive":
		{
			string text2 = this.mTitle.text;
			string text3 = this.mTag.text;
			if (this.helper != null)
			{
				this.helper.startLive(text3, text2, this.gameUid, this.gameToken, this.phone, this.name, this.extraData, this.orentation, this.resolution, new DemoRec.OnLiveCallBack());
			}
			else
			{
				this.rec.startLive(text3, text2, this.gameUid, this.gameToken, this.phone, this.name, this.extraData, this.orentation, this.resolution, new DemoRec.OnLiveCallBack());
			}
			return;
		}
		case "recordfile":
		{
			string text4 = this.mTag.text;
			if (this.helper != null)
			{
				this.helper.startRecordFile(text4, this.gameUid, this.gameToken, this.phone, this.name, this.extraData, this.orentation, this.resolution, new DemoRec.LocalCallBack(this));
			}
			else
			{
				this.rec.startRecordFile(text4, this.gameUid, this.gameToken, this.phone, this.name, this.extraData, this.orentation, this.resolution, new DemoRec.LocalCallBack(this));
			}
			return;
		}
		case "videomanager":
		{
			string text5 = this.mTag.text;
			this.rec.openVideoManager(text5, this.gameUid, this.gameToken, this.extraData);
			return;
		}
		case "stop":
			if (this.helper != null)
			{
				this.helper.stop();
			}
			return;
		}
		Debug.LogError("none");
	}
}
