using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class GetItems_ShowCharm : MonoBehaviour
	{
		public UICenterHelper centerHelper;

		public UILabel textLabel;

		private void Awake()
		{
		}

		private void OnDestroy()
		{
		}

		public void ShowCharmIncrement(int delta)
		{
			if (delta > 0)
			{
				this.textLabel.text = "魅力值 +" + delta;
				base.gameObject.SetActive(true);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
