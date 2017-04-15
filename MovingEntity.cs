using System;

public class MovingEntity : Entity
{
	protected override void OnCreate()
	{
		this.isMovingEntity = true;
		this.moveController = base.AddUnitComponent<MoveController>();
		base.OnCreate();
	}
}
