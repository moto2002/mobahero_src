using System;

public class BaseGameModule : IGameModule
{
	public virtual void Init()
	{
	}

	public virtual void Uninit()
	{
	}

	public virtual void OnGameStateChange(GameState oldState, GameState newState)
	{
	}
}
