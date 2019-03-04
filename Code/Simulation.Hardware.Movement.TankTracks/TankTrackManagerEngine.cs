using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackManagerEngine : SingleEntityViewEngine<TankTrackNode>, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float NO_INPUT_THRESHOLD = 0.01f;

		private const float MIN_TURN_SPEED_FOR_DAMPING = 0.08726646f;

		private const float TURNING_ON_SPOT_THRESHOLD = 75f;

		private FasterList<TankTrackNode> _fastSideTracks = new FasterList<TankTrackNode>(5);

		private FasterList<TankTrackNode> _slowSideTracks = new FasterList<TankTrackNode>(5);

		[Inject]
		internal PlayerStrafeDirectionManager strafeDirectionManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(TankTrackNode node)
		{
			if (node.ownerComponent.ownedByMe || node.ownerComponent.ownedByAi)
			{
				int machineId = node.ownerComponent.machineId;
				SetUpdateCachedValuesRequired(machineId);
				node.hardwareDisabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)HandleMovementPartDestroyed);
			}
		}

		protected override void Remove(TankTrackNode node)
		{
			if (node.ownerComponent.ownedByMe || node.ownerComponent.ownedByAi)
			{
				node.hardwareDisabledComponent.isPartDisabled.StopNotify((Action<int, bool>)HandleMovementPartDestroyed);
			}
		}

		private void HandleMovementPartDestroyed(int i, bool b)
		{
			TankTrackNode tankTrackNode = default(TankTrackNode);
			if (entityViewsDB.TryQueryEntityView<TankTrackNode>(i, ref tankTrackNode))
			{
				int machineId = tankTrackNode.ownerComponent.machineId;
				SetUpdateCachedValuesRequired(machineId);
			}
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<TankTrackMachineNode> val = entityViewsDB.QueryEntityViews<TankTrackMachineNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				TankTrackMachineNode tankTrackMachineNode = val.get_Item(i);
				int count = default(int);
				TankTrackNode[] tankTracks = entityViewsDB.QueryGroupedEntityViewsAsArray<TankTrackNode>(tankTrackMachineNode.get_ID(), ref count);
				CalculateNumGroundedTracks(tankTrackMachineNode, tankTracks, count);
				if (tankTrackMachineNode.cacheUpdateComponent.updateRequired)
				{
					UpdateAvgValues(tankTrackMachineNode, tankTracks, count);
				}
				if (!tankTrackMachineNode.visibilityComponent.offScreen && tankTrackMachineNode.numGroundedTracksComponent.groundedTracks > 0)
				{
					Vector3 inputVector = UpdateInputValues(tankTrackMachineNode);
					SetTracksTurningToDirection(tankTrackMachineNode, ref inputVector, tankTracks, count);
					CheckIsRobotStoppedAndMovementDirection(tankTrackMachineNode, inputVector, tankTracks, count);
					CalculateMotorForces(tankTrackMachineNode, inputVector, tankTracks, count);
				}
			}
		}

		private Vector3 UpdateInputValues(TankTrackMachineNode node)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = default(Vector3);
			val._002Ector(node.strafingCustomInputComponent.forwardInput, node.strafingCustomInputComponent.turningInput, node.strafingCustomInputComponent.strafingInput);
			if (strafeDirectionManager.strafingEnabled && node.ownerComponent.ownedByMe)
			{
				val.y = CalculateCameraRelativeTurningInputVector(val, node.strafingCustomAngleToStraightComponent.angleToStraight, node.movingBackwardsComponent.movingBackwards);
			}
			return val;
		}

		private void SetUpdateCachedValuesRequired(int machineId)
		{
			TankTrackMachineNode tankTrackMachineNode = default(TankTrackMachineNode);
			if (entityViewsDB.TryQueryEntityView<TankTrackMachineNode>(machineId, ref tankTrackMachineNode))
			{
				tankTrackMachineNode.cacheUpdateComponent.updateRequired = true;
			}
		}

		private void UpdateAvgValues(TankTrackMachineNode node, TankTrackNode[] tankTracks, int count)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			for (int i = 0; i < count; i++)
			{
				TankTrackNode tankTrackNode = tankTracks[i];
				if (!tankTrackNode.hardwareDisabledComponent.disabled)
				{
					num2 += tankTrackNode.turnAccelerationComponent.turnAcceleration;
					num3 += tankTrackNode.maxTurnRateMovingComponent.maxTurnRateMoving;
					num4 += tankTrackNode.maxTurnRateStoppedComponent.maxTurnRateStopped;
					num += tankTrackNode.lateralAccelerationComponent.acceleration;
					num5 += 1f;
				}
			}
			float num6 = 1f / num5;
			node.avgLateralAccelerationComponent.acceleration = num * num6;
			node.avgTurnAccelerationComponent.turnAcceleration = num2 * num6;
			node.avgMaxTurnRateMovingComponent.maxTurnRateMoving = num3 * num6;
			node.avgMaxTurnRateStoppedComponent.maxTurnRateStopped = num4 * num6;
			node.cacheUpdateComponent.updateRequired = false;
		}

		private void CheckIsRobotStoppedAndMovementDirection(TankTrackMachineNode node, Vector3 inputVector, TankTrackNode[] tankTracks, int count)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Quaternion val = Quaternion.Inverse(rb.get_rotation());
			Vector3 val2 = val * rb.get_velocity();
			float z = val2.z;
			float num = Mathf.Abs(z);
			bool flag = false;
			flag = ((!(Mathf.Abs(inputVector.y) > 0f)) ? (num < 0.2f && Mathf.Abs(inputVector.x) < 0.01f) : (num < 1f && Mathf.Abs(inputVector.x) < 0.01f));
			if (flag != node.machineStoppedComponent.stopped)
			{
				node.machineStoppedComponent.stopped = flag;
				for (int i = 0; i < count; i++)
				{
					tankTracks[i].trackStoppedComponent.stopped = flag;
				}
			}
			bool movingBackwards = false;
			if (!node.machineStoppedComponent.stopped)
			{
				movingBackwards = (z < 0f);
			}
			node.movingBackwardsComponent.movingBackwards = movingBackwards;
		}

		private void SetTracksTurningToDirection(TankTrackMachineNode node, ref Vector3 inputVector, TankTrackNode[] tankTracks, int count)
		{
			bool turning = false;
			if (Mathf.Abs(node.strafingCustomAngleToStraightComponent.angleToStraight) > 75f)
			{
				inputVector.x = (inputVector.z = 0f);
				turning = true;
			}
			node.turningToDriveDirection.turning = turning;
			for (int i = 0; i < count; i++)
			{
				tankTracks[i].turningToDriveDirection.turning = turning;
			}
		}

		private float CalculateCameraRelativeTurningInputVector(Vector3 inputVector, float angleToStraight, bool movingBackwards)
		{
			float num = 0f;
			bool flag = strafeDirectionManager.tracksTurningOnSpotEnabled || Mathf.Abs(inputVector.x) > 0f || Mathf.Abs(inputVector.z) > 0f;
			if (Mathf.Abs(angleToStraight) > 5f && flag)
			{
				if (angleToStraight < 0f)
				{
					num = 1f;
				}
				else if (angleToStraight > 0f)
				{
					num = -1f;
				}
				if (movingBackwards)
				{
					num *= -1f;
				}
			}
			return num;
		}

		private void CalculateMotorForces(TankTrackMachineNode node, Vector3 inputVector, TankTrackNode[] tankTracks, int count)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			HandleForwardInput(node, inputVector, tankTracks, count);
			HandleHorizontalInput(node, inputVector.y, tankTracks, count);
		}

		private void CalculateNumGroundedTracks(TankTrackMachineNode node, TankTrackNode[] tankTracks, int count)
		{
			int num = 0;
			float maxSpeed = 0f;
			for (int i = 0; i < count; i++)
			{
				if (tankTracks[i].trackGroundedComponent.grounded)
				{
					num++;
					maxSpeed = tankTracks[i].maxSpeedComponent.maxSpeed;
				}
			}
			node.numGroundedTracksComponent.groundedTracks = num;
			node.avgMaxSpeedComponent.maxSpeed = maxSpeed;
		}

		private void HandleForwardInput(TankTrackMachineNode node, Vector3 input, TankTrackNode[] tankTracks, int count)
		{
			if (Mathf.Abs(input.x) > 0.01f)
			{
				ApplyForwardForces(node, input.x, tankTracks, count);
			}
			else if (!node.machineStoppedComponent.stopped)
			{
				ApplyBrakingForce(node, input.y, tankTracks, count);
			}
			else
			{
				ApplyStoppedForce(node, tankTracks, count);
			}
		}

		private void HandleHorizontalInput(TankTrackMachineNode node, float turningInput, TankTrackNode[] tankTracks, int count)
		{
			if (Mathf.Abs(turningInput) > 0.01f)
			{
				CalculateTurningForces(node, turningInput, tankTracks, count);
			}
			else
			{
				CalculateTurnDamping(node, tankTracks, count);
			}
		}

		private void ApplyForwardForces(TankTrackMachineNode node, float inputValue, TankTrackNode[] tankTracks, int count)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			float num = rb.get_mass() * node.avgLateralAccelerationComponent.acceleration;
			Vector3 val = transform.get_forward() * (num * inputValue);
			for (int i = 0; i < count; i++)
			{
				TankTrackNode tankTrackNode = tankTracks[i];
				IPendingForceComponent pendingForceComponent = tankTrackNode.pendingForceComponent;
				pendingForceComponent.pendingForce += val;
			}
		}

		private void ApplyStoppedForce(TankTrackMachineNode node, TankTrackNode[] tankTracks, int count)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			int groundedTracks = node.numGroundedTracksComponent.groundedTracks;
			if (groundedTracks > 0)
			{
				Vector3 val = -rb.get_velocity() / (float)groundedTracks;
				for (int i = 0; i < count; i++)
				{
					IPendingForceComponent pendingForceComponent = tankTracks[i].pendingForceComponent;
					pendingForceComponent.pendingVelocityChangeForce += val;
				}
			}
		}

		private void ApplyBrakingForce(TankTrackMachineNode node, float horizontalInput, TankTrackNode[] tankTracks, int count)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			Vector3 velocity = rb.get_velocity();
			float num = Vector3.Dot(velocity, transform.get_forward());
			float maxSpeed = node.avgMaxSpeedComponent.maxSpeed;
			float num2 = Mathf.Clamp01(Mathf.Abs(num) / maxSpeed);
			Vector3 val = transform.get_forward() * (0f - Mathf.Sign(num));
			float num3 = rb.get_mass() * node.avgLateralAccelerationComponent.acceleration;
			float num4 = num3 * num2;
			Vector3 val2 = val * num4;
			float num5 = Vector3.Dot(val, Physics.get_gravity());
			Vector3 val3 = val * num5;
			val3 *= rb.get_mass() * -1f;
			val3 /= (float)count;
			val2 += val3;
			for (int i = 0; i < count; i++)
			{
				TankTrackNode tankTrackNode = tankTracks[i];
				MachineSide machineSide = tankTrackNode.machineSideComponent.machineSide;
				if (Mathf.Abs(horizontalInput) < 0.01f || (horizontalInput < 0f && machineSide == MachineSide.left) || (horizontalInput > 0f && machineSide == MachineSide.right))
				{
					IPendingForceComponent pendingForceComponent = tankTrackNode.pendingForceComponent;
					pendingForceComponent.pendingForce += val2;
				}
			}
		}

		private void CalculateTurningForces(TankTrackMachineNode node, float turningInput, TankTrackNode[] tankTracks, int count)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (Mathf.Abs(turningInput) > 0.01f)
			{
				Rigidbody rb = node.rigidbodyComponent.rb;
				Transform transform = rb.get_transform();
				float maxTurnRateMoving = node.avgMaxTurnRateMovingComponent.maxTurnRateMoving;
				float maxTurnRateStopped = node.avgMaxTurnRateStoppedComponent.maxTurnRateStopped;
				float maxSpeed = node.avgMaxSpeedComponent.maxSpeed;
				Vector3 velocity = rb.get_velocity();
				float num = 1f - Mathf.Clamp01(velocity.get_magnitude() / maxSpeed);
				float num2 = Vector3.Dot(rb.get_angularVelocity(), transform.get_up());
				float num3 = maxTurnRateStopped - maxTurnRateMoving;
				float num4 = maxTurnRateMoving + num3 * num;
				float num5 = num4 * ((float)Math.PI / 180f);
				if (Mathf.Abs(num2) < num5)
				{
					CalculateDesiredTotalAccelerationTurningTorque(node, turningInput);
					CalculateTotalFastSidedTorque(turningInput, tankTracks, count, node);
					CalculateTurningTorque(node);
				}
				if (node.numGroundedTracksComponent.groundedTracks > 0)
				{
					CalculateAntiRoll(node);
				}
			}
		}

		private void CalculateAntiRoll(TankTrackMachineNode node)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(rb.get_worldCenterOfMass(), Vector3.get_down(), ref val, 2f, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK))
			{
				Vector3 val2 = -val.get_normal();
				val2 = Vector3.ProjectOnPlane(val2, rb.get_transform().get_forward());
				float num = Vector3.Angle(val2, -rb.get_transform().get_up());
				if (num > 3f)
				{
					float num2 = Vector3.Dot(rb.get_angularVelocity(), rb.get_transform().get_forward());
					Vector3 val3 = rb.get_transform().get_forward() * num2;
					rb.AddTorque(-val3, 2);
				}
			}
		}

		private void CalculateTurningTorque(TankTrackMachineNode node)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			Vector3 val = node.desiredTurningTorqueComponent.desiredTorque - node.fastSideTurningTorqueComponent.fastSideTorque;
			float num = Vector3.Dot(transform.get_transform().get_up(), val) * 0.5f;
			float torquePerTrack = num / (float)_slowSideTracks.get_Count();
			float torquePerTrack2 = num / (float)_fastSideTracks.get_Count();
			AddTurningForce(node, _fastSideTracks, torquePerTrack2, node.movingBackwardsComponent.movingBackwards);
			AddTurningForce(node, _slowSideTracks, torquePerTrack, !node.movingBackwardsComponent.movingBackwards);
		}

		private void AddTurningForce(TankTrackMachineNode node, FasterList<TankTrackNode> tracksToApplyForce, float torquePerTrack, bool movingBackwards)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < tracksToApplyForce.get_Count(); i++)
			{
				TankTrackNode tankTrackNode = tracksToApplyForce.get_Item(i);
				float num = tankTrackNode.distanceToCOMComponent.distance;
				if (tankTrackNode.distanceToCOMComponent.distance > 1f)
				{
					num = 1f / tankTrackNode.distanceToCOMComponent.distance;
				}
				float num2 = torquePerTrack * num;
				Vector3 val = node.rigidbodyComponent.rb.get_transform().get_forward();
				if (!movingBackwards)
				{
					val *= -1f;
				}
				Vector3 val2 = val * Mathf.Abs(num2);
				IPendingForceComponent pendingForceComponent = tankTrackNode.pendingForceComponent;
				pendingForceComponent.pendingForce += val2;
			}
		}

		private void CalculateDesiredTotalAccelerationTurningTorque(TankTrackMachineNode node, float turningInput)
		{
			float angularAccelerationMagnitude = (0f - turningInput) * node.avgTurnAccelerationComponent.turnAcceleration * ((float)Math.PI / 180f);
			CalculateDesiredTotalTurningTorque(node, angularAccelerationMagnitude);
		}

		private void CalculateDesiredTotalDampingTurningTorque(TankTrackMachineNode node, float currentTurnRateRadians)
		{
			float angularAccelerationMagnitude = 0f - currentTurnRateRadians;
			CalculateDesiredTotalTurningTorque(node, angularAccelerationMagnitude);
		}

		private void CalculateDesiredTotalTurningTorque(TankTrackMachineNode node, float angularAccelerationMagnitude)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			Vector3 val = transform.get_up() * angularAccelerationMagnitude;
			Quaternion val2 = transform.get_rotation() * rb.get_inertiaTensorRotation();
			Vector3 desiredTorque = val2 * Vector3.Scale(rb.get_inertiaTensor(), Quaternion.Inverse(val2) * val);
			node.desiredTurningTorqueComponent.desiredTorque = desiredTorque;
		}

		private void CalculateTotalFastSidedTorque(float inputValue, TankTrackNode[] tankTracks, int count, TankTrackMachineNode node)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			_fastSideTracks.FastClear();
			_slowSideTracks.FastClear();
			Vector3 worldCenterOfMass = node.rigidbodyComponent.rb.get_worldCenterOfMass();
			for (int i = 0; i < count; i++)
			{
				TankTrackNode tankTrackNode = tankTracks[i];
				if (!tankTrackNode.hardwareDisabledComponent.disabled)
				{
					MachineSide machineSide = tankTrackNode.machineSideComponent.machineSide;
					if ((inputValue < 0f && machineSide == MachineSide.left) || (inputValue > 0f && machineSide == MachineSide.right))
					{
						val += Vector3.Cross(worldCenterOfMass - tankTrackNode.transformComponent.T.get_position(), tankTrackNode.pendingForceComponent.pendingForce);
						_fastSideTracks.Add(tankTrackNode);
					}
					else if ((inputValue > 0f && machineSide == MachineSide.left) || (inputValue < 0f && machineSide == MachineSide.right))
					{
						_slowSideTracks.Add(tankTrackNode);
					}
				}
			}
			node.fastSideTurningTorqueComponent.fastSideTorque = val;
		}

		private void CalculateTurnDamping(TankTrackMachineNode node, TankTrackNode[] tankTracks, int count)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			float num = Vector3.Dot(rb.get_angularVelocity(), transform.get_up());
			if (Mathf.Abs(num) > 0.08726646f)
			{
				CalculateDesiredTotalDampingTurningTorque(node, num);
				float inputValue = 0f - Mathf.Sign(num);
				CalculateTotalFastSidedTorque(inputValue, tankTracks, count, node);
				CalculateTurningTorque(node);
			}
		}
	}
}
