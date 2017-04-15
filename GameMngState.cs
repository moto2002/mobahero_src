using System;

public class GameMngState
{
	protected GameState state;

	private GameManager gameMng;

	public GameState State
	{
		get
		{
			return this.state;
		}
	}

	public virtual void Begin()
	{
	}

	public virtual void End()
	{
	}

	public virtual void Update()
	{
	}
}
