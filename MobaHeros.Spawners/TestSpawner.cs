using System;

namespace MobaHeros.Spawners
{
	public class TestSpawner : GameSpawner
	{
		public static bool OpenTestView;

		public override bool IsReady
		{
			get
			{
				return true;
			}
		}

		protected override void OnRequestSpawn()
		{
		}

		protected override void OnSpawnStop()
		{
		}

		protected override void OpenViews()
		{
			this.OpenTestModeView();
		}

		protected override void CloseViews()
		{
			this.CloseTestModeView();
		}

		protected override BaseVictoryChecker CreateVictoryChecker()
		{
			return new NeverEndVictoryChecker();
		}

		private void OpenTestModeView()
		{
			if (!TestSpawner.OpenTestView)
			{
				return;
			}
		}

		private void CloseTestModeView()
		{
			if (!TestSpawner.OpenTestView)
			{
				return;
			}
		}

		private void OnSpawnHero(bool isopen)
		{
			if (isopen)
			{
				this.OpenViews();
			}
			else
			{
				MapManager.Instance.ClearHeros();
				this.CloseViews();
			}
		}

		private void OnSpawnMonster(bool isopen)
		{
			if (!isopen)
			{
				MapManager.Instance.ClearMonsters();
			}
		}

		private void OnSpawnTower(bool isopen)
		{
			if (!isopen)
			{
				MapManager.Instance.ClearTowers();
			}
		}

		private void OnSpawnBuffItem(bool isopen)
		{
			if (isopen)
			{
			}
		}
	}
}
