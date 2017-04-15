using System;
using UnityEngine;

namespace SkillMechanism
{
	public class SkillInfo : ScriptableObject
	{
		[SerializeField]
		private string _name;

		public new string name
		{
			get
			{
				return this._name;
			}
		}

		public void ctor(string skillName)
		{
			this._name = skillName;
		}
	}
}
