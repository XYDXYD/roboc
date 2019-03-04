using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class AerofoilPhysicEngine : SingleEntityViewEngine<LocalMachineAerofoilNode>
	{
		private const float LIFT_SPEED_THRESHOLD = 10f;

		private const float LATERAL_FORCE_MULTIPLIER = 0.05f;

		private const float LATERAL_OFFSET_FORCE_MULTIPLIER = 0.05f;

		private const float ELEVATION_STEEPNESS = 0.85f;

		private const float BARREL_STEEPNESS = 0.9f;

		private const float VTOL_VELOCITY = 30f;

		private const float MIN_SPEED_RATIO = 0.75f;

		private const float OFFSET_INFLUENCE = 10f;

		private const float FORWARD_BIAS = 5f;

		private const float LIFT_BIAS = 0.4f;

		private LocalMachineAerofoilNode _localMachineAerofoilNode;

		private Rigidbody _rb;

		private float _rudderSpeed;

		private float _speedRatio;

		private float _barrelSpeed;

		private Transform _transform;

		private float _elevationSpeed;

		private float _liftCapacityRatio;

		private float _liftBinaryRatio;

		private Vector3 _lateralForceOffset;

		private float _lateralCapacityRatio;

		private float _thrust;

		private float _stopperCapacityRatio;

		private float _bankSpeed;

		private float _rudderCapacityRatio;

		private float _barrelOffset;

		private float _elevationOffset;

		private float _vtolVelocity;

		private float[] _aDragParams;

		private Vector3 _input;

		private ITaskRoutine _fixedTask;

		[Inject]
		internal CeilingHeightManager ceilingHeightManager
		{
			private get;
			set;
		}

		public AerofoilPhysicEngine()
		{
			_fixedTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetScheduler(StandardSchedulers.get_physicScheduler())
				.SetEnumeratorProvider((Func<IEnumerator>)FixedUpdate);
		}

		protected override void Add(LocalMachineAerofoilNode node)
		{
			_localMachineAerofoilNode = node;
			_rb = _localMachineAerofoilNode.rigidbodyComponent.rb;
			_transform = _localMachineAerofoilNode.transformComponent.T;
			_fixedTask.Start((Action<PausableTaskException>)null, (Action)null);
		}

		protected override void Remove(LocalMachineAerofoilNode node)
		{
			_fixedTask.Stop();
			_localMachineAerofoilNode = null;
		}

		private IEnumerator FixedUpdate()
		{
			while (_rb != null)
			{
				IMachineAerofoilComponent aerofoilComponent = _localMachineAerofoilNode.aerofoilComponent;
				if (aerofoilComponent.numAerofoils > 0)
				{
					_rudderSpeed = aerofoilComponent.rudderSpeed;
					_barrelSpeed = aerofoilComponent.barrelSpeed;
					_speedRatio = aerofoilComponent.speedRatio;
					_elevationSpeed = aerofoilComponent.elevationSpeed;
					_liftCapacityRatio = Mathf.Pow(aerofoilComponent.liftCapacityRatio, 2f);
					_liftBinaryRatio = Mathf.Floor(aerofoilComponent.liftCapacityRatio);
					_lateralForceOffset = aerofoilComponent.lateralForceOffset;
					_lateralCapacityRatio = aerofoilComponent.lateralCapacityRatio;
					_thrust = aerofoilComponent.thrust;
					_rudderCapacityRatio = aerofoilComponent.rudderCapacityRatio;
					_stopperCapacityRatio = aerofoilComponent.stopperCapacityRatio;
					_bankSpeed = aerofoilComponent.bankSpeed;
					_rudderCapacityRatio = aerofoilComponent.rudderCapacityRatio;
					_barrelOffset = aerofoilComponent.barrelOffset;
					_elevationOffset = aerofoilComponent.elevationOffset;
					_vtolVelocity = aerofoilComponent.vtolVelocity;
					_aDragParams = aerofoilComponent.aDragParams;
					_input = aerofoilComponent.aerofoilInput;
					CalculateSpeedRatio();
					ApplyBarrelAndRudder();
					ApplyElevation();
					ApplyBanking();
					ApplyLift();
					ApplyLateralForce();
					ApplyForwardThrust();
					ApplyVTOL();
					ApplyLateralOffset();
					ApplyAngularDamping();
				}
				yield return null;
			}
		}

		private void CalculateSpeedRatio()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			_localMachineAerofoilNode.aerofoilComponent.speedRatio = Mathf.Clamp01(Mathf.Abs(Vector3.Dot(_rb.get_velocity(), _transform.get_forward()) / 10f));
		}

		private void ApplyBarrelAndRudder()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _transform.get_forward();
			Vector3 val = default(Vector3);
			val._002Ector(forward.x, 0f, forward.z);
			Vector3 normalized = val.get_normalized();
			Vector3 val2 = Vector3.Cross(normalized, -Vector3.get_up());
			Vector3 right = _transform.get_right();
			float num = Mathf.Clamp(_speedRatio, 0.75f, _speedRatio);
			float num2 = Vector3.Dot(Vector3.Cross(forward, val2), right);
			float num3 = (0f - _input.x) * 0.9f * num;
			float num4 = (num3 - num2) * num * (_barrelSpeed + _rudderSpeed);
			Vector3 val3 = _transform.get_up() * num4;
			Vector3 worldCenterOfMass = _rb.get_worldCenterOfMass();
			Vector3 val4 = right * (_barrelOffset * 10f);
			_rb.AddForceAtPosition(val3, worldCenterOfMass + val4, 5);
			_rb.AddForceAtPosition(-val3, worldCenterOfMass - val4, 5);
		}

		private void ApplyElevation()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Cross(_transform.get_right(), Vector3.get_up());
			Vector3 normalized = val.get_normalized();
			Vector3 up = _transform.get_up();
			float num = Vector3.Dot(up, normalized);
			Vector3 forward = _transform.get_forward();
			float num2 = (0f - _input.y) * Mathf.Sign(Vector3.Dot(_rb.get_velocity() + 5f * forward, forward)) * 0.85f * _speedRatio;
			float num3 = (num - num2) * _speedRatio * _elevationSpeed;
			Vector3 val2 = up * num3;
			Vector3 worldCenterOfMass = _rb.get_worldCenterOfMass();
			Vector3 val3 = forward * (_elevationOffset * 10f);
			_rb.AddForceAtPosition(val2, worldCenterOfMass + val3, 5);
			_rb.AddForceAtPosition(-val2, worldCenterOfMass - val3, 5);
		}

		private void ApplyBanking()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(Vector3.get_up(), _transform.get_right());
			float num2 = -1f * num * _bankSpeed * Mathf.Sign(Vector3.Dot(_rb.get_velocity(), _transform.get_forward()));
			float num3 = num2;
			Vector3 angularVelocity = _rb.get_angularVelocity();
			Vector3 val = default(Vector3);
			val._002Ector(0f, num3 - angularVelocity.y, 0f);
			_rb.AddTorque(val, 5);
		}

		private void ApplyLift()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			Vector3 up = _transform.get_up();
			float num = Mathf.Clamp01(Mathf.Abs(up.y) + 0.4f);
			Vector3 val = -Physics.get_gravity() * (_liftBinaryRatio * _speedRatio * num);
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = val;
			Vector3 position = _transform.get_position();
			val = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y);
			_rb.AddForce(val, 5);
		}

		private void ApplyLateralForce()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _transform.InverseTransformVector(_rb.get_velocity());
			Vector3 val2 = new Vector3((0f - val.x) * Mathf.Clamp01(_rudderCapacityRatio + _liftCapacityRatio), (0f - val.y) * _lateralCapacityRatio, (0f - val.z) * _stopperCapacityRatio) * 0.05f;
			Vector3 val3 = _transform.TransformVector(val2);
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = val3;
			Vector3 position = _transform.get_position();
			val3 = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y);
			_rb.AddForce(val3, 2);
		}

		private void ApplyForwardThrust()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _transform.get_forward() * (_input.z * _thrust * Mathf.Clamp01(_liftCapacityRatio + _rudderCapacityRatio));
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = val;
			Vector3 position = _transform.get_position();
			val = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y);
			if (val.get_sqrMagnitude() > 0f)
			{
				_rb.AddForce(val, 5);
			}
		}

		private void ApplyVTOL()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f - _speedRatio;
			Vector3 up = _transform.get_up();
			float num2 = Mathf.Abs(up.y);
			Vector3 velocity = _rb.get_velocity();
			float y = velocity.y;
			float num3 = num * _liftBinaryRatio * num2 * 30f * _input.y;
			Vector3 val = Vector3.get_up() * Mathf.Clamp(num3 - y, 0f - _vtolVelocity, _vtolVelocity);
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = val;
			Vector3 position = _transform.get_position();
			val = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y);
			_rb.AddForce(val, 5);
		}

		private void ApplyLateralOffset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			if (_lateralForceOffset != Vector3.get_zero())
			{
				float magnitude = _lateralForceOffset.get_magnitude();
				Vector3 velocity = _rb.get_velocity();
				float num = magnitude * velocity.get_magnitude() * 0.05f;
				Vector3 worldCenterOfMass = _rb.get_worldCenterOfMass();
				Vector3 up = _transform.get_up();
				Vector3 val = _transform.TransformDirection(_lateralForceOffset.get_normalized());
				_rb.AddForceAtPosition(up * num, worldCenterOfMass + val, 5);
				_rb.AddForceAtPosition(up * (0f - num), worldCenterOfMass - val, 5);
			}
		}

		private void ApplyAngularDamping()
		{
			if (_aDragParams != null)
			{
				float num = Mathf.Max(_aDragParams);
				if (_rb.get_angularDrag() < num)
				{
					_rb.set_angularDrag(num);
				}
			}
		}
	}
}
