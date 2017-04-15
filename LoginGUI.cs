using System;
using UnityEngine;

public class LoginGUI : MonoBehaviour
{
	private bool isTest;

	public bool hiddenBattleUI;

	public bool openBattleEquipmentSystem;

	private TextAsset text;

	private void Awake()
	{
		this.text = (Resources.Load("BundleVersion") as TextAsset);
	}

	private void OnGUI()
	{
	}
}
