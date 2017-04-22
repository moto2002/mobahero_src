using System;
/// <summary>
/// unit控制类型
/// </summary>
public enum UnitControlType
{
	None = -1,
    /// <summary>
    /// 自由
    /// </summary>
	Free,
    /// <summary>
    /// 回放
    /// </summary>
	Replay,
    /// <summary>
    /// pvp自己控制
    /// </summary>
	PvpMyControl,
    /// <summary>
    /// pvp AI控制
    /// </summary>
	PvpAIControl,
    /// <summary>
    /// PVP网络控制
    /// </summary>
	PvpNetControl
}
