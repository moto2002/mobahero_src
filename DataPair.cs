using System;
using UnityEngine;

public class DataPair : MonoBehaviour
{
	public UILabel lbl_des;

	public UILabel lbl_val;

	public void SetContent(string name, float val)
	{
		this.lbl_des.text = name;
		this.lbl_val.text = val.ToString();
	}
}
