using Com.Game.Module;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/HUD Text")]
public class HUDText : MonoBehaviour
{
	protected class Entry
	{
		public float time;

		public float stay;

		public float offset;

		public float offset_x;

		public float val;

		public UIJumpWord jumpWord;

		public Color color;

		public float movementStart
		{
			get
			{
				return this.time + this.stay;
			}
		}
	}

	public UIFont font;

	public UIFont fontOblique;

	private Color32 blue = new Color32(41, 202, 255, 255);

	private Color32 blueOutLine = new Color32(0, 29, 59, 255);

	private Color32 green = new Color32(0, 224, 42, 255);

	private Color32 greenOutLine = new Color32(0, 35, 8, 255);

	private Color32 red = new Color32(255, 16, 16, 255);

	private Color32 redOutLine = new Color32(59, 0, 0, 255);

	private Color32 yellow = new Color32(255, 168, 16, 255);

	private Color32 yellowOutLine = new Color32(67, 44, 0, 255);

	private Color32 blackOutLine = new Color32(0, 0, 0, 255);

	private Color32 gray = new Color32(236, 22, 255, 255);

	private Color32 grayOutLine = new Color32(16, 0, 35, 255);

	public static Color goldColor = new Color(1f, 1f, 0f, 1f);

	private StringBuilder _strBuilder = new StringBuilder();

