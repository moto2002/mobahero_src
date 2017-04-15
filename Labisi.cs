using System;

public class Labisi : Monster
{
	public bool IsFollow;

	public string EventID = string.Empty;

	protected override void OnInit(bool isRebirth = false)
	{
		base.OnInit(isRebirth);
		if (base.ParentUnit != null)
		{
			base.trans.rotation = base.ParentUnit.trans.rotation;
		}
		if (this.EventID == string.Empty)
		{
			return;
		}
		AudioMgr.Play(this.EventID, base.gameObject, false, false);
	}

	protected override void OnStart()
	{
		base.OnStart();
	}

	protected override void OnUpdate(float delta)
	{
		base.OnUpdate(delta);
		if (base.ParentUnit != null && this.IsFollow)
		{
			base.trans.position = base.ParentUnit.trans.position;
		}
	}

	public override void RealDeath(Units attacker)
	{
		base.RealDeath(attacker);
		if (base.mMecanim != null && base.mMecanim.animator != null)
		{
			base.mMecanim.animator.Play("death");
		}
	}
}
