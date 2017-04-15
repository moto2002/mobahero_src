using System;
using UnityEngine;

public class FixMapValue : MonoBehaviour
{
	public float minX;

	public float maxX;

	public float minY;

	public float maxY;

	private void Start()
	{
	}

	public void SetValue(float _minX, float _maxX, float _minY, float _maxY)
	{
		this.minX = _minX;
		this.maxX = _maxX;
		this.minY = _minY;
		this.maxY = _maxY;
	}

	private void Update()
	{
	}
}
