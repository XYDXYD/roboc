using System;
using UnityEngine;

public class CameraPreviewUtility
{
	public struct CameraConfiguration
	{
		public readonly float distanceFromTargetMinimum;

		public readonly float distanceFromTargetMaximum;

		public readonly float pitchMin;

		public readonly float pitchMax;

		public readonly float zoomSpeed;

		public readonly float draggingRotationSpeed;

		public readonly float FOVAtMaxZoomOut;

		public readonly float FOVNormal;

		public readonly float distanceToNearestWall;

		public readonly float zoomLevelToBeginHuggingWall;

		public CameraConfiguration(float distanceFromTargetMinimum_, float distanceFromTargetMaximum_, float pitchMin_, float pitchMax_, float zoomSpeed_, float draggingRotationSpeed_, float FOVAtMaxZoomOut_, float FOVNormal_, float distanceToNearestWall_, float zoomLevelToBeginHuggingWall_)
		{
			distanceFromTargetMinimum = distanceFromTargetMinimum_;
			distanceFromTargetMaximum = distanceFromTargetMaximum_;
			pitchMin = pitchMin_;
			pitchMax = pitchMax_;
			zoomSpeed = zoomSpeed_;
			draggingRotationSpeed = draggingRotationSpeed_;
			FOVAtMaxZoomOut = FOVAtMaxZoomOut_;
			FOVNormal = FOVNormal_;
			distanceToNearestWall = distanceToNearestWall_;
			zoomLevelToBeginHuggingWall = zoomLevelToBeginHuggingWall_;
		}
	}

	private const float INITIAL_YAW = 45f;

	private const float INITIAL_PITCH = 30f;

	private CameraConfiguration? _cameraConfig;

	private Vector3? _centre;

	private Vector3? _machineBoundsMin;

	private Vector3? _machineBoundsMax;

	private float _desiredZoomAmount = 0.5f;

	private float _currentYaw = 45f;

	private float _currentPitch = 30f;

	private float _yOffset;

	public CameraConfiguration? GetConfig()
	{
		return _cameraConfig;
	}

	public void SetConfig(CameraConfiguration config)
	{
		_cameraConfig = config;
	}

	public Vector3? GetCentre()
	{
		return _centre;
	}

