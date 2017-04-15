using System;

public interface IGameModule
{
	void Init();

	void Uninit();

	void OnGameStateChange(GameState oldState, GameState newState);
}
