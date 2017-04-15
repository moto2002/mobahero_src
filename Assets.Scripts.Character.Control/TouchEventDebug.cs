using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class TouchEventDebug
	{
		private string eventStr = string.Empty;

		private string handlerStr = string.Empty;

		private UILabel lbEvent;

		private UILabel lbHandler;

		private GameObject root;

		private bool valid;

		public void OnInit()
		{
			GameObject gameObject = Resources.Load("Prefab/Debug/touchEventDebug") as GameObject;
			if (!(null == gameObject))
			{
				GameObject gameObject2 = GameObject.Find("ViewRoot/Camera");
				if (!(null == gameObject2))
				{
					this.root = NGUITools.AddChild(gameObject2, gameObject);
					if (!(null == this.root))
					{
						this.root.name = "TouchEventDebug";
						Transform transform = this.root.transform.FindChild("event");
						if (!(null == transform))
						{
							this.lbEvent = transform.gameObject.GetComponent<UILabel>();
							if (!(null == this.lbEvent))
							{
								transform = this.root.transform.FindChild("handler");
								if (!(null == transform))
								{
									this.lbHandler = transform.gameObject.GetComponent<UILabel>();
									if (!(null == this.lbEvent))
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

		public void OnExit()
		{
			if (this.valid)
			{
				UnityEngine.Object.Destroy(this.root);
				this.root = null;
				this.valid = false;
			}
		}

		public void AddEventInfo(string e)
		{
			if (this.valid)
			{
				this.lbEvent.text = e;
			}
		}

		public void AddHandlerInfo(string e)
		{
			if (this.valid)
			{
				this.lbHandler.text = e;
			}
		}
	}
}
