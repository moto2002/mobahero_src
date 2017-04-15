using System;
using System.Collections.Generic;

public class EditorBuffManager : EditorMono, IEditorUnitCompoent
{
	private EditorUnit self;

	public List<EditorBuff> buffs = new List<EditorBuff>();

	public void Init(EditorUnit unit)
	{
		this.self = unit;
		if (this.self.Unit is Hero)
		{
		}
	}

	public void Add(string buffId, EditorSkill skill)
	{
		EditorBuff editorBuff = new EditorBuff();
		editorBuff.Start(buffId, skill, this.self);
		this.buffs.Add(editorBuff);
	}

	public void Remove(EditorBuff buff)
	{
		buff.OnRemove();
		this.buffs.Remove(buff);
	}

	protected override void Update()
	{
		base.Update();
		if (this.buffs.Count == 0)
		{
			return;
		}
		for (int i = this.buffs.Count - 1; i > 0; i--)
		{
			this.buffs[i].OnUpdate();
			if (this.buffs[i].IsOver())
			{
				this.Remove(this.buffs[i]);
			}
		}
	}
}
