using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏计时器模块
/// </summary>
public class GameTimer : BaseGameModule
{
    /// <summary>
    /// 游戏各种类型的时间计时器缩放系数列表
    /// </summary>
	private static readonly Dictionary<string, float> _timeScales = new Dictionary<string, float>();
    /// <summary>
    /// 记录游戏进行的开始时间
    /// </summary>
	private float _startTimestamp;
    /// <summary>
    /// 记录游戏停止的开始时间
    /// </summary>
	private float _stopTimestamp;
    /// <summary>
    /// 游戏进行中的持续时间
    /// </summary>
	public float TotalPlayingSeconds
	{
		get
		{
			if (GameManager.IsPlaying())
			{
				return RealTime.time - this._startTimestamp;
			}
			return this._stopTimestamp - this._startTimestamp;
		}
	}
    /// <summary>
    /// 游戏状态变更，通知游戏计时器更新相关数值状态
    /// </summary>
    /// <param name="oldState"></param>
    /// <param name="newState"></param>
	public override void OnGameStateChange(GameState oldState, GameState newState)
	{
		if (newState == GameState.Game_Playing)  //如果进入状态为运行，记录运行的开始时间
		{
			this._startTimestamp = RealTime.time;
			this._stopTimestamp = 0f;
		}
		else if (newState == GameState.Game_Pausing) //如果进入状态为暂停，记录停止的开始时间
		{
			if (this._stopTimestamp == 0f)
			{
				this._stopTimestamp = RealTime.time;
			}
		}
		else if (newState == GameState.Game_Resume) //如果进入状态为暂停重新开始，运行时间要减去暂停的时间
		{
			this._startTimestamp += RealTime.time - this._stopTimestamp;
			this._stopTimestamp = 0f;
		}
		else if (newState == GameState.Game_Over && this._stopTimestamp == 0f) //如果进入游戏结束，并且暂停时间0，则暂停开始时间为当前
		{
			this._stopTimestamp = RealTime.time;
		}
	}
    /// <summary>
    /// 设置游戏的指定类型计时器的时间缩放系数
    /// </summary>
    /// <param name="useType"></param>
    /// <param name="scale"></param>
	public static void SetTimeScale(string useType, float scale)
	{
		GameTimer._timeScales[useType] = scale;
		float num = 1f;
		foreach (KeyValuePair<string, float> current in GameTimer._timeScales)
		{
			num *= current.Value; //累计计算所有时间缩放因子的叠加
		}
		Time.timeScale = num;  //所有时间缩放因子对时间的总缩放影响
	}
    /// <summary>
    /// 重置时间缩放因子，清楚游戏各种类型时间缩放因子的影响
    /// </summary>
	public static void NormalTimeScale()
	{
		Time.timeScale = 1f;
		GameTimer._timeScales.Clear();
	}
    /// <summary>
    /// 停止时间缩放????????
    /// </summary>
	public static void StopTimeScale()
	{
	}
}
