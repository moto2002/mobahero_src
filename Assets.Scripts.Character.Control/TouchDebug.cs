using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class TouchDebug
	{
		private string ret1 = string.Empty;

		private string ret2 = string.Empty;

		private string ret = string.Empty;

		private int testTouchNum;

		private int createEventTimes;

		private int callTimes;

		private int updateTimes;

		private Touch pointA;

		private Touch pointB;

		private UILabel labelRet1;

		private UILabel labelRet2;

		private UILabel labelRet;

		private UILabel eventLabel;

		private UILabel labelA;

		private UILabel labelB;

		private List<ControlEvent> eList;

		private GameObject root;

		private bool valid;

		public void OnInit()
		{
			GameObject gameObject = Resources.Load("Prefab/Debug/touchDebug") as GameObject;
			if (!(null == gameObject))
			{
				GameObject gameObject2 = GameObject.Find("ViewRoot/Camera");
				if (!(null == gameObject2))
				{
					this.root = NGUITools.AddChild(gameObject2, gameObject);
					if (!(null == this.root))
					{
						this.root.name = "TouchDebug";
						Transform transform = this.root.transform.FindChild("ret1");
						if (!(null == transform))
						{
							this.labelRet1 = transform.gameObject.GetComponent<UILabel>();
							if (!(null == this.labelRet1))
							{
								transform = this.root.transform.FindChild("ret2");
								if (!(null == transform))
								{
									this.labelRet2 = transform.gameObject.GetComponent<UILabel>();
									if (!(null == this.labelRet2))
									{
										transform = this.root.transform.FindChild("ret");
										if (!(null == transform))
										{
											this.labelRet = transform.gameObject.GetComponent<UILabel>();
											if (!(null == this.labelRet))
											{
												transform = this.root.transform.FindChild("event");
												if (!(null == transform))
												{
													this.eventLabel = transform.gameObject.GetComponent<UILabel>();
													if (!(null == this.eventLabel))
													{
														transform = this.root.transform.FindChild("pointA");
														if (!(null == transform))
														{
															this.labelA = transform.gameObject.GetComponent<UILabel>();
															if (!(null == this.labelA))
															{
																transform = this.root.transform.FindChild("pointB");
																if (!(null == transform))
																{
																	this.labelB = transform.gameObject.GetComponent<UILabel>();
																	if (!(null == this.labelB))
																	{
																		this.valid = true;
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void OnExit()
		{
			if (this.valid && null != this.root)
			{
				UnityEngine.Object.Destroy(this.root);
				this.root = null;
				this.valid = false;
			}
		}

		public void UpdateList(List<ControlEvent> list)
		{
			if (this.valid)
			{
				this.eList = list;
				this.RefreshDebugInfo_touchPoint();
				this.RefreshDebugInfo_ret();
			}
		}

		private void RefreshDebugInfo_ret()
		{
			for (int i = 0; i < this.eList.Count; i++)
			{
				ControlEvent controlEvent = this.eList[i];
				if (controlEvent != null)
				{
					if (controlEvent.id == 0)
					{
						this.ret1 = controlEvent.ToString();
						this.labelRet1.text = this.ret1;
					}
					else
					{
						this.ret2 = controlEvent.ToString();
						this.labelRet2.text = this.ret2;
					}
				}
			}
			if (this.eList.Count > 0 && this.eList[0] != null)
			{
				this.ret = "ret= " + this.eList[0].ToString();
				this.labelRet.text = this.ret;
			}
		}

		private void RefreshDebugInfo_touchPoint()
		{
			this.testTouchNum = Input.touchCount;
			if (this.testTouchNum > 0)
			{
				this.pointA = Input.GetTouch(0);
				this.labelA.text = string.Format("Touch 1: Phase={0},Id = {1},pos={2}", this.pointA.phase.ToString(), this.pointA.fingerId, this.pointA.position);
			}
			if (this.testTouchNum > 1)
			{
				this.pointB = Input.GetTouch(1);
				this.labelB.text = string.Format("Touch 2: Phase={0},Id = {1},pos={2}", this.pointB.phase.ToString(), this.pointB.fingerId, this.pointB.position);
			}
		}

		public static void PrintDebugInfo(string str)
		{
		}
	}
}
