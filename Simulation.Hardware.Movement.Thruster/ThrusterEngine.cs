using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class ThrusterEngine : SingleEntityViewEngine<MachineThrusterView>, IQueryingEntityViewEngine, IEngine
	{
		private const float FULL_SPEED_ROTATION_THRESHOLD_MULTIPLIER = 5f;

		private const float STOPPED_SPEED_THRESHOLD = 0.01f;

		private const float K_FACTOR = 5f;

		private const float VERTICAL_ERROR_THRESHOLD = 0.085f;

		private ITaskRoutine _physicsTask;

		private ITaskRoutine _updateTask;

		private int _nodesCount;

		private int _physicsNodesCount;

		[Inject]
		internal CeilingHeightManager ceilingHeightManager
		{
			private get;
			set;
		}

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

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<MachineThrusterView> machineViews = entityViewsDB.QueryEntityViews<MachineThrusterView>();
				for (int i = 0; i < machineViews.get_Count(); i++)
				{
					MachineThrusterView machineThrusterView = machineViews.get_Item(i);
					bool flag = !machineThrusterView.stunComponent.stunned && machineThrusterView.rectifierComponent.functionalsEnabled;
					int num = default(int);
					ThrusterNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<ThrusterNode>(machineThrusterView.get_ID(), ref num);
					for (int j = 0; j < num; j++)
					{
						ThrusterNode thrusterNode = array[j];
						thrusterNode.inputReceivedComponent.received = 0f;
						if (!thrusterNode.disabledComponent.disabled && flag)
						{
							if (strafeDirectionManager.strafingEnabled && thrusterNode.ownerComponent.ownedByMe)
							{
								UpdateInput(thrusterNode, thrusterNode.facingComponent.legacyDirection, legacy: true, machineThrusterView.inputComponent.digitalInput);
							}
							else
							{
								UpdateLegacyInput(thrusterNode, machineThrusterView.inputComponent.digitalInput);
							}
							ProcessParticles(thrusterNode);
						}
						else
						{
							StopParticles(thrusterNode);
						}
					}
				}
				yield return null;
			}
		}

		private IEnumerator PhysicsTick()
		{
			while (true)
			{
				FasterReadOnlyList<MachineThrusterView> machineViews = entityViewsDB.QueryEntityViews<MachineThrusterView>();
				for (int i = 0; i < machineViews.get_Count(); i++)
				{
					MachineThrusterView machineThrusterView = machineViews.get_Item(i);
					bool flag = !machineThrusterView.stunComponent.stunned && machineThrusterView.rectifierComponent.functionalsEnabled;
					bool flag2 = machineThrusterView.ownerComponent.ownedByAi || machineThrusterView.ownerComponent.ownedByMe;
					if (flag && flag2)
					{
						UpdateNodes(machineThrusterView, Time.get_fixedDeltaTime());
					}
				}
				yield return null;
			}
		}

		private void UpdateNodes(MachineThrusterView machine, float deltaSec)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			int num = default(int);
			ThrusterNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<ThrusterNode>(machine.get_ID(), ref num);
			for (int i = 0; i < num; i++)
			{
				ThrusterNode thrusterNode = array[i];
				if (thrusterNode.disabledComponent.disabled)
				{
					continue;
				}
				if (strafeDirectionManager.strafingEnabled && thrusterNode.ownerComponent.ownedByMe)
				{
					ApplyStrafingForces(thrusterNode, deltaSec, machine.inputComponent.digitalInput);
					continue;
				}
				if (thrusterNode.inputReceivedComponent.received != 0f)
				{
					ApplyForwardThrust(thrusterNode, thrusterNode.inputReceivedComponent.received);
					continue;
				}
				thrusterNode.forceAppliedComponent.forceApplied = false;
				thrusterNode.forceAppliedComponent.forceDirection = Vector3.get_zero();
				if (thrusterNode.accelerationDelayComponent.accelerationDelay > 0f)
				{
					thrusterNode.accelerationDelayComponent.accelerationPercent = 0f;
					thrusterNode.accelerationDelayComponent.startApplyForceTime = Time.get_timeSinceLevelLoad();
				}
			}
		}

		private void ApplyStrafingForces(ThrusterNode node, float dt, Vector4 cubeInput)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			node.forceAppliedComponent.forceApplied = false;
			Vector3 zero = Vector3.get_zero();
			Vector3 inputVector = default(Vector3);
			inputVector._002Ector(cubeInput.z, cubeInput.y, cubeInput.x);
			Vector3 val = strafeDirectionManager.cameraForwardDirection;
			Vector3 val2 = strafeDirectionManager.cameraRightDirection;
			float angleToStraight = strafeDirectionManager.angleToStraight;
			StrafingCustomAngleToStraightNode strafingCustomAngleToStraightNode = default(StrafingCustomAngleToStraightNode);
			if (entityViewsDB.TryQueryEntityView<StrafingCustomAngleToStraightNode>(node.ownerComponent.machineId, ref strafingCustomAngleToStraightNode) && strafingCustomAngleToStraightNode.customAngleToStraightComponent.customAngleUsed)
			{
				angleToStraight = strafingCustomAngleToStraightNode.customAngleToStraightComponent.angleToStraight;
				float num = angleToStraight - strafeDirectionManager.angleToStraight;
				Quaternion val3 = Quaternion.AngleAxis(0f - num, node.rigidbodyComponent.rb.get_transform().get_up());
				val = val3 * val;
				val2 = val3 * val2;
				inputVector.x = strafingCustomAngleToStraightNode.customInputComponent.forwardInput;
				inputVector.z = strafingCustomAngleToStraightNode.customInputComponent.strafingInput;
			}
			zero += GetStrafingLateralForces(node, inputVector, val, val2);
			zero += GetStrafingTurningForces(node, angleToStraight);
			if (strafeDirectionManager.verticalStrafingEnabled)
			{
				zero += GetVerticalAlignmentForces(node, dt);
				ApplyVerticalAlignmentDamping(node);
			}
			ApplyStrafingForwardThrust(node, zero);
		}

		private void ApplyForwardThrust(ThrusterNode node, float ratio = 1f)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			node.forceAppliedComponent.forceApplied = false;
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform t = node.transformComponent.T;
			Vector3 val = node.forceMagnitudeComponent.direction * ratio;
			float maxSpeed = node.maxVelocityComponent.maxSpeed;
			float num = Vector3.Dot(rb.get_velocity(), val);
			float curve = InterpollationCurves.GetCurve(Mathf.Clamp(num, 0f, maxSpeed), maxSpeed, InterpollationCurve.SharpInverseSquare);
			Vector3 val2 = val * node.forceMagnitudeComponent.force * curve;
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = val2;
			Vector3 position = t.get_position();
			val2 = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y);
			Debug.DrawRay(t.get_position(), val2 * 0.01f, Color.get_green());
			if (val2.get_sqrMagnitude() > 0.01f)
			{
				node.forceAppliedComponent.forceApplied = true;
			}
			UpdateAccelerationDelay(node, val);
			val2 *= node.accelerationDelayComponent.accelerationPercent;
			rb.AddForceAtPosition(val2, t.get_position());
		}

		private void UpdateAccelerationDelay(ThrusterNode node, Vector3 forceDirection)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (node.accelerationDelayComponent.accelerationDelay > 0f)
			{
				if (Vector3.Dot(forceDirection, node.forceAppliedComponent.forceDirection) <= 0f)
				{
					node.accelerationDelayComponent.startApplyForceTime = Time.get_timeSinceLevelLoad();
				}
				node.forceAppliedComponent.forceDirection = forceDirection;
				float num = Time.get_timeSinceLevelLoad() - node.accelerationDelayComponent.startApplyForceTime;
				node.accelerationDelayComponent.accelerationPercent = Mathf.Clamp01(num / node.accelerationDelayComponent.accelerationDelay);
			}
		}

		private Vector3 GetForwardThrust(ThrusterNode node, float ratio = 1f)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			Vector3 direction = node.forceMagnitudeComponent.direction;
			Vector3 val = direction * node.forceMagnitudeComponent.force;
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = val;
			Vector3 position = t.get_position();
			val = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y);
			val *= ratio;
			Debug.DrawRay(t.get_position(), val * 0.01f, Color.get_green());
			return val;
		}

		private void ApplyStrafingForwardThrust(ThrusterNode node, Vector3 force)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			Transform t = node.transformComponent.T;
			Vector3 normalized = force.get_normalized();
			float maxSpeed = node.maxVelocityComponent.maxSpeed;
			float num = Vector3.Dot(rb.get_velocity(), normalized);
			float curve = InterpollationCurves.GetCurve(Mathf.Clamp(num, 0f, maxSpeed), maxSpeed, InterpollationCurve.SharpInverseSquare);
			float sqrMagnitude = force.get_sqrMagnitude();
			float force2 = node.forceMagnitudeComponent.force;
			if (sqrMagnitude > force2 * force2)
			{
				force = normalized * force2;
			}
			force *= curve;
			if (force.get_sqrMagnitude() > 0.01f)
			{
				node.forceAppliedComponent.forceApplied = true;
			}
			UpdateAccelerationDelay(node, normalized);
			force *= node.accelerationDelayComponent.accelerationPercent;
			rb.AddForceAtPosition(force, t.get_position());
		}

		private Vector3 GetStrafingLateralForces(ThrusterNode node, Vector3 inputVector, Vector3 camForwardDirection, Vector3 camRightDirection)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			Vector3 zero = Vector3.get_zero();
			Vector3 direction = node.forceMagnitudeComponent.direction;
			if (strafeDirectionManager.sidewaysDrivingEnabled)
			{
				zero += GetLateralForceInDirection(node, direction, camForwardDirection, inputVector.x);
			}
			else
			{
				Transform transform = node.rigidbodyComponent.rb.get_transform();
				zero += GetLateralForceInDirection(node, direction, transform.get_forward(), inputVector.x);
			}
			zero += GetLateralForceInDirection(node, direction, camRightDirection, inputVector.z);
			if (strafeDirectionManager.verticalStrafingEnabled)
			{
				return zero + GetLateralForceInDirection(node, direction, strafeDirectionManager.cameraUpDirection, inputVector.y);
			}
			return zero + GetLateralForceInDirection(node, direction, Vector3.get_up(), inputVector.y);
		}

		private Vector3 GetLateralForceInDirection(ThrusterNode node, Vector3 thrusterForward, Vector3 movementDirection, float input)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			float num = Vector3.Dot(movementDirection, thrusterForward);
			if (Mathf.Abs(num) < 0.1f)
			{
				return val;
			}
			float num2 = Mathf.Sign(num * input);
			if (node.typeComponent.type == ThrusterType.DualDirection || num2 > 0f)
			{
				val += GetForwardThrust(node, num * input);
				Debug.DrawRay(node.transformComponent.T.get_position(), val * 0.01f, Color.get_green());
			}
			return val;
		}

		private Vector3 GetStrafingTurningForces(ThrusterNode node, float angleToStraight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			float num = Mathf.Abs(angleToStraight);
			if (strafeDirectionManager.IsAngleGreaterThanThreshold(angleToStraight) && (node.facingComponent.legacyDirection == CubeFace.Left || node.facingComponent.legacyDirection == CubeFace.Right))
			{
				float num2 = (num - 5f) / 5f * 5f;
				num2 = Mathf.Clamp01(num2);
				float num3 = Mathf.Sign(angleToStraight * ((node.facingComponent.legacyDirection != CubeFace.Left) ? (-1f) : 1f));
				if (node.typeComponent.type == ThrusterType.DualDirection || num3 > 0f)
				{
					val += GetForwardThrust(node, num2 * num3);
				}
			}
			return val;
		}

		private Vector3 GetVerticalAlignmentForces(ThrusterNode node, float dt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			float num = Mathf.Abs(strafeDirectionManager.verticalAngleError);
			if (num > 0.085f && (node.facingComponent.pitchDirection == CubeFace.Up || node.facingComponent.pitchDirection == CubeFace.Down))
			{
				Rigidbody rb = node.rigidbodyComponent.rb;
				float verticalAngleError = strafeDirectionManager.verticalAngleError;
				float num2 = -5f * verticalAngleError;
				Vector3 val2 = rb.get_transform().InverseTransformVector(rb.get_angularVelocity());
				float x = val2.x;
				float num3 = (num2 - x) / dt;
				Vector3 val3 = rb.get_transform().get_right() * num3;
				Quaternion val4 = rb.get_transform().get_rotation() * rb.get_inertiaTensorRotation();
				Vector3 val5 = val4 * Vector3.Scale(rb.get_inertiaTensor(), Quaternion.Inverse(val4) * val3);
				Vector3 val6 = rb.get_worldCenterOfMass() - node.transformComponent.T.get_position();
				float magnitude = val6.get_magnitude();
				Vector3 val7 = val5 / magnitude;
				float magnitude2 = val7.get_magnitude();
				int numUpThrusters = node.verticalCountComponent.numUpThrusters;
				int numDownThrusters = node.verticalCountComponent.numDownThrusters;
				int num4 = (node.facingComponent.pitchDirection != 0) ? numDownThrusters : numUpThrusters;
				float num5 = Mathf.Sign(verticalAngleError * ((node.facingComponent.pitchDirection != 0) ? (-1f) : 1f));
				if (node.typeComponent.type == ThrusterType.DualDirection || num5 > 0f)
				{
					magnitude2 *= 1f / (float)num4;
					float num6 = magnitude2 / node.forceMagnitudeComponent.force;
					num6 = Mathf.Clamp01(num6);
					val += GetForwardThrust(node, num6 * num5);
				}
			}
			return val;
		}

		private void ApplyVerticalAlignmentDamping(ThrusterNode node)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = node.rigidbodyComponent.rb;
			float num = Vector3.Dot(rb.get_angularVelocity(), rb.get_transform().get_right());
			if ((node.facingComponent.pitchDirection == CubeFace.Up || node.facingComponent.pitchDirection == CubeFace.Down) && Mathf.Abs(num) > 0.01f)
			{
				float verticalAngleError = strafeDirectionManager.verticalAngleError;
				float num2 = Mathf.Abs(verticalAngleError);
				Vector3 val = Vector3.get_zero();
				int num3 = node.verticalCountComponent.numUpThrusters + node.verticalCountComponent.numDownThrusters;
				float num4 = 0f;
				if (num3 > 0)
				{
					num4 = 1f / (float)num3;
				}
				if (num2 < 0.085f)
				{
					val = rb.get_transform().get_right() * num * (0f - node.stoppingComponent.stoppingDampingScale) * num4;
				}
				else if (num2 < 0.17f)
				{
					val = rb.get_transform().get_right() * num * (0f - node.stoppingComponent.slowingDampingScale) * num4;
				}
				rb.AddTorque(val, 2);
			}
		}

		private void UpdateLegacyInput(ThrusterNode node, Vector4 input)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			UpdateInput(node, node.facingComponent.legacyDirection, legacy: true, input);
			if (node.facingComponent.actualDirection == CubeFace.Left)
			{
				if (node.typeComponent.type == ThrusterType.DualDirection && input.w > 0f)
				{
					node.inputReceivedComponent.received = -1f;
				}
				if (input.w < 0f)
				{
					node.inputReceivedComponent.received = 1f;
				}
			}
			else if (node.facingComponent.actualDirection == CubeFace.Right)
			{
				if (input.w > 0f)
				{
					node.inputReceivedComponent.received = 1f;
				}
				if (node.typeComponent.type == ThrusterType.DualDirection && input.w < 0f)
				{
					node.inputReceivedComponent.received = -1f;
				}
			}
		}

		private void UpdateInput(ThrusterNode node, CubeFace facingDirection, bool legacy, Vector4 input)
		{
			node.inputReceivedComponent.received = 0f;
			switch (facingDirection)
			{
			case CubeFace.Up:
				if (legacy || strafeDirectionManager.verticalStrafingEnabled)
				{
					if (node.typeComponent.type == ThrusterType.DualDirection && input.y < 0f)
					{
						node.inputReceivedComponent.received -= 1f;
					}
					if (input.y > 0f)
					{
						node.inputReceivedComponent.received += 1f;
					}
				}
				break;
			case CubeFace.Down:
				if (legacy || strafeDirectionManager.verticalStrafingEnabled)
				{
					if (input.y < 0f)
					{
						node.inputReceivedComponent.received += 1f;
					}
					if (node.typeComponent.type == ThrusterType.DualDirection && input.y > 0f)
					{
						node.inputReceivedComponent.received -= 1f;
					}
				}
				break;
			case CubeFace.Front:
				if (input.z > 0f)
				{
					node.inputReceivedComponent.received += 1f;
				}
				if (node.typeComponent.type == ThrusterType.DualDirection && input.z < 0f)
				{
					node.inputReceivedComponent.received -= 1f;
				}
				break;
			case CubeFace.Back:
				if (input.z < 0f)
				{
					node.inputReceivedComponent.received += 1f;
				}
				if (node.typeComponent.type == ThrusterType.DualDirection && input.z > 0f)
				{
					node.inputReceivedComponent.received -= 1f;
				}
				break;
			case CubeFace.Right:
				if (input.x > 0f)
				{
					node.inputReceivedComponent.received += 1f;
				}
				if (node.typeComponent.type == ThrusterType.DualDirection && input.x < 0f)
				{
					node.inputReceivedComponent.received -= 1f;
				}
				break;
			case CubeFace.Left:
				if (node.typeComponent.type == ThrusterType.DualDirection && input.x > 0f)
				{
					node.inputReceivedComponent.received -= 1f;
				}
				if (input.x < 0f)
				{
					node.inputReceivedComponent.received += 1f;
				}
				break;
			}
		}

		private void ProcessParticles(ThrusterNode node)
		{
			bool flag = node.visibilityComponent.offScreen || (!strafeDirectionManager.strafingEnabled && node.inputReceivedComponent.received == 0f) || (strafeDirectionManager.strafingEnabled && !node.forceAppliedComponent.forceApplied) || node.disabledComponent.disabled;
			for (int i = 0; i < node.particleEffects.particleSystems.Length; i++)
			{
				ParticleSystem val = node.particleEffects.particleSystems[i];
				if (!(val != null))
				{
					continue;
				}
				if (flag)
				{
					if (val.get_isEmitting())
					{
						val.Stop();
					}
				}
				else if (!val.get_isEmitting())
				{
					val.Play();
				}
			}
		}

		private void StopParticles(ThrusterNode node)
		{
			for (int i = 0; i < node.particleEffects.particleSystems.Length; i++)
			{
				ParticleSystem val = node.particleEffects.particleSystems[i];
				if (val != null && val.get_isEmitting())
				{
					val.Stop();
				}
			}
		}

		public void Ready()
		{
			_physicsTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)PhysicsTick)
				.SetScheduler(StandardSchedulers.get_physicScheduler());
			_updateTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update)
				.SetScheduler(StandardSchedulers.get_updateScheduler());
		}

		protected override void Add(MachineThrusterView entityView)
		{
			if ((entityView.ownerComponent.ownedByMe || entityView.ownerComponent.ownedByAi) && _physicsNodesCount++ == 0)
			{
				_physicsTask.Start((Action<PausableTaskException>)null, (Action)null);
			}
			if (_nodesCount++ == 0)
			{
				_updateTask.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(MachineThrusterView entityView)
		{
			if ((entityView.ownerComponent.ownedByMe || entityView.ownerComponent.ownedByAi) && --_physicsNodesCount == 0)
			{
				_physicsTask.Pause();
			}
			if (--_nodesCount == 0)
			{
				_updateTask.Pause();
			}
		}
	}
}
