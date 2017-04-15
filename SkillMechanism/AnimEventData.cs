using System;
using UnityEngine;

namespace SkillMechanism
{
	public class AnimEventData
	{
		private int _frame;

		protected string _funcName;

		public int frame
		{
			get
			{
				return this._frame;
			}
		}

		public virtual string funcName
		{
			get
			{
				return this._funcName;
			}
		}

		public AnimEventData(int frame, string funcName)
		{
			this._frame = frame;
			this._funcName = funcName;
		}

		public AnimEventData()
		{
		}

		public virtual string write2Line()
		{
			Debug.LogError(base.GetType().ToString() + " override this!");
			return null;
		}

		public virtual void readFromSplits(string[] splits)
		{
			Debug.LogError(base.GetType().ToString() + " override this!");
		}
	}
}
