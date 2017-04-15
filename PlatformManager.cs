using System;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
	private PlatformInterface platform;

	public ManagerPlatformType ManagerPlatformType;

	public bool isLogined
	{
		get
		{
			return this.platform.isLogined();
		}
	}

	public string loginUin
	{
		get
		{
			return this.platform.LoginUin();
		}
	}

	public string SessionId
	{
		get
		{
			return this.platform.SessionId();
		}
	}

	public string UserNick
	{
		get
		{
			return this.platform.UserNick();
		}
	}

	public bool isMobilePlatform
	{
		get
		{
			return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
		}
	}

	private void Awake()
	{
		this.Init();
	}

	private void Start()
	{
	}

	public void Init()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
	}

	public void Login()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
		this.platform.Login();
	}

	public void GuestLogin()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
		this.platform.LoginEx();
	}

	public void Logout()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
		this.platform.Logout();
	}

	public void Pause()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
		this.platform.Pause();
	}

	public void ShowToolbar()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
		this.platform.ShowToolbar();
	}

	public void HideToolbar()
	{
		if (!this.isMobilePlatform)
		{
			return;
		}
		this.platform.HideToolbar();
	}

	public int GetCurrentLoginState()
	{
		return this.platform.GetCurrentLoginState();
	}

	public int StartPay(string cooOrderSerial, string productId, string productName, float productPrice, int productCount, string payDescription)
	{
		if (!this.isMobilePlatform)
		{
			return -1;
		}
		return this.platform.StartPay(Guid.NewGuid().ToString(), "2", "PEAR", 1f, 2, "GAMEZOON2");
	}
}
