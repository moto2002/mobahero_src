using System;

namespace MobaProtocol.Data
{
	public enum PvpUseSkillErrorCode : byte
	{
		OK,
		InStateZhimang,
		InStateChenmo,
		InCd = 20,
		LackMp,
		LackHp,
		NoTarget = 30,
		OutOfRange = 40,
		Other = 100
	}
}
