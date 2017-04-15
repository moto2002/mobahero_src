using System;
using UnityEngine;

public class UIHUDView : MonoBehaviour
{
	public UILabel level_text;

	public UILabel name_text;

	public UITexture avatar_icon;

	public UISprite avatar_frame;

	public Transform Summoner;

	private void Awake()
	{
		if (this.level_text == null)
		{
			this.level_text = base.transform.Find("Anchor/GradeBack/gradeNumber").GetComponent<UILabel>();
		}
		if (this.name_text == null)
		{
			this.name_text = base.transform.Find("Anchor/NameBack/name").GetComponent<UILabel>();
		}
		if (this.Summoner == null)
		{
			this.Summoner = base.transform.Find("Anchor/Summoner");
		}
		if (this.avatar_icon == null)
		{
			this.avatar_icon = this.Summoner.GetComponent<UITexture>();
		}
		if (this.avatar_frame == null)
		{
			this.avatar_frame = this.Summoner.transform.Find("SummonerFrame").GetComponent<UISprite>();
		}
	}
}
