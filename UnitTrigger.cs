using System;

public class UnitTrigger : VTrigger
{
	public int teamtype;

	public int unit_id;

	public string unit_tag;

	public UnitTrigger(int id) : base(id)
	{
		this.trigger_id = id;
		this.trigger_type = 2;
	}
}
