using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using UnityEngine;

public class PVPHeroCard : MonoBehaviour
{
	private const float interval = 0.05f;

	private const float finishWait = 0.5f;

	[SerializeField]
	private UITexture tex_heroIcon;

	[SerializeField]
	private UILabel label_heroName;

	[SerializeField]
	private UILabel label_summonerName;

	[SerializeField]
	private UILabel label_progress;

	[SerializeField]
	private SelectHeroSkillItem com_selectHeroSkillItem;

	[SerializeField]
	private UITexture SkillTexture;

	[SerializeField]
	private UILabel delay;

	[SerializeField]
	private UISprite loadingFrame;

	private PVPCardInfo heroShowInfo;

	private int progress;

	private int displayProgress;

	private float lastUpdateTime;

	private bool finishFlag;

	public PVPCardInfo InitInfo
	{
		get
		{
			return this.heroShowInfo;
		}
		set
		{
			this.heroShowInfo = value;
			this.RefreshUI_init();
		}
	}

	public int Progress
	{
		get
		{
			return this.progress;
		}
		set
		{
			if (this.progress < value)
			{
				this.progress = value;
			}
		}
	}

	private void Update()
	{
		if (this.finishFlag)
		{
			return;
		}
		if (this.displayProgress != 100)
		{
			if (this.displayProgress < this.Progress && (this.lastUpdateTime == 0f || this.lastUpdateTime + 0.05f < Time.realtimeSinceStartup))
			{
				this.lastUpdateTime = Time.realtimeSinceStartup;
				if (this.progress > 100)
				{
					this.displayProgress = 100;
				}
				else if (this.Progress >= 100)
				{
					this.displayProgress = Math.Min(100, this.displayProgress + 10);
				}
				else
				{
					this.displayProgress = Math.Min(100, this.displayProgress + 1);
				}
				this.RefreshUI_progress();
			}
		}
		else if (this.lastUpdateTime + 0.5f < Time.realtimeSinceStartup)
		{
			this.finishFlag = true;
			if (this.InitInfo != null && this.InitInfo.OnLoadFinish != null)
			{
				this.InitInfo.OnLoadFinish(this);
			}
		}
	}

	private void RefreshUI_init()
	{
		PVPCardInfo initInfo = this.InitInfo;
		this.tex_heroIcon.mainTexture = initInfo.texture;
		this.label_heroName.text = initInfo.HeroName;
		this.label_summonerName.text = initInfo.SummonerName;
		if (initInfo.lastCharmRank <= 50)
		{
			this.label_summonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(initInfo.lastCharmRank);
		}
		this.label_progress.text = "0%";
		ToolsFacade.Instance.GetLoadingFrame(initInfo.RankFrame, this.loadingFrame);
		this.RefreshUI_summonerSkill();
	}

	private void RefreshUI_progress()
	{
		this.label_progress.text = this.displayProgress.ToString() + "%";
	}

	private void RefreshUI_summonerSkill()
	{
		string skillID = this.heroShowInfo.SkillID;
		bool flag = !string.IsNullOrEmpty(skillID);
		this.com_selectHeroSkillItem.gameObject.SetActive(flag);
		if (flag)
		{
			SysSummonersSkillVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersSkillVo>(skillID);
			SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(dataById.skill_id);
			this.SkillTexture.mainTexture = ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false);
		}
	}

	public void ShowDelay()
	{
		this.delay.gameObject.SetActive(true);
	}

	public void ClearResources()
	{
		if (this.heroShowInfo != null)
		{
			this.heroShowInfo.ClearResources();
			this.heroShowInfo = null;
		}
		if (this.com_selectHeroSkillItem != null)
		{
			this.com_selectHeroSkillItem.ClearResources();
		}
		if (this.tex_heroIcon != null && this.tex_heroIcon.mainTexture != null)
		{
			Resources.UnloadAsset(this.tex_heroIcon.mainTexture);
		}
		if (this.SkillTexture != null && this.SkillTexture.mainTexture != null)
		{
			Resources.UnloadAsset(this.SkillTexture.mainTexture);
		}
	}
}
