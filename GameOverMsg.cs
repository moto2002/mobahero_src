using System;

public class GameOverMsg : GameMessage
{
	public GameOverMsg()
	{
		MessageManager.dispatch(this);
	}
}
