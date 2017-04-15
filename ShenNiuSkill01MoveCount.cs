using System;
using System.Collections.Generic;
using UnityEngine;

public class ShenNiuSkill01MoveCount : MonoBehaviour
{
	public float DelayTime = 0.5f;

	private float mTimeElapsed;

	private bool mTimeElaspedOpen;

	private Dictionary<GameObject, GameObject> mMoveUnitsMap = new Dictionary<GameObject, GameObject>();

	private void Awake()
	{
	}

	private void Start()
	{
		this.mTimeElapsed = 0f;
		this.mTimeElaspedOpen = false;
		this.mMoveUnitsMap.Clear();
	}

	private void Update()
	{
		if (this.mMoveUnitsMap == null)
		{
			return;
		}
		if (this.mTimeElaspedOpen)
		{
			this.mTimeElapsed += Time.deltaTime;
		}
		if (this.mTimeElapsed > this.DelayTime)
		{
			foreach (KeyValuePair<GameObject, GameObject> current in this.mMoveUnitsMap)
			{
				if (!(current.Value == null))
				{
					if (!(current.Key == null))
					{
						string text = string.Concat(new string[]
						{
							"Move Unit : [",
							current.Key.name,
							"] by Collider : [",
							current.Value.name,
							"]\n"
						});
						GameObject value = current.Value;
						Vector3 position = value.transform.position;
						Vector3 forward = value.transform.forward;
						Vector3 vector = new Vector3(-forward.z, forward.y, forward.x);
						Vector3 vector2 = new Vector3(forward.z, forward.y, -forward.x);
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							"col Pos : [",
							position.x,
							",",
							position.y,
							",",
							position.z,
							"]\n"
						});
						text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							"col Forward : [",
							forward.x,
							",",
							forward.y,
							",",
							forward.z,
							"]\n"
						});
						text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							"col ForwardLeft : [",
							vector.x,
							",",
							vector.y,
							",",
							vector.z,
							"]\n"
						});
						text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							"col ForwardRight : [",
							vector2.x,
							",",
							vector2.y,
							",",
							vector2.z,
							"]\n"
						});
						BoxCollider component = value.GetComponent<BoxCollider>();
						float num = component.size.x / 2f;
						GameObject key = current.Key;
						if (!key.CompareTag("Building"))
						{
							Vector3 position2 = key.transform.position;
							text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"Units Pos : [",
								position2.x,
								",",
								position2.y,
								",",
								position2.z,
								"]\n"
							});
							Vector3 rhs = position2 - position;
							float sqrMagnitude = rhs.sqrMagnitude;
							rhs.Normalize();
							text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"Units Vec : [",
								rhs.x,
								",",
								rhs.y,
								",",
								rhs.z,
								"], Distance : [",
								sqrMagnitude,
								"]\n"
							});
							float num2 = Vector3.Dot(vector2, rhs);
							text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"Dot Col Forward Right Result : [",
								num2,
								"]\n"
							});
							float num3 = (1f - Mathf.Abs(num2)) * num;
							CharacterController component2 = key.GetComponent<CharacterController>();
							if (component2 != null)
							{
								num3 += component2.radius / 2f;
							}
							text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"Move Distance : [",
								num3,
								"]\n"
							});
							Vector3 b = num3 * ((num2 < 0f) ? vector : vector2);
							text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"Move Dir : [",
								b.x,
								",",
								b.y,
								",",
								b.z,
								"]\n"
							});
							key.transform.position += b;
						}
					}
				}
			}
			this.mTimeElapsed = 0f;
			this.mTimeElaspedOpen = false;
			this.mMoveUnitsMap.Clear();
		}
	}

	public void AddCollider(GameObject unitsObj, GameObject colObj)
	{
		if (!this.mMoveUnitsMap.ContainsKey(unitsObj))
		{
			Units component = unitsObj.GetComponent<Units>();
			if (component != null)
			{
				if (component.isHome)
				{
					return;
				}
				CombinationUnit component2 = colObj.GetComponent<CombinationUnit>();
				BoxCollider component3 = colObj.GetComponent<BoxCollider>();
				if (component2 != null && component3 != null && component2.index == 0)
				{
					float num = component3.size.z / 2f;
					Vector3 position = colObj.transform.position;
					Vector3 forward = colObj.transform.forward;
					Vector3 position2 = unitsObj.transform.position;
					Vector3 lhs = position2 - position;
					float sqrMagnitude = lhs.sqrMagnitude;
					lhs.Normalize();
					float num2 = Vector3.Dot(lhs, forward);
					if (sqrMagnitude > num * 0.9f && num2 < -0.9f)
					{
						return;
					}
				}
				this.mMoveUnitsMap.Add(unitsObj, colObj);
				this.mTimeElaspedOpen = true;
			}
		}
	}
}
