using System;
using UnityEngine;

public class SummonerSkillItem : MonoBehaviour
{
	[SerializeField]
	private UILabel S_Name;

	[SerializeField]
	private UISprite S_Mask;

	[SerializeField]
	private UITexture S_Texture;

	private SkillDataItem S_skillDataItem;

	private Callback<GameObject> clickItemCallBack;

	public Callback<GameObject> ClickSkillItemCallBack
	{
		private get
		{
			return this.clickItemCallBack;
		}
		set
		{
			this.clickItemCallBack = value;
		}
	}

	public void Init(SkillDataItem skillData)
	{
		this.S_skillDataItem = skillData;
		this.S_Name.text = skillData.Name;
		this.S_Texture.mainTexture = skillData.Texture;
		if (skillData.SummonerGrade < skillData.UnclockGrade)
		{
			this.S_Texture.material = CharacterDataMgr.instance.ReturnMaterialType(9);
		}
		else
		{
			this.S_Texture.material = CharacterDataMgr.instance.ReturnMaterialType(1);
		}
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickSummonerSkillItem);
	}

	public void SetChoseState(bool state)
	{
		UIWidget arg_53_0 = this.S_Mask;
		Color color = (!state) ? new Color32(191, 196, 199, 255) : new Color32(253, 196, 46, 255);
		this.S_Name.color = color;
		arg_53_0.color = color;
	}

	public bool IsCanChoice()
	{
		return this.S_skillDataItem != null && this.S_skillDataItem.SummonerGrade >= this.S_skillDataItem.UnclockGrade;
	}

	private void ClickSummonerSkillItem(GameObject obj = null)
	{
		if (this.ClickSkillItemCallBack != null)
		{
			this.ClickSkillItemCallBack(obj);
		}
	}
}
