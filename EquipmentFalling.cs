using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EquipmentFalling : MonoBehaviour
{
	public class equipment_item
	{
		private int ID;

		private string Name;

		private string Type;

		private int Number;

		private string Effect;

		private string Details;
	}

	public List<EquipmentFalling.equipment_item> Equipment_List = new List<EquipmentFalling.equipment_item>();

	public int EquipmetNumber;

	public int[] Equipment;

	[DebuggerHidden]
	public IEnumerator Falling_Equipment(Units deader, Units attacker)
	{
		EquipmentFalling.<Falling_Equipment>c__Iterator1A1 <Falling_Equipment>c__Iterator1A = new EquipmentFalling.<Falling_Equipment>c__Iterator1A1();
		<Falling_Equipment>c__Iterator1A.attacker = attacker;
		<Falling_Equipment>c__Iterator1A.deader = deader;
		<Falling_Equipment>c__Iterator1A.<$>attacker = attacker;
		<Falling_Equipment>c__Iterator1A.<$>deader = deader;
		<Falling_Equipment>c__Iterator1A.<>f__this = this;
		return <Falling_Equipment>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator Equipment_FallingAnimation(Units deather, int ID)
	{
		EquipmentFalling.<Equipment_FallingAnimation>c__Iterator1A2 <Equipment_FallingAnimation>c__Iterator1A = new EquipmentFalling.<Equipment_FallingAnimation>c__Iterator1A2();
		<Equipment_FallingAnimation>c__Iterator1A.deather = deather;
		<Equipment_FallingAnimation>c__Iterator1A.<$>deather = deather;
		return <Equipment_FallingAnimation>c__Iterator1A;
	}

	private void ClickBox()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 100f) && raycastHit.collider.tag == "Home")
		{
			this.BoxAnimation();
		}
	}

	private void BoxAnimation()
	{
	}

	public int[] SendServer_GoToGame()
	{
		int[] array = new int[]
		{
			1,
			3,
			6,
			2,
			5,
			4
		};
		for (int i = 0; i < array.Length; i++)
		{
			int j = i + 1;
			while (j < array.Length)
			{
				if (array[i] > array[j])
				{
					int num = array[j];
					array[j] = array[i];
					array[i] = num;
				}
				i++;
			}
		}
		return array;
	}
}
