using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewDisplayEffect : MonoBehaviour
	{
		private object[] mgs;

		private GameObject hitchPoint;

		private GameObject targetModel;

		private GameObject perfabObject1;

		private GameObject perfabObject2;

		private GameObject perfabObject3;

		private GameObject showEffectObject1;

		private GameObject showEffectObject2;

		private GameObject showEffectObject3;

		private Coroutine delay_Coroutine;

		private Coroutine stop_Coroutine;

		private EffectItem currEffectItem;

		private Dictionary<int, GameObject> objs = new Dictionary<int, GameObject>();

		private void Awake()
		{
			this.mgs = new object[]
			{
				ClientV2C.propviewClickCollectionItem,
				ClientV2C.runesviewCloseView
			};
		}

		private void OnEnable()
		{
			this.Register();
			this.DestroyClone();
		}

		private void OnDisable()
		{
			this.Unregister();
			base.StopAllCoroutines();
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void Initialize()
		{
		}

		private void OnMsg_propviewClickCollectionItem(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				this.currEffectItem = null;
				this.currEffectItem = (EffectItem)msg.Param;
				if (null != this.currEffectItem)
				{
					this.DisplayEffects();
				}
			}
		}

		private void OnMsg_runesviewCloseView(MobaMessage msg)
		{
			if (msg != null)
			{
				this.DestroyClone();
			}
		}

		private void DisplayEffects()
		{
			base.StopAllCoroutines();
			string text = string.Empty;
			PropViewModel propViewModel = null;
			if (null != base.transform)
			{
				propViewModel = base.transform.GetComponent<PropViewModel>();
			}
			if (null != propViewModel && null != propViewModel.Smodel)
			{
				this.targetModel = propViewModel.Smodel;
			}
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(this.currEffectItem.ModelID.ToString());
			text = dataById.hero_decorate_param;
			int hero_decorate_type = dataById.hero_decorate_type;
			if (hero_decorate_type == 1)
			{
				text += "_smodel";
			}
			if (hero_decorate_type == 4)
			{
				string resId = text;
				string resId2 = text + "_end";
				string resId3 = text + "_2";
				this.perfabObject1 = ResourceManager.Load<GameObject>(resId, true, true, null, 0, false);
				this.perfabObject2 = ResourceManager.Load<GameObject>(resId2, true, true, null, 0, false);
				this.perfabObject3 = ResourceManager.Load<GameObject>(resId3, true, true, null, 0, false);
				if (this.objs.ContainsKey(1) && null != this.objs[1] && this.perfabObject1.name.CompareTo(this.SubString(this.objs[1].name)) == 0)
				{
					this.showEffectObject1.SetActive(false);
					this.showEffectObject1.SetActive(true);
				}
				else
				{
					if (this.objs.ContainsKey(1) && null != this.objs[1])
					{
						UnityEngine.Object.DestroyImmediate(this.objs[1]);
					}
					this.showEffectObject1 = (GameObject)UnityEngine.Object.Instantiate(this.perfabObject1, this.targetModel.transform.position, this.targetModel.transform.rotation);
					Transform[] componentsInChildren = this.showEffectObject1.GetComponentsInChildren<Transform>();
					Transform[] array = componentsInChildren;
					for (int i = 0; i < array.Length; i++)
					{
						Transform transform = array[i];
						transform.gameObject.layer = 13;
					}
					if (null != this.showEffectObject1)
					{
						this.showEffectObject1.transform.parent = this.targetModel.transform;
						this.objs[1] = this.showEffectObject1;
					}
				}
				if (this.delay_Coroutine != null)
				{
					base.StopCoroutine("DelayDisplay");
				}
				if (this.stop_Coroutine != null)
				{
					base.StopCoroutine("DelayDisplay");
				}
				this.delay_Coroutine = base.StartCoroutine("DelayDisplay", true);
				this.stop_Coroutine = base.StartCoroutine("DelayDisplay", false);
			}
			else
			{
				this.perfabObject1 = ResourceManager.Load<GameObject>(text, true, true, null, 0, false);
				if (null != this.perfabObject1)
				{
					if (hero_decorate_type == 2)
					{
						if (this.objs.ContainsKey(3) && null != this.objs[3] && this.perfabObject1.name.CompareTo(this.SubString(this.objs[3].transform.GetChild(0).name)) == 0)
						{
							this.objs[3].transform.GetChild(0).gameObject.SetActive(false);
							this.objs[3].transform.GetChild(0).gameObject.SetActive(true);
						}
					}
					else if (this.objs.ContainsKey(3) && null != this.objs[3] && this.perfabObject1.name.CompareTo(this.SubString(this.objs[3].name)) == 0)
					{
						this.objs[3].SetActive(false);
						this.objs[3].SetActive(true);
					}
					if (this.objs.ContainsKey(3) && null != this.objs[3])
					{
						if ("Hitch" == this.objs[3].name && hero_decorate_type == 2)
						{
							UnityEngine.Object.DestroyImmediate(this.objs[3].transform.GetChild(0).gameObject);
						}
						else
						{
							UnityEngine.Object.DestroyImmediate(this.objs[3]);
						}
					}
					if (this.objs.ContainsKey(1) && null != this.objs[1])
					{
						UnityEngine.Object.DestroyImmediate(this.objs[1]);
					}
					if (this.objs.ContainsKey(2) && null != this.objs[2])
					{
						UnityEngine.Object.DestroyImmediate(this.objs[2]);
					}
					this.showEffectObject1 = (GameObject)UnityEngine.Object.Instantiate(this.perfabObject1, this.targetModel.transform.position, this.targetModel.transform.rotation);
					Transform[] componentsInChildren = this.showEffectObject1.GetComponentsInChildren<Transform>();
					Transform[] array2 = componentsInChildren;
					for (int j = 0; j < array2.Length; j++)
					{
						Transform transform2 = array2[j];
						transform2.gameObject.layer = 13;
					}
					if (null != this.showEffectObject1)
					{
						if (hero_decorate_type == 2)
						{
							if (null == this.hitchPoint)
							{
								this.hitchPoint = new GameObject("Hitch");
								this.hitchPoint.transform.parent = this.targetModel.transform;
								this.hitchPoint.transform.localPosition = new Vector3(0f, 1f, 0f);
								this.hitchPoint.AddComponent<SelfRotate>();
								this.hitchPoint.AddComponent<Hover>();
								this.hitchPoint.GetComponent<SelfRotate>().yRotate = 120f;
								this.hitchPoint.GetComponent<Hover>().hoverDistance = 2f;
								this.hitchPoint.GetComponent<Hover>().hoverSpeedX = 0f;
								this.hitchPoint.GetComponent<Hover>().hoverSpeedY = 3.5f;
								this.hitchPoint.GetComponent<Hover>().hoverSpeedZ = 0f;
							}
							this.showEffectObject1.transform.parent = this.hitchPoint.transform;
							this.showEffectObject1.transform.localPosition = new Vector3(0f, 0f, 1.5f);
							this.objs[3] = this.hitchPoint;
						}
						else
						{
							this.showEffectObject1.transform.parent = this.targetModel.transform;
							this.objs[3] = this.showEffectObject1;
							if (hero_decorate_type == 1)
							{
								this.showEffectObject1.transform.localPosition = new Vector3(0.7f, 0f, 0.9f);
							}
						}
					}
				}
			}
		}

		private void DestroyClone()
		{
			if (this.objs.ContainsKey(1) && null != this.objs[1])
			{
				UnityEngine.Object.Destroy(this.objs[1]);
			}
			if (this.objs.ContainsKey(2) && null != this.objs[2])
			{
				UnityEngine.Object.Destroy(this.objs[2]);
			}
			if (this.objs.ContainsKey(3) && null != this.objs[3])
			{
				UnityEngine.Object.Destroy(this.objs[3]);
			}
			if (null != this.hitchPoint)
			{
				UnityEngine.Object.Destroy(this.hitchPoint);
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayDisplay(bool isStart)
		{
			PropViewDisplayEffect.<DelayDisplay>c__Iterator180 <DelayDisplay>c__Iterator = new PropViewDisplayEffect.<DelayDisplay>c__Iterator180();
			<DelayDisplay>c__Iterator.isStart = isStart;
			<DelayDisplay>c__Iterator.<$>isStart = isStart;
			<DelayDisplay>c__Iterator.<>f__this = this;
			return <DelayDisplay>c__Iterator;
		}

		private string SubString(string name)
		{
			int num = name.IndexOf("(");
			return (num != -1) ? name.Substring(0, num) : name;
		}
	}
}
