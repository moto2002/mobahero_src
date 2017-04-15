using CodeStage.AdvancedFPSCounter.Labels;
using System;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter
{
	public class APITester : MonoBehaviour
	{
		private int selectedTab;

		private readonly string[] tabs = new string[]
		{
			"Common",
			"FPS Counter",
			"Memory Counter",
			"Device info"
		};

		private void OnGUI()
		{
			GUILayout.BeginArea(new Rect(40f, 110f, (float)(Screen.width - 80), (float)(Screen.height - 140)));
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.alignment = TextAnchor.UpperCenter;
			gUIStyle.richText = true;
			GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.label);
			gUIStyle2.richText = true;
			GUILayout.Label("<b>Public API usage examples</b>", gUIStyle, new GUILayoutOption[0]);
			this.selectedTab = GUILayout.Toolbar(this.selectedTab, this.tabs, new GUILayoutOption[0]);
			if (this.selectedTab == 0)
			{
				GUILayout.Space(10f);
				GUILayout.Label("Operation Mode", new GUILayoutOption[0]);
				int num = (int)AFPSCounter.Instance.OperationMode;
				num = GUILayout.Toolbar(num, new string[]
				{
					AFPSCounterOperationMode.Disabled.ToString(),
					AFPSCounterOperationMode.Background.ToString(),
					AFPSCounterOperationMode.Normal.ToString()
				}, new GUILayoutOption[0]);
				AFPSCounter.Instance.OperationMode = (AFPSCounterOperationMode)num;
				GUILayout.Label("Hot Key", new GUILayoutOption[0]);
				int num2;
				if (AFPSCounter.Instance.hotKey == KeyCode.BackQuote)
				{
					num2 = 1;
				}
				else
				{
					num2 = (int)AFPSCounter.Instance.hotKey;
				}
				num2 = GUILayout.Toolbar(num2, new string[]
				{
					"None (disabled)",
					"BackQoute (`)"
				}, new GUILayoutOption[0]);
				if (num2 == 1)
				{
					AFPSCounter.Instance.hotKey = KeyCode.BackQuote;
				}
				else
				{
					AFPSCounter.Instance.hotKey = KeyCode.None;
				}
				AFPSCounter.Instance.keepAlive = GUILayout.Toggle(AFPSCounter.Instance.keepAlive, "Keep Alive", new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				AFPSCounter.Instance.ForceFrameRate = GUILayout.Toggle(AFPSCounter.Instance.ForceFrameRate, "Force FPS", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				AFPSCounter.Instance.ForcedFrameRate = (int)this.SliderLabel((float)AFPSCounter.Instance.ForcedFrameRate, -1f, 100f);
				GUILayout.EndHorizontal();
				float num3 = AFPSCounter.Instance.AnchorsOffset.x;
				float num4 = AFPSCounter.Instance.AnchorsOffset.y;
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Pixel offset X", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				num3 = (float)((int)this.SliderLabel(num3, 0f, 100f));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Pixel offset Y", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				num4 = (float)((int)this.SliderLabel(num4, 0f, 100f));
				GUILayout.EndHorizontal();
				AFPSCounter.Instance.AnchorsOffset = new Vector2(num3, num4);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Font Size", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				AFPSCounter.Instance.FontSize = (int)this.SliderLabel((float)AFPSCounter.Instance.FontSize, 0f, 100f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Line spacing", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				AFPSCounter.Instance.LineSpacing = this.SliderLabel(AFPSCounter.Instance.LineSpacing, 0f, 10f);
				GUILayout.EndHorizontal();
			}
			else if (this.selectedTab == 1)
			{
				GUILayout.Space(10f);
				AFPSCounter.Instance.fpsCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.Enabled, "Enabled", new GUILayoutOption[0]);
				GUILayout.Space(10f);
				AFPSCounter.Instance.fpsCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.fpsCounter.Anchor, new string[]
				{
					"UpperLeft",
					"UpperRight",
					"LowerLeft",
					"LowerRight"
				}, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Update Interval", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				AFPSCounter.Instance.fpsCounter.UpdateInterval = this.SliderLabel(AFPSCounter.Instance.fpsCounter.UpdateInterval, 0.1f, 10f);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				AFPSCounter.Instance.fpsCounter.ShowAverage = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.ShowAverage, "Average FPS", new GUILayoutOption[0]);
				if (AFPSCounter.Instance.fpsCounter.ShowAverage)
				{
					GUILayout.Label("Samples", new GUILayoutOption[]
					{
						GUILayout.Width(60f)
					});
					AFPSCounter.Instance.fpsCounter.AverageFromSamples = (int)this.SliderLabel((float)AFPSCounter.Instance.fpsCounter.AverageFromSamples, 0f, 100f);
					GUILayout.Space(10f);
					AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetAverageOnNewScene, "Reset Average On New Scene Load", new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					});
					if (GUILayout.Button("Reset now!", new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						AFPSCounter.Instance.fpsCounter.ResetAverage();
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				AFPSCounter.Instance.fpsCounter.ShowMinMax = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.ShowMinMax, "MinMax FPS", new GUILayoutOption[0]);
				if (AFPSCounter.Instance.fpsCounter.ShowMinMax)
				{
					AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene = GUILayout.Toggle(AFPSCounter.Instance.fpsCounter.resetMinMaxOnNewScene, "Reset MinMax On New Scene Load", new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					});
					if (GUILayout.Button("Reset now!", new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						AFPSCounter.Instance.fpsCounter.ResetMinMax();
					}
				}
				GUILayout.EndHorizontal();
			}
			else if (this.selectedTab == 2)
			{
				GUILayout.Space(10f);
				AFPSCounter.Instance.memoryCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Enabled, "Enabled", new GUILayoutOption[0]);
				GUILayout.Space(10f);
				AFPSCounter.Instance.memoryCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.memoryCounter.Anchor, new string[]
				{
					"UpperLeft",
					"UpperRight",
					"LowerLeft",
					"LowerRight"
				}, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label("Update Interval", new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
				AFPSCounter.Instance.memoryCounter.UpdateInterval = this.SliderLabel(AFPSCounter.Instance.memoryCounter.UpdateInterval, 0.1f, 10f);
				GUILayout.EndHorizontal();
				AFPSCounter.Instance.memoryCounter.PreciseValues = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.PreciseValues, "Precise (uses more system resources)", new GUILayoutOption[0]);
				AFPSCounter.Instance.memoryCounter.TotalReserved = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.TotalReserved, "Show total reserved memory size", new GUILayoutOption[0]);
				AFPSCounter.Instance.memoryCounter.Allocated = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.Allocated, "Show allocated memory size", new GUILayoutOption[0]);
				AFPSCounter.Instance.memoryCounter.MonoUsage = GUILayout.Toggle(AFPSCounter.Instance.memoryCounter.MonoUsage, "Show mono memory usage", new GUILayoutOption[0]);
			}
			else if (this.selectedTab == 3)
			{
				GUILayout.Space(10f);
				AFPSCounter.Instance.deviceInfoCounter.Enabled = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.Enabled, "Enabled", new GUILayoutOption[0]);
				GUILayout.Space(10f);
				AFPSCounter.Instance.deviceInfoCounter.Anchor = (LabelAnchor)GUILayout.Toolbar((int)AFPSCounter.Instance.deviceInfoCounter.Anchor, new string[]
				{
					"UpperLeft",
					"UpperRight",
					"LowerLeft",
					"LowerRight"
				}, new GUILayoutOption[0]);
				GUILayout.Space(10f);
				AFPSCounter.Instance.deviceInfoCounter.CpuModel = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.CpuModel, "Show CPU model and maximum threads count", new GUILayoutOption[0]);
				AFPSCounter.Instance.deviceInfoCounter.GpuModel = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.GpuModel, "Show GPU model and total VRAM count", new GUILayoutOption[0]);
				AFPSCounter.Instance.deviceInfoCounter.RamSize = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.RamSize, "Show total RAM size", new GUILayoutOption[0]);
				AFPSCounter.Instance.deviceInfoCounter.ScreenData = GUILayout.Toggle(AFPSCounter.Instance.deviceInfoCounter.ScreenData, "Show resolution, window size and DPI (if possible)", new GUILayoutOption[0]);
			}
			int lastAverageValue = AFPSCounter.Instance.fpsCounter.lastAverageValue;
			GUILayout.Label("<b>Raw counters values</b> (read using API)", gUIStyle2, new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			GUILayout.Label(string.Concat(new object[]
			{
				"  FPS: ",
				AFPSCounter.Instance.fpsCounter.lastValue,
				"  AVG: ",
				lastAverageValue,
				"\n  MIN: ",
				AFPSCounter.Instance.fpsCounter.lastMinimumValue,
				"  MAX: ",
				AFPSCounter.Instance.fpsCounter.lastMaximumValue
			}), new GUILayoutOption[0]);
			if (AFPSCounter.Instance.memoryCounter.PreciseValues)
			{
				GUILayout.Label(string.Concat(new object[]
				{
					"  Memory (Total, Allocated, Mono):\n  ",
					AFPSCounter.Instance.memoryCounter.lastTotalValue / 1048576f,
					", ",
					AFPSCounter.Instance.memoryCounter.lastAllocatedValue / 1048576f,
					", ",
					(float)AFPSCounter.Instance.memoryCounter.lastMonoValue / 1048576f
				}), new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label(string.Concat(new object[]
				{
					"  Memory (Total, Allocated, Mono):\n  ",
					AFPSCounter.Instance.memoryCounter.lastTotalValue,
					", ",
					AFPSCounter.Instance.memoryCounter.lastAllocatedValue,
					", ",
					AFPSCounter.Instance.memoryCounter.lastMonoValue
				}), new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.Label(AFPSCounter.Instance.deviceInfoCounter.lastValue, new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private float SliderLabel(float sliderValue, float sliderMinValue, float sliderMaxValue)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			sliderValue = GUILayout.HorizontalSlider(sliderValue, sliderMinValue, sliderMaxValue, new GUILayoutOption[0]);
			GUILayout.Space(10f);
			GUILayout.Label(string.Format("{0:F2}", sliderValue), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(false)
			});
			GUILayout.EndHorizontal();
			return sliderValue;
		}
	}
}
