using System;
using UnityEngine;

namespace SkillMechanism
{
	public class SpellEventData : AnimEventData
	{
		public const string EVENT_ID = "Spell";

		private string _goName;

		private string _spellPath;

		private BindType _bindType;

		public new int frame;

		public GameObject spellRes;

		public string bindName
		{
			get
			{
				return this._goName;
			}
			set
			{
				this._goName = value;
			}
		}

		public string spellPath
		{
			get
			{
				return this._spellPath;
			}
			set
			{
				this._spellPath = value;
			}
		}

		public BindType bindType
		{
			get
			{
				return this._bindType;
			}
			set
			{
				this._bindType = value;
			}
		}

		public SpellEventData(string goName, string spellPath, BindType bindType)
		{
			this._goName = goName;
			this._spellPath = spellPath;
			this._bindType = bindType;
		}

		public override string write2Line()
		{
			return string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				"Spell",
				this._goName,
				this._spellPath,
				(int)this._bindType
			});
		}
	}
}
