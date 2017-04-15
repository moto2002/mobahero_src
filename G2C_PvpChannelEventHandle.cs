using MobaClient;
using System;

public class G2C_PvpChannelEventHandle : ClientMsgRecv, INetEventHandleBase
{
	public G2C_PvpChannelEventHandle() : base(string.Empty)
	{
	}

	protected override void RegistCmds()
	{
	}
}
