using System;
using UnityEngine;

public class FxmTestSingleMain : MonoBehaviour
{
	public GameObject[] m_EffectPrefabs = new GameObject[1];

	public GUIText m_EffectGUIText;

	public int m_nIndex;

	public float m_fCreateScale = 1f;

	public int m_nCreateCount = 1;

	public float m_fRandomRange = 1f;

	public FxmTestSingleMouse m_FXMakerMouse;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void Start()
	{
		Resources.UnloadUnusedAssets();
		base.Invoke("CreateEffect", 1f);
	}

	private void CreateEffect()
	{
		if (this.m_EffectPrefabs[this.m_nIndex] == null)
		{
			return;
		}
		if (this.m_EffectGUIText != null)
		{
			this.m_EffectGUIText.text = this.m_EffectPrefabs[this.m_nIndex].name;
		}
		float num = 0f;
		if (1 < this.m_nCreateCount)
		{
			num = this.m_fRandomRange;
		}
		for (int i = 0; i < this.GetInstanceRoot().transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(this.GetInstanceRoot().transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.m_nCreateCount; j++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.m_EffectPrefabs[this.m_nIndex], new Vector3(UnityEngine.Random.Range(-num, num), 0f, UnityEngine.Random.Range(-num, num)), Quaternion.identity);
			gameObject.transform.localScale = gameObject.transform.localScale * this.m_fCreateScale;
			NsEffectManager.PreloadResource(gameObject);
			gameObject.transform.parent = this.GetInstanceRoot().transform;
			FxmTestSingleMain.SetActiveRecursively(gameObject, true);
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		float num = (float)(Screen.height / 10);
		float num2 = GUI.VerticalSlider(new Rect(10f, num + 10f + 30f, 25f, (float)Screen.height - (num + 10f + 50f)), this.GetFXMakerMouse().m_fDistance, this.GetFXMakerMouse().m_fDistanceMin, this.GetFXMakerMouse().m_fDistanceMax);
		if (num2 != this.GetFXMakerMouse().m_fDistance)
		{
			this.GetFXMakerMouse().SetDistance(num2);
		}
		if (GUI.Button(FxmTestSingleMain.GetButtonRect(0), "Next"))
		{
			if (this.m_nIndex < this.m_EffectPrefabs.Length - 1)
			{
				this.m_nIndex++;
			}
			else
			{
				this.m_nIndex = 0;
			}
			this.CreateEffect();
		}
		if (GUI.Button(FxmTestSingleMain.GetButtonRect(1), "Recreate"))
		{
			this.CreateEffect();
		}
	}

	public FxmTestSingleMouse GetFXMakerMouse()
	{
		if (this.m_FXMakerMouse == null)
		{
			this.m_FXMakerMouse = base.GetComponentInChildren<FxmTestSingleMouse>();
		}
		return this.m_FXMakerMouse;
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	public static Rect GetButtonRect()
	{
		int num = 2;
		return new Rect((float)(Screen.width - Screen.width / 10 * num), (float)(Screen.height - Screen.height / 10), (float)(Screen.width / 10 * num), (float)(Screen.height / 10));
	}

	public static Rect GetButtonRect(int nIndex)
	{
		return new Rect((float)(Screen.width - Screen.width / 10 * (nIndex + 1)), (float)(Screen.height - Screen.height / 10), (float)(Screen.width / 10), (float)(Screen.height / 10));
	}

	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
		int num = target.transform.childCount - 1;
		while (0 <= num)
		{
			if (num < target.transform.childCount)
			{
				FxmTestSingleMain.SetActiveRecursively(target.transform.GetChild(num).gameObject, bActive);
			}
			num--;
		}
		target.SetActive(bActive);
	}
}
