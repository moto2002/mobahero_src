using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class HomeController : BaseBuildingController
{
	public Transform RotationReference;

	public GameObject LoopFx;

	public GameObject ExplodeFx;

	public GameObject FlameFx;

	public bool TestMode;

	public override void OnCreate(Tower owner)
	{
		BaseBuildingController.SetActive(this.LoopFx, true);
		BaseBuildingController.SetActive(this.ExplodeFx, false);
		BaseBuildingController.SetActive(this.FlameFx, false);
	}

	public override void OnDamage(Units attacker, float damage)
	{
	}

	public override void OnDead()
	{
		BaseBuildingController.SetActive(this.LoopFx, false);
		BaseBuildingController.SetActive(this.ExplodeFx, true);
		this.ExplodeFx.transform.rotation = this.RotationReference.rotation;
		BaseBuildingController.SetActive(this.FlameFx, true);
	}

	[DebuggerHidden]
	public override IEnumerator OnPrewarm()
	{
		HomeController.<OnPrewarm>c__Iterator1D3 <OnPrewarm>c__Iterator1D = new HomeController.<OnPrewarm>c__Iterator1D3();
		<OnPrewarm>c__Iterator1D.<>f__this = this;
		return <OnPrewarm>c__Iterator1D;
	}
}
