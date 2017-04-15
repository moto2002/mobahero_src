using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : BaseGameModule
{
	private static readonly Dictionary<string, float> _timeScales = new Dictionary<string, float>();

	private float _startTimestamp;

	private float _stopTimestamp;

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

	public override void OnGameStateChange(GameState oldState, GameState newState)
	{
		if (newState == GameState.Game_Playing)
		{
			this._startTimestamp = RealTime.time;
			this._stopTimestamp = 0f;
		}
		else if (newState == GameState.Game_Pausing)
		{
			if (this._stopTimestamp == 0f)
			{
				this._stopTimestamp = RealTime.time;
			}
		}
		else if (newState == GameState.Game_Resume)
		{
			this._startTimestamp += RealTime.time - this._stopTimestamp;
			this._stopTimestamp = 0f;
		}
		else if (newState == GameState.Game_Over && this._stopTimestamp == 0f)
		{
			this._stopTimestamp = RealTime.time;
		}
	}

	public static void SetTimeScale(string useType, float scale)
	{
		GameTimer._timeScales[useType] = scale;
		float num = 1f;
		foreach (KeyValuePair<string, float> current in GameTimer._timeScales)
		{
			num *= current.Value;
		}
		Time.timeScale = num;
	}

	public static void NormalTimeScale()
	{
		Time.timeScale = 1f;
		GameTimer._timeScales.Clear();
	}

	public static void StopTimeScale()
	{
	}
}
