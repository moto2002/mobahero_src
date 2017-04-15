using System;

public class Pet : Monster
{
	private float fIdleTimer;

	protected override void OnUpdate(float delta)
	{
		base.OnUpdate(delta);
		if (this.moveController != null && !this.moveController.isMoving)
		{
			this.fIdleTimer += delta;
		}
		else
		{
			this.fIdleTimer = 0f;
		}
		if (this.fIdleTimer > 3f)
		{
			this.fIdleTimer = 0f;
			if (this.moveController != null && this.animController != null)
			{
				this.animController.RandomIdle();
			}
		}
	}
}
