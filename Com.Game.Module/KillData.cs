using System;

namespace Com.Game.Module
{
	internal class KillData
	{
		public int myMonsterKillNum;

		public int myHeroKillNum;

		public int myTowerKillNum;

		public int enemyMonsterKillNum;

		public int enemyHeroKillNum;

		public int enemyTowerKillNum;

		public int attrMax;

		public int killMonsterAddition;

		public int killTowerAddition;

		public int killHeroAddition;

		public int interval;

		public int timeAddtion;

		private float _myCurrentPercent;

		private float _enemyCurrentPercent;

		public float timePercent;

		public float myKillPercent;

		public float enemyKillPercent;

		public float myCurrentPercent
		{
			get
			{
				this._myCurrentPercent = this.myKillPercent + this.timePercent;
				if (this._myCurrentPercent > (float)this.attrMax)
				{
					return (float)this.attrMax;
				}
				return this._myCurrentPercent;
			}
			set
			{
				this._myCurrentPercent = value;
			}
		}

		public float enemyCurrentPercent
		{
			get
			{
				this._enemyCurrentPercent = this.enemyKillPercent + this.timePercent;
				if (this._enemyCurrentPercent > (float)this.attrMax)
				{
					return (float)this.attrMax;
				}
				return this._enemyCurrentPercent;
			}
			set
			{
				this._enemyCurrentPercent = value;
			}
		}

		public KillData()
		{
			this.myMonsterKillNum = 0;
			this.myHeroKillNum = 0;
			this.myTowerKillNum = 0;
			this.enemyHeroKillNum = 0;
			this.enemyMonsterKillNum = 0;
			this.enemyTowerKillNum = 0;
			this.attrMax = 0;
			this.killMonsterAddition = 0;
			this.killTowerAddition = 0;
			this.killHeroAddition = 0;
			this.interval = 0;
			this.timeAddtion = 0;
			this.myCurrentPercent = 0f;
			this.enemyCurrentPercent = 0f;
			this.timePercent = 0f;
			this.myKillPercent = 0f;
			this.enemyKillPercent = 0f;
		}
	}
}
