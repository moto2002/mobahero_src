using System;

public interface IGameEvent
{
	void OnGameStateChange(GameState oldState, GameState newState);
}
