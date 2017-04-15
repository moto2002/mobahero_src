using System;
using UnityEngine;

public class EditorScreenContorller : EditorMono, IEditorGamePlayCompoent
{
	private EditorGamePlay gamePlay;

	public EditorUnit Player
	{
		get;
		private set;
	}

	protected override void Awake()
	{
	}

	protected override void Update()
	{
		this.UpdateInput();
	}

	public void Init(EditorGamePlay gamePlay)
	{
		this.gamePlay = gamePlay;
		GameObject gameObject = GameObject.Find("_System/Actors/Self").transform.GetChild(0).gameObject;
		this.SetPlayer(gameObject);
	}

	public void SetPlayer(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		this.Player = null;
		this.Player = obj.GetComponent<EditorUnit>();
		if (this.Player == null)
		{
			this.Player = obj.AddComponent<EditorUnit>();
		}
		this.gamePlay.MobaCamera.SetTargetTransform(obj.transform);
		AudioMgr.AddLisener(base.gameObject);
	}

	private void UpdateInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (this.Player == null)
			{
				return;
			}
			RaycastHit raycastHit;
			if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 9999f, LayerMask.GetMask(new string[]
			{
				"UI"
			})))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 9999f, LayerMask.GetMask(new string[]
				{
					"Ground",
					"Unit",
					"Monster"
				})) && raycastHit.collider != null)
				{
					EditorUnit component = raycastHit.collider.GetComponent<EditorUnit>();
					if (component != null)
					{
						if (component == this.Player)
						{
							return;
						}
						this.Player.SkillManager.Attack(component);
					}
					else
					{
						this.Player.MoveController.Move(raycastHit.point, 0.2f);
					}
				}
			}
		}
	}
}
