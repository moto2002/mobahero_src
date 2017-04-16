using System;
/// <summary>
/// 游戏模块接口
/// </summary>
public interface IGameModule
{
    /// <summary>
    /// 初始化
    /// </summary>
	void Init();
    /// <summary>
    /// 销毁
    /// </summary>
	void Uninit();
    /// <summary>
    /// 游戏状态变化回调接口
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
	void OnGameStateChange(GameState oldState, GameState newState);
}
