using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SkinDrawer : MobaMono
{
	private const int WIDTH = 1000;

	private const int HEIGHT = 100;

	private const int DRAG_THRESHOLD = 400;

	private const int DEPTH_INTER = 1;

	[SerializeField]
	private UILabel _skinNameLabel;

	[SerializeField]
	private GameObject _buyBtn;

	[SerializeField]
	private Transform cost;

	[SerializeField]
	private Transform wear;

	[SerializeField]
	private Transform source;

	[SerializeField]
	private Transform switchBth;

	[SerializeField]
	private UILabel content;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private UILabel wearLabel;

	private List<GoodsData> list_goodsdata = new List<GoodsData>();

	private List<Skin> _skins;

	private List<Vector3> _poses;

	private List<float> _alphas;

	private List<float> _scales;

	private List<Vector3> _oldPoses;

	private float _grossDrag;

	private float _lerp;

	private bool iswear;

	private string strSwitch;

	private int activityID;

	private long heroid = Singleton<PropertyView>.Instance.HeroID;

	private int BASE_DEPTH = 120;

	private Skin _copy;

	private Action<int> _onSelect;

	private string _heroName;

	public event Action<int> onSelectionChanged
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.onSelectionChanged = (Action<int>)Delegate.Combine(this.onSelectionChanged, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.onSelectionChanged = (Action<int>)Delegate.Remove(this.onSelectionChanged, value);
		}
	}

	public event Action<int> onBuyBtn
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.onBuyBtn = (Action<int>)Delegate.Combine(this.onBuyBtn, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.onBuyBtn = (Action<int>)Delegate.Remove(this.onBuyBtn, value);
		}
	}

	public event Action<Transform, bool> onWearBtn
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.onWearBtn = (Action<Transform, bool>)Delegate.Combine(this.onWearBtn, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.onWearBtn = (Action<Transform, bool>)Delegate.Remove(this.onWearBtn, value);
		}
	}

	public bool isWear
	{
		get
		{
			return this.iswear;
		}
		set
		{
			this.iswear = value;
		}
	}

	public bool animLerping
	{
		get
		{
			foreach (Skin current in this._skins)
			{
				if (current.animLerping)
				{
					return true;
				}
			}
			return false;
		}
	}

	public int selection
	{
		get
		{
			return this._skins[0].getSkinInfo().skinId;
		}
	}

	public void ClearResources()
	{
		if (this._skins != null && this._skins.Count > 0)
		{
			for (int i = 0; i < this._skins.Count; i++)
			{
				Skin skin = this._skins[i];
				if (skin != null)
				{
					skin.ClearResources();
				}
			}
			this._skins.Clear();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.animLerping)
		{
			return;
		}
		this._grossDrag += delta.x;
		this._lerp = this._grossDrag / 400f;
		this.applyLerp();
	}

	private void applyLerp()
	{
		if (this._skins == null || this._skins.Count == 0)
		{
			return;
		}
		if (this._poses == null || this._poses.Count == 0)
		{
			return;
		}
		this._lerp = Mathf.Clamp(this._lerp, -0.999f, 0.999f);
		if (this._lerp < 0f)
		{
			int num = Mathf.Max(this._poses.Count - 1, 0);
			int index = Mathf.Max(this._poses.Count - 2, 0);
			this._copy.fromPos = this._poses[num];
			this._copy.toPos = this._poses[index];
			this._copy.fromScale = this._scales[num];
			this._copy.toScale = this._scales[index];
			this._copy.fromAlpha = 0f;
			this._copy.toAlpha = this._alphas[index];
			this._copy.setSkinInfo(this._skins[0].getSkinInfo(), this.IsPossess(this._skins[0].getSkinInfo().skinId), this.IsWear(this.heroid, this._skins[0].getSkinInfo().skinId));
			this._copy.depth = this._skins[this._skins.Count - 1].depth - 1;
			foreach (Skin current in this._skins)
			{
				num = this._skins.IndexOf(current) + 1;
				current.fromPos = this._poses[num];
				current.fromScale = this._scales[num];
				current.fromAlpha = this._alphas[num];
				index = num - 1;
				current.toScale = this._scales[index];
				current.toPos = this._poses[index];
				current.toAlpha = this._alphas[index];
			}
		}
		else if (this._lerp >= 0f)
		{
			this._copy.fromPos = this._poses[0];
			this._copy.toPos = this._poses[1];
			this._copy.fromAlpha = 0f;
			this._copy.toAlpha = 1f;
			this._copy.fromScale = 1f;
			this._copy.toScale = 1f;
			this._copy.setSkinInfo(this._skins[this._skins.Count - 1].getSkinInfo(), this.IsPossess(this._skins[this._skins.Count - 1].getSkinInfo().skinId), this.IsWear(this.heroid, this._skins[this._skins.Count - 1].getSkinInfo().skinId));
			this._copy.depth = this._skins[0].depth + 1;
			foreach (Skin current2 in this._skins)
			{
				int num2 = this._skins.IndexOf(current2) + 1;
				int index2 = num2 + 1;
				current2.fromPos = this._poses[num2];
				current2.fromScale = this._scales[num2];
				current2.fromAlpha = this._alphas[num2];
				current2.toScale = this._scales[index2];
				current2.toPos = this._poses[index2];
				current2.toAlpha = this._alphas[index2];
			}
		}
		this._copy.applyLerp(this._lerp);
		foreach (Skin current3 in this._skins)
		{
			current3.applyLerp(this._lerp);
		}
	}

	private void onPress()
	{
		this._grossDrag = 0f;
		this._lerp = 0f;
	}

	private void onRelease()
	{
		this.checkAnim();
	}

	public void OnChangeAlpha()
	{
		foreach (Skin current in this._skins)
		{
			int index = this._skins.IndexOf(current) + 1;
			current.trans.localPosition = this._poses[index];
			current.trans.localScale = new Vector3(this._scales[index], this._scales[index], 1f);
			current.getComponent<TweenAlpha>(true).from = this._alphas[index];
			current.getComponent<TweenAlpha>(true).to = this._alphas[index];
		}
	}

	private void checkAnim()
	{
		if (this._lerp == 0f)
		{
			return;
		}
		if (Mathf.Abs(this._lerp) < 0.3f)
		{
			this._copy.animToOrigin(0.06f);
			foreach (Skin current in this._skins)
			{
				current.animToOrigin(0.06f);
			}
		}
		else
		{
			this._copy.animToTarget(0.06f);
			foreach (Skin current2 in this._skins)
			{
				current2.animToTarget(0.06f);
			}
			if (this._lerp < -0.3f)
			{
				Skin copy = this._skins[0];
				this._skins.RemoveAt(0);
				this._skins.Add(this._copy);
				this._copy = copy;
			}
			else if (this._lerp > 0.3f)
			{
				Skin copy2 = this._skins[this._skins.Count - 1];
				this._skins.RemoveAt(this._skins.Count - 1);
				this._skins.Insert(0, this._copy);
				this._copy = copy2;
			}
			this.selectSkin();
		}
		this._grossDrag = 0f;
		this._lerp = 0f;
		this.resetDepth();
		if (this._onSelect != null)
		{
			this._onSelect(this.selection);
		}
	}

	private void selectSkin()
	{
		this.SetBuyBtnState(this.selection);
		if (this.onSelectionChanged != null)
		{
			this.onSelectionChanged(this.selection);
		}
	}

	private void resetDepth()
	{
		foreach (Skin current in this._skins)
		{
			int idx = this._skins.IndexOf(current) + 1;
			current.depth = this.getDepth(idx);
		}
	}

	private int getDepth(int idx)
	{
		this.BASE_DEPTH = base.transform.GetComponentInParent<UIPanel>().depth;
		if (this.BASE_DEPTH > 120)
		{
			this.BASE_DEPTH = base.transform.parent.parent.parent.GetComponent<UIPanel>().depth;
		}
		return this._skins.Count - idx + 1 + this.BASE_DEPTH;
	}

	public void setOnSelect(Action<int> onSelect)
	{
		this._onSelect = onSelect;
	}

	public object ShallowCopy()
	{
		return base.MemberwiseClone();
	}

	public void setHeroName(string heroId, long heroid, int skinId = 0, bool isSame = false)
	{
		if (Singleton<PvpSelectHeroView>.Instance.gameObject != null && Singleton<PvpSelectHeroView>.Instance.gameObject.activeInHierarchy)
		{
			Singleton<PropertyView>.Instance.HeroNpc = heroId;
		}
		this.list_goodsdata.Clear();
		this.list_goodsdata.AddRange(ModelManager.Instance.Get_ShopGoodsList());
		if (this.list_goodsdata == null)
		{
			return;
		}
		string price = null;
		List<GoodsData> list = new List<GoodsData>();
		list = this.list_goodsdata.FindAll((GoodsData obj) => obj.Type == 2);
		List<GoodsData> list2 = new List<GoodsData>();
		if (list == null)
		{
			return;
		}
		list2 = list.FindAll((GoodsData obj) => obj.ElementId == skinId.ToString());
		if (list2 == null)
		{
			return;
		}
		if (list2.Count == 0)
		{
			price = null;
		}
		else
		{
			price = this.StringPrice(list2);
		}
		this.SetWearBtnState(heroid, skinId);
		this.SetBuyBtnState(skinId);
		this._heroName = heroId;
		if (this._skins == null)
		{
			this._skins = new List<Skin>();
		}
		else
		{
			foreach (Skin current in this._skins)
			{
				UnityEngine.Object.Destroy(current.gameObject);
			}
			if (this._copy != null)
			{
				UnityEngine.Object.Destroy(this._copy.gameObject);
			}
			this._skins.Clear();
		}
		List<SkinInfo> list3 = new List<SkinInfo>();
		List<SkinInfo> list4 = new List<SkinInfo>();
		SysHeroMainVo sysHeroMainVo = BaseDataMgr.instance.GetDicByType<SysHeroMainVo>()[heroId] as SysHeroMainVo;
		SysGameResVo sysGameResVo = BaseDataMgr.instance.GetDicByType<SysGameResVo>()[sysHeroMainVo.Loading_icon] as SysGameResVo;
		Texture tex = CachedRes.getTex(sysGameResVo.path);
		list4.Add(new SkinInfo(tex, this.GetSkinPrice(0)));
		if (sysHeroMainVo.skin_id != "[]")
		{
			int[] stringToInt = StringUtils.GetStringToInt(sysHeroMainVo.skin_id, ',');
			if (skinId == 0)
			{
				list3.Add(new SkinInfo(tex, this.GetSkinPrice(0)));
				int[] array = stringToInt;
				int id;
				for (int i = 0; i < array.Length; i++)
				{
					id = array[i];
					if (list != null)
					{
						list2 = list.FindAll((GoodsData obj) => obj.ElementId == id.ToString());
						if (list2 == null || list2.Count == 0)
						{
							price = null;
						}
						else
						{
							price = this.StringPrice(list2);
						}
					}
					list3.Add(new SkinInfo(id, this.ParsePrice(price), SkinPanel.IsPossessSkinId(id) || SkinPanel.IsWearSkin(heroid, id)));
				}
			}
			else
			{
				list3.Add(new SkinInfo(skinId, this.ParsePrice(price), SkinPanel.IsPossessSkinId(skinId) || SkinPanel.IsWearSkin(heroid, skinId)));
				int[] array2 = stringToInt;
				int it2;
				for (int j = 0; j < array2.Length; j++)
				{
					it2 = array2[j];
					if (list != null)
					{
						list2 = list.FindAll((GoodsData obj) => obj.ElementId == it2.ToString());
						if (list2 == null || list2.Count == 0)
						{
							price = null;
						}
						else
						{
							price = this.StringPrice(list2);
						}
					}
					if (it2 > skinId)
					{
						list3.Add(new SkinInfo(it2, this.ParsePrice(price), SkinPanel.IsPossessSkinId(it2) || SkinPanel.IsWearSkin(heroid, it2)));
					}
				}
				if (!list3.Contains(list3.Find((SkinInfo obj) => obj.skinId == 0)))
				{
					list3.Add(new SkinInfo(tex, this.GetSkinPrice(0)));
				}
				int[] array3 = stringToInt;
				int it;
				for (int k = 0; k < array3.Length; k++)
				{
					it = array3[k];
					if (list != null)
					{
						list2 = list.FindAll((GoodsData obj) => obj.ElementId == it.ToString());
						if (list2 == null || list2.Count == 0)
						{
							price = null;
						}
						else
						{
							price = this.StringPrice(list2);
						}
					}
					if (it < skinId)
					{
						list3.Add(new SkinInfo(it, this.ParsePrice(price), SkinPanel.IsPossessSkinId(it) || SkinPanel.IsWearSkin(heroid, it)));
					}
				}
			}
		}
		if (!list3.Contains(list3.Find((SkinInfo obj) => obj.skinId == 0)))
		{
			list3.Add(new SkinInfo(tex, this.GetSkinPrice(0)));
		}
		this._copy = NGUITools.AddChild(base.gameObject, Skin.prefab).GetComponent<Skin>();
		List<Skin> list5 = new List<Skin>();
		for (int l = 0; l < list3.Count; l++)
		{
			if (!(ToolsFacade.ServerCurrentTime < new DateTime(2016, 12, 19, 0, 0, 0)) || list3[l].skinId != 102802)
			{
				Skin component = NGUITools.AddChild(base.gameObject, Skin.prefab).GetComponent<Skin>();
				SkinInfo skinInfo = list3[l];
				component.name = "texture:" + l;
				component.setSkinInfo(skinInfo, this.IsPossess(skinInfo.skinId), this.IsWear(heroid, skinInfo.skinId));
				component.toAlpha = 1f;
				list5.Add(component);
			}
		}
		if (!isSame)
		{
			base.gameObject.transform.localPosition = Vector3.zero;
			if (list5.Count >= 2)
			{
				if (list5.Count >= 3)
				{
					base.gameObject.transform.localPosition = base.gameObject.transform.localPosition + new Vector3(-81f, -81f, 0f);
				}
				else
				{
					base.gameObject.transform.localPosition = base.gameObject.transform.localPosition + new Vector3(-45f, -81f, 0f);
				}
			}
			else
			{
				base.gameObject.transform.localPosition = base.gameObject.transform.localPosition + new Vector3(0f, -81f, 0f);
			}
		}
		for (int m = 0; m < list5.Count; m++)
		{
			this._skins.Add(list5[m]);
		}
		this._oldPoses = this._poses;
		Vector3 vector = new Vector3(-300f, 0f, 0f);
		Vector3 a = new Vector3(0f, 0f, 0f);
		Vector3 a2 = new Vector3(142f, 12f, 0f);
		float num = 0.8f;
		this._skins[0].toPos = vector;
		this._poses = new List<Vector3>();
		this._alphas = new List<float>();
		this._poses.Add(vector);
		for (int n = 0; n <= this._skins.Count; n++)
		{
			Vector3 b = a2 * (float)n;
			this._poses.Add(a + b);
		}
		this._scales = new List<float>();
		this._scales.Add(1f);
		for (int num2 = 0; num2 <= this._skins.Count; num2++)
		{
			if (num2 == 0)
			{
				this._scales.Add(1f);
			}
			else
			{
				float item = (num2 < 2) ? num : (Mathf.Pow(num, (float)num2) - 0.064f);
				this._scales.Add(item);
			}
		}
		this._alphas.Add(0f);
		for (int num3 = 0; num3 < this._skins.Count; num3++)
		{
			if (num3 > 2)
			{
				this._alphas.Add(0f);
			}
			else
			{
				this._alphas.Add(1f / (float)(num3 + 1));
			}
		}
		this._alphas.Add(0f);
		if (isSame && this._oldPoses != null)
		{
			this._poses = this._oldPoses;
		}
		foreach (Skin current2 in this._skins)
		{
			int index = this._skins.IndexOf(current2) + 1;
			current2.trans.localPosition = this._poses[index];
			current2.trans.localScale = new Vector3(this._scales[index], this._scales[index], 1f);
			current2.SetAlpha(this._alphas[index]);
		}
		this.resetDepth();
	}

	public void SetBuyBtnState(int skinId)
	{
		NGUITools.SetActive(this._buyBtn, !this.IsPossess(skinId));
		if (skinId != 0)
		{
			if (this.IsPossess(skinId))
			{
				this.source.gameObject.SetActive(false);
				return;
			}
			this.wearLabel.gameObject.SetActive(false);
			this.wear.gameObject.SetActive(false);
			SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(skinId.ToString());
			string text = string.Empty;
			string[] array = dataById.source.Split(new char[]
			{
				'|'
			});
			if (0 <= array.Length && array[0].CompareTo("1") == 0)
			{
				this.source.gameObject.SetActive(false);
				return;
			}
			this.source.gameObject.SetActive(true);
			this._buyBtn.SetActive(false);
			if (array.Length > 1)
			{
				string text2 = array[0];
				if (text2 != null)
				{
					if (SkinDrawer.<>f__switch$map33 == null)
					{
						SkinDrawer.<>f__switch$map33 = new Dictionary<string, int>(2)
						{
							{
								"7",
								0
							},
							{
								"8",
								1
							}
						};
					}
					int num;
					if (SkinDrawer.<>f__switch$map33.TryGetValue(text2, out num))
					{
						if (num != 0)
						{
							if (num == 1)
							{
								text = LanguageManager.Instance.GetStringById(array[1]);
								this.switchBth.gameObject.SetActive(false);
								this.description.gameObject.SetActive(true);
								this.description.text = text;
							}
						}
						else
						{
							this.switchBth.gameObject.SetActive(true);
							this.description.gameObject.SetActive(false);
							text = LanguageManager.Instance.GetStringById("HeroSkinAltar_HeroSkinSource_" + array[0]);
							this.content.text = text;
							this.activityID = int.Parse(array[1]);
						}
					}
				}
			}
			else
			{
				this.switchBth.gameObject.SetActive(true);
				this.description.gameObject.SetActive(false);
				text = LanguageManager.Instance.GetStringById("HeroSkinAltar_HeroSkinSource_" + dataById.source);
				this.content.text = text;
			}
			this.strSwitch = array[0];
		}
		else
		{
			this.source.gameObject.SetActive(false);
		}
	}

	public void SetWearBtnState(long heroid, int skinid)
	{
		NGUITools.SetActive(this.wearLabel.gameObject, this.IsWear(heroid, skinid));
		NGUITools.SetActive(this.wear.gameObject, !this.IsWear(heroid, skinid) && 0L != heroid);
	}

	public void buyBtnEvent()
	{
		if (this.onBuyBtn != null)
		{
			this.onBuyBtn(this.selection);
		}
	}

	public void wearBtnEvent()
	{
		if (this.onWearBtn != null)
		{
			this.onWearBtn(this.wear, this.isWear);
		}
	}

	public void SwitchBtnEvent()
	{
		if (Singleton<PvpSelectHeroView>.Instance.IsOpen)
		{
			Singleton<TipView>.Instance.ShowViewSetText("选择英雄时不可前往!!!", 1f);
			return;
		}
		string text = this.strSwitch;
		switch (text)
		{
		case "2":
			MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
			break;
		case "3":
			CtrlManager.OpenWindow(WindowID.SignView, null);
			break;
		case "4":
			CtrlManager.OpenWindow(WindowID.AchievementView, null);
			break;
		case "5":
			CtrlManager.OpenWindow(WindowID.RankView, null);
			break;
		case "6":
			CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			break;
		}
	}

	public bool IsPossess(int skinId)
	{
		return skinId == 0 || SkinPanel.IsPossessSkinId(skinId);
	}

	public bool IsWear(long heroid, int skinid)
	{
		return SkinPanel.IsWearSkin(heroid, skinid);
	}

	public bool IsPossessByIndex(int index)
	{
		if (index < 0 || index > this._skins.Count)
		{
			index = 0;
		}
		return SkinPanel.IsPossessSkinId(this._skins[index].getSkinInfo().skinId);
	}

	public int GetSkinByIndex(int index)
	{
		return this._skins[index].getSkinInfo().skinId;
	}

	public void ReRefreshUISkinPanel(string name)
	{
		if (name == null)
		{
			return;
		}
		if (this._skins.Count == 0)
		{
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(name);
			if (heroInfoData == null)
			{
				this.setHeroName(name, 0L, 0, false);
			}
			else
			{
				this.setHeroName(name, heroInfoData.HeroId, heroInfoData.CurrSkin, false);
			}
		}
		NGUITools.SetActive(this._buyBtn, !this.IsPossess(this._skins[0].getSkinInfo().skinId) && !this.SourceState(this._skins[0].getSkinInfo().skinId));
		this._skins[0].setSkinInfo(this._skins[0].getSkinInfo(), this.IsPossess(this._skins[0].getSkinInfo().skinId), this.IsWear(this.heroid, this._skins[0].getSkinInfo().skinId));
		this.wearLabel.SetActive(CharacterDataMgr.instance.OwenHeros.Contains(name) && this.IsWear(this.heroid, this._skins[0].getSkinInfo().skinId));
		this.source.gameObject.SetActive(this.SourceState(this._skins[0].getSkinInfo().skinId));
	}

	public void RefreshPrice(long heroid, int skinid)
	{
		if (this._skins == null || this._skins.Count == 0)
		{
			return;
		}
		foreach (Skin current in this._skins)
		{
			bool flag;
			if (heroid == 0L)
			{
				flag = this.IsPossess(current.getSkinInfo().skinId);
			}
			else
			{
				flag = (this.IsPossess(current.getSkinInfo().skinId) || this.IsWear(heroid, current.getSkinInfo().skinId));
			}
			current.SetPriceActice(!flag);
		}
	}

	public int GetSkinPrice(int skinId)
	{
		if (skinId == 0)
		{
			return 0;
		}
		return 0;
	}

	private bool SourceState(int skinid)
	{
		if (skinid == 0)
		{
			return false;
		}
		if (this.IsPossess(skinid))
		{
			return false;
		}
		SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(skinid.ToString());
		if (dataById != null)
		{
			string[] array = dataById.source.Split(new char[]
			{
				'|'
			});
			return 0 > array.Length || array[0].CompareTo("1") != 0;
		}
		return false;
	}

	private List<float> ParsePrice(string price)
	{
		if (price == null)
		{
			return null;
		}
		string[] array = price.Split(new char[]
		{
			','
		});
		if (array != null || "[]" != array[0])
		{
			return this.ParseTypes(array.Length, array);
		}
		return null;
	}

	private List<float> ParseTypes(int num, string[] details)
	{
		List<float> list = new List<float>();
		if (num != 1)
		{
			if (num == 2)
			{
				list = this.ParseDetails(details[0], details[1]);
				list.Add(2f);
			}
		}
		else
		{
			list = this.ParseDetails(details[0], null);
			list.Add(1f);
		}
		return list;
	}

	private List<float> ParseDetails(string details, string details_another = null)
	{
		List<float> list = new List<float>();
		if (details_another == null)
		{
			string[] array = details.Split(new char[]
			{
				'|'
			});
			if (array.Length > 1)
			{
				if (this.ParseDiscount(float.Parse(array[2])) == 1)
				{
					list.Add((float)this.ParseDiscount(float.Parse(array[2])));
					list.Add(float.Parse(array[2]));
					list.Add((float)int.Parse(array[0]));
					list.Add(float.Parse(array[2]) * (float)int.Parse(array[1]));
				}
				else
				{
					list.Add((float)this.ParseDiscount(float.Parse(array[2])));
					list.Add((float)int.Parse(array[0]));
					list.Add(float.Parse(array[1]));
				}
			}
		}
		else
		{
			string[] array2 = details.Split(new char[]
			{
				'|'
			});
			string[] array3 = details_another.Split(new char[]
			{
				'|'
			});
			if (array2.Length > 1 && array3.Length > 1)
			{
				if (this.ParseDiscount(float.Parse(array2[2])) == 1 || this.ParseDiscount(float.Parse(array3[2])) == 1)
				{
					if (this.ParseDiscount(float.Parse(array2[2])) == 1 && this.ParseDiscount(float.Parse(array3[2])) == 1)
					{
						list.Add(1f);
						list.Add((float.Parse(array2[0]) != 2f) ? ((float.Parse(array3[0]) != 2f) ? float.Parse(array2[2]) : float.Parse(array3[2])) : float.Parse(array2[2]));
						list.Add((float.Parse(array2[0]) != 2f) ? ((float.Parse(array3[0]) != 2f) ? float.Parse(array2[0]) : float.Parse(array3[0])) : float.Parse(array2[0]));
						list.Add((float)((float.Parse(array2[0]) != 2f) ? ((float.Parse(array3[0]) != 2f) ? (int.Parse(array2[1]) * this.ParseDiscount(float.Parse(array2[2]))) : (int.Parse(array3[1]) * this.ParseDiscount(float.Parse(array3[2])))) : (int.Parse(array2[1]) * this.ParseDiscount(float.Parse(array2[2])))));
					}
					else
					{
						list.Add((float)((this.ParseDiscount(float.Parse(array2[2])) != 1) ? this.ParseDiscount(float.Parse(array3[2])) : this.ParseDiscount(float.Parse(array2[2]))));
						list.Add((this.ParseDiscount(float.Parse(array2[2])) != 1) ? float.Parse(array3[2]) : float.Parse(array2[2]));
						list.Add((float)((this.ParseDiscount(float.Parse(array2[2])) != 1) ? int.Parse(array3[0]) : int.Parse(array2[0])));
						list.Add((float)((this.ParseDiscount(float.Parse(array2[2])) != 1) ? (int.Parse(array3[1]) * this.ParseDiscount(float.Parse(array3[2]))) : (int.Parse(array2[1]) * this.ParseDiscount(float.Parse(array2[2])))));
					}
				}
				else
				{
					list.Add((float)this.ParseDiscount(float.Parse(array2[2])));
					list.Add((float)int.Parse(array2[0]));
					list.Add((float)int.Parse(array3[0]));
					list.Add((float)int.Parse(array2[1]));
					list.Add((float)int.Parse(array3[1]));
				}
			}
		}
		return list;
	}

	private int ParseDiscount(float discount)
	{
		if (discount == 1f)
		{
			return 0;
		}
		return 1;
	}

	private string StringPrice(List<GoodsData> goodsData)
	{
		if (goodsData.Count == 1)
		{
			return goodsData[0].Price;
		}
		List<float> info = this.ParsePrice(goodsData[0].Price);
		List<float> info_another = this.ParsePrice(goodsData[1].Price);
		int index = this.CompareItems(info, info_another);
		return goodsData[index].Price;
	}

	private int CompareItems(List<float> info, List<float> info_another)
	{
		int result;
		if (info[0] == 1f || info_another[0] == 1f)
		{
			result = ((info[0] != 1f) ? 1 : 0);
		}
		else if (info[1] != info_another[1])
		{
			if (info[1] == 9f || info_another[1] == 9f)
			{
				result = ((info[1] != 9f) ? 0 : 1);
			}
			else if (info[1] > info_another[1])
			{
				result = 0;
			}
			else
			{
				result = 1;
			}
		}
		else if (info[1] == 9f || info_another[1] == 9f)
		{
			result = ((info[1] != 9f) ? 0 : 1);
		}
		else if (info[1] > info_another[1])
		{
			result = 0;
		}
		else
		{
			result = 1;
		}
		return result;
	}
}