	public void SetCentre(Vector3 centre)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_centre = centre;
	}

	public Vector3? GetMachineBoundsMin()
	{
		return _machineBoundsMin;
	}

	public Vector3? GetMachineBoundsMax()
	{
		return _machineBoundsMax;
	}

	public float GetDesiredZoomAmount()
	{
		return _desiredZoomAmount;
	}

	public void SetDesiredZoomAmount(float value)
	{
		_desiredZoomAmount = value;
	}

	public float GetCurrentYaw()
	{
		return _currentYaw;
	}

	public void SetCurrentYaw(float value)
	{
		_currentYaw = value;
	}

	public float GetCurrentPitch()
	{
		return _currentPitch;
	}

	public void SetCurrentPitch(float value)
	{
		_currentPitch = value;
	}

	public void ResetYawAndPitch()
	{
		_currentPitch = 30f;
		_currentYaw = 45f;
	}

	public void SetMachineSize(Vector3 min, Vector3 max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_machineBoundsMin = min;
		_machineBoundsMax = max;
		_yOffset = (max.y + min.y) / 2f;
	}

	public Vector3 CalculateCameraTarget()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 value = _centre.Value;
		value.y += _yOffset;
		return value;
	}

	public void ReFrameRobotInMiddleOfCamera(Camera camera, float currentDistanceFromCameraTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CalculateCameraTarget();
		Vector3 val2 = _machineBoundsMax.Value - _machineBoundsMin.Value;
		Vector3 val3 = val2 / 2f;
		float num = val.y + val2.y;
		CalculateZoomOffsetToApplyToCamera(camera, new Vector3(val.x + val3.x, num, val.z + val3.z), out float zoomOut, out float zoomIn);
		float num2 = zoomOut;
		float num3 = zoomIn;
		CalculateZoomOffsetToApplyToCamera(camera, new Vector3(val.x - val3.x, num, val.z + val3.z), out zoomOut, out zoomIn);
		if (zoomOut > num2)
		{
			num2 = zoomOut;
		}
		if (zoomIn < num3)
		{
			num3 = zoomIn;
		}
		CalculateZoomOffsetToApplyToCamera(camera, new Vector3(val.x + val3.x, num, val.z - val3.z), out zoomOut, out zoomIn);
		if (zoomOut > num2)
		{
			num2 = zoomOut;
		}
		if (zoomIn < num3)
		{
			num3 = zoomIn;
		}
		CalculateZoomOffsetToApplyToCamera(camera, new Vector3(val.x - val3.x, num, val.z - val3.z), out zoomOut, out zoomIn);
		if (zoomOut > num2)
		{
			num2 = zoomOut;
		}
		if (zoomIn < num3)
		{
			num3 = zoomIn;
		}
		Debug.Log((object)("Camera adjustment required zoom out= " + num2 + " zoom in " + num3));
		if (num2 > 0f)
		{
			ApplyWorldSpaceDistanceToCameraZoom(num2, currentDistanceFromCameraTarget);
		}
		else if (num3 > 0f)
		{
			ApplyWorldSpaceDistanceToCameraZoom(0f - num3, currentDistanceFromCameraTarget);
		}
	}

	public void UpdateCameraFOV(Camera camera, bool revertToNormal)
	{
		if (revertToNormal)
		{
			CameraConfiguration value = _cameraConfig.Value;
			camera.set_fieldOfView(value.FOVNormal);
			return;
		}
		CameraConfiguration value2 = _cameraConfig.Value;
		float fOVNormal = value2.FOVNormal;
		CameraConfiguration value3 = _cameraConfig.Value;
		camera.set_fieldOfView(Mathf.Lerp(fOVNormal, value3.FOVAtMaxZoomOut, _desiredZoomAmount));
	}

	public float RecalculateCameraPosition(Camera camera, out Vector3 calculatedPosition)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 rayCastDirectionNormalised = GetRayCastDirectionNormalised();
		float num = CalculateMaxDistance();
		float num2 = CalculateMinimumDistance(0f, 0f);
		float num3 = num2;
		float num4 = num2;
		for (int i = -20; i < 20; i++)
		{
			float num5 = (float)i / 2f;
			float num6 = Mathf.Cos(num5 * 18f * ((float)Math.PI / 180f));
			float num7 = CalculateMinimumDistance(0f, num5);
			float num8 = Mathf.Lerp(num2, num7, num6);
			if (num8 > num4)
			{
				num4 = num8;
			}
		}
		CameraConfiguration value = _cameraConfig.Value;
		float distanceFromTargetMinimum = value.distanceFromTargetMinimum;
		float desiredZoomAmount = _desiredZoomAmount;
		CameraConfiguration value2 = _cameraConfig.Value;
		float num13;
		if (desiredZoomAmount > value2.zoomLevelToBeginHuggingWall)
		{
			float num9 = distanceFromTargetMinimum;
			CameraConfiguration value3 = _cameraConfig.Value;
			float num10 = Mathf.Lerp(num9, value3.distanceToNearestWall, _desiredZoomAmount);
			float num11 = Mathf.Lerp(distanceFromTargetMinimum, num, _desiredZoomAmount);
			CameraConfiguration value4 = _cameraConfig.Value;
			float num12 = Mathf.InverseLerp(value4.zoomLevelToBeginHuggingWall, 1f, _desiredZoomAmount);
			num13 = Mathf.Lerp(num10, num11, num12);
		}
		else
		{
			float num14 = distanceFromTargetMinimum;
			CameraConfiguration value5 = _cameraConfig.Value;
			num13 = Mathf.Lerp(num14, value5.distanceToNearestWall, _desiredZoomAmount);
		}
		if (num2 > num13)
		{
			num13 = num2;
		}
		if (num4 > num13)
		{
			num13 = num4;
		}
		calculatedPosition = num13 * rayCastDirectionNormalised + CalculateCameraTarget();
		return num13;
	}

	private void ApplyWorldSpaceDistanceToCameraZoom(float offsetToApply, float currentDistanceFromCameraTarget)
	{
		float num = CalculateMaxDistance();
		CameraConfiguration value = _cameraConfig.Value;
		float distanceFromTargetMinimum = value.distanceFromTargetMinimum;
		float num2 = currentDistanceFromCameraTarget + offsetToApply;
		num2 = Mathf.Clamp(num2, distanceFromTargetMinimum, num);
		float num3 = Mathf.InverseLerp(distanceFromTargetMinimum, num, num2);
		if (offsetToApply < 0f && num3 < 0.5f)
		{
			num3 = 0.5f;
		}
		num3 = (_desiredZoomAmount = Mathf.Clamp(num3, 0f, 1f));
	}

	private void CalculateZoomOffsetToApplyToCamera(Camera camera, Vector3 testBoundPoint, out float zoomOut, out float zoomIn)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		zoomOut = 0f;
		zoomIn = 0f;
		Vector3 val = camera.ViewportToWorldPoint(new Vector3(0f, 0f, camera.get_nearClipPlane()));
		Vector3 val2 = camera.ViewportToWorldPoint(new Vector3(1f, 1f, camera.get_nearClipPlane()));
		Vector3 val3 = val2 - val;
		val3.y = 0f;
		Vector3 val4 = (val2 + val) / 2f;
		Vector3 val5 = camera.get_transform().get_position() - val4;
		float magnitude = val5.get_magnitude();
		float magnitude2 = val3.get_magnitude();
		Vector3 val6 = camera.get_transform().InverseTransformPoint(testBoundPoint);
		float num = magnitude;
		float num2 = magnitude2 / 2f;
		float num3 = Mathf.Abs(val6.x);
		float num4 = num3 * (num / num2);
		float num5 = num4 - val6.z;
		if (num5 < 0f)
		{
			zoomIn = 0f - num5;
			zoomOut = 0f;
		}
		else
		{
			zoomIn = 0f;
			zoomOut = num5;
		}
	}

	private Vector3 GetRayCastDirectionNormalised(float angleOffsetPitch = 0f, float angleOffsetYaw = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_forward();
		val = Quaternion.AngleAxis(0f - _currentPitch - angleOffsetPitch, Vector3.get_right()) * val;
		val = Quaternion.AngleAxis(_currentYaw - angleOffsetYaw, Vector3.get_up()) * val;
		val.Normalize();
		return val;
	}

	private float CalculateMinimumDistance(float angleOffsetPitch, float angleOffsetYaw)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 rayCastDirectionNormalised = GetRayCastDirectionNormalised(angleOffsetPitch, angleOffsetYaw);
		CameraConfiguration value = _cameraConfig.Value;
		float result = value.distanceFromTargetMinimum;
		CameraConfiguration value2 = _cameraConfig.Value;
		float distanceFromTargetMaximum = value2.distanceFromTargetMaximum;
		int num = 1024;
		Vector3 val = CalculateCameraTarget();
		Vector3 val2 = rayCastDirectionNormalised;
		CameraConfiguration value3 = _cameraConfig.Value;
		RaycastHit val3 = default(RaycastHit);
		if (Physics.Raycast(val + val2 * value3.distanceFromTargetMaximum, -rayCastDirectionNormalised, ref val3, distanceFromTargetMaximum, num))
		{
			result = Vector3.Distance(CalculateCameraTarget(), val3.get_point()) + 1f;
		}
		return result;
	}

	private float CalculateMaxDistance()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		CameraConfiguration value = _cameraConfig.Value;
		float num = value.distanceFromTargetMaximum;
		int num2 = 536870912;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(CalculateCameraTarget(), GetRayCastDirectionNormalised(), ref val, num, num2))
		{
			num = Mathf.Min(num, val.get_distance());
		}
		return num;
	}
}
