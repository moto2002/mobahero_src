using System;

public class GameTrigger : VTrigger
{
	public GameTrigger(int id) : base(id)
	{
		this.trigger_id = id;
		this.trigger_type = 1;
	}
}
