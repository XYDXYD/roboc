using System;
using UnityEngine;

namespace Simulation
{
	internal class AlignmentRectifierBehaviour
	{
		private const float RESPONSIVENESS_TIME = 0.25f;

		private const float BOUNDING_SPHERE_MULTIPLIER_TOLERANCE = 1.05f;

		private Vector3 _externalInputSignal;

		private float _externalInputVelocityLocal;

		private float _externalInputAngularVelocityLocal;

		private float _externalInputMaxVelocity;

		private float _externalInputMaxAngularVelocity;

		protected Vector3 _collisionSphererigidBodyOffsetLocal;

		private bool _rotationBehaviourInitialised;

		private bool _liftUpPhase;

		protected Rigidbody _rigidBody;

		private float _alignmentDuration;

		private float _liftUpHeight;

		private ArriveSteeringBehaviour _arriveBehaviour;

		private RotationArriveSteeringBehaviour _rotationArriveBehaviour;

		protected float _boundingSphereWorldAlignedRadius;

		protected float _elapsedTime;

		private bool _gravityActive;

		private int _toBeCompletedBehavioursCount;

		private float _translationUpDuration;

		private AlignmentRectifierCollisionDetection _alignmentRectifierCollisionDetection;

		private bool _isIdle = true;

		private Action<float> _physicsUpdateFunction;

		public event Action<float> OnAlignmentComplete = delegate
		{
		};

		public AlignmentRectifierBehaviour(Rigidbody rigidbody, MachineInfo machineInfo)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			_physicsUpdateFunction = IdleFixedUpdate;
			_rigidBody = rigidbody;
			ComputeBoundingSphere(machineInfo.MachineSize, machineInfo.MachineCenter);
			_alignmentRectifierCollisionDetection = _rigidBody.get_gameObject().AddComponent<AlignmentRectifierCollisionDetection>();
		}

		protected AlignmentRectifierBehaviour(Rigidbody rigidbody, Vector3[] minmax)
		{
			_physicsUpdateFunction = IdleFixedUpdate;
			_rigidBody = rigidbody;
			ComputeBoundingSphere(minmax);
			_alignmentRectifierCollisionDetection = _rigidBody.get_gameObject().AddComponent<AlignmentRectifierCollisionDetection>();
		}

		public void Activate(float duration)
		{
			_alignmentDuration = duration;
			ComputeExternalInputMaximumVelocities();
			_physicsUpdateFunction = OnActivateFixedUpdate;
			_rotationBehaviourInitialised = false;
			_liftUpPhase = true;
			_isIdle = false;
		}

		public void TimeTick(float dt)
		{
			_elapsedTime += dt;
		}

		public void PhysicsUpdate(float dt)
		{
			_physicsUpdateFunction(dt);
		}

