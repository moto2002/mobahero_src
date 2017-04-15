using System;

namespace MobaProtocol.Data
{
	public enum UnitType
	{
		None,
		Hero = 4,
		Monster = 8,
		Tower = 16,
		Home = 32,
		MapItem = 64,
		CreepBoss = 128,
		EyeItem = 256,
		AssistCreepBoss = 512,
		Soldier = 1024,
		FenShenHero = 2048,
		EyeUnit = 4096,
		SummonMonster = 8192,
		Pet = 16384,
		BoxUnit = 32768,
		LabisiUnit = 65536
	}
}
