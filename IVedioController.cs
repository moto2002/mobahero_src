using System;
using UnityEngine;

internal interface IVedioController
{
	int ID
	{
		get;
		set;
	}

	bool Enable
	{
		get;
		set;
	}

	bool Play
	{
		get;
		set;
	}

	bool Loop
	{
		get;
		set;
	}

	string Resource
	{
		get;
		set;
	}

	PlayState State
	{
		get;
	}

	GameObject Obj
	{
		get;
	}

	Action<IVedioController> Callback_Ready
	{
		get;
		set;
	}

	Action<IVedioController> Callback_End
	{
		get;
		set;
	}

	Action<IVedioController> Callback_Start
	{
		get;
		set;
	}

	void Unload();
}