		public Vector3 GetBoundingsphereCentreLocal()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _collisionSphererigidBodyOffsetLocal;
		}

		public float GetBoundingsphereRadius()
		{
			return _boundingSphereWorldAlignedRadius;
		}

		public void ResetInputSignal()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_externalInputSignal = Vector3.get_zero();
		}

		public void ForwardSignal()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			_externalInputSignal += Vector3.get_forward();
		}

		public void RightSignal()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			_externalInputSignal += Vector3.get_right();
		}

		public void BackSignal()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			_externalInputSignal -= Vector3.get_forward();
		}

		public void LeftSignal()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			_externalInputSignal -= Vector3.get_right();
		}

		private void ComputeExternalInputMaximumVelocities()
		{
			_externalInputMaxVelocity = 2f * _boundingSphereWorldAlignedRadius / _alignmentDuration;
			_externalInputMaxAngularVelocity = (float)Math.PI / _alignmentDuration;
		}

		private void ComputeBoundingSphere(Vector3 machineSize, Vector3 machineCenter)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			_boundingSphereWorldAlignedRadius = machineSize.get_magnitude() * 0.5f;
			_collisionSphererigidBodyOffsetLocal = machineCenter;
		}

		private void ComputeBoundingSphere(Vector3[] minmax)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = (minmax[0] + minmax[1]) * 0.5f;
			_boundingSphereWorldAlignedRadius = Vector3.Distance(val, minmax[0]);
			_collisionSphererigidBodyOffsetLocal = val;
		}

		private void OnActivateFixedUpdate(float dt)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _rigidBody.get_transform().TransformPoint(_collisionSphererigidBodyOffsetLocal);
			Vector3 val2 = val + Vector3.get_up() * (_boundingSphereWorldAlignedRadius * 1.05f);
			RaycastHit val3 = default(RaycastHit);
			if (Physics.SphereCast(val, _boundingSphereWorldAlignedRadius, Vector3.get_up(), ref val3, 10f * _boundingSphereWorldAlignedRadius, GameLayers.PROPS))
			{
				Vector3 val4 = val + Vector3.get_up() * val3.get_distance();
				if (val4.y < val2.y)
				{
					val2 = val4;
				}
			}
			_translationUpDuration = _alignmentDuration * 0.5f;
			Vector3 val5 = val - val2;
			float maxSpeed = val5.get_magnitude() / (_translationUpDuration - 0.5f);
			if (_arriveBehaviour == null)
			{
				_arriveBehaviour = new ArriveSteeringBehaviour(maxSpeed, 0.25f, dt);
			}
			_arriveBehaviour.SetMaxSpeed(maxSpeed);
			_liftUpHeight = val2.y;
			_arriveBehaviour.SetDesiredPosition(val2);
			_arriveBehaviour.OnArrived += HandleOnTranslationComplete;
			InitializeRotationBehaviour(_rigidBody.get_transform().get_up(), dt, _alignmentDuration * 0.25f);
			_elapsedTime = 0f;
			_gravityActive = _rigidBody.get_useGravity();
			_rigidBody.set_useGravity(false);
			_toBeCompletedBehavioursCount = 2;
			ActiveFixedUpdate(dt);
			_physicsUpdateFunction = ActiveFixedUpdate;
		}

		private void IdleFixedUpdate(float dt)
		{
		}

		private Vector3 ComputeDesiredExternalInputVelocity(float dt)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			float num = _externalInputSignal.z * _externalInputMaxVelocity;
			float responsivenessConstant = _arriveBehaviour.GetResponsivenessConstant();
			_externalInputVelocityLocal = _externalInputVelocityLocal * (1f - responsivenessConstant * dt) + num * responsivenessConstant * dt;
			return _rigidBody.get_transform().get_forward() * _externalInputVelocityLocal;
		}

		private Vector3 ComputeDesiredExternalInputAngularVelocity(float dt)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			float num = _externalInputSignal.x * _externalInputMaxAngularVelocity;
			float responsivenessConstant = _arriveBehaviour.GetResponsivenessConstant();
			_externalInputAngularVelocityLocal = _externalInputAngularVelocityLocal * (1f - responsivenessConstant * dt) + num * responsivenessConstant * dt;
			return _rigidBody.get_transform().get_up() * _externalInputAngularVelocityLocal;
		}

		private void ActiveFixedUpdate(float dt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			Vector3 zero = Vector3.get_zero();
			Vector3 zero2 = Vector3.get_zero();
			Vector3 val = _rigidBody.get_transform().TransformPoint(_collisionSphererigidBodyOffsetLocal);
			if (_liftUpPhase)
			{
				if (!_rotationBehaviourInitialised)
				{
					if (!Physics.CheckSphere(val, _boundingSphereWorldAlignedRadius, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK))
					{
						_rotationBehaviourInitialised = true;
						StartOrientationAlignmentBehaviour(Vector3.get_up());
					}
					else if (_elapsedTime > _translationUpDuration)
					{
						_rotationBehaviourInitialised = true;
						StartOrientationAlignmentBehaviour(Vector3.get_up());
					}
				}
				Vector3 desiredPosition = default(Vector3);
				desiredPosition._002Ector(val.x, _liftUpHeight, val.z);
				_arriveBehaviour.SetDesiredPosition(desiredPosition);
				zero = _arriveBehaviour.ComputeDesiredVelocity(val);
			}
			else
			{
				_arriveBehaviour.SetDesiredPosition(val - Vector3.get_up() * (_boundingSphereWorldAlignedRadius * 10f));
				zero = _arriveBehaviour.ComputeDesiredVelocity(val);
			}
			zero2 = _rotationArriveBehaviour.ComputeDesiredAngularVelocity(_rigidBody.get_transform().get_up(), _rigidBody.get_transform().get_forward());
			Vector3 velocity = _rigidBody.get_velocity();
			velocity.x = 0f;
			velocity.z = 0f;
			zero = _arriveBehaviour.FixedUpdate(zero, velocity, dt);
			Vector3 val2 = ComputeDesiredExternalInputVelocity(dt);
			val2.y = 0f;
			zero += val2;
			Vector3 curAngularVel = _rigidBody.get_angularVelocity() - _rigidBody.get_transform().get_up() * Vector3.Dot(_rigidBody.get_angularVelocity(), _rigidBody.get_transform().get_up());
			zero2 = _rotationArriveBehaviour.FixedUpdate(zero2, curAngularVel, dt);
			zero2 += ComputeDesiredExternalInputAngularVelocity(dt);
			_rigidBody.AddForce(zero - _rigidBody.get_velocity(), 2);
			_rigidBody.AddTorque(zero2 - _rigidBody.get_angularVelocity(), 2);
		}

		private void InitializeRotationBehaviour(Vector3 desiredUp, float fixedUpdateDt, float rotationDuration)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Angle(_rigidBody.get_transform().get_up(), desiredUp);
			num = num * (float)Math.PI / 180f;
			float maxAngularVelocity = Mathf.Max(num / rotationDuration, (float)Math.PI / 4f);
			if (_rotationArriveBehaviour == null)
			{
				_rotationArriveBehaviour = new RotationArriveSteeringBehaviour(maxAngularVelocity, 0.25f, fixedUpdateDt);
			}
			_rotationArriveBehaviour.SetMaxAngularVelocity(maxAngularVelocity);
			_rotationArriveBehaviour.SetDesiredUp(desiredUp);
		}

		private void StartOrientationAlignmentBehaviour(Vector3 desiredUp)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			float num = _alignmentDuration * 0.75f - _elapsedTime - 0.5f;
			num = Mathf.Max(num, 0.25f);
			float num2 = Vector3.Angle(_rigidBody.get_transform().get_up(), desiredUp);
			num2 = num2 * (float)Math.PI / 180f;
			float maxAngularVelocity = num2 / num;
			_rotationArriveBehaviour.SetDesiredUp(Vector3.get_up());
			_rotationArriveBehaviour.SetMaxAngularVelocity(maxAngularVelocity);
			_rotationArriveBehaviour.SetDesiredUp(desiredUp);
			_rotationArriveBehaviour.OnArrived += HandleOnRotationComplete;
		}

		private void HandleOnTranslationComplete()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			_arriveBehaviour.OnArrived -= HandleOnTranslationComplete;
			_toBeCompletedBehavioursCount--;
			if (_toBeCompletedBehavioursCount == 0)
			{
				HandleOnLiftUpComplete();
			}
			else if (!_rotationBehaviourInitialised)
			{
				_rotationBehaviourInitialised = true;
				StartOrientationAlignmentBehaviour(Vector3.get_up());
			}
		}

		private void HandleOnRotationComplete()
		{
			_rotationArriveBehaviour.OnArrived -= HandleOnRotationComplete;
			_toBeCompletedBehavioursCount--;
			if (_toBeCompletedBehavioursCount == 0)
			{
				HandleOnLiftUpComplete();
			}
		}

		private void HandleOnLiftUpComplete()
		{
			_liftUpPhase = false;
			_alignmentRectifierCollisionDetection.set_enabled(true);
			_alignmentRectifierCollisionDetection.OnCollisionDetected += HandleOnAlignmentRectifierCollisionDetected;
		}

		public bool IsIdle()
		{
			return _isIdle;
		}

		public virtual void GoIdle()
		{
			_alignmentRectifierCollisionDetection.set_enabled(false);
			_alignmentRectifierCollisionDetection.OnCollisionDetected -= HandleOnAlignmentRectifierCollisionDetected;
			_physicsUpdateFunction = IdleFixedUpdate;
			_rigidBody.set_useGravity(_gravityActive);
			_isIdle = true;
		}

		protected virtual void HandleOnAlignmentRectifierCollisionDetected()
		{
			this.OnAlignmentComplete(_elapsedTime);
		}
	}
}
