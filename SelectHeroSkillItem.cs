using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class SelectHeroSkillItem : MonoBehaviour
{
	[SerializeField]
	private UITexture texture;

	public void ShowSKill(string str)
	{
		SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(str);
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById.skill_id);
		this.texture.mainTexture = ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false);
		this.texture.depth = 2;
	}

	public void ClearResources()
	{
		if (this.texture != null && this.texture.mainTexture != null)
		{
			Resources.UnloadAsset(this.texture.mainTexture);
		}
	}
}
