using System;
using System.Collections.Generic;

public class AkMultiPosEvent
{
	public List<AkAmbient> list = new List<AkAmbient>();

	public bool eventIsPlaying;

	public void FinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
	{
		this.eventIsPlaying = false;
	}
}
