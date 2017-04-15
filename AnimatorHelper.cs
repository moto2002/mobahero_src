using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
	private Animator animator;

	private bool isAction;

	private bool isCircle;

	private AnimationInfo[] animationinfo;

	[SerializeField]
	private Transform[] Circle_Start;

	[SerializeField]
	private Transform[] Circle_End;

	[SerializeField]
	private Transform[] Action_Start;

	[SerializeField]
	private Transform[] Action_End;

	[SerializeField]
	private Transform[] Access_Stop;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		this.SetActive(this.Action_Start, false);
	}

	private void Start()
	{
		this.isAction = false;
		this.isCircle = true;
		this.animator = base.GetComponent<Animator>();
		Transform[] circle_Start = this.Circle_Start;
		for (int i = 0; i < circle_Start.Length; i++)
		{
			Transform transform = circle_Start[i];
			if (null != transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
		Transform[] action_Start = this.Action_Start;
		for (int j = 0; j < action_Start.Length; j++)
		{
			Transform transform2 = action_Start[j];
			if (null != transform2)
			{
				transform2.gameObject.SetActive(false);
			}
		}
	}

	private void Update()
	{
		this.animationinfo = this.animator.GetCurrentAnimationClipState(0);
		if (this.animationinfo.Length > 0)
		{
			if ("breath2".CompareTo(this.animationinfo[0].clip.name) == 0)
			{
				this.Fx_Start("Circle");
				this.Fx_End("Circle");
				this.isAction = true;
			}
			else if ("breath3".CompareTo(this.animationinfo[0].clip.name) == 0)
			{
				this.Fx_Start("Action");
				this.Fx_End("Action");
				this.isCircle = true;
			}
		}
	}

	private void Fx_Start(string name)
	{
		if (name != null)
		{
			if (AnimatorHelper.<>f__switch$map21 == null)
			{
				AnimatorHelper.<>f__switch$map21 = new Dictionary<string, int>(2)
				{
					{
						"Circle",
						0
					},
					{
						"Action",
						1
					}
				};
			}
			int num;
			if (AnimatorHelper.<>f__switch$map21.TryGetValue(name, out num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						if (this.isAction)
						{
							this.SetActive(this.Action_Start, false);
							this.SetActive(this.Action_Start, true);
							this.SetActive(this.Access_Stop, false);
						}
						this.isAction = false;
					}
				}
				else
				{
					if (this.isCircle)
					{
						this.SetActive(this.Circle_Start, false);
						this.SetActive(this.Circle_Start, true);
					}
					this.isCircle = false;
				}
			}
		}
	}

	private void Fx_End(string name)
	{
		if (name != null)
		{
			if (AnimatorHelper.<>f__switch$map22 == null)
			{
				AnimatorHelper.<>f__switch$map22 = new Dictionary<string, int>(2)
				{
					{
						"Circle",
						0
					},
					{
						"Action",
						1
					}
				};
			}
			int num;
			if (AnimatorHelper.<>f__switch$map22.TryGetValue(name, out num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.SetActive(this.Action_End, false);
					}
				}
				else
				{
					this.SetActive(this.Circle_End, false);
				}
			}
		}
	}

	private void SetActive(Transform[] trans, bool isActive)
	{
		for (int i = 0; i < trans.Length; i++)
		{
			Transform transform = trans[i];
			if (null != transform)
			{
				transform.gameObject.SetActive(isActive);
			}
		}
	}
}
