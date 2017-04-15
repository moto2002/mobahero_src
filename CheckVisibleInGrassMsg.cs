using System;

public class CheckVisibleInGrassMsg : GameMessage
{
	private Units _u;

	public Units unit
	{
		get
		{
			return this._u;
		}
	}

	public CheckVisibleInGrassMsg(Units requester)
	{
		this._u = requester;
		MessageManager.dispatch(this);
	}
}
