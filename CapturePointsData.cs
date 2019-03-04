using Svelto.ES.Legacy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CapturePointsData : MonoBehaviour, IComponent
{
	[NonSerialized]
	public Transform capturePoint0;

	[NonSerialized]
	public Transform capturePoint1;

	[NonSerialized]
	public Transform capturePoint2;

	public float captureRadius = 14f;

	public event Action<IComponent> OnReady = delegate
	{
	};

	public CapturePointsData()
		: this()
	{
	}

	private void Start()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		List<Transform> list = new List<Transform>();
		IEnumerator enumerator = this.get_transform().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform item = enumerator.Current;
				list.Add(item);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		list.Sort((Transform a, Transform b) => a.get_name().CompareTo(b.get_name()));
		AssignPositions(list);
		this.OnReady(this);
	}

	private void AssignPositions(List<Transform> capturePoints)
	{
		capturePoint0 = capturePoints[0];
		capturePoint1 = capturePoints[1];
		capturePoint2 = capturePoints[2];
	}
}
