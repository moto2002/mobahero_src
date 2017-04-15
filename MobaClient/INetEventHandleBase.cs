using ExitGames.Client.Photon;
using System;

namespace MobaClient
{
	public interface INetEventHandleBase
	{
		void OnResponse(OperationResponse operationResponse);
	}
}
