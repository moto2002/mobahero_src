using System;
using UnityEngine;

public class YzHero : MonoBehaviour
{
	public UIProgressBar hpbar;

	public UILabel lvLbl;

	public UITexture heroIcon;

	public UISprite headBg;

	public UILabel warnLbl;

	public UISprite warnBg;

	public UISprite[] stars;

	public UIGrid grid;

	public void SetHeroInfo(float percent, int lv, string hero, int grade, bool isWarn, int star)
	{
		this.hpbar.value = percent;
		this.lvLbl.text = lv.ToString();
		this.heroIcon.mainTexture = ResourceManager.Load<Texture>(hero, true, true, null, 0, false);
		this.warnLbl.enabled = isWarn;
		this.warnBg.enabled = isWarn;
		this.headBg.color = CharacterDataMgr.instance.GetFrameColor(grade, false);
		for (int i = 0; i < 5; i++)
		{
			this.stars[i].gameObject.SetActive(false);
		}
		this.grid.Reposition();
	}
}
