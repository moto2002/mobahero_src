using System;
/// <summary>
/// 游戏模块基类，方法接口空实现
/// </summary>
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
