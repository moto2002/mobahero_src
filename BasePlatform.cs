using System;

public interface BasePlatform
{
	void Init();

	void Login();

	void ExitGame();

	void Feedback();

	bool IsLogined();

	void CheckUpdate();

	void SetDebugMode();

	void EnterPlatform();

	void HideToolbar();

	void ShowToolbar(int flag);

	void LoginOut(int autologin);

	void StartPay(string serverId, float money);

	void StartPay(string serial, string pid, string name, double price, double originalPrice, int count, string area);
}
