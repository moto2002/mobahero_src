using System;

public class ToggleSoundMsg : GameMessage
{
	private bool _on;

	public bool on
	{
		get
		{
			return this._on;
		}
	}

	public ToggleSoundMsg(bool on)
	{
		this._on = on;
		MessageManager.dispatch(this);
	}
}
