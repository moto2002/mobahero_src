using System;

public interface PlatformInterface
{
	void Init();

	void Exit();

	void Login();

	void Pause();

	void Logout();

	void LoginEx();

	bool isLogined();

	void ShowToolbar();

	void HideToolbar();

	void EnterPlatform();

	string LoginUin();

	string SessionId();

	string UserNick();

	int GetCurrentLoginState();

	int StartPay(string cooOrderSerial, string productId, string productName, float productPrice, int productCount, string payDescription);
}
