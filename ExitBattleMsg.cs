using System;

public class ExitBattleMsg : GameMessage
{
	public ExitBattleMsg()
	{
		MessageManager.dispatch(this);
	}
}
