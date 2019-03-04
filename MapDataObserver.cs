using System;
using UnityEngine;

internal sealed class MapDataObserver
{
	public event Action<Transform, Transform> OnInitializationData = delegate
	{
	};

	public void InitializeData(Transform bottomR, Transform topL)
	{
		this.OnInitializationData(bottomR, topL);
	}
}
