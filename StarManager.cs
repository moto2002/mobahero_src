using System;

public class StarManager : BaseGameModule
{
	private const int DEFAULT_STAR_NUM = 3;

	private int _starNum;

	public int GetStartNum(bool isWin = true)
	{
		if (isWin)
		{
			int userHeroDeadCount = StatisticsManager.userHeroDeadCount;
			if (userHeroDeadCount <= 0)
			{
				this._starNum = 2;
			}
			else
			{
				this._starNum = 1;
			}
			if (StatisticsManager.userHeroFirstBlood)
			{
				this._starNum++;
			}
		}
		else
		{
			this._starNum = 0;
		}
		return this._starNum;
	}

	public override void Init()
	{
		this._starNum = 0;
		StatisticsManager.userHeroFirstBlood = false;
		StatisticsManager.canSetHeroFirstBlood = true;
		StatisticsManager.FirstBloodUnitUniqueId = 0;
		StatisticsManager.ClearDeadCount();
	}

	public override void Uninit()
	{
		this._starNum = 0;
		StatisticsManager.userHeroFirstBlood = false;
		StatisticsManager.canSetHeroFirstBlood = true;
		StatisticsManager.FirstBloodUnitUniqueId = 0;
		StatisticsManager.ClearDeadCount();
	}

	public bool isPlayerNeverDied()
	{
		return StatisticsManager.userHeroDeadCount <= 0;
	}

	public bool isPlayerGetFirstBlood()
	{
		return StatisticsManager.userHeroFirstBlood;
	}
}