	public AnimationCurve offsetCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.33f, 15f),
		new Keyframe(0.67f, 35f),
		new Keyframe(1f, 50f)
	});

	public AnimationCurve offsetCurve1 = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, -20f),
		new Keyframe(1f, 70f)
	});

	public AnimationCurve alphaCurve1 = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.4f),
		new Keyframe(0.7f, 1f),
		new Keyframe(0.9f, 1f),
		new Keyframe(1f, 0.8f)
	});

	public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0.5f),
		new Keyframe(0.33f, 0.7f),
		new Keyframe(0.67f, 1f),
		new Keyframe(1f, 1f)
	});

	public AnimationCurve scaleCurve1 = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1.2f)
	});

	public AnimationCurve scaleCurve2 = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 2f),
		new Keyframe(0.5f, 1.2f),
		new Keyframe(0.8f, 1f)
	});

	public AnimationCurve offsetCurve_y = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.2f, 120f),
		new Keyframe(0.3f, 140f),
		new Keyframe(0.8f, 70f)
	});

	public AnimationCurve offsetCurve_x = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.8f, 130f)
	});

	public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.05f, 1f),
		new Keyframe(0.3f, 1f),
		new Keyframe(0.5f, 0.6f),
		new Keyframe(0.7f, 0f),
		new Keyframe(0.8f, 0f)
	});

	public AnimationCurve pos_y = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 30f),
		new Keyframe(0.1f, -10f),
		new Keyframe(0.6f, 30f),
		new Keyframe(1.3f, 120f)
	});

	public AnimationCurve scale_x_y = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1.2f),
		new Keyframe(0.1f, 1f),
		new Keyframe(0.4f, 1f),
		new Keyframe(1.3f, 0.9f)
	});

	public AnimationCurve alpha = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.1f, 1f),
		new Keyframe(0.4f, 0.8f),
		new Keyframe(1.3f, 0f)
	});

	private List<HUDText.Entry> mList = new List<HUDText.Entry>();

	private int counter;

	private Keyframe[] offsets;

	private Keyframe[] alphas;

	private Keyframe[] scales;

	private HUDRoot hudRoot;

	private bool isShow;

	public bool isVisible
	{
		get
		{
			return this.mList.Count != 0;
		}
	}

	private static int Comparison(HUDText.Entry a, HUDText.Entry b)
	{
		if (a.movementStart < b.movementStart)
		{
			return -1;
		}
		if (a.movementStart > b.movementStart)
		{
			return 1;
		}
		return 0;
	}

	private void Start()
	{
		this.hudRoot = base.transform.parent.GetComponent<HUDRoot>();
		this.isShow = false;
		this.offsets = this.offsetCurve.keys;
		this.alphas = this.alphaCurve.keys;
		this.scales = this.scaleCurve.keys;
	}

	private HUDText.Entry Create(int type = 0)
	{
		this.isShow = true;
		HUDText.Entry entry = new HUDText.Entry
		{
			time = Time.realtimeSinceStartup,
			jumpWord = Singleton<CharacterView>.Instance.CreateJumpWord(base.gameObject.transform)
		};
		entry.jumpWord.transform.localPosition = Vector3.zero;
		if (type == 0)
		{
			entry.jumpWord.ShowDamage(true);
			entry.jumpWord.label.name = this.counter.ToString();
			entry.jumpWord.label.spacingX = -2;
			entry.jumpWord.label.effectStyle = UILabel.Effect.Shadow;
			entry.jumpWord.label.effectDistance = Vector2.one;
		}
		else if (type == 1)
		{
			entry.jumpWord.ShowGold();
		}
		this.mList.Add(entry);
		this.counter++;
		return entry;
	}

	private void Delete(HUDText.Entry ent)
	{
		this.mList.Remove(ent);
		Singleton<CharacterView>.Instance.DestroyJumpWord(ent.jumpWord);
		if (this.mList.Count == 0)
		{
			this.isShow = false;
		}
	}

	public void Add(object obj, Color c, float stayDuration)
	{
		if (!base.enabled)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bool flag = false;
		float num = 0f;
		if (obj is float)
		{
			flag = true;
			num = (float)obj;
		}
		else if (obj is int)
		{
			flag = true;
			num = (float)((int)obj);
		}
		if (flag)
		{
			if (num == 0f)
			{
				return;
			}
			int i = this.mList.Count;
			while (i > 0)
			{
				HUDText.Entry entry = this.mList[--i];
				if (entry.time + 1f >= realtimeSinceStartup)
				{
					if (entry.val != 0f)
					{
						if (entry.val < 0f && num < 0f)
						{
							entry.val += num;
							entry.jumpWord.label.text = Mathf.RoundToInt(entry.val).ToString();
							return;
						}
						if (entry.val > 0f && num > 0f)
						{
							entry.val += num;
							entry.jumpWord.label.text = "+" + Mathf.RoundToInt(entry.val);
							return;
						}
					}
				}
			}
		}
		int type = 0;
		if (c == HUDText.goldColor)
		{
			type = 1;
		}
		HUDText.Entry entry2 = this.Create(type);
		entry2.color = c;
		entry2.stay = stayDuration;
		entry2.jumpWord.label.applyGradient = false;
		entry2.val = num;
		if (flag)
		{
			entry2.jumpWord.label.text = ((num >= 0f) ? ("+" + Mathf.RoundToInt(entry2.val)) : Mathf.RoundToInt(entry2.val).ToString());
		}
		else
		{
			entry2.jumpWord.label.text = obj.ToString();
		}
		entry2.jumpWord.strValue = entry2.jumpWord.label.text;
	}

	private void OnDisable()
	{
		this.ClearJumpWords();
	}

	private void ClearJumpWords()
	{
		int i = this.mList.Count;
		while (i > 0)
		{
			HUDText.Entry entry = this.mList[--i];
			if (entry.jumpWord.label != null)
			{
				Singleton<CharacterView>.Instance.DestroyJumpWord(entry.jumpWord);
			}
		}
		this.mList.Clear();
	}

	public void ShowHudText(bool shown)
	{
		if (!shown)
		{
			this.ClearJumpWords();
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (this.hudRoot == null)
		{
			this.hudRoot = base.transform.parent.GetComponent<HUDRoot>();
			if (this.hudRoot == null)
			{
				return;
			}
		}
		if (this.isShow)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float time = this.offsets[this.offsets.Length - 1].time;
			float time2 = this.alphas[this.alphas.Length - 1].time;
			float time3 = this.scales[this.scales.Length - 1].time;
			float num = Mathf.Max(time3, Mathf.Max(time, time2));
			float num2 = 200f;
			int i = this.mList.Count;
			while (i > 0)
			{
				HUDText.Entry entry = this.mList[--i];
				entry.jumpWord.label.pivot = UIWidget.Pivot.Center;
				float num3 = realtimeSinceStartup - entry.time;
				if (entry.color == HUDText.goldColor)
				{
					entry.jumpWord.ShowGold();
				}
				else
				{
					entry.jumpWord.ShowDamage(entry.color == Color.clear);
				}
				if (entry.color == Color.white)
				{
					entry.jumpWord.label.color = Color.white;
					entry.jumpWord.label.effectStyle = UILabel.Effect.Outline;
					entry.jumpWord.label.effectColor = this.blackOutLine;
					entry.jumpWord.label.fontSize = 17;
					entry.offset = this.offsetCurve1.Evaluate(num3);
					entry.jumpWord.label.alpha = this.alphaCurve1.Evaluate(num3);
					float d = this.scaleCurve1.Evaluate(num3) * (float)this.hudRoot.FontSize;
					entry.jumpWord.label.cachedTransform.localScale = Vector3.one * d;
					entry.jumpWord.label.applyGradient = false;
				}
				else if (entry.color == Color.grey)
				{
					entry.jumpWord.label.color = Color.yellow;
					entry.jumpWord.label.effectStyle = UILabel.Effect.Outline;
					entry.jumpWord.label.effectColor = this.yellowOutLine;
					entry.jumpWord.label.fontSize = 17;
					entry.offset = this.offsetCurve1.Evaluate(num3);
					entry.jumpWord.label.alpha = this.alphaCurve1.Evaluate(num3);
					float d2 = this.scaleCurve1.Evaluate(num3) * (float)this.hudRoot.FontSize;
					entry.jumpWord.label.cachedTransform.localScale = Vector3.one * d2;
					entry.jumpWord.label.applyGradient = false;
				}
				else if (entry.color == Color.yellow || entry.color == Color.red || entry.color == Color.blue || entry.color == Color.green || entry.color == Color.gray)
				{
					entry.jumpWord.label.fontSize = 15;
					if (entry.color == Color.yellow)
					{
						entry.jumpWord.label.color = this.yellow;
						entry.jumpWord.label.effectColor = this.yellowOutLine;
					}
					else if (entry.color == Color.red)
					{
						entry.jumpWord.label.color = this.red;
						entry.jumpWord.label.effectColor = this.redOutLine;
					}
					else if (entry.color == Color.blue)
					{
						entry.jumpWord.label.color = this.blue;
						entry.jumpWord.label.effectColor = this.blueOutLine;
					}
					else if (entry.color == Color.green)
					{
						entry.jumpWord.label.color = this.green;
						entry.jumpWord.label.effectColor = this.greenOutLine;
					}
					else if (entry.color == Color.gray)
					{
						entry.jumpWord.label.color = this.gray;
						entry.jumpWord.label.effectColor = this.grayOutLine;
					}
					entry.jumpWord.label.effectStyle = UILabel.Effect.Outline;
					entry.offset = this.offsetCurve_y.Evaluate(num3);
					entry.offset_x = this.offsetCurve_x.Evaluate(num3);
					entry.jumpWord.label.alpha = this.alphaCurve.Evaluate(num3);
					float d3 = this.scaleCurve2.Evaluate(num3) * (float)this.hudRoot.FontSize;
					entry.jumpWord.label.cachedTransform.localScale = Vector3.one * d3;
				}
				else if (entry.color == Color.black)
				{
					entry.jumpWord.label.color = Color.white;
					entry.jumpWord.label.effectColor = this.blackOutLine;
					entry.jumpWord.label.fontSize = 15;
					entry.offset = this.pos_y.Evaluate(num3);
					entry.jumpWord.label.alpha = this.alpha.Evaluate(num3);
					float d4 = this.scale_x_y.Evaluate(num3) * (float)this.hudRoot.FontSize;
					entry.jumpWord.label.cachedTransform.localScale = Vector3.one * d4 * 1.4f;
				}
				else if (entry.color == Color.clear)
				{
					entry.jumpWord.label.pivot = UIWidget.Pivot.Left;
					entry.jumpWord.label.text = this.GetCriticalDamageStrByDamageVal(entry.jumpWord.label.text);
					entry.jumpWord.label.overflowMethod = UILabel.Overflow.ResizeFreely;
					entry.jumpWord.label.fontSize = 40;
					entry.jumpWord.label.color = Color.white;
					entry.jumpWord.label.gradientBottom = new Color32(255, 100, 0, 255);
					entry.jumpWord.label.gradientTop = new Color32(255, 216, 0, 255);
					entry.jumpWord.label.effectStyle = UILabel.Effect.Outline;
					entry.jumpWord.label.effectDistance = new Vector2(1f, 1f);
					entry.jumpWord.label.applyGradient = true;
					entry.jumpWord.label.effectColor = new Color32(129, 0, 0, 255);
					entry.jumpWord.sprite.gameObject.SetActive(true);
					entry.jumpWord.label.effectColor = this.redOutLine;
					entry.offset = this.offsetCurve_y.Evaluate(num3);
					entry.offset_x = this.offsetCurve_x.Evaluate(num3);
					entry.jumpWord.label.alpha = this.alphaCurve.Evaluate(num3);
					entry.jumpWord.label.cachedTransform.localScale = Vector3.one;
				}
				else if (entry.color == HUDText.goldColor)
				{
					entry.jumpWord.textMeshPro.text = this.GetGoldDispStrByGoldVal(entry.jumpWord.strValue);
					entry.offset = this.offsetCurve_x.Evaluate(num3);
					float d5 = this.scale_x_y.Evaluate(num3) * 10f;
					entry.jumpWord.textMeshPro.transform.localScale = Vector3.one * d5 * 1f;
				}
				if (num3 > num)
				{
					this.Delete(entry);
				}
				else
				{
					entry.jumpWord.label.enabled = true;
				}
			}
			int j = this.mList.Count;
			while (j > 0)
			{
				HUDText.Entry entry2 = this.mList[--j];
				if (entry2.color == Color.red || entry2.color == Color.clear)
				{
					entry2.jumpWord.label.cachedTransform.localPosition = new Vector3(entry2.offset_x - (float)((this.mList.Count - j) * 10), entry2.offset + (float)((this.mList.Count - j) * 35), 0f);
				}
				else if (entry2.color == Color.yellow || entry2.color == Color.green || entry2.color == Color.gray)
				{
					entry2.jumpWord.label.cachedTransform.localPosition = new Vector3(-entry2.offset_x + (float)(j * 10), entry2.offset + (float)(j * 50), 0f);
				}
				else if (entry2.color == Color.blue)
				{
					entry2.jumpWord.label.cachedTransform.localPosition = new Vector3(-entry2.offset_x - (float)(j * 20), entry2.offset + (float)(j * 30), 0f);
				}
				else if (entry2.color == Color.white)
				{
					num2 = entry2.offset + num2;
					entry2.jumpWord.label.cachedTransform.localPosition = new Vector3(59.47f, -100f + (float)(0 * (this.mList.Count - j)) + num2, 0f);
				}
				else if (entry2.color == Color.black)
				{
					entry2.jumpWord.label.cachedTransform.localPosition = new Vector3(0f, entry2.offset, 0f);
				}
				else if (entry2.color == HUDText.goldColor)
				{
					entry2.jumpWord.label.cachedTransform.localPosition = new Vector3(0f, entry2.offset + (float)(j * 5), 0f);
					entry2.jumpWord.textMeshPro.transform.localPosition = new Vector3(59.47f, entry2.offset / 1.3f + (float)(j * 5), 0f);
				}
			}
		}
	}

	private string GetGoldDispStrByGoldVal(string inGoldVal)
	{
		if (string.IsNullOrEmpty(inGoldVal))
		{
			return string.Empty;
		}
		this._strBuilder.Remove(0, this._strBuilder.Length);
		this._strBuilder.Append("n");
		int length = inGoldVal.Length;
		for (int i = 0; i < length; i++)
		{
			char c = inGoldVal[i];
			if (c.Equals('0'))
			{
				this._strBuilder.Append('a');
			}
			else if (c.Equals('1'))
			{
				this._strBuilder.Append('b');
			}
			else if (c.Equals('2'))
			{
				this._strBuilder.Append('c');
			}
			else if (c.Equals('3'))
			{
				this._strBuilder.Append('d');
			}
			else if (c.Equals('4'))
			{
				this._strBuilder.Append('e');
			}
			else if (c.Equals('5'))
			{
				this._strBuilder.Append('f');
			}
			else if (c.Equals('6'))
			{
				this._strBuilder.Append('g');
			}
			else if (c.Equals('7'))
			{
				this._strBuilder.Append('h');
			}
			else if (c.Equals('8'))
			{
				this._strBuilder.Append('i');
			}
			else if (c.Equals('9'))
			{
				this._strBuilder.Append('j');
			}
		}
		return this._strBuilder.ToString();
	}

	private string GetCriticalDamageStrByDamageVal(string inVal)
	{
		if (string.IsNullOrEmpty(inVal))
		{
			return string.Empty;
		}
		this._strBuilder.Remove(0, this._strBuilder.Length);
		int length = inVal.Length;
		for (int i = 0; i < length; i++)
		{
			char c = inVal[i];
			if (c.Equals('0') || c.Equals('A'))
			{
				this._strBuilder.Append('A');
			}
			else if (c.Equals('1') || c.Equals('B'))
			{
				this._strBuilder.Append('B');
			}
			else if (c.Equals('2') || c.Equals('C'))
			{
				this._strBuilder.Append('C');
			}
			else if (c.Equals('3') || c.Equals('D'))
			{
				this._strBuilder.Append('D');
			}
			else if (c.Equals('4') || c.Equals('E'))
			{
				this._strBuilder.Append('E');
			}
			else if (c.Equals('5') || c.Equals('F'))
			{
				this._strBuilder.Append('F');
			}
			else if (c.Equals('6') || c.Equals('G'))
			{
				this._strBuilder.Append('G');
			}
			else if (c.Equals('7') || c.Equals('H'))
			{
				this._strBuilder.Append('H');
			}
			else if (c.Equals('8') || c.Equals('I'))
			{
				this._strBuilder.Append('I');
			}
			else if (c.Equals('9') || c.Equals('J'))
			{
				this._strBuilder.Append('J');
			}
		}
		return this._strBuilder.ToString();
	}

	private string GetStr(string str)
	{
		int num = str.IndexOf('-');
		if (num >= 0)
		{
			return str.Substring(num + 1);
		}
		num = str.IndexOf('+');
		if (num >= 0)
		{
			return str.Substring(num + 1);
		}
		return str;
	}
}
