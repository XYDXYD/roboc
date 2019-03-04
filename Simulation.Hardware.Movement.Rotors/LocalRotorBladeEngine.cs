using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class LocalRotorBladeEngine : MultiEntityViewsEngine<RotorBladeNode, LocalMachineRotorNode>, ITickable, IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float DATA_UPDATE_RATE = 1f;

		private const float STRAFE_TILT_MULTIPLIER = 0.5f;

		public const float STOPPED = 0.01f;

		public const float SLOW = 0.1f;

		public const float MINIMUM_STEER_FORCE_DIST = 0.6f;

		private FasterList<RotorBladeNode> _localHoverRotors = new FasterList<RotorBladeNode>();

		private FasterList<RotorBladeNode> _localPropRotors = new FasterList<RotorBladeNode>();

		private int _localMachineId;

		private Vector3 _compositeForce = Vector3.get_zero();

		private Vector3 _compositeTorque = Vector3.get_zero();

		private Vector3 _gravity;

		private bool _machineStunned;

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

		protected override void Add(LocalMachineRotorNode obj)
		{
			_localMachineId = obj.get_ID();
			obj.machineStunComponent.machineStunned.subscribers += HandleMachineStunned;
			InitialiseCachedValues();
		}

		protected override void Remove(LocalMachineRotorNode obj)
		{
			obj.machineStunComponent.machineStunned.subscribers -= HandleMachineStunned;
		}

		protected override void Add(RotorBladeNode obj)
		{
			if (obj.ownerComponent.ownedByMe)
			{
				obj.disabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)OnNumRotorsChanged);
				if (!obj.disabledComponent.isPartDisabled.get_value())
				{
					RegisterRotor(obj);
				}
				LocalMachineRotorNode localMachineRotorNode = default(LocalMachineRotorNode);
				if (entityViewsDB.TryQueryEntityView<LocalMachineRotorNode>(obj.ownerComponent.machineId, ref localMachineRotorNode))
				{
					localMachineRotorNode.cacheUpdateComponent.updateRequired = true;
				}
			}
		}

		protected override void Remove(RotorBladeNode obj)
		{
			if (obj.ownerComponent.ownedByMe)
			{
				UnregisterRotor(obj);
				obj.disabledComponent.isPartDisabled.StopNotify((Action<int, bool>)OnNumRotorsChanged);
			}
		}

		private void HandleMachineStunned(IMachineStunComponent sender, int machineId)
		{
			_machineStunned = sender.stunned;
		}

		public void Tick(float deltaSec)
		{
			LocalMachineRotorNode localMachineRotorNode = default(LocalMachineRotorNode);
			if (entityViewsDB.TryQueryEntityView<LocalMachineRotorNode>(_localMachineId, ref localMachineRotorNode) && !_machineStunned && !(localMachineRotorNode.rigidbodyComponent.rb == null) && (_localHoverRotors.get_Count() != 0 || _localPropRotors.get_Count() != 0))
			{
				ReadInput(localMachineRotorNode);
				CalculatePowerValue(localMachineRotorNode);
				if (localMachineRotorNode.cacheUpdateComponent.updateRequired)
				{
					UpdateCacheValues(localMachineRotorNode);
				}
				SetMovementTilt(localMachineRotorNode);
			}
		}

		public void PhysicsTick(float deltaSec)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			LocalMachineRotorNode localMachineRotorNode = default(LocalMachineRotorNode);
			if (!entityViewsDB.TryQueryEntityView<LocalMachineRotorNode>(_localMachineId, ref localMachineRotorNode) || _machineStunned)
			{
				return;
			}
			Rigidbody rb = localMachineRotorNode.rigidbodyComponent.rb;
			if (rb == null)
			{
				return;
			}
			UpdateValues(localMachineRotorNode, deltaSec);
			if (localMachineRotorNode.cacheUpdateComponent.updateRequired)
			{
				UpdateCacheValues(localMachineRotorNode);
			}
			CalculateIfGrounded(localMachineRotorNode);
			_compositeForce = Vector3.get_zero();
			_compositeTorque = Vector3.get_zero();
			if (localMachineRotorNode.functionalsEnabledComponent.functionalsEnabled)
			{
				if (_localHoverRotors.get_Count() > 0)
				{
					ApplyHoverForces(localMachineRotorNode, deltaSec);
				}
				if (_localPropRotors.get_Count() > 0)
				{
					ApplyPropForces(localMachineRotorNode, deltaSec);
				}
				Vector3 val = rb.get_worldCenterOfMass() + rb.get_transform().get_rotation() * localMachineRotorNode.forceOffsetComponent.localForceOffset;
				rb.AddForceAtPosition(_compositeForce, val);
				rb.AddTorque(_compositeTorque);
			}
		}

		private void CalculatePowerValue(LocalMachineRotorNode machineNode)
		{
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			float num = 0f;
			if (inputComponent.inputRise != inputComponent.inputLower)
			{
				num += 1f;
			}
			if (inputComponent.inputRight != inputComponent.inputLeft)
			{
				num += 1f;
			}
			if (inputComponent.inputForward != inputComponent.inputBack)
			{
				num += 1f;
			}
			num *= 0.333333f;
			machineNode.powerValueComponent.power = num;
			machineNode.audioLiftingLoweringComponent.lifting = inputComponent.inputRise;
			machineNode.audioLiftingLoweringComponent.lowering = inputComponent.inputLower;
		}

		private void UpdateValues(LocalMachineRotorNode machineNode, float deltaTime)
		{
			ITimerComponent timerComponent = machineNode.timerComponent;
			timerComponent.timer -= deltaTime;
			if (timerComponent.timer <= 0f)
			{
				UpdateCOMRelatedValues(machineNode);
				FindForcePosition(machineNode);
				timerComponent.timer = 1f;
			}
		}

		private void ReadInput(LocalMachineRotorNode machineNode)
		{
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			if (machineNode.functionalsEnabledComponent.functionalsEnabled)
			{
				IMachineControl machineInput = machineNode.inputWrapperComponent.machineInput;
				if (machineInput.moveUpwards != machineInput.moveDown)
				{
					inputComponent.inputRise = machineInput.moveUpwards;
					inputComponent.inputLower = machineInput.moveDown;
				}
				else
				{
					IRotorInputComponent rotorInputComponent = inputComponent;
					bool inputRise = inputComponent.inputLower = false;
					rotorInputComponent.inputRise = inputRise;
				}
				inputComponent.inputRight = (machineInput.horizontalAxis > 0.01f);
				inputComponent.inputLeft = (machineInput.horizontalAxis < -0.01f);
				inputComponent.inputForward = (machineInput.forwardAxis > 0.01f);
				inputComponent.inputBack = (machineInput.forwardAxis < -0.01f);
				inputComponent.inputLegacyStrafeLeft = machineInput.strafeLeft;
				inputComponent.inputLegacyStrafeRight = machineInput.strafeRight;
			}
			else
			{
				inputComponent.inputRise = false;
				inputComponent.inputLower = false;
				inputComponent.inputRight = false;
				inputComponent.inputLeft = false;
				inputComponent.inputForward = false;
				inputComponent.inputBack = false;
				inputComponent.inputLegacyStrafeLeft = false;
				inputComponent.inputLegacyStrafeRight = false;
			}
		}

		private void InitialiseCachedValues()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			_gravity = Physics.get_gravity();
		}

		private void CalculateIfGrounded(LocalMachineRotorNode machineNode)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			IRotorsGroundedComponent groundedComponent = machineNode.groundedComponent;
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			Vector3 position = machineNode.rigidbodyComponent.rb.get_position();
			float y = position.y;
			Vector3 prevPosition = groundedComponent.prevPosition;
			float num = y - prevPosition.y;
			bool flag = num < 0f;
			if (groundedComponent.prevDescending && !flag && inputComponent.inputLower)
			{
				groundedComponent.grounded = true;
			}
			else if (inputComponent.inputRise || inputComponent.inputRight || inputComponent.inputLeft || inputComponent.inputForward || inputComponent.inputBack || inputComponent.inputLegacyStrafeLeft || inputComponent.inputLegacyStrafeRight)
			{
				groundedComponent.grounded = false;
			}
			groundedComponent.prevDescending = flag;
			groundedComponent.prevPosition = position;
		}

		private void SetMovementTilt(LocalMachineRotorNode machineNode)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			Vector3 val = Vector3.get_zero();
			Vector3 val2 = Vector3.get_zero();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = _localHoverRotors.get_Count() == 1;
			bool flag4 = _localHoverRotors.get_Count() > 0;
			if (inputComponent.inputForward)
			{
				val += Vector3.get_right();
				flag = true;
			}
			if (inputComponent.inputBack)
			{
				val += Vector3.get_left();
				flag = true;
			}
			if (flag3)
			{
				if (inputComponent.inputRight)
				{
					val += Vector3.get_back();
					flag = true;
				}
				if (inputComponent.inputLeft)
				{
					val += Vector3.get_forward();
					flag = true;
				}
			}
			if (flag4)
			{
				if (inputComponent.inputRight || inputComponent.inputLegacyStrafeRight)
				{
					val2 += Vector3.get_back();
					flag2 = true;
				}
				if (inputComponent.inputLeft || inputComponent.inputLegacyStrafeLeft)
				{
					val2 += Vector3.get_forward();
					flag2 = true;
				}
			}
			IMachineTiltComponent machineTiltComponent = machineNode.machineTiltComponent;
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			machineTiltComponent.localMovementTilt = Vector3.get_zero();
			if (flag)
			{
				IMachineTiltComponent machineTiltComponent2 = machineTiltComponent;
				machineTiltComponent2.localMovementTilt += val.get_normalized() * averageMovementValuesComponent.avgMovementTilt;
			}
			if (flag2)
			{
				IMachineTiltComponent machineTiltComponent3 = machineTiltComponent;
				machineTiltComponent3.localMovementTilt += val2.get_normalized() * averageMovementValuesComponent.avgBankTilt;
			}
		}

		private void ApplyHoverForces(LocalMachineRotorNode machineNode, float deltaTime)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (_localHoverRotors.get_Count() > 0)
			{
				CalculateRobotTilt(machineNode);
				IRobotHeightComponent robotHeightComponent = machineNode.robotHeightComponent;
				Vector3 position = machineNode.rigidbodyComponent.rb.get_position();
				robotHeightComponent.robotHeight = position.y * machineNode.averageMovementValuesComponent.avgCeilingHeightModifier;
				SetModifiedMass(machineNode);
				UpdateCubeValues(machineNode, deltaTime);
				ApplyHoverHeightForce(machineNode);
				ApplyHoverDriftForce(machineNode);
				ApplyHoverStrafeForce(machineNode);
				ApplyHoverSteerTorque(machineNode, deltaTime);
				ApplyLevelingTorque(machineNode, deltaTime);
			}
		}

		private void CalculateRobotTilt(LocalMachineRotorNode machineNode)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			IMachineTiltComponent machineTiltComponent = machineNode.machineTiltComponent;
			float num = 1f;
			if (strafeDirectionManager.strafingEnabled && strafeDirectionManager.verticalStrafingEnabled && machineNode.ownerComponent.ownedByMe)
			{
				num = 0.5f;
			}
			Vector3 localMovementTilt = machineTiltComponent.localMovementTilt;
			localMovementTilt.x *= num;
			machineTiltComponent.targetLocalTilt = localMovementTilt;
			if (!machineNode.groundedComponent.grounded)
			{
				Vector3 localBalanceTilt = machineTiltComponent.localBalanceTilt;
				localBalanceTilt.x *= num;
				IMachineTiltComponent machineTiltComponent2 = machineTiltComponent;
				machineTiltComponent2.targetLocalTilt += localBalanceTilt;
			}
		}

		private void SetModifiedMass(LocalMachineRotorNode machineNode)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			float num = Vector3.Dot(machineNode.rigidbodyComponent.rb.get_transform().get_up(), Vector3.get_up());
			float num2 = Mathf.Acos(num);
			if (num2 > (float)Math.PI / 2f)
			{
				num2 = Mathf.Abs((float)Math.PI - num2);
			}
			float num3 = 1f;
			if (num2 > averageMovementValuesComponent.avgFullHoverAngle)
			{
				num3 = averageMovementValuesComponent.avgMinHoverRatio;
				if (num2 > averageMovementValuesComponent.avgFullHoverAngle && num2 < averageMovementValuesComponent.avgMinHoverAngle)
				{
					num3 = (num2 - averageMovementValuesComponent.avgFullHoverAngle) / (averageMovementValuesComponent.avgMinHoverAngle - averageMovementValuesComponent.avgFullHoverAngle);
				}
			}
			machineNode.massComponent.mass = Mathf.Min(averageMovementValuesComponent.maxCarryingMass, machineNode.rigidbodyComponent.rb.get_mass());
			machineNode.massComponent.modifiedMass = num3 * machineNode.massComponent.mass;
		}

		private void UpdateCubeValues(LocalMachineRotorNode machineNode, float deltaTime)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			machineNode.localCOMComponent.localCOM = machineNode.rigidbodyComponent.rb.get_centerOfMass();
			for (int i = 0; i < _localHoverRotors.get_Count(); i++)
			{
				UpdateRotorVelocity(deltaTime, _localHoverRotors.get_Item(i));
			}
			for (int j = 0; j < _localPropRotors.get_Count(); j++)
			{
				UpdateRotorVelocity(deltaTime, _localPropRotors.get_Item(j));
			}
		}

		private void UpdateRotorVelocity(float deltaTime, RotorBladeNode rotor)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = rotor.transformComponent.T.get_position();
			Vector3 localPosition = rotor.transformComponent.T.get_localPosition();
			RotorData rotorData = rotor.rotorDataComponent.rotorData;
			rotorData.localVel = (localPosition - rotorData.prevLocalPos) / deltaTime;
			rotorData.prevLocalPos = localPosition;
			rotorData.worldVel = (position - rotorData.prevWorldPos) / deltaTime;
			rotorData.prevWorldPos = position;
		}

		private void ApplyPropForces(LocalMachineRotorNode machineNode, float deltaTime)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = machineNode.rigidbodyComponent.rb;
			for (int i = 0; i < _localPropRotors.get_Count(); i++)
			{
				RotorBladeNode rotorBladeNode = _localPropRotors.get_Item(i);
				RotorData rotorData = rotorBladeNode.rotorDataComponent.rotorData;
				Vector3 up = rotorBladeNode.transformComponent.T.get_up();
				float direc = 1f;
				bool flag = ShouldCubeThrust(machineNode, rotorData.facingDirection, out direc);
				bool flag2 = false;
				if (!flag)
				{
					flag2 = ShouldCubeBrake(rb, rotorBladeNode, up, out direc);
				}
				if (flag || flag2)
				{
					Vector3 position = rotorBladeNode.transformComponent.T.get_position();
					Vector3 inputForce = up * direc * rotorBladeNode.forceMagnitudeComponent.forceMagnitude;
					inputForce = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y * machineNode.averageMovementValuesComponent.avgCeilingHeightModifier);
					rb.AddForceAtPosition(inputForce, position);
				}
			}
		}

		private void OnNumRotorsChanged(int id, bool b)
		{
			RotorBladeNode rotorBladeNode = default(RotorBladeNode);
			if (entityViewsDB.TryQueryEntityView<RotorBladeNode>(id, ref rotorBladeNode))
			{
				if (b)
				{
					UnregisterRotor(rotorBladeNode);
				}
				else
				{
					RegisterRotor(rotorBladeNode);
				}
				LocalMachineRotorNode localMachineRotorNode = default(LocalMachineRotorNode);
				if (entityViewsDB.TryQueryEntityView<LocalMachineRotorNode>(rotorBladeNode.ownerComponent.machineId, ref localMachineRotorNode))
				{
					localMachineRotorNode.cacheUpdateComponent.updateRequired = true;
				}
			}
		}

		private void RegisterRotor(RotorBladeNode rotorNode)
		{
			RotorData rotorData = rotorNode.rotorDataComponent.rotorData;
			rotorData.hoveringRotor = (rotorData.facingDirection == CubeFace.Up || rotorData.facingDirection == CubeFace.Down);
			if (rotorData.hoveringRotor)
			{
				_localHoverRotors.Add(rotorNode);
			}
			else
			{
				_localPropRotors.Add(rotorNode);
			}
		}

		private void UnregisterRotor(RotorBladeNode rotorNode)
		{
			_localHoverRotors.UnorderedRemove(rotorNode);
			_localPropRotors.UnorderedRemove(rotorNode);
		}

		private void UpdateCacheValues(LocalMachineRotorNode machineNode)
		{
			machineNode.cacheUpdateComponent.updateRequired = false;
			UpdateAverageValues(machineNode);
			FindForcePosition(machineNode);
		}

		private void UpdateAverageValues(LocalMachineRotorNode machineNode)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			int count = _localHoverRotors.get_Count();
			int count2 = _localPropRotors.get_Count();
			machineNode.localCOMComponent.localCOM = machineNode.rigidbodyComponent.rb.get_centerOfMass();
			averageMovementValuesComponent.avgCeilingHeightModifier = 0f;
			averageMovementValuesComponent.maxCarryingMass = 0f;
			averageMovementValuesComponent.avgMaxHeightChangeSpeed = 0f;
			averageMovementValuesComponent.avgHeightAcceleration = 0f;
			averageMovementValuesComponent.avgStrafeAcceleration = 0f;
			averageMovementValuesComponent.avgTurnAcceleration = 0f;
			averageMovementValuesComponent.avgTurnMaxRate = 0f;
			averageMovementValuesComponent.avgTurnTangentalAcceleration = 0f;
			averageMovementValuesComponent.avgTurnTangentalMaxSpeed = 0f;
			averageMovementValuesComponent.avgLevelAcceleration = 0f;
			averageMovementValuesComponent.avgLevelRate = 0f;
			averageMovementValuesComponent.avgDriftAcceleration = 0f;
			averageMovementValuesComponent.avgDriveMaxSpeed = 100f;
			averageMovementValuesComponent.avgDriftMaxSpeedAngle = 0f;
			averageMovementValuesComponent.avgRotorSize = 0f;
			averageMovementValuesComponent.avgZeroTiltSize = 0f;
			averageMovementValuesComponent.avgTilt = 0f;
			averageMovementValuesComponent.avgMovementTilt = 0f;
			averageMovementValuesComponent.avgBankTilt = 0f;
			averageMovementValuesComponent.avgFullHoverAngle = 0f;
			averageMovementValuesComponent.avgMinHoverAngle = 0f;
			averageMovementValuesComponent.avgMinHoverRatio = 0f;
			averageMovementValuesComponent.avgHoverRadiusSqr = 0f;
			averageMovementValuesComponent.avgForcePos = Vector3.get_zero();
			for (int i = 0; i < count; i++)
			{
				RotorBladeNode rotorBladeNode = _localHoverRotors.get_Item(i);
				Vector3 localPosition = rotorBladeNode.transformComponent.T.get_localPosition();
				averageMovementValuesComponent.avgCeilingHeightModifier += rotorBladeNode.ceilingHeightModifierComponent.ceilingHeightModifier;
				averageMovementValuesComponent.maxCarryingMass += rotorBladeNode.maxCarryMassComponent.maxCarryMass;
				averageMovementValuesComponent.avgMaxHeightChangeSpeed += rotorBladeNode.heightChangeComponent.heightMaxChangeSpeed;
				averageMovementValuesComponent.avgHeightAcceleration += rotorBladeNode.heightChangeComponent.heightAcceleration;
				averageMovementValuesComponent.avgStrafeAcceleration += rotorBladeNode.strafeComponent.strafeAcceleration;
				averageMovementValuesComponent.avgTurnAcceleration += rotorBladeNode.turningComponent.turnAcceleration;
				averageMovementValuesComponent.avgTurnMaxRate += rotorBladeNode.turningComponent.turnMaxRate;
				averageMovementValuesComponent.avgTurnTangentalAcceleration += rotorBladeNode.turningComponent.turnTangentalAcceleration;
				averageMovementValuesComponent.avgTurnTangentalMaxSpeed += rotorBladeNode.turningComponent.turnTangentalMaxSpeed;
				averageMovementValuesComponent.avgLevelAcceleration += rotorBladeNode.levelingComponent.levelAcceleration;
				averageMovementValuesComponent.avgLevelRate += rotorBladeNode.levelingComponent.levelMaxRate;
				averageMovementValuesComponent.avgDriftAcceleration += rotorBladeNode.driftComponent.driftAcceleration;
				averageMovementValuesComponent.avgDriftMaxSpeedAngle += rotorBladeNode.driftComponent.driftMaxSpeedAngle;
				averageMovementValuesComponent.avgRotorSize += rotorBladeNode.tiltComponent.rotorRadius;
				averageMovementValuesComponent.avgZeroTiltSize += rotorBladeNode.tiltComponent.zeroTiltRadius;
				averageMovementValuesComponent.avgTilt += rotorBladeNode.tiltComponent.tiltDegrees;
				averageMovementValuesComponent.avgMovementTilt += rotorBladeNode.tiltComponent.movementTilt;
				averageMovementValuesComponent.avgBankTilt += rotorBladeNode.tiltComponent.bankTilt;
				averageMovementValuesComponent.avgFullHoverAngle += rotorBladeNode.tiltComponent.fullHoverAngle;
				averageMovementValuesComponent.avgMinHoverAngle += rotorBladeNode.tiltComponent.minHoverAngle;
				averageMovementValuesComponent.avgMinHoverRatio += rotorBladeNode.tiltComponent.minHoverRatio;
				Vector3 val = localPosition - machineNode.localCOMComponent.localCOM;
				val.y = 0f;
				averageMovementValuesComponent.avgHoverRadiusSqr += Mathf.Max(val.get_sqrMagnitude(), 1f);
				IAverageMovementValuesComponent averageMovementValuesComponent2 = averageMovementValuesComponent;
				averageMovementValuesComponent2.avgForcePos += val / rotorBladeNode.forcePointScaleComponent.forcePointScale;
			}
			for (int j = 0; j < count2; j++)
			{
				RotorBladeNode rotorBladeNode2 = _localPropRotors.get_Item(j);
				averageMovementValuesComponent.avgCeilingHeightModifier += rotorBladeNode2.ceilingHeightModifierComponent.ceilingHeightModifier;
			}
			if (count > 0)
			{
				float num = 1f / (float)count;
				averageMovementValuesComponent.avgMaxHeightChangeSpeed *= num;
				averageMovementValuesComponent.avgHeightAcceleration *= num;
				averageMovementValuesComponent.avgStrafeAcceleration *= num;
				averageMovementValuesComponent.avgTurnAcceleration *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgTurnMaxRate *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgTurnTangentalAcceleration *= num;
				averageMovementValuesComponent.avgTurnTangentalMaxSpeed *= num;
				averageMovementValuesComponent.avgLevelAcceleration *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgLevelRate *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgDriftAcceleration *= num;
				averageMovementValuesComponent.avgDriftMaxSpeedAngle *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgTilt *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgMovementTilt *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgBankTilt *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgFullHoverAngle *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgMinHoverAngle *= num * ((float)Math.PI / 180f);
				averageMovementValuesComponent.avgMinHoverRatio *= num;
				averageMovementValuesComponent.avgHoverRadiusSqr *= num;
				IAverageMovementValuesComponent averageMovementValuesComponent3 = averageMovementValuesComponent;
				averageMovementValuesComponent3.avgForcePos *= num;
				IAverageMovementValuesComponent averageMovementValuesComponent4 = averageMovementValuesComponent;
				averageMovementValuesComponent4.avgForcePos += machineNode.localCOMComponent.localCOM;
			}
			if (count + count2 > 0)
			{
				float num2 = 1f / (float)(count + count2);
				averageMovementValuesComponent.avgCeilingHeightModifier *= num2;
			}
		}

		private void UpdateCOMRelatedValues(LocalMachineRotorNode machineNode)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			int count = _localHoverRotors.get_Count();
			machineNode.localCOMComponent.localCOM = machineNode.rigidbodyComponent.rb.get_centerOfMass();
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			averageMovementValuesComponent.avgHoverRadiusSqr = 0f;
			averageMovementValuesComponent.avgForcePos = Vector3.get_zero();
			for (int i = 0; i < count; i++)
			{
				RotorBladeNode rotorBladeNode = _localHoverRotors.get_Item(i);
				Vector3 localPosition = rotorBladeNode.transformComponent.T.get_localPosition();
				Vector3 val = localPosition - machineNode.localCOMComponent.localCOM;
				val.y = 0f;
				averageMovementValuesComponent.avgHoverRadiusSqr += Mathf.Max(val.get_sqrMagnitude(), 1f);
				IAverageMovementValuesComponent averageMovementValuesComponent2 = averageMovementValuesComponent;
				averageMovementValuesComponent2.avgForcePos += val / rotorBladeNode.forcePointScaleComponent.forcePointScale;
			}
			if (count > 0)
			{
				float num = 1f / (float)count;
				averageMovementValuesComponent.avgHoverRadiusSqr *= num;
				IAverageMovementValuesComponent averageMovementValuesComponent3 = averageMovementValuesComponent;
				averageMovementValuesComponent3.avgForcePos *= num;
				IAverageMovementValuesComponent averageMovementValuesComponent4 = averageMovementValuesComponent;
				averageMovementValuesComponent4.avgForcePos += machineNode.localCOMComponent.localCOM;
			}
		}

		private void FindForcePosition(LocalMachineRotorNode machineNode)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			IMachineDriftComponent machineDriftComponent = machineNode.machineDriftComponent;
			IMachineTiltComponent machineTiltComponent = machineNode.machineTiltComponent;
			IForceOffsetComponent forceOffsetComponent = machineNode.forceOffsetComponent;
			forceOffsetComponent.localForceOffset = Vector3.get_zero();
			machineTiltComponent.localBalanceTilt = Vector3.get_zero();
			machineDriftComponent.targetLocalDrift = Vector3.get_zero();
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			Vector3 centerOfMass = machineNode.rigidbodyComponent.rb.get_centerOfMass();
			int count = _localHoverRotors.get_Count();
			if (count <= 0)
			{
				return;
			}
			Vector3 val = averageMovementValuesComponent.avgForcePos - centerOfMass;
			val.y = 0f;
			float magnitude = val.get_magnitude();
			if (magnitude > averageMovementValuesComponent.avgRotorSize)
			{
				forceOffsetComponent.localForceOffset = val;
			}
			else
			{
				if (!(magnitude > averageMovementValuesComponent.avgZeroTiltSize))
				{
					return;
				}
				float num = Mathf.Clamp01(magnitude / averageMovementValuesComponent.avgRotorSize);
				Quaternion val2 = Quaternion.FromToRotation(val, Vector3.get_up());
				float num2 = averageMovementValuesComponent.avgTilt / ((float)Math.PI / 2f);
				Vector3 eulerAngles = val2.get_eulerAngles();
				for (int i = 0; i < 3; i++)
				{
					if (eulerAngles.get_Item(i) > 180f)
					{
						int num3;
						eulerAngles.set_Item(num3 = i, eulerAngles.get_Item(num3) - 360f);
					}
				}
				machineTiltComponent.localBalanceTilt = eulerAngles * ((float)Math.PI / 180f) * num2;
				IMachineDriftComponent machineDriftComponent2 = machineDriftComponent;
				Vector3 val3 = -val;
				machineDriftComponent2.targetLocalDrift = val3.get_normalized() * averageMovementValuesComponent.avgDriveMaxSpeed * num;
			}
		}

		private void ApplyHoverHeightForce(LocalMachineRotorNode machineNode)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			Vector3 zero = Vector3.get_zero();
			float num = 0f;
			float num2 = 0f;
			if (inputComponent.inputRise)
			{
				num = 1f;
				num2 = 1f;
			}
			else if (inputComponent.inputLower)
			{
				num = -1f;
				num2 = 1f;
			}
			Vector3 velocity = machineNode.rigidbodyComponent.rb.get_velocity();
			float y = velocity.y;
			float currentHeight = machineNode.robotHeightComponent.robotHeight * averageMovementValuesComponent.avgCeilingHeightModifier;
			float num3 = ceilingHeightManager.ApplyMaxCeilingToForce(averageMovementValuesComponent.avgMaxHeightChangeSpeed, currentHeight);
			float bobModification = ceilingHeightManager.GetBobModification(currentHeight);
			float num4 = num2 * y + num * averageMovementValuesComponent.avgMaxHeightChangeSpeed;
			if (num4 > num3)
			{
				num4 = num3;
			}
			num4 += bobModification;
			float num5 = num4 - y;
			float num6 = Mathf.Min(averageMovementValuesComponent.avgHeightAcceleration, Mathf.Abs(num5)) * Mathf.Sign(num5);
			zero = Vector3.get_up() * num6;
			Debug.DrawRay(machineNode.rigidbodyComponent.rb.get_worldCenterOfMass(), zero, Color.get_magenta());
			zero -= _gravity;
			Vector3 val = zero * machineNode.massComponent.modifiedMass;
			_compositeForce += val;
		}

		private void ApplyHoverDriftForce(LocalMachineRotorNode machineNode)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			IMachineDriftComponent machineDriftComponent = machineNode.machineDriftComponent;
			Rigidbody rb = machineNode.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			Vector3 up = transform.get_up();
			Quaternion rotation = transform.get_rotation();
			float num = Vector3.Angle(Vector3.get_up(), up) * ((float)Math.PI / 180f);
			num = Mathf.Clamp(num, 0f, averageMovementValuesComponent.avgMovementTilt);
			Vector3 val = up;
			val.y = 0f;
			val = Quaternion.Inverse(rotation) * val;
			val.y = 0f;
			machineDriftComponent.targetDriftSpeed = averageMovementValuesComponent.avgDriveMaxSpeed * Mathf.Clamp01(num / averageMovementValuesComponent.avgDriftMaxSpeedAngle);
			machineDriftComponent.targetLocalDrift = val.get_normalized() * machineDriftComponent.targetDriftSpeed;
			Vector3 val2 = Vector3.get_zero();
			Vector3 val3 = Quaternion.Inverse(rotation) * rb.get_velocity();
			Vector3 val4 = machineDriftComponent.targetLocalDrift - val3;
			for (int i = 0; i < 3; i += 2)
			{
				Vector3 targetLocalDrift = machineDriftComponent.targetLocalDrift;
				float num2 = targetLocalDrift.get_Item(i);
				float num3 = val4.get_Item(i);
				if (Mathf.Abs(num2) > 0.01f && Mathf.Abs(num3) > 0f && Mathf.Sign(num3) == Mathf.Sign(num2))
				{
					val2.set_Item(i, Mathf.Min(num2, num3));
				}
			}
			if (val2.get_sqrMagnitude() > averageMovementValuesComponent.avgDriftAcceleration * averageMovementValuesComponent.avgDriftAcceleration)
			{
				val2 = val2.get_normalized() * averageMovementValuesComponent.avgDriftAcceleration;
			}
			Vector3 val5 = rotation * val2 * machineNode.massComponent.mass;
			val5.y = 0f;
			_compositeForce += val5;
		}

		private void ApplyHoverStrafeForce(LocalMachineRotorNode machineNode)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			IMachineDriftComponent machineDriftComponent = machineNode.machineDriftComponent;
			Rigidbody rb = machineNode.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			float num = machineNode.massComponent.modifiedMass;
			Vector3 forward = transform.get_forward();
			forward.y = 0f;
			Quaternion val = Quaternion.FromToRotation(forward, Vector3.get_forward());
			Vector3 val2 = val * rb.get_velocity();
			val2.y = 0f;
			float avgStrafeAcceleration = averageMovementValuesComponent.avgStrafeAcceleration;
			Vector3 val3 = Vector3.get_zero();
			if (inputComponent.inputForward)
			{
				val3 += Vector3.get_forward() * avgStrafeAcceleration;
			}
			if (inputComponent.inputBack)
			{
				val3 += Vector3.get_back() * avgStrafeAcceleration;
			}
			if (_localHoverRotors.get_Count() == 1 || strafeDirectionManager.strafingEnabled)
			{
				if (inputComponent.inputRight || inputComponent.inputLegacyStrafeRight)
				{
					val3 += Vector3.get_right() * avgStrafeAcceleration;
				}
				if (inputComponent.inputLeft || inputComponent.inputLegacyStrafeLeft)
				{
					val3 += Vector3.get_left() * avgStrafeAcceleration;
				}
			}
			else
			{
				if (inputComponent.inputLegacyStrafeRight)
				{
					val3 += Vector3.get_right() * avgStrafeAcceleration;
				}
				if (inputComponent.inputLegacyStrafeLeft)
				{
					val3 += Vector3.get_left() * avgStrafeAcceleration;
				}
			}
			if (val3.get_sqrMagnitude() < 0.01f && val2.get_sqrMagnitude() > 0.01f)
			{
				Vector3 normalized = val2.get_normalized();
				Vector3 val4 = machineDriftComponent.targetLocalDrift - val2;
				for (int i = 0; i < 3; i += 2)
				{
					Vector3 targetLocalDrift = machineDriftComponent.targetLocalDrift;
					if (Mathf.Sign(targetLocalDrift.get_Item(i)) != Mathf.Sign(val2.get_Item(i)))
					{
						continue;
					}
					Vector3 targetLocalDrift2 = machineDriftComponent.targetLocalDrift;
					float num2 = Mathf.Abs(targetLocalDrift2.get_Item(i));
					float num3 = val2.get_Item(i);
					Vector3 targetLocalDrift3 = machineDriftComponent.targetLocalDrift;
					if (num2 > num3 * Mathf.Sign(targetLocalDrift3.get_Item(i)))
					{
						normalized.set_Item(i, 0f);
						continue;
					}
					float num4 = val4.get_Item(i);
					Vector3 targetLocalDrift4 = machineDriftComponent.targetLocalDrift;
					if (num4 > 0f - Mathf.Abs(targetLocalDrift4.get_Item(i)))
					{
						int num5;
						int num6 = num5 = i;
						float num7 = normalized.get_Item(num5);
						float num8 = Mathf.Abs(val4.get_Item(i));
						Vector3 targetLocalDrift5 = machineDriftComponent.targetLocalDrift;
						normalized.set_Item(num6, num7 * (num8 / Mathf.Abs(targetLocalDrift5.get_Item(i))));
					}
				}
				val3 = -normalized * avgStrafeAcceleration;
				num = machineNode.massComponent.mass;
			}
			if (val3.get_sqrMagnitude() > avgStrafeAcceleration * avgStrafeAcceleration)
			{
				val3 = val3.get_normalized() * avgStrafeAcceleration;
			}
			Vector3 val5 = Quaternion.Inverse(val) * val3;
			Vector3 val6 = ceilingHeightManager.ApplyMaxCeilingToForce(val5 * num, machineNode.robotHeightComponent.robotHeight);
			_compositeForce += val6;
		}

		private void ApplyHoverSteerTorque(LocalMachineRotorNode machineNode, float deltaTime)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			if (_localHoverRotors.get_Count() > 1)
			{
				IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
				Rigidbody rb = machineNode.rigidbodyComponent.rb;
				Transform transform = rb.get_transform();
				Vector3 up = transform.get_up();
				float num = machineNode.massComponent.modifiedMass;
				float num2 = Vector3.Dot(rb.get_angularVelocity(), up);
				float num3 = 0f;
				if (!strafeDirectionManager.strafingEnabled)
				{
					num3 += CalculateLegacyRotationAcceleration(machineNode.inputComponent, averageMovementValuesComponent.avgTurnAcceleration);
				}
				else
				{
					float angleToStraight = strafeDirectionManager.angleToStraight;
					num3 += CalculateStrafeRotationAcceleration(angleToStraight, averageMovementValuesComponent.avgTurnAcceleration);
				}
				if (Mathf.Abs(num2 + num3 * deltaTime) > averageMovementValuesComponent.avgTurnMaxRate)
				{
					num3 = averageMovementValuesComponent.avgTurnMaxRate * Mathf.Sign(num2) - num2;
				}
				if (num3 == 0f && Mathf.Abs(num2) > 0.01f)
				{
					num3 = (0f - Mathf.Min(averageMovementValuesComponent.avgTurnAcceleration, Mathf.Abs(num2))) * Mathf.Sign(num2);
					num = machineNode.massComponent.mass;
				}
				Vector3 val = up * num3;
				Vector3 val2 = val * averageMovementValuesComponent.avgHoverRadiusSqr * num;
				_compositeTorque += val2;
			}
		}

		private float CalculateLegacyRotationAcceleration(IRotorInputComponent inputComponent, float avgTurnAcceleration)
		{
			float num = 0f;
			if (inputComponent.inputRight)
			{
				num += avgTurnAcceleration;
			}
			if (inputComponent.inputLeft)
			{
				num -= avgTurnAcceleration;
			}
			return num;
		}

		private float CalculateStrafeRotationAcceleration(float angleToStraight, float avgTurnAcceleration)
		{
			float num = 0f;
			if (strafeDirectionManager.IsAngleToStraightGreaterThanThreshold())
			{
				if (angleToStraight < 0f)
				{
					num += avgTurnAcceleration;
				}
				else if (angleToStraight > 0f)
				{
					num -= avgTurnAcceleration;
				}
			}
			return num;
		}

		private void ApplyHoverSteerForce(LocalMachineRotorNode machineNode, float deltaTime)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			if (_localHoverRotors.get_Count() <= 1)
			{
				return;
			}
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			Rigidbody rb = machineNode.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			Vector3 localCOM = machineNode.localCOMComponent.localCOM;
			float y = localCOM.y;
			Quaternion rotation = transform.get_rotation();
			for (int i = 0; i < _localHoverRotors.get_Count(); i++)
			{
				RotorBladeNode rotorBladeNode = _localHoverRotors.get_Item(i);
				Vector3 val = rotorBladeNode.transformComponent.T.get_localPosition() - machineNode.localCOMComponent.localCOM;
				val.y = 0f;
				Vector3 val2 = Vector3.Cross(transform.get_up(), val);
				Vector3 normalized = val2.get_normalized();
				Vector3 val3 = Vector3.get_zero();
				if (inputComponent.inputRight)
				{
					val3 += normalized;
				}
				if (inputComponent.inputLeft)
				{
					val3 += -normalized;
				}
				RotorData rotorData = rotorBladeNode.rotorDataComponent.rotorData;
				if (val3.get_sqrMagnitude() < 0.01f)
				{
					float num = Vector3.Dot(transform.get_rotation() * normalized, rotorData.worldVel);
					if (Mathf.Abs(num) > 0.1f)
					{
						val3 += normalized * Mathf.Min(Mathf.Abs(num), 1f) * (0f - Mathf.Sign(num));
						if (Mathf.Abs(num) < 1f)
						{
							val3 *= Mathf.Abs(num) * deltaTime;
						}
					}
				}
				float num2 = Vector3.Dot(val3, rotorData.localVel);
				float num3 = averageMovementValuesComponent.avgTurnTangentalMaxSpeed - num2;
				float num4 = Mathf.Sign(num3);
				Vector3 val4 = val3 * num4 * Mathf.Min(averageMovementValuesComponent.avgTurnTangentalAcceleration, Mathf.Abs(num3 / deltaTime));
				if (inputComponent.inputForward && val4.z < 0f)
				{
					val4.z = 0f;
				}
				if (inputComponent.inputBack && val4.z > 0f)
				{
					val4.z = 0f;
				}
				Vector3 val5 = rotation * val4;
				Vector3 val6 = ceilingHeightManager.ApplyMaxCeilingToForce(val5 * machineNode.massComponent.modifiedMass, machineNode.robotHeightComponent.robotHeight);
				Vector3 val7 = rotorBladeNode.transformComponent.T.get_localPosition();
				val7.y = y;
				if (val.get_sqrMagnitude() < 0.36f)
				{
					val7 = machineNode.localCOMComponent.localCOM + val.get_normalized() * 0.6f;
				}
				Vector3 val8 = transform.get_position() + rotation * val7;
				rb.AddForceAtPosition(val6, val8);
			}
		}

		private void ApplyLevelingTorque(LocalMachineRotorNode machineNode, float deltaTime)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			IAverageMovementValuesComponent averageMovementValuesComponent = machineNode.averageMovementValuesComponent;
			Rigidbody rb = machineNode.rigidbodyComponent.rb;
			Transform transform = rb.get_transform();
			Quaternion rotation = transform.get_rotation();
			Vector3 val = Vector3.get_zero();
			Quaternion localRotation = transform.get_localRotation();
			Vector3 val2 = localRotation.get_eulerAngles();
			val2.y = 0f;
			if (val2.x > 180f)
			{
				val2.x -= 360f;
			}
			if (val2.z > 180f)
			{
				val2.z -= 360f;
			}
			val2 *= (float)Math.PI / 180f;
			Vector3 val3 = Quaternion.Inverse(rotation) * rb.get_angularVelocity();
			val3.y = 0f;
			Vector3 val4 = CalculateLevelingDistanceToOrientated(machineNode, val2);
			float num = val3.get_magnitude() / averageMovementValuesComponent.avgLevelAcceleration;
			Vector3 val5 = val3;
			for (int i = 0; i < 3; i += 2)
			{
				int num2;
				val5.set_Item(num2 = i, val5.get_Item(num2) * ((num + 1f) * 0.5f));
				float num3 = Mathf.Abs(val3.get_Item(i));
				float num4 = Mathf.Abs(val4.get_Item(i));
				if (num4 > 0.01f || num3 > 0.01f)
				{
					if (num3 > averageMovementValuesComponent.avgLevelRate)
					{
						val.set_Item(i, 0f - val3.get_Item(i));
					}
					else if (Mathf.Abs(val5.get_Item(i)) > num4)
					{
						val.set_Item(i, 0f - val3.get_Item(i));
					}
					else
					{
						val.set_Item(i, Mathf.Min(averageMovementValuesComponent.avgLevelAcceleration, num4 / deltaTime) * Mathf.Sign(val4.get_Item(i)));
					}
				}
			}
			float avgLevelAcceleration = averageMovementValuesComponent.avgLevelAcceleration;
			if (val.get_sqrMagnitude() > avgLevelAcceleration * avgLevelAcceleration)
			{
				val = val.get_normalized() * avgLevelAcceleration;
			}
			Vector3 val6 = rotation * val;
			Vector3 val7 = ceilingHeightManager.ApplyMaxCeilingToForce(val6 * averageMovementValuesComponent.avgHoverRadiusSqr * machineNode.massComponent.modifiedMass, machineNode.robotHeightComponent.robotHeight);
			_compositeTorque += val7;
		}

		private Vector3 CalculateLevelingDistanceToOrientated(LocalMachineRotorNode machineNode, Vector3 localRotation)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (machineNode.ownerComponent.ownedByMe && strafeDirectionManager.strafingEnabled && strafeDirectionManager.verticalStrafingEnabled)
			{
				Vector3 targetLocalTilt = machineNode.machineTiltComponent.targetLocalTilt;
				targetLocalTilt.x += strafeDirectionManager.angleToHorizontal * ((float)Math.PI / 180f);
				return targetLocalTilt - localRotation;
			}
			return machineNode.machineTiltComponent.targetLocalTilt - localRotation;
		}

		private bool ShouldCubeThrust(LocalMachineRotorNode machineNode, CubeFace facing, out float direc)
		{
			IRotorInputComponent inputComponent = machineNode.inputComponent;
			bool flag = false;
			bool flag2 = false;
			switch (facing)
			{
			case CubeFace.Up:
				flag = inputComponent.inputRise;
				flag2 = inputComponent.inputLower;
				break;
			case CubeFace.Down:
				flag = inputComponent.inputLower;
				flag2 = inputComponent.inputRise;
				break;
			case CubeFace.Front:
				flag = inputComponent.inputForward;
				flag2 = inputComponent.inputBack;
				break;
			case CubeFace.Back:
				flag = inputComponent.inputBack;
				flag2 = inputComponent.inputForward;
				break;
			case CubeFace.Right:
				flag = inputComponent.inputRight;
				flag2 = inputComponent.inputLeft;
				break;
			case CubeFace.Left:
				flag = inputComponent.inputLeft;
				flag2 = inputComponent.inputRight;
				break;
			}
			if (flag)
			{
				direc = 1f;
				return true;
			}
			if (flag2)
			{
				direc = -1f;
				return true;
			}
			direc = 1f;
			return false;
		}

		private bool ShouldCubeBrake(Rigidbody rb, RotorBladeNode rotor, Vector3 rotorDirec, out float direcScale)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(rotor.rotorDataComponent.rotorData.worldVel, rotorDirec);
			if (Mathf.Abs(num) > 0.01f)
			{
				direcScale = 0f - Mathf.Sign(num);
				return true;
			}
			Vector3 val = Quaternion.Inverse(rb.get_transform().get_rotation()) * rb.get_angularVelocity();
			if (Mathf.Abs(val.y) > 0.01f)
			{
				direcScale = 0f - Mathf.Sign(val.y);
				return true;
			}
			direcScale = 1f;
			return false;
		}

		public void Ready()
		{
		}
	}
}
