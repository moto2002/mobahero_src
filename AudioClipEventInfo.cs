using System;
using UnityEngine;

public class AudioClipEventInfo
{
	public string funcname = string.Empty;

	private float _time = 1f;

	private string _clipName = string.Empty;

	private float _volume = 1f;

	private bool _updated;

	public int priority;

	public bool deleted;

	public float time
	{
		get
		{
			return this._time;
		}
		set
		{
			if (value != this._time)
			{
				this._updated = true;
			}
			this._time = value;
		}
	}

	public string clipName
	{
		get
		{
			return this._clipName;
		}
		set
		{
			if (value != this._clipName)
			{
				this._updated = true;
			}
			this._clipName = value;
		}
	}

	public float volume
	{
		get
		{
			return this._volume;
		}
		set
		{
			if (value != this._volume)
			{
				this._updated = true;
			}
			this._volume = value;
		}
	}

	public bool updated
	{
		get
		{
			return this._updated;
		}
	}

	public void resetUpdated()
	{
		this._updated = false;
	}

	public string toStringParam()
	{
		return string.Format("{0},{1},{2},{3}", new object[]
		{
			this.time,
			this.clipName,
			this.volume,
			this.priority
		});
	}

	public void parseFromString(string str)
	{
		string[] array = str.Split(new char[]
		{
			','
		});
		if (array.Length != 4)
		{
			Debug.LogError("wrong format:" + str);
		}
		this.time = float.Parse(array[0]);
		this.clipName = array[1];
		this.volume = float.Parse(array[2]);
		this.priority = int.Parse(array[3]);
	}
}
