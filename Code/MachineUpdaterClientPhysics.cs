using Svelto.Command;
using System;
using UnityEngine;
using Utility;

internal sealed class MachineUpdaterClientPhysics : IMachineUpdater
{
	private ICommandFactory _commandFactory;

	private PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> _machineHistory;

	private Rigidbody _rbData;

	private WeaponRaycast _weaponRaycast;

	private float _currentTime;

	private PlayerMachineMotionHistoryFrame[] _lastFrames = new PlayerMachineMotionHistoryFrame[3];

	private PlayerMachineMotionHistoryFrame _lastLatestState;

	private int _owner;

	private float _antiLatency = 1f;

	private float _timeInCurrentState;

	private Ray _motionCollisiontest;

	private Vector3 _lastKnownPosition = Vector3.get_zero();

	private Vector3 _predictedPositionFromLastKnown = Vector3.get_zero();

	private Vector3 _predictedPositionFromCurrent = Vector3.get_zero();

	private Quaternion _lastKnownRotation = Quaternion.get_identity();

	private Quaternion _predictedRotationFromLastKnown = Quaternion.get_identity();

	private Quaternion _predictedRotationFromCurrent = Quaternion.get_identity();

	private float _approxMachineRadius;

	private float pingFactor = 2f;

	private float interpFactor = 0.49f;

	private float pingRefreshRate = 10f;

	private float pingRefreshTimer = 10f;

	private float _timeDiff = 0.2f;

	private bool _resetValues;

	private float interpolateRate = 0.1f;

	private float _interpValue = 1f;

