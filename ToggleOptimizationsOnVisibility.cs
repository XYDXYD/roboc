using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ToggleOptimizationsOnVisibility : MonoBehaviour
{
	private float _findViewportTolerance = 0.2f;

	private float _loseViewportTolerance = 0.1f;

	private float _maxCameraActivityDistance;

	private Rigidbody _rb;

	private List<MonoBehaviour> _scriptsToDisableOffScreen = new List<MonoBehaviour>();

	private HashSet<MonoBehaviour> _scriptsToDisableMultiplayer = new HashSet<MonoBehaviour>();

	private List<HeadLightSwitch> _lights = new List<HeadLightSwitch>();

	private Camera mainCam;

	private Transform mainCamTransform;

	private float _sqrMaxCameraActivityDistance;

	[Inject]
	internal ZoomEngine zoom
	{
		private get;
		set;
	}

	public bool isPaused
	{
		get;
		private set;
	}

	public bool toggleScripts
	{
		get;
		set;
	}

	public ToggleOptimizationsOnVisibility()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		isPaused = false;
		toggleScripts = true;
		_rb = this.GetComponent<Rigidbody>();
		_lights.AddRange(this.GetComponentsInChildren<HeadLightSwitch>());
		MonoBehaviour[] componentsInChildren = this.GetComponentsInChildren<MonoBehaviour>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] is IDisableOffScreen)
			{
				_scriptsToDisableOffScreen.Add(componentsInChildren[i]);
			}
		}
		mainCam = Camera.get_main();
		mainCamTransform = mainCam.get_transform();
		Debug.Log((object)("Actual Render Path: " + mainCam.get_actualRenderingPath()), this);
	}

	private void Start()
	{
		if (!toggleScripts)
		{
			return;
		}
		MonoBehaviour[] componentsInChildren = this.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] array = componentsInChildren;
		foreach (MonoBehaviour val in array)
		{
			if (val is IDisableInMultiplayer)
			{
				_scriptsToDisableMultiplayer.Add(val);
				val.set_enabled(!toggleScripts);
			}
		}
	}

	public void InitialiseOptimizationsOnVisibilitySettings(OptimizationsOnVisibilitySettings toggleKinematicInfo)
	{
		_findViewportTolerance = toggleKinematicInfo.findViewportTolerance;
		_loseViewportTolerance = toggleKinematicInfo.loseViewportTolerance;
		_maxCameraActivityDistance = toggleKinematicInfo.maxCameraActivityDistance;
		_sqrMaxCameraActivityDistance = _maxCameraActivityDistance * _maxCameraActivityDistance;
	}

	private void Update()
	{
		if (!(mainCam == null))
		{
			bool flag = IsVisible();
			if (!isPaused != flag)
			{
				isPaused = !flag;
				this.StopAllCoroutines();
				this.StartCoroutine(EnableObjects(flag));
			}
		}
	}

	private IEnumerator EnableObjects(bool visible)
	{
		if (_scriptsToDisableOffScreen.Count > 0)
		{
			yield return null;
		}
		for (int j = 0; j < _scriptsToDisableOffScreen.Count; j++)
		{
			MonoBehaviour val = _scriptsToDisableOffScreen[j];
			if (val != null)
			{
				if (!_scriptsToDisableMultiplayer.Contains(val))
				{
					val.set_enabled(visible);
				}
			}
			else
			{
				List<MonoBehaviour> scriptsToDisableOffScreen = _scriptsToDisableOffScreen;
				int index;
				j = (index = j) - 1;
				scriptsToDisableOffScreen.UnorderredListRemoveAt(index);
			}
		}
		if (_lights.Count > 0)
		{
			yield return null;
		}
		for (int j = 0; j < _lights.Count; j++)
		{
			HeadLightSwitch headLightSwitch = _lights[j];
			if (headLightSwitch != null && headLightSwitch.get_gameObject().get_activeSelf())
			{
				headLightSwitch.SetLight(visible);
			}
		}
	}

	private bool IsVisible()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (_rb == null)
		{
			this.set_enabled(false);
			return false;
		}
		float num = zoom.sqrDrawDistanceScale;
		if (num == 0f)
		{
			num = 1f;
		}
		Vector3 val = mainCam.WorldToViewportPoint(_rb.get_worldCenterOfMass());
		float num2 = (!isPaused) ? _loseViewportTolerance : _findViewportTolerance;
		num2 *= num;
		bool flag = val.y >= 0f - num2 && val.y <= 1f + num2 && val.x >= 0f - num2 && val.x <= 1f + num2 && val.z >= 0f;
		Vector3 val2 = _rb.get_worldCenterOfMass() - mainCamTransform.get_position();
		float sqrMagnitude = val2.get_sqrMagnitude();
		return flag & (sqrMagnitude <= _sqrMaxCameraActivityDistance * num);
	}
}
