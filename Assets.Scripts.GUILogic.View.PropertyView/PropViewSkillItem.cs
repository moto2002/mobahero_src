using Com.Game.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewSkillItem : MonoBehaviour
	{
		[SerializeField]
		private UISprite skillFrame;

		[SerializeField]
		private UISprite skillPic;

		private SkillIndex skillIndex;

		public Callback<GameObject, bool> DragCallBack;

		public SkillIndex SkillEnum
		{
			get
			{
				return this.skillIndex;
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public void Init(int idx, string idxName, UIAtlas atlas)
		{
			this.skillIndex = idx + SkillIndex._1st;
			this.skillFrame.spriteName = "Hero_skill_frame";
			SysSkillMainVo skillData = SkillUtility.GetSkillData(idxName, -1, -1);
			this.skillPic.atlas = atlas;
			this.skillPic.spriteName = skillData.skill_icon;
			UIEventListener.Get(this.skillFrame.gameObject).onMobileHover = new UIEventListener.BoolDelegate(this.CheckState);
		}

		private void CheckState(GameObject obj, bool isIn)
		{
			if (null != obj)
			{
				this.skillFrame.spriteName = ((!isIn) ? "Hero_skill_frame" : "Hero_skill_select");
				this.DragCallBack(obj, isIn);
			}
		}
	}
}
