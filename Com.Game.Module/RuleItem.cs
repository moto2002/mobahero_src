using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class RuleItem : MonoBehaviour
	{
		public UILabel title;

		public UILabel context;

		public void SetText(string _title, string _context)
		{
			this.title.text = _title;
			this.context.text = _context;
			base.transform.GetComponent<UIWidget>().ResetAnchors();
			this.context.GetComponent<UIWidget>().ResetAnchors();
		}

		public void SetTitle(string _title)
		{
			this.title.text = _title;
		}

		public void SetContext(string _context)
		{
			this.context.text = _context;
		}
	}
}
