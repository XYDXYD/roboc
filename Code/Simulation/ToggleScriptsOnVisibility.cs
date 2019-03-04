using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class ToggleScriptsOnVisibility : MonoBehaviour
	{
		private Rigidbody _rb;

		private float _findViewportTolerance = 0.2f;

		private float _loseViewportTolerance = 0.1f;

		private float _cameraLowerInactivityDistance;

		private float _cameraUpperInactivityDistance;

		private float _maxCameraActivityDistance;

		private float _sqrMaxCameraActivityDistance;

		private List<IVisibilityTracker> _visiblityTrackers = new List<IVisibilityTracker>();

		private Camera mainCam;

		private Transform mainCamTransform;

		private bool _isOffScreen;

		private bool _isInRange;

		[Inject]
		internal ZoomEngine zoom
		{
			private get;
			set;
		}

		public ToggleScriptsOnVisibility()
			: this()
		{
		}

		private void Awake()
		{
			_rb = this.GetComponent<Rigidbody>();
			MonoBehaviour[] componentsInChildren = this.GetComponentsInChildren<MonoBehaviour>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] is IVisibilityTracker)
				{
					_visiblityTrackers.Add(componentsInChildren[i] as IVisibilityTracker);
				}
			}
			mainCam = Camera.get_main();
			mainCamTransform = mainCam.get_transform();
		}

		public void InitialiseScriptsOnVisibilitySettings(ScriptsOnVisibilitySettings toggleScriptsSettings)
		{
			_findViewportTolerance = toggleScriptsSettings.findViewportTolerance;
			_loseViewportTolerance = toggleScriptsSettings.loseViewportTolerance;
			_cameraLowerInactivityDistance = toggleScriptsSettings.cameraLowerInactivityDistance;
			_cameraUpperInactivityDistance = toggleScriptsSettings.cameraUpperInactivityDistance;
			_maxCameraActivityDistance = toggleScriptsSettings.maxCameraActivityDistance;
			_sqrMaxCameraActivityDistance = _maxCameraActivityDistance * _maxCameraActivityDistance;
		}

		private void Update()
		{
			if (mainCam == null)
			{
				return;
			}
			if (_rb == null)
			{
				this.set_enabled(false);
				return;
			}
			bool flag = IsVisible();
			bool flag2 = IsInRange();
			if (!_isOffScreen == flag && _isInRange == flag2)
			{
				return;
			}
			_isOffScreen = !flag;
			_isInRange = flag2;
			for (int i = 0; i < _visiblityTrackers.Count; i++)
			{
				IVisibilityTracker visibilityTracker = _visiblityTrackers[i];
				if (visibilityTracker != null)
				{
					visibilityTracker.isOffScreen = !flag;
					visibilityTracker.isInRange = flag2;
				}
				else
				{
					_visiblityTrackers.UnorderredListRemoveAt(i--);
				}
			}
		}

		private bool IsVisible()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			float num = zoom.sqrDrawDistanceScale;
			if (num == 0f)
			{
				num = 1f;
			}
			Vector3 val = mainCam.WorldToViewportPoint(_rb.get_worldCenterOfMass());
			float num2 = (!_isOffScreen) ? _loseViewportTolerance : _findViewportTolerance;
			num2 *= num;
			bool flag = val.y >= 0f - num2 && val.y <= 1f + num2 && val.x >= 0f - num2 && val.x <= 1f + num2 && val.z >= 0f;
			Vector3 val2 = _rb.get_worldCenterOfMass() - mainCamTransform.get_position();
			float sqrMagnitude = val2.get_sqrMagnitude();
			return flag & (sqrMagnitude <= _sqrMaxCameraActivityDistance * num);
		}

		private bool IsInRange()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _rb.get_worldCenterOfMass() - mainCamTransform.get_position();
			float sqrMagnitude = val.get_sqrMagnitude();
			if (!_isInRange)
			{
				return sqrMagnitude <= _cameraLowerInactivityDistance * _cameraLowerInactivityDistance;
			}
			return sqrMagnitude <= _cameraUpperInactivityDistance * _cameraUpperInactivityDistance;
		}
	}
}