	public MachineUpdaterClientPhysics(int owner, PlayerHistoryBuffer<PlayerMachineMotionHistoryFrame> machineHistory, Rigidbody rbData, WeaponRaycast weaponRaycast, ICommandFactory commandFactory, Vector3 machineSize)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		_owner = owner;
		_commandFactory = commandFactory;
		_machineHistory = machineHistory;
		_rbData = rbData;
		_weaponRaycast = weaponRaycast;
		_approxMachineRadius = machineSize.y * 0.5f;
		InitialiseStartingValues(rbData);
		_currentTime = 0f;
	}

	public void SetPing(float ping)
	{
		_antiLatency = ping * pingFactor;
		_antiLatency = Mathf.Min(_antiLatency, 1f);
	}

	public float GetCurrentTime()
	{
		return _currentTime;
	}

	private void InitialiseStartingValues(Rigidbody rb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 centerOfMass = rb.get_centerOfMass();
		Vector3 position = rb.get_position();
		Quaternion rotation = rb.get_rotation();
		_predictedPositionFromCurrent = (_predictedPositionFromLastKnown = (_lastKnownPosition = position + rotation * centerOfMass));
		_lastKnownRotation = rotation;
		_predictedRotationFromLastKnown = rotation;
		_predictedRotationFromCurrent = rotation;
		_resetValues = false;
	}

	public void OnEnable()
	{
		_resetValues = true;
	}

	public void Tick(float deltaTime)
	{
		_currentTime += deltaTime;
		UpdateMotion(deltaTime);
	}

	private void UpdateRefreshPing(float deltaTime)
	{
		pingRefreshTimer -= deltaTime;
		if (pingRefreshTimer <= 0f)
		{
			pingRefreshTimer = pingRefreshRate;
			RequestPingClientCommand requestPingClientCommand = _commandFactory.Build<RequestPingClientCommand>();
			requestPingClientCommand.Inject(new RequestPingDependency(Time.get_time(), _owner));
			requestPingClientCommand.Execute();
		}
	}

	public void OnDrawGizmos()
	{
	}

	private void UpdateMotion(float deltaTime)
	{
		if (_resetValues)
		{
			InitialiseStartingValues(_rbData);
		}
		UpdateLatestFrame(deltaTime, ref _lastFrames);
		UpdatePredictedPosition(deltaTime);
		ActuallyMoveRBs(deltaTime);
	}

	private Vector3 ExtrapolatePosition(PlayerMachineMotionHistoryFrame[] lastFrames, float timeInFuture)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] outputValues = (Vector3[])new Vector3[3]
		{
			lastFrames[2].rbState.worldCOM,
			lastFrames[1].rbState.worldCOM,
			lastFrames[0].rbState.worldCOM
		};
		float[] inputTimes = new float[3]
		{
			lastFrames[2].timeStamp,
			lastFrames[1].timeStamp,
			lastFrames[0].timeStamp
		};
		return LagrangeInterpolate.Interpolate(inputTimes, outputValues, lastFrames[0].timeStamp + timeInFuture);
	}

	private void UpdatePredictedPosition(float deltaTime)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		if (_machineHistory != null && !(_rbData == null) && !(_lastFrames[0].timeStamp < 0f) && !(_lastFrames[1].timeStamp < 0f) && !(_lastFrames[2].timeStamp < 0f))
		{
			Vector3 val = (ExtrapolatePosition(_lastFrames, deltaTime) - _lastFrames[0].rbState.worldCOM) / _timeDiff;
			Vector3 val2 = val;
			val2 = UpdateRespondToCollision(_lastKnownPosition, _lastKnownRotation, val);
			_predictedPositionFromLastKnown = _lastKnownPosition + deltaTime * val2;
			_predictedRotationFromLastKnown = Quaternion.Euler(_lastFrames[0].rbState.angularVelocity * 57.29578f * deltaTime) * _lastKnownRotation;
			val2 = UpdateRespondToCollision(_predictedPositionFromCurrent, _predictedRotationFromCurrent, val);
			_predictedPositionFromCurrent += deltaTime * val2;
			_predictedRotationFromCurrent = Quaternion.Euler(_lastFrames[0].rbState.angularVelocity * 57.29578f * deltaTime) * _predictedRotationFromCurrent;
			_predictedPositionFromCurrent = Vector3.Lerp(_predictedPositionFromCurrent, _predictedPositionFromLastKnown, interpolateRate);
			_predictedRotationFromCurrent = Quaternion.Slerp(_predictedRotationFromCurrent, _predictedRotationFromLastKnown, interpolateRate);
		}
	}

	private Vector3 Vector3Interpolate(Vector3 from, Vector3 to, float interpValue)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return (to - from) * interpValue + from;
	}

	private void ActuallyMoveRBs(float deltaTime)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (!(_rbData == null))
		{
			try
			{
				_rbData.set_position(_predictedPositionFromCurrent - _predictedRotationFromCurrent * _rbData.get_centerOfMass());
				_rbData.set_rotation(_predictedRotationFromCurrent);
			}
			catch (Exception ex)
			{
				Console.LogException(ex);
			}
			if (_weaponRaycast != null && _lastFrames[0] != null && _lastFrames[0].weaponRaycast != null)
			{
				_weaponRaycast.aimPoint = Vector3.Lerp(_weaponRaycast.aimPoint, _lastFrames[0].weaponRaycast.aimPoint, _interpValue);
			}
		}
	}

	private void UpdateLatestFrame(float deltaTime, ref PlayerMachineMotionHistoryFrame[] lastFrames)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		int lastWrittenStateIndex = _machineHistory.GetLastWrittenStateIndex();
		lastFrames[2] = _machineHistory.GetState((lastWrittenStateIndex - 2 + _machineHistory.GetStateCount()) % _machineHistory.GetStateCount());
		lastFrames[1] = _machineHistory.GetState((lastWrittenStateIndex - 1 + _machineHistory.GetStateCount()) % _machineHistory.GetStateCount());
		lastFrames[0] = _machineHistory.GetState(lastWrittenStateIndex);
		if (lastFrames[0] != _lastLatestState)
		{
			_timeDiff = lastFrames[0].timeStamp - lastFrames[1].timeStamp;
			if (_timeDiff <= 0.0001f)
			{
				_timeDiff = 0.0001f;
			}
			_timeInCurrentState = 0f;
			_lastLatestState = lastFrames[0];
			_lastKnownPosition = lastFrames[0].rbState.worldCOM;
			_lastKnownRotation = lastFrames[0].rbState.rotation;
		}
		else
		{
			_timeInCurrentState += deltaTime;
		}
		_interpValue = interpFactor * _timeInCurrentState / _timeDiff;
	}

	private Vector3 UpdateRespondToCollision(Vector3 position, Quaternion rotation, Vector3 velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		_motionCollisiontest = new Ray(position, velocity + velocity.get_normalized() * _approxMachineRadius);
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(_motionCollisiontest, ref val, velocity.get_magnitude(), GameLayers.ENVIRONMENT_LAYER_MASK))
		{
			Vector3 result = val.get_point() + val.get_normal() * _approxMachineRadius - position;
			result = result.get_normalized() * velocity.get_magnitude();
			return result;
		}
		return velocity;
	}

	private void NormalizeQuaternion(ref Quaternion q)
	{
		float num = 0f;
		for (int i = 0; i < 4; i++)
		{
			num += q.get_Item(i) * q.get_Item(i);
		}
		float num2 = 1f / Mathf.Sqrt(num);
		for (int j = 0; j < 4; j++)
		{
			int num3;
			q.set_Item(num3 = j, q.get_Item(num3) * num2);
		}
	}
}
