using Com.Game.Utils;
using System;
using UnityEngine;

public class SurrenderComps : MonoBehaviour
{
	public UILabel TipLabel;

	public UISlider RatioSlider;

	public UIGrid MembersGrid;

	public GameObject[] Masks;

	public UIButton BtnYes;

	public UIButton BtnNo;

	public Transform votePanel;

	public Transform resultPanel;

	public UILabel voteResult;

	public UISprite resultBG;

	public Action<bool> OnConfirmSurrender;

	public void SetMemberCount(int cnt)
	{
		GridHelper.FillGridWithFirstChild<UISprite>(this.MembersGrid, cnt, delegate(int idx, UISprite cmp)
		{
			cmp.width = 236 / cnt - 2;
			cmp.gameObject.SetActive(true);
		});
		this.MembersGrid.cellWidth = 236f / (float)cnt;
		this.MembersGrid.Reposition();
	}

	public void SetCanSurrender(bool enabled)
	{
		GameObject[] masks = this.Masks;
		for (int i = 0; i < masks.Length; i++)
		{
			GameObject gameObject = masks[i];
			if (gameObject)
			{
				gameObject.SetActive(!enabled);
			}
		}
	}

	public void SetMemberStates(bool?[] states)
	{
		GridHelper.FillGridWithFirstChild<UISprite>(this.MembersGrid, states.Length, delegate(int idx, UISprite cmp)
		{
			if (states[idx].HasValue)
			{
				cmp.spriteName = ((!states[idx].Value) ? "PVP_surrender_02" : "PVP_surrender_01");
			}
			else
			{
				cmp.spriteName = "PVP_surrender_03";
			}
		});
	}

	public void SetResultText(string str)
	{
		this.voteResult.text = str;
		Bounds bounds = this.voteResult.CalculateBounds();
		this.resultBG.SetDimensions(66 + (int)bounds.size.x, this.resultBG.height);
	}

	public void ShowVotePanel(bool b)
	{
		if (this.votePanel.gameObject.activeSelf != b)
		{
			this.votePanel.gameObject.SetActive(b);
		}
	}

	public void ShowResultPanel(bool b)
	{
		if (this.resultPanel.gameObject.activeSelf != b)
		{
			this.resultPanel.gameObject.SetActive(b);
		}
	}

	public void DoYes()
	{
		if (this.OnConfirmSurrender != null)
		{
			this.OnConfirmSurrender(true);
		}
	}

	public void DoNo()
	{
		if (this.OnConfirmSurrender != null)
		{
			this.OnConfirmSurrender(false);
		}
	}
}
