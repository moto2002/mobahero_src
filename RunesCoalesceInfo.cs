using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class RunesCoalesceInfo : MonoBehaviour
{
	[SerializeField]
	private UILabel Name;

	[SerializeField]
	private UILabel Effect;

	[SerializeField]
	private UILabel CountLabel;

	private int quality;

	private int count;

	private string modelid = string.Empty;

	private Color color2 = new Color(0f, 0.7058824f, 1f, 1f);

	private Color color3 = new Color(0.894117653f, 0f, 1f, 1f);

	public int Quality
	{
		get
		{
			return this.quality;
		}
	}

	public int Count
	{
		get
		{
			return this.count;
		}
	}

	public string ModelID
	{
		get
		{
			return this.modelid;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init(int _ModelID, int _count)
	{
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(_ModelID.ToString());
		this.quality = dataById.quality;
		this.count = _count;
		this.modelid = _ModelID.ToString();
		this.Name.color = ((this.quality != 2) ? this.color3 : this.color2);
		this.Name.text = LanguageManager.Instance.GetStringById(dataById.name);
		this.CountLabel.text = this.count.ToString();
		string[] array = dataById.attribute.Split(new char[]
		{
			'|'
		});
		this.Effect.text = ((!(array[1].Substring(array[1].Length - 1, 1) == "%")) ? ("+" + float.Parse(array[1]).ToString(array[2]) + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)) : ("+" + array[1] + LanguageManager.Instance.GetStringById(BaseDataMgr.instance.GetAttrNumberData(array[0]).attrName)));
		this.Effect.text = "(" + this.Effect.text + ")";
	}
}
