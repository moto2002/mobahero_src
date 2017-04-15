using System;
using UnityEngine;

public class EditorGamePlay : EditorMono
{
	public Moba_Camera MobaCamera;

	public EditorUnit SkillTargetUnit;

	public Transform SkillPosition;

	public static EditorGamePlay Ins
	{
		get;
		private set;
	}

	public EditorScreenContorller ScreenController
	{
		get;
		set;
	}

	protected override void Awake()
	{
		EditorGamePlay.Ins = this;
		MapManager.Instance.Init();
		this.ScreenController = this.AddEditorComponet<EditorScreenContorller>();
	}

	private T AddEditorComponet<T>() where T : MonoBehaviour, IEditorGamePlayCompoent
	{
		T result = base.gameObject.AddComponent<T>();
		result.Init(this);
		return result;
	}
}
