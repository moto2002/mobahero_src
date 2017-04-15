using System;

namespace MobaProtocol
{
	public enum MobaMasterCode : byte
	{
		CheckMD5,
		Login,
		HulaiLogin,
		LoginByPlatformUid,
		LoginOut,
		GuestUpgradeToOfficial,
		LoginByChannelId,
		SessionData,
		Register = 10,
		GetPhoneCode = 12,
		CheckPhoneAndCode,
		FindMyAccountPasswd,
		ModifyAccountPasswd,
		SelectGameArea,
		GetGateServer = 20,
		GetAllGameServers = 101,
		GetClientVersion = 132,
		QueryUserById = 135,
		Setting,
		GuestUpgrade,
		SystemNotice,
		UpgradeUrl = 141,
		SyncGameData,
		GetUserProps = 145,
		DeviceInfo = 150,
		BindChannelId = 152,
		ResourceUrl = 158,
		Authenticate = 230
	}
}
