using System;

public class StartFightMsg : GameMessage
{
	public StartFightMsg()
	{
		MessageManager.dispatch(this);
	}
}
