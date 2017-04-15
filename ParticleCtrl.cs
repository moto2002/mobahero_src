using System;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleCtrl : MonoBehaviour
{
	public static readonly int LEVEL_MAX = 4;

	public bool apply;

	public bool test;

	public bool isEnableAll = true;

	private bool hasDisable;

	private void Awake()
	{
		this.Init();
	}

	private void OnEnable()
	{
		if (this.hasDisable)
		{
			this.Init();
			this.hasDisable = false;
		}
	}

	private void OnDisable()
	{
		this.hasDisable = true;
	}

	private void Update()
	{
		if (this.apply)
		{
			this.Init();
			this.apply = false;
		}
	}

	private void Init()
	{
		int oldLevel = GlobalSettings.Instance.QualitySetting.OldLevel;
		Transform transform = base.transform;
		this.SetAllEnable(transform);
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			int num = 0;
			if (int.TryParse(child.name, out num) && num > oldLevel)
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	private void SetAllEnable(Transform trans)
	{
		if (!this.isEnableAll)
		{
			return;
		}
		trans.gameObject.SetActive(true);
		foreach (Transform allEnable in trans)
		{
			this.SetAllEnable(allEnable);
		}
	}

	public bool Test(bool showSuccess = true)
	{
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			Transform transform = base.transform.Find(i.ToString());
			if (!(null == transform))
			{
				flag = true;
				if (transform.transform.localPosition != Vector3.zero)
				{
					Debug.LogError(string.Concat(new object[]
					{
						base.name,
						",Node ",
						i,
						" position is not zero"
					}));
					return false;
				}
			}
		}
		if (flag)
		{
			if (showSuccess)
			{
				Debug.LogError(base.name + ",Success!!");
			}
		}
		else
		{
			Debug.LogError(base.name + ", has no node");
		}
		return flag;
	}

	public bool CheckAllEnable()
	{
		return !this.isEnableAll || this._checkEnable(base.transform);
	}

	private bool _checkEnable(Transform trans)
	{
		if (!trans.gameObject.activeSelf)
		{
			return false;
		}
		foreach (Transform trans2 in trans)
		{
			if (!this._checkEnable(trans2))
			{
				return false;
			}
		}
		return true;
	}
}
