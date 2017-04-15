using MobaClient;
using System;

namespace HomeState
{
	internal class HomeState_connectGame : HomeStateBase
	{
		public HomeState_connectGame() : base(HomeStateCode.HomeState_connectGame, MobaPeerType.C2GateServer)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.Connect();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void RegistHandler()
		{
			base.RegistHandler();
		}

		protected override void UnregistHandler()
		{
			base.UnregistHandler();
		}

		private void Connect()
		{
			NetWorkHelper.Instance.DisconnectFromMasterServer();
			NetWorkHelper.Instance.ConnectToGateServer();
		}

		protected override void OnConnected_game(MobaMessage msg)
		{
			HomeManager.Instance.ChangeState(HomeStateCode.HomeState_gameLoginAndRegist);
		}

		protected override void OnDisconnected_game(MobaMessage msg)
		{
		}
	}
}
