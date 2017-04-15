using System;

public class TimerTrigger : VTrigger
{
	public float duration_time;

	public float interval_time;

	public bool is_period;

	public bool is_start;

	public float start_time = -1f;

	public TimerTrigger(int id) : base(id)
	{
		this.trigger_id = id;
		this.trigger_type = 3;
	}
}
