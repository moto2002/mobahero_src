using System;
using UnityEngine;

public class NewHeroHpItem : MonoBehaviour
{
	public enum State
	{
		Fighting,
		Dead,
		Ready,
		NotReady
	}

	public SimpleHeroItem heroItem;

	public UISlider heroHp;

	public GameObject tagRangeNear;

	public GameObject tagRangeFar;

	public GameObject tagDead;

	public UIWidget grayMask;

	public GameObject tagReady;

	public void SetState(NewHeroHpItem.State state)
	{
		this.tagDead.SetActive(state == NewHeroHpItem.State.Dead);
		this.tagReady.SetActive(state == NewHeroHpItem.State.Ready);
		this.grayMask.alpha = ((state != NewHeroHpItem.State.Dead && state != NewHeroHpItem.State.NotReady) ? 1f : 0.3f);
		if (state == NewHeroHpItem.State.Ready || state == NewHeroHpItem.State.NotReady)
		{
			this.heroHp.value = 1f;
		}
		if (state == NewHeroHpItem.State.Dead)
		{
			this.heroHp.value = 0f;
		}
		this.SetAsGray(state == NewHeroHpItem.State.Dead);
	}

	public void SetAsGray(bool gray)
	{
		int type = (!gray) ? 1 : 8;
		this.heroItem.HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(type);
	}
}
