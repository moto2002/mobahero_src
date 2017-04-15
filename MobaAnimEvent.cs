using System;

public class MobaAnimEvent
{
	public int tag_hash;

	public float nTime;

	public MobaAnimEvent(int tag_hash, float time)
	{
		this.tag_hash = tag_hash;
		this.nTime = time;
	}
}
