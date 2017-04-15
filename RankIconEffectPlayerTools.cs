using Com.Game.Utils;
using System;
using UnityEngine;

public class RankIconEffectPlayerTools : MonoBehaviour
{
	private string[] Path_lv = new string[]
	{
		string.Empty,
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal1_iron",
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal2_silver",
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal3_golden",
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal4_pt",
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal5_demand",
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal6_master",
		"Prefab/Effects/UIEffect/1031add/Fx_home_medal7_king"
	};

	[SerializeField]
	private UIEffectSort renderCtrl;

	[SerializeField]
	private EffectDelayActive activeCtrl;

	private int rankLevelEffectRecord;

	private GameObject effectPrefabRefBuffer;

	public UIPanel SortPanel
	{
		set
		{
			if (this.renderCtrl != null)
			{
				this.renderCtrl.panel = value;
			}
		}
	}

	public UIWidget SortWidget
	{
		set
		{
			if (this.renderCtrl != null)
			{
				this.renderCtrl.widgetInBack = value;
			}
		}
	}

	public int RankLevel
	{
		get
		{
			return this.rankLevelEffectRecord;
		}
		set
		{
			if (value < 1 || value > this.Path_lv.Length)
			{
				return;
			}
			if (this.rankLevelEffectRecord != value)
			{
				this.ClearEffect();
				this.rankLevelEffectRecord = value;
				this.LoadEffect(this.Path_lv[value]);
			}
		}
	}

	private void OnDestroy()
	{
		if (this.effectPrefabRefBuffer != null)
		{
			this.effectPrefabRefBuffer = null;
		}
	}

	private void LoadEffect(string _path)
	{
		GameObject gameObject = Resources.Load<GameObject>(_path);
		if (gameObject == null)
		{
			ClientLogger.Error(_path + " Rank Effect Path Error.");
			return;
		}
		this.effectPrefabRefBuffer = NGUITools.AddChild(base.gameObject, gameObject);
		this.effectPrefabRefBuffer.SetActive(false);
		this.renderCtrl = this.effectPrefabRefBuffer.GetComponent<UIEffectSort>();
		this.activeCtrl = this.effectPrefabRefBuffer.GetComponent<EffectDelayActive>();
	}

	private void ClearEffect()
	{
		if (this.effectPrefabRefBuffer == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(this.effectPrefabRefBuffer);
		this.effectPrefabRefBuffer = null;
	}

	public void SetEffectActive(bool _isActive, float _delay = 0f)
	{
		if (this.activeCtrl != null)
		{
			this.activeCtrl.SetActive(_isActive, _delay);
		}
	}

	public void SetScale(int picWidth)
	{
		float d = (float)picWidth / 434f;
		if (this.effectPrefabRefBuffer != null)
		{
			this.effectPrefabRefBuffer.transform.localScale = Vector3.one * d;
		}
	}
}
