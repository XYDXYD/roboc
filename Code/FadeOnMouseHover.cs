using System;
using System.Collections.Generic;
using UnityEngine;

internal class FadeOnMouseHover : MonoBehaviour
{
	[Tooltip("The gameobject for which the mouseover is being detected")]
	public GameObject[] HoverTargets;

	[Tooltip("Check this to include children of the main hover target")]
	public bool IncludeChildren;

	[Tooltip("The game obects to fade (including their children)")]
	public UIWidget[] FadeObjects;

	[Range(0f, 1f)]
	public float MinAlpha;

	[Range(0f, 1f)]
	public float MaxAlpha = 1f;

	[Tooltip("How long it takes to transition from the min to max value, in seconds. Must be >= 0")]
	public float Duration;

	[Tooltip("Check this to fade out on mouseover (normal behaviour is to fade in on mouseover)")]
	public bool Invert;

	private float _lastAlpha;

	private float _rate;

	private HashSet<GameObject> _allTargets;

	public FadeOnMouseHover()
		: this()
	{
	}

	public void Start()
	{
		_allTargets = new HashSet<GameObject>(HoverTargets);
		if (IncludeChildren)
		{
			for (int i = 0; i < HoverTargets.Length; i++)
			{
				GameObject val = HoverTargets[i];
				BoxCollider[] componentsInChildren = val.GetComponentsInChildren<BoxCollider>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					_allTargets.Add(componentsInChildren[j].get_gameObject());
				}
			}
		}
		CalculateRate();
	}

	public void CalculateRate()
	{
		if (MinAlpha >= MaxAlpha)
		{
			throw new Exception("MinAlpha must be < MaxAlpha");
		}
		_rate = Math.Abs(MaxAlpha - MinAlpha) / Duration;
		if (Invert)
		{
			_rate = 0f - _rate;
		}
		if (Duration < 0f)
		{
			throw new Exception("Duration must be >= 0");
		}
	}

	public void Update()
	{
		bool flag = false;
		GameObject hoveredObject = UICamera.get_hoveredObject();
		foreach (GameObject allTarget in _allTargets)
		{
			if (allTarget == hoveredObject)
			{
				flag = true;
				break;
			}
		}
		float num = _lastAlpha + ((!flag) ? (0f - _rate) : _rate) * Time.get_deltaTime();
		if (Math.Abs(num - _lastAlpha) > float.Epsilon)
		{
			SetAlpha(num);
		}
	}

	private void SetAlpha(float alpha)
	{
		alpha = Mathf.Clamp(alpha, MinAlpha, MaxAlpha);
		for (int i = 0; i < FadeObjects.Length; i++)
		{
			UIWidget val = FadeObjects[i];
			val.set_alpha(alpha);
		}
		_lastAlpha = alpha;
	}
}
