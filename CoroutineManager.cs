using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineManager
{
	private readonly List<Task> taskList;

	public CoroutineManager()
	{
		this.taskList = new List<Task>();
	}

	public Task StartCoroutine(IEnumerator routine, bool autoStart = true)
	{
		Task task = new Task(routine, autoStart);
		this.taskList.Add(task);
		return task;
	}

	public void StopCoroutine(Task t)
	{
		if (this.taskList.Contains(t))
		{
			t.Stop();
			this.taskList.Remove(t);
		}
	}

	public void PauseCoroutine(Task t)
	{
		if (this.taskList.Contains(t))
		{
			t.Pause();
		}
	}

	public void ResumeCoroutine(Task t)
	{
		if (this.taskList.Contains(t))
		{
			t.Unpause();
		}
	}

	public void PauseAllCoroutine()
	{
		for (int i = 0; i < this.taskList.Count; i++)
		{
			this.taskList[i].Pause();
		}
	}

	public void ResumeAllCoroutine()
	{
		for (int i = 0; i < this.taskList.Count; i++)
		{
			this.taskList[i].Unpause();
		}
	}

	public void StopAllCoroutine()
	{
		for (int i = 0; i < this.taskList.Count; i++)
		{
			this.taskList[i].Stop();
		}
		this.taskList.Clear();
	}
}
