using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal class WheeledMachineEngine : MultiEntityViewsEngine<WheelNode, WheeledMachineNode>, IQueryingEntityViewEngine, IEngine
	{
		private struct ForceStruct
		{
			public Vector3 forcePoint;

			public Vector3 force;

			public Rigidbody rigidbody;

			public ForceMode forceMode;

			public ForceStruct(Rigidbody rb, Vector3 force, ForceMode forceMode)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				this.force = force;
				forcePoint = Vector3.get_zero();
				rigidbody = rb;
				this.forceMode = forceMode;
			}

			public ForceStruct(Rigidbody rb, Vector3 force, Vector3 forcePoint, ForceMode forceMode)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				this = new ForceStruct(rb, force, forceMode);
				this.forcePoint = forcePoint;
			}
		}

		private struct WheelValues
		{
			public float motorTorque;

			public float stiffness;

			public WheelCollider wheelCollider;

			public WheelValues(float stiffness, float motorTorque, WheelCollider wheelCollider)
			{
				this.stiffness = stiffness;
				this.motorTorque = motorTorque;
				this.wheelCollider = wheelCollider;
			}
		}

		private struct TorqueStruct
		{
			public Rigidbody rigidbody;

			public Vector3 torque;

			public TorqueStruct(Rigidbody rigidbody, Vector3 torque)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				this.rigidbody = rigidbody;
				this.torque = torque;
			}
		}

		private struct SteerValueStruct
		{
			public WheelCollider collider;

			public float currentSteeringAngle;

			public SteerValueStruct(WheelCollider collider, float currentSteeringAngle)
			{
				this.collider = collider;
				this.currentSteeringAngle = currentSteeringAngle;
			}
		}

		private class WheelDistanceComparer : IComparer<WheelNode>
		{
			public int Compare(WheelNode x, WheelNode y)
			{
				if (x.comDistanceComponent.distanceFactor < y.comDistanceComponent.distanceFactor)
				{
					return -1;
				}
				if (x.comDistanceComponent.distanceFactor > y.comDistanceComponent.distanceFactor)
				{
					return 1;
				}
				return 0;
			}
		}

		private const float MIN_FORCE = 0.001f;

		private const float NO_INPUT_THRESHOLD = 0.01f;

		private const float STOPPED_SPEED_THRESHOLD = 0.2f;

		private const float SIDEWAYS_SLIP_THRESHOLD = 0.2f;

		private const float DELTA_VERTICAL_SPEED_TRESHOLD = 0.5f;

		private FasterList<WheelNode> _sortedWheelsPerMachinePerFrame;

		private ThreadSafeDictionary<int, FasterList<WheelNode>> _antiRollWheelsPerMachine;

		private static WheelDistanceComparer _wheelsComparer;

		private FasterListThreadSafe<Vector3> _inputVector;

		private FasterList<ForceStruct> _forcesAtPosition;

		private FasterList<ForceStruct> _forces;

		private FasterList<WheelValues> _wheelValues;

		private FasterList<TorqueStruct> _torques;

		private FasterList<SteerValueStruct> _steerValues;

		private int _machineAdded;

		private float _deltaTime;

		private ITaskRoutine _applyComputedForcesTask;

		private ITaskRoutine _safeCopyAndComputePhysicValuesTask;

		private volatile bool _multithreadedComputationDone;

		private bool _valuesAreApplied;

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
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			_sortedWheelsPerMachinePerFrame = new FasterList<WheelNode>(20);
			_antiRollWheelsPerMachine = new ThreadSafeDictionary<int, FasterList<WheelNode>>(20);
			_wheelsComparer = new WheelDistanceComparer();
			_inputVector = new FasterListThreadSafe<Vector3>(new FasterList<Vector3>());
			_forcesAtPosition = new FasterList<ForceStruct>(20);
			_forces = new FasterList<ForceStruct>(20);
			_wheelValues = new FasterList<WheelValues>(20);
			_torques = new FasterList<TorqueStruct>(20);
			_steerValues = new FasterList<SteerValueStruct>(20);
			_safeCopyAndComputePhysicValuesTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update)
				.SetScheduler(StandardSchedulers.get_physicScheduler());
		}

		protected override void Add(WheeledMachineNode node)
		{
			if (_machineAdded++ == 0)
			{
				_safeCopyAndComputePhysicValuesTask.ThreadSafeStart((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(WheeledMachineNode node)
		{
			if (--_machineAdded == 0)
			{
				_safeCopyAndComputePhysicValuesTask.Stop();
			}
		}

		protected override void Add(WheelNode node)
		{
			int machineId = node.ownerComponent.machineId;
			if (!_antiRollWheelsPerMachine.ContainsKey(node.ownerComponent.machineId))
			{
				_antiRollWheelsPerMachine.set_Item(machineId, new FasterList<WheelNode>());
			}
			SetUpdateCachedValuesRequired(machineId);
			node.hardwareDisabledComponent.isPartDisabled.NotifyOnValueSet((Action<int, bool>)HandleMovementPartDestroyed);
		}

		protected override void Remove(WheelNode node)
		{
			int machineId = node.ownerComponent.machineId;
			SetUpdateCachedValuesRequired(machineId);
			node.hardwareDisabledComponent.isPartDisabled.StopNotify((Action<int, bool>)HandleMovementPartDestroyed);
		}

		private void HandleMovementPartDestroyed(int i, bool b)
		{
			WheelNode wheelNode = default(WheelNode);
			if (entityViewsDB.TryQueryEntityView<WheelNode>(i, ref wheelNode))
			{
				int machineId = wheelNode.ownerComponent.machineId;
				SetUpdateCachedValuesRequired(machineId);
				WheelCollider wheelCollider = wheelNode.wheelColliderDataComponent.wheelColliderData.wheelCollider;
				if (wheelCollider != null)
				{
					wheelNode.steeringComponent.currentSteeringAngle = 0f;
					wheelCollider.set_steerAngle(0f);
				}
			}
		}

		private void SetUpdateCachedValuesRequired(int machineId)
		{
			WheeledMachineNode wheeledMachineNode = default(WheeledMachineNode);
			if (entityViewsDB.TryQueryEntityView<WheeledMachineNode>(machineId, ref wheeledMachineNode))
			{
				wheeledMachineNode.cacheUpdateComponent.updateRequired = true;
			}
		}

		public IEnumerator Update()
		{
			while (true)
			{
				_deltaTime = Time.get_fixedDeltaTime();
				ThreadSafeCopy();
				WheelPhysicComputation(_deltaTime);
				ApplyPhysicValues();
				yield return null;
			}
		}

		private void ThreadSafeCopy()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<WheeledMachineNode> val = entityViewsDB.QueryEntityViews<WheeledMachineNode>();
			_inputVector.FastClear();
			for (int i = 0; i < val.get_Count(); i++)
			{
				WheeledMachineNode wheeledMachineNode = val.get_Item(i);
				int count = default(int);
				WheelNode[] wheels = entityViewsDB.QueryGroupedEntityViewsAsArray<WheelNode>(wheeledMachineNode.get_ID(), ref count);
				_inputVector.Add(UpdateInputValues(val.get_Item(i), strafeDirectionManager));
				FetchComponents(wheeledMachineNode, wheels, count);
				ComputGroundWheels(wheeledMachineNode, wheels, count);
			}
		}

		private void ApplyPhysicValues()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _forcesAtPosition.get_Count(); i++)
			{
				ForceStruct forceStruct = _forcesAtPosition.get_Item(i);
				Rigidbody rigidbody = forceStruct.rigidbody;
				if (rigidbody != null)
				{
					rigidbody.AddForceAtPosition(forceStruct.force, forceStruct.forcePoint, forceStruct.forceMode);
				}
			}
			for (int j = 0; j < _forces.get_Count(); j++)
			{
				ForceStruct forceStruct2 = _forces.get_Item(j);
				Rigidbody rigidbody2 = forceStruct2.rigidbody;
				if (rigidbody2 != null)
				{
					rigidbody2.AddForce(forceStruct2.force, forceStruct2.forceMode);
				}
			}
			for (int k = 0; k < _torques.get_Count(); k++)
			{
				TorqueStruct torqueStruct = _torques.get_Item(k);
				Rigidbody rigidbody3 = torqueStruct.rigidbody;
				if (rigidbody3 != null)
				{
					rigidbody3.AddTorque(torqueStruct.torque, 2);
				}
			}
			for (int l = 0; l < _wheelValues.get_Count(); l++)
			{
				WheelValues wheelValues = _wheelValues.get_Item(l);
				WheelCollider wheelCollider = wheelValues.wheelCollider;
				if (wheelCollider != null)
				{
					WheelFrictionCurve sidewaysFriction = wheelCollider.get_sidewaysFriction();
					sidewaysFriction.set_stiffness(wheelValues.stiffness);
					wheelCollider.set_sidewaysFriction(sidewaysFriction);
					wheelCollider.set_motorTorque(wheelValues.motorTorque);
				}
			}
			for (int m = 0; m < _steerValues.get_Count(); m++)
			{
				SteerValueStruct steerValueStruct = _steerValues.get_Item(m);
				WheelCollider collider = steerValueStruct.collider;
				if (collider != null)
				{
					collider.set_steerAngle(steerValueStruct.currentSteeringAngle);
				}
			}
		}

		private static void FetchComponents(WheeledMachineNode node, WheelNode[] wheels, int count)
		{
			Transform t = node.transformComponent.T;
			Vector3 forward = t.get_forward();
			Vector3 right = t.get_right();
			Vector3 localPosition = t.get_localPosition();
			node.transformComponentThreadSafe.TTS = new TransformThreadSafe(forward, right, localPosition);
			Rigidbody rb = node.rigidBodyComponent.rb;
			Vector3 worldCenterOfMass = rb.get_worldCenterOfMass();
			Vector3 angularVelocity = rb.get_angularVelocity();
			Vector3 velocity = rb.get_velocity();
			float mass = rb.get_mass();
			IThreadSafeRigidBodyComponent rigidbodyComponentThreadSafe = node.rigidbodyComponentThreadSafe;
			rigidbodyComponentThreadSafe.RBTS = new RigidbodyThreadSafe(worldCenterOfMass, angularVelocity, velocity, mass);
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheels[i];
				Transform t2 = wheelNode.transformComponent.T;
				Vector3 forward2 = t2.get_forward();
				Vector3 right2 = t2.get_right();
				Vector3 localPosition2 = t2.get_localPosition();
				Vector3 position = t2.get_position();
				wheelNode.transformComponentThreadSafe.TTS = new TransformThreadSafe(forward2, right2, localPosition2, position);
				wheelNode.threadSafeWheelComponent.forcePointComponentTS = wheelNode.forcePointComponent.forcePointTransform.get_position();
			}
		}

		private static void ComputGroundWheels(WheeledMachineNode node, WheelNode[] wheels, int count)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheels[i];
				IWheelColliderDataComponent wheelColliderDataComponent = wheelNode.wheelColliderDataComponent;
				WheelCollider wheelCollider = wheelColliderDataComponent.wheelColliderData.wheelCollider;
				if (!(wheelCollider != null))
				{
					continue;
				}
				WheelColliderDataThreadSafe wheelColliderDataThreadSafe = default(WheelColliderDataThreadSafe);
				Transform transform = wheelCollider.get_transform();
				ISlipComponent slipComponent = wheelNode.slipComponent;
				IGroundedComponent groundedComponent = wheelNode.groundedComponent;
				WheelHit val = default(WheelHit);
				if (wheelCollider.GetGroundHit(ref val))
				{
					Vector3 val2 = wheelColliderDataThreadSafe.inversePos = transform.InverseTransformPoint(val.get_point());
					slipComponent.forwardSlip = val.get_forwardSlip();
					slipComponent.sidewaysSlip = val.get_sidewaysSlip();
					slipComponent.sidewaysDir = val.get_sidewaysDir();
					groundedComponent.hitNormal = val.get_normal();
					groundedComponent.grounded = true;
					num2++;
					if (wheelNode.lateralAccelerationComponent.motorized)
					{
						num++;
					}
				}
				else
				{
					slipComponent.forwardSlip = 0f;
					slipComponent.sidewaysSlip = 0f;
					groundedComponent.grounded = false;
				}
				wheelColliderDataThreadSafe.radius = wheelCollider.get_radius();
				wheelColliderDataThreadSafe.suspensionDistance = wheelCollider.get_suspensionDistance();
				wheelColliderDataThreadSafe.position = transform.get_position();
				wheelColliderDataThreadSafe.steerAngle = wheelCollider.get_steerAngle();
				wheelNode.wheelColliderDataComponent.wheelColliderDataThreadSafe = wheelColliderDataThreadSafe;
			}
			node.numGroundedWheelsComponent.groundedParts = num2;
			node.numGroundedWheelsComponent.groundedMotorizedParts = num;
		}

		private void WheelPhysicComputation(float deltaSec)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			_multithreadedComputationDone = false;
			ThreadUtility.MemoryBarrier();
			_wheelValues.FastClear();
			_forcesAtPosition.FastClear();
			_forces.FastClear();
			_torques.FastClear();
			_steerValues.FastClear();
			FasterReadOnlyList<WheeledMachineNode> val = entityViewsDB.QueryEntityViews<WheeledMachineNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				WheeledMachineNode wheeledMachineNode = val.get_Item(i);
				int num = default(int);
				WheelNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<WheelNode>(wheeledMachineNode.get_ID(), ref num);
				if (num == 0)
				{
					continue;
				}
				if (wheeledMachineNode.cacheUpdateComponent.updateRequired)
				{
					if (wheeledMachineNode.cacheUpdateComponent.updateSteeringRequired)
					{
						SetupSteeringThreadSafe(wheeledMachineNode, array, num, strafeDirectionManager);
					}
					UpdateAvgValuesThreadSafe(wheeledMachineNode, array, num);
					UpdateComDistanceFactorThreadSafe(wheeledMachineNode, array, num, _sortedWheelsPerMachinePerFrame);
					UpdateAntiRollNodesThreadSafe(wheeledMachineNode);  //  给轮子按前后排序
					wheeledMachineNode.cacheUpdateComponent.updateRequired = false;
				}
				UpdateRatiosThreadSafe(wheeledMachineNode);
				Vector3 val2 = _inputVector.get_Item(i);
				ApplySteeringThreadSafe(wheeledMachineNode, array, num, val2.y, deltaSec);
				if (wheeledMachineNode.numGroundedWheelsComponent.groundedMotorizedParts > 0)
				{
					HandleForwardInputThreadSafe(wheeledMachineNode, val2.x, deltaSec, array, num);
				}
				if (wheeledMachineNode.numGroundedWheelsComponent.groundedParts > 0 && wheeledMachineNode.speedComponent.currentSpeed > 0.2f)
				{
					ApplyAntiRollForceThreadSafe(wheeledMachineNode, _antiRollWheelsPerMachine, _forcesAtPosition);
					ApplyAntiSlipForceThreadSafe(wheeledMachineNode, array, num, _forcesAtPosition);
				}
				ApplyVerticalDampingForceThreadSafe(wheeledMachineNode, _torques);
				for (int j = 0; j < num; j++)
				{
					WheelNode wheelNode = array[j];
					if (wheelNode.wheelColliderDataComponent.wheelColliderData.wheelCollider != null 
						&& (wheelNode.ownerComponent.ownedByMe || wheelNode.ownerComponent.ownedByAi) 
						&& !wheelNode.hardwareDisabledComponent.disabled)
					{
						ApplyFrictionThreadSafe(wheelNode, wheeledMachineNode.speedComponent.currentSpeed, _wheelValues);
						RigidbodyThreadSafe rBTS = wheeledMachineNode.rigidbodyComponentThreadSafe.RBTS;
						UpdateForcePointThreadSafe(wheelNode, rBTS); // 修改node.forcePointComponent.forcePoint
						if (wheelNode.lateralAccelerationComponent.motorized)
						{
							ApplyForcesThreadSafe(wheelNode, _forcesAtPosition, _forces);
						}
					}
				}
			}
			_multithreadedComputationDone = true;
			ThreadUtility.MemoryBarrier();
		}

		private static void ApplyVerticalDampingForceThreadSafe(WheeledMachineNode node, FasterList<TorqueStruct> torques)
		{f
			RigidbodyThreadSafe rBTS = node.rigidbodyComponentThreadSafe.RBTS;
			TransformThreadSafe tTS = node.transformComponentThreadSafe.TTS;
			float num = Vector3.Dot(rBTS.angularVelocity, tTS.right);  // 俯仰角速度？
			if (node.numGroundedWheelsComponent.groundedParts > 0)  // 着地

			{
				float num2 = num - node.angualarVelocityComponent.prevRightAngularSpeed;
				if (Mathf.Abs(num2) > 0.5f)
				{
					Vector3 val = tTS.right * (num * node.angualarVelocityComponent.avgAngularDamping);
					torques.Add(new TorqueStruct(node.rigidBodyComponent.rb, -val));
				}
			}
			node.angualarVelocityComponent.prevRightAngularSpeed = num;
		}

		private static void SetupSteeringThreadSafe(WheeledMachineNode node, WheelNode[] wheels, int count, PlayerStrafeDirectionManager strafeDirectionManager)
		{
			bool flag = node.ownerComponent.ownedByAi || (node.ownerComponent.ownedByMe && !strafeDirectionManager.strafingEnabled);
			WheelNode wheelNode = null;
			WheelNode wheelNode2 = null;
			int num = 0;
			while (num < count)
			{
				WheelNode wheelNode3 = wheels[num];
				TransformThreadSafe tTS;
				if (wheelNode3.machineSideComponent.xSide == WheelXSide.right)
				{
					tTS = wheelNode3.transformComponentThreadSafe.TTS;
					if (wheelNode != null)
					{
						TransformThreadSafe tTS2 = wheelNode.transformComponentThreadSafe.TTS;
						float z = tTS.localPosition.z;  //  当前的
						TransformThreadSafe tTS3 = wheelNode2.transformComponentThreadSafe.TTS; // 上一个
						if (z <= tTS2.localPosition.z)  
						{
							if (z >= tTS3.localPosition.z)  // node.z >= cur.z >= node2.z
							{
							}
							else
							{
								wheelNode2 = wheelNode3;
							}
							goto set_value;
						}
					}
					wheelNode = wheelNode3;
					if (wheelNode2 != null)
					{
						float z2 = tTS.localPosition.z;  //  当前的
						TransformThreadSafe tTS3 = wheelNode2.transformComponentThreadSafe.TTS; // 上一个
						if (z2 >= tTS3.localPosition.z)
						{
							goto set_value;
						}
					}
					wheelNode2 = wheelNode3;
					goto set_value;
				}
				goto set_value;
				set_value:
				if (flag)
				{
					wheelNode3.steeringComponent.maxSteeringAngle = wheelNode3.steeringComponent.maxSteeringAngleKeyboard;
					wheelNode3.steeringComponent.maxSteeringReduction = wheelNode3.steeringComponent.maxSteeringReductionKeyboard;
					wheelNode3.steeringComponent.steeringForceMultiplier = 0f;
				}
				num++;
			}
			TransformThreadSafe tTS4 = wheelNode.transformComponentThreadSafe.TTS;  // 最前的轮子
			TransformThreadSafe tTS5 = wheelNode2.transformComponentThreadSafe.TTS;  // 最后的轮子
			float maxSteeringMultiplier = 1f;
			if (wheelNode != wheelNode2 && wheelNode.steeringComponent.steerable != wheelNode2.steeringComponent.steerable)
			{
				float num2 = tTS4.localPosition.z - tTS5.localPosition.z;
				WheelNode wheelNode4 = (!wheelNode.steeringComponent.steerable) ? wheelNode2 : wheelNode;  // 能转
				WheelNode wheelNode5 = (!wheelNode.steeringComponent.steerable) ? wheelNode : wheelNode2;  // 不能转
				float num3 = num2 / Mathf.Tan(wheelNode4.steeringComponent.maxSteeringAngle * ((float)Math.PI / 180f)); // tan(最大转向弧度)
				float num4 = num3 * 0.5f;
				num4 = Mathf.Max(num4, 3f);
				float num5 = (num2 - num4 * Mathf.Tan(wheelNode5.steeringComponent.maxSteeringAngle * ((float)Math.PI / 180f))) / num4;
				float num6 = Mathf.Atan(num5) * 57.29578f;
				maxSteeringMultiplier = num6 / wheelNode4.steeringComponent.maxSteeringAngle;
			}
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode6 = wheels[i];
				if (wheelNode6.steeringComponent.steerable)
				{
					wheelNode6.steeringComponent.maxSteeringMultiplier = maxSteeringMultiplier;
				}
				else
				{
					wheelNode6.steeringComponent.steerable = true;
				}
			}
			node.cacheUpdateComponent.updateSteeringRequired = false;
		}

		private static void ApplyAntiSlipForceThreadSafe(WheeledMachineNode node, WheelNode[] wheels, int count, FasterList<ForceStruct> forcesAtPosition)
		{
			float num = Mathf.Clamp01((float)(node.numGroundedWheelsComponent.groundedParts - 1) / 3f);
			float num2 = 0f;
			Vector3 val = Vector3.get_zero();
			Vector3 val2 = Vector3.get_zero();
			float num3 = 0f;
			Vector3 val3 = Vector3.get_zero();
			Vector3 val4 = Vector3.get_zero();
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheels[i];
				if (!wheelNode.groundedComponent.grounded)
				{
					continue; // 离地不算
				}
				float sidewaysSlip = wheelNode.slipComponent.sidewaysSlip;
				if (Mathf.Abs(sidewaysSlip) > 0.2f)
				{
					if (wheelNode.comDistanceComponent.distanceFactor < 0f)  // 后轮
					{
						num2 += 1f;
						val += wheelNode.forcePointComponent.forcePoint;
						val2 += wheelNode.slipComponent.sidewaysSlip * wheelNode.slipComponent.sidewaysDir;
					}
					else  // 前轮
					{
						num3 += 1f;
						val3 += wheelNode.forcePointComponent.forcePoint;
						val4 += wheelNode.slipComponent.sidewaysSlip * wheelNode.slipComponent.sidewaysDir;
					}
				}
			}
			Rigidbody rb = node.rigidBodyComponent.rb;
			if (num3 > 0f)
			{
				Vector3 force = -val4 * (num / num3);
				Vector3 forcePoint = val3 / num3;
				if (force.get_sqrMagnitude() > 0.001f)
				{
					forcesAtPosition.Add(new ForceStruct(rb, force, forcePoint, 2));
				}
			}
			if (num2 > 0f)
			{
				Vector3 force2 = -val2 * (num / num2);
				Vector3 forcePoint2 = val / num2;
				if (force2.get_sqrMagnitude() > 0.001f)
				{
					forcesAtPosition.Add(new ForceStruct(rb, force2, forcePoint2, 2));
				}
			}
		}

		private static void ApplyAntiRollForceThreadSafe(WheeledMachineNode node, ThreadSafeDictionary<int, FasterList<WheelNode>> antiRollWheelsPerMachine, FasterList<ForceStruct> forcesAtPosition)
		{
			FasterList<WheelNode> val = default(FasterList<WheelNode>);
			if (!antiRollWheelsPerMachine.TryGetValue(node.get_ID(), ref val) || val.get_Count() <= 0)
			{
				return;
			}
			for (int i = 0; i < val.get_Count(); i++)
			{
				WheelNode wheelNode = val.get_Item(i);
				wheelNode.antirollComponent.antiRollForceApplied = false;
				float travel = 1f;
				if (wheelNode.groundedComponent.grounded)  // 轮子离地之后就不防翻了
				{
					WheelColliderDataThreadSafe wheelColliderDataThreadSafe = wheelNode.wheelColliderDataComponent.wheelColliderDataThreadSafe;
					travel = (0f - wheelColliderDataThreadSafe.inversePos.y - wheelColliderDataThreadSafe.radius) / wheelColliderDataThreadSafe.suspensionDistance;
					travel = Mathf.Clamp01(travel); // 弹簧伸长成度，越长travel越大
				}
				wheelNode.wheelColliderDataComponent.travel = travel;
			}
			// 前后各一对轮子。
			for (int j = 0; j < val.get_Count(); j += 2)
			{
				WheelNode leftNode = val.get_Item(j);
				WheelNode rightNode = val.get_Item(j + 1);
				Rigidbody rb = node.rigidBodyComponent.rb;
				RigidbodyThreadSafe rBTS = node.rigidbodyComponentThreadSafe.RBTS;
				if (ApplyAntiRollToAxleThreadSafe(rBTS.mass, rb, leftNode, rightNode, forcesAtPosition))
				{
					break;
				}
			}
			for (int num = val.get_Count() - 1; num >= 0; num -= 2)
			{
				WheelNode wheelNode2 = val.get_Item(num - 1);
				WheelNode wheelNode3 = val.get_Item(num);
				if (wheelNode3.antirollComponent.antiRollForceApplied || wheelNode2.antirollComponent.antiRollForceApplied)
				{
					break;
				}
				Rigidbody rb2 = node.rigidBodyComponent.rb;
				RigidbodyThreadSafe rBTS2 = node.rigidbodyComponentThreadSafe.RBTS;
				if (ApplyAntiRollToAxleThreadSafe(rBTS2.mass, rb2, wheelNode2, wheelNode3, forcesAtPosition))
				{
					break;
				}
			}
		}

		private static bool ApplyAntiRollToAxleThreadSafe(float mass, Rigidbody rb, WheelNode leftNode, WheelNode rightNode, FasterList<ForceStruct> forcesAtPosition)
		{
			float num = (leftNode.wheelColliderDataComponent.travel - rightNode.wheelColliderDataComponent.travel) * mass;
			if (leftNode.groundedComponent.grounded || rightNode.groundedComponent.grounded)
			{
				WheelCollider wheelCollider = leftNode.wheelColliderDataComponent.wheelColliderData.wheelCollider;
				if (wheelCollider != null)
				{
					leftNode.antirollComponent.antiRollForceApplied = true;
					Vector3 val = (!leftNode.groundedComponent.grounded) ? rightNode.groundedComponent.hitNormal : leftNode.groundedComponent.hitNormal;
					Vector3 force = val * ((0f - num) * leftNode.antirollComponent.antirollForce/*10*/);
					WheelColliderDataThreadSafe wheelColliderDataThreadSafe = leftNode.wheelColliderDataComponent.wheelColliderDataThreadSafe;
					Vector3 position = wheelColliderDataThreadSafe.position;
					if (force.get_sqrMagnitude() > 0.001f)
					{
						forcesAtPosition.Add(new ForceStruct(rb, force, position, 0));
					}
				}
				wheelCollider = rightNode.wheelColliderDataComponent.wheelColliderData.wheelCollider;
				if (wheelCollider != null)
				{
					rightNode.antirollComponent.antiRollForceApplied = true;
					Vector3 val2 = (!rightNode.groundedComponent.grounded) ? leftNode.groundedComponent.hitNormal : rightNode.groundedComponent.hitNormal;
					Vector3 force2 = val2 * (num * rightNode.antirollComponent.antirollForce);
					WheelColliderDataThreadSafe wheelColliderDataThreadSafe2 = rightNode.wheelColliderDataComponent.wheelColliderDataThreadSafe;
					Vector3 position2 = wheelColliderDataThreadSafe2.position;
					if (force2.get_sqrMagnitude() > 0.001f)
					{
						forcesAtPosition.Add(new ForceStruct(rb, force2, position2, 0));
					}
				}
			}
			return rightNode.antirollComponent.antiRollForceApplied || leftNode.antirollComponent.antiRollForceApplied;
		}

		private static void UpdateComDistanceFactorThreadSafe(WheeledMachineNode machineNode, WheelNode[] wheels, int count, FasterList<WheelNode> sortedWheels)
		{
			sortedWheels.FastClear();
			RigidbodyThreadSafe rBTS = machineNode.rigidbodyComponentThreadSafe.RBTS;
			Vector3 worldCenterOfMass = rBTS.worldCenterOfMass;
			TransformThreadSafe tTS = machineNode.transformComponentThreadSafe.TTS;
			Vector3 forward = tTS.forward;
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheels[i];
				if (!wheelNode.hardwareDisabledComponent.disabled)
				{
					TransformThreadSafe tTS2 = wheelNode.transformComponentThreadSafe.TTS;
					float distanceFactor = (float)Math.Round(Vector3.Dot(tTS2.position - worldCenterOfMass, forward), 2); // 从前往后排
					wheelNode.comDistanceComponent.distanceFactor = distanceFactor;  // 以此对轮子排序
					sortedWheels.Add(wheelNode);
				}
			}
			sortedWheels.Sort((IComparer<WheelNode>)_wheelsComparer);
		}

		private void UpdateAntiRollNodesThreadSafe(WheeledMachineNode machineNode)
		{
			FasterList<WheelNode> val = default(FasterList<WheelNode>);
			if (!_antiRollWheelsPerMachine.TryGetValue(machineNode.get_ID(), ref val))
			{
				return;
			}
			val.FastClear();
			int num;
			for (num = 0; num < _sortedWheelsPerMachinePerFrame.get_Count(); num++)
			{
				WheelNode wheelNode = _sortedWheelsPerMachinePerFrame.get_Item(num);
				WheelNode wheelNode2;
				WheelNode wheelNode3;
				if (wheelNode.machineSideComponent.xSide == WheelXSide.left)
				{
					wheelNode2 = wheelNode;
					if (num + 1 >= _sortedWheelsPerMachinePerFrame.get_Count())
					{
						break;
					}
					num++;
					wheelNode = _sortedWheelsPerMachinePerFrame.get_Item(num);
					if (wheelNode.machineSideComponent.xSide == WheelXSide.right)
					{
						Byte3 gridPosition = wheelNode.gridLocationComponent.gridPosition;
						byte y = gridPosition.y;
						Byte3 gridPosition2 = wheelNode2.gridLocationComponent.gridPosition;
						if (y == gridPosition2.y && wheelNode.levelComponent.level == wheelNode2.levelComponent.level)
						{
							wheelNode3 = wheelNode;
							goto IL_0173;
						}
					}
					num--;
					continue;
				}
				wheelNode3 = wheelNode;
				if (num + 1 >= _sortedWheelsPerMachinePerFrame.get_Count())
				{
					break;
				}
				num++;
				wheelNode = _sortedWheelsPerMachinePerFrame.get_Item(num);
				if (wheelNode.machineSideComponent.xSide == WheelXSide.left)
				{
					Byte3 gridPosition3 = wheelNode.gridLocationComponent.gridPosition;
					byte y2 = gridPosition3.y;
					Byte3 gridPosition4 = wheelNode3.gridLocationComponent.gridPosition;
					if (y2 == gridPosition4.y && wheelNode.levelComponent.level == wheelNode3.levelComponent.level)
					{
						wheelNode2 = wheelNode;
						goto IL_0173;
					}
				}
				num--;
				continue;
				IL_0173:
				val.Add(wheelNode2);
				val.Add(wheelNode3);
			}
		}

		private void UpdateRatiosThreadSafe(WheeledMachineNode node)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			RigidbodyThreadSafe rBTS = node.rigidbodyComponentThreadSafe.RBTS;
			Vector3 velocity = rBTS.velocity;
			Vector3 val = velocity;
			TransformThreadSafe tTS = node.transformComponentThreadSafe.TTS;
			float num = Vector3.Dot(val, tTS.forward);
			float num2 = Mathf.Abs(num);
			ISpeedComponent speedComponent = node.speedComponent;
			speedComponent.currentSpeed = num2;
			bool flag = num2 < 0.2f;
			speedComponent.movingBackwards = (num < 0f && !flag);
		}

		private static void UpdateAvgValuesThreadSafe(WheeledMachineNode node, WheelNode[] wheelNodes, int count)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheelNodes[i];
				ILateralAccelerationComponent lateralAccelerationComponent = wheelNode.lateralAccelerationComponent;
				if (!wheelNode.hardwareDisabledComponent.disabled && lateralAccelerationComponent.motorized)
				{
					num += lateralAccelerationComponent.acceleration;
					num6 += lateralAccelerationComponent.timeToMaxAcceleration;
					num4 += wheelNode.steeringComponent.steeringForceMultiplier;
					num3 += wheelNode.brakeComponent.brakeForce;
					num5 += wheelNode.angularDampingComponent.angularDamping;
					num2 += 1f;
				}
			}
			if (num2 > 0f)
			{
				float num7 = 1f / num2;
				IAccelerationComponent accelerationComponent = node.accelerationComponent;
				accelerationComponent.avgAcceleration = num * num7;
				node.brakingComponent.avgBrake = num3 * num7;
				accelerationComponent.avgSteeringForceMultiplier = num4 * num7;
				accelerationComponent.avgTimeToMaxAcceleration = num6 * num7;
				node.angualarVelocityComponent.avgAngularDamping = num5 * num7;
				float num8 = Mathf.InverseLerp(4f, 10f, num2) + 1f;
				accelerationComponent.avgSteeringForceMultiplier /= num8;
			}
		}

		private static void HandleForwardInputThreadSafe(WheeledMachineNode node, float x, float deltaSec, WheelNode[] wheels, int count)
		{
			UpdateAccelerationScalerThreadSafe(node, x, deltaSec);
			if (Mathf.Abs(x) > 0.01f)
			{
				ApplyForwardForcesThreadSafe(node, x, wheels, count);
			}
			else
			{
				ApplyBrakeForceThreadSafe(node, deltaSec, wheels, count);
			}
		}

		private static void ApplyBrakeForceThreadSafe(WheeledMachineNode node, float fixedDeltaTime, WheelNode[] wheels, int count)
		{
			IThreadSafeRigidBodyComponent rigidbodyComponentThreadSafe = node.rigidbodyComponentThreadSafe;
			RigidbodyThreadSafe rBTS = rigidbodyComponentThreadSafe.RBTS;
			TransformThreadSafe tTS = node.transformComponentThreadSafe.TTS;
			float num = Vector3.Dot(rBTS.velocity, tTS.forward);
			float num2 = Math.Abs(num);
			int groundedMotorizedParts = node.numGroundedWheelsComponent.groundedMotorizedParts;
			float num3 = node.brakingComponent.avgBrake * fixedDeltaTime * (float)groundedMotorizedParts;
			float num4 = Mathf.Max(num2 - num3, 0f);
			float num5 = (num2 - num4) * (0f - Mathf.Sign(num)) / (float)groundedMotorizedParts;
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheels[i];
				IPendingForceComponent pendingForceComponent = wheelNode.pendingForceComponent;
				pendingForceComponent.pendingForce = Vector3.get_zero();
				pendingForceComponent.pendingVelocityChangeForce = Vector3.get_zero();
				wheelNode.wheelLoadComponent.wheelLoad = 0f;
				IWheelColliderDataComponent wheelColliderDataComponent = wheelNode.wheelColliderDataComponent;
				if (wheelColliderDataComponent.wheelColliderData.wheelCollider != null && !wheelNode.hardwareDisabledComponent.disabled && node.machineRectifierComponent.functionalsEnabled && wheelNode.lateralAccelerationComponent.motorized)
				{
					WheelColliderDataThreadSafe wheelColliderDataThreadSafe = wheelColliderDataComponent.wheelColliderDataThreadSafe;
					Vector3 hitNormal = wheelNode.groundedComponent.hitNormal;
					Vector3 val = Vector3.Cross(tTS.right, hitNormal);
					Vector3 val2 = Quaternion.AngleAxis(wheelColliderDataThreadSafe.steerAngle, hitNormal) * val;
					Vector3 val3 = val2 * num5;
					IPendingForceComponent pendingForceComponent2 = pendingForceComponent;
					pendingForceComponent2.pendingVelocityChangeForce += val3;
				}
			}
		}

		private static void ApplyForwardForcesThreadSafe(WheeledMachineNode node, float inputValue, WheelNode[] wheels, int count)
		{
			RigidbodyThreadSafe rBTS = node.rigidbodyComponentThreadSafe.RBTS;
			TransformThreadSafe tTS = node.transformComponentThreadSafe.TTS;
			IAccelerationComponent accelerationComponent = node.accelerationComponent;
			float curve = InterpollationCurves.GetCurve(accelerationComponent.accelerationScaler, 1f, InterpollationCurve.FastIncrease); // 加速度系数
			// curve增加先快后慢，类似log
			float num = rBTS.mass * accelerationComponent.avgAcceleration; // 力
			float num2 = Math.Sign(inputValue); // 输入
			float currentSpeed = node.speedComponent.currentSpeed; 
			float steeringForceMultiplier = accelerationComponent.avgSteeringForceMultiplier;
			float num3 = (!node.speedComponent.movingBackwards) ? 1f : (-1f);  // 倒退-1，前进1
			if (num3 * num2 < 0f)  // 方向和输入相反
			{
				steeringForceMultiplier = 0f;
			}
			for (int i = 0; i < count; i++)
			{
				WheelNode wheelNode = wheels[i];
				IPendingForceComponent pendingForceComponent = wheelNode.pendingForceComponent;
				pendingForceComponent.pendingForce = Vector3.get_zero();
				pendingForceComponent.pendingVelocityChangeForce = Vector3.get_zero();
				IWheelColliderDataComponent wheelColliderDataComponent = wheelNode.wheelColliderDataComponent;
				if (wheelColliderDataComponent.wheelColliderData.wheelCollider != null 
					&& !wheelNode.hardwareDisabledComponent.disabled 
					&& node.machineRectifierComponent.functionalsEnabled 
					&& wheelNode.lateralAccelerationComponent.motorized 
					&& Math.Abs(wheelNode.slipComponent.sidewaysSlip) < 0.2f)
				{
					float num4 = Mathf.Clamp01(currentSpeed / wheelNode.maxSpeedComponent.maxSpeed);  // 当前速度/轮子的最大速度
					float steeringModifier = GetSteeringModifier(wheelNode, steeringForceMultiplier, num4);  // 转向力系数
					WheelColliderDataThreadSafe wheelColliderDataThreadSafe = wheelColliderDataComponent.wheelColliderDataThreadSafe;
					Vector3 hitNormal = wheelNode.groundedComponent.hitNormal;
					Vector3 val = Vector3.Cross(tTS.right, hitNormal);   // 前向
					Vector3 val2 = Quaternion.AngleAxis(wheelColliderDataThreadSafe.steerAngle, hitNormal) * val;  // 转向后的朝向
					float num5 = wheelNode.downForceComponent.downAngleForce * num4 * num2;
					Quaternion val3 = Quaternion.AngleAxis(num5, Vector3.Cross(hitNormal, val2));  // 上下的转向！！！！！！！！！！！！！！！
					val2 = val3 * val2;
					float num6 = num * inputValue * curve * steeringModifier; // 模
					Vector3 val4 = val2 * num6;  // 方向 x 长度
					IPendingForceComponent pendingForceComponent2 = pendingForceComponent;
					pendingForceComponent2.pendingForce += val4;  // 把力算出来放在轮子里面，待使用
					wheelNode.wheelLoadComponent.wheelLoad = Mathf.Abs(num6 / num);
				}
			}
		}

		private static float GetSteeringModifier(WheelNode node, float steeringForceMultiplier, float speedRatio)
		{
			float result = 1f;
			if (steeringForceMultiplier == 0f)
			{
				return 1f;
			}
			float num = (node.machineSideComponent.zSide != WheelZSide.Rear) ? 1f : (-1f);
			float num2 = node.steeringComponent.currentSteeringAngle * num;  // 
			WheelXSide xSide = node.machineSideComponent.xSide;
			if ((xSide == WheelXSide.right && num2 > 0f) || (xSide == WheelXSide.left && num2 < 0f))
			{
				result = Mathf.Lerp((0f - steeringForceMultiplier) * 0.5f, 1f, speedRatio);
			}
			else if ((xSide == WheelXSide.left && num2 > 0f) || (xSide == WheelXSide.right && num2 < 0f))
			{
				result = Mathf.Lerp(steeringForceMultiplier, 1f, speedRatio);
			}
			return result;
		}

		private static void UpdateAccelerationScalerThreadSafe(WheeledMachineNode node, float inputValue, float deltaSec)  // 算加速系数
		{ 
			IAccelerationComponent accelerationComponent = node.accelerationComponent;
			if (accelerationComponent.avgTimeToMaxAcceleration == 0f) // 到最大加速的时间为0，瞬间到最大
			{
				accelerationComponent.accelerationScaler = 1f;
				return;
			}
			if (node.numGroundedWheelsComponent.groundedMotorizedParts > 0)
			{
				if (Mathf.Abs(inputValue) < 0.01f)
				{
					accelerationComponent.accelerationScaler = Mathf.Clamp01(accelerationComponent.accelerationScaler - deltaSec / accelerationComponent.avgTimeToMaxAcceleration);
				}  // 逐渐变为0
				else if (node.inputHistoryComponent.prevForwardInput * inputValue > 0f)  // 和上一帧输入相同
				{
					accelerationComponent.accelerationScaler = Mathf.Clamp01(accelerationComponent.accelerationScaler + deltaSec / accelerationComponent.avgTimeToMaxAcceleration);
				}
				else  //相反
				{
					accelerationComponent.accelerationScaler *= 0.5f;
				}
			}
			else
			{
				accelerationComponent.accelerationScaler = 0f;
			}
			node.inputHistoryComponent.prevForwardInput = inputValue;
		}

		private void ApplySteeringThreadSafe(WheeledMachineNode managerNode, WheelNode[] wheelNodes, int count, float input, float fixedDeltaTime)
		{
			for (int i = 0; i < count; i++) // 求每个轮子的currentSteeringAngle
			{
				WheelNode wheelNode = wheelNodes[i];
				ISteeringComponent steeringComponent = wheelNode.steeringComponent;
				if (wheelNode.hardwareDisabledComponent.disabled || !steeringComponent.steerable || !managerNode.machineRectifierComponent.functionalsEnabled)
				{
					continue;
				}
				float num = Mathf.Clamp01(managerNode.speedComponent.currentSpeed / wheelNode.maxSpeedComponent.maxSpeed);  // 当前速率
				float maxSteeringReduction = steeringComponent.maxSteeringReduction;
				float num2 = 1f - maxSteeringReduction + maxSteeringReduction * (1f - num);  // 速度最慢时为1，最块是为0.3
				float num3 = steeringComponent.maxSteeringAngle * num2;
				float num4 = (wheelNode.machineSideComponent.zSide != WheelZSide.Rear) ? 1f : (-1f);
				if (Mathf.Abs(input) > 0.01f)  // 转向输入
				{
					steeringComponent.currentSteeringAngle += input * steeringComponent.steeringSpeed * fixedDeltaTime * num4;
					steeringComponent.currentSteeringAngle = Mathf.Clamp(steeringComponent.currentSteeringAngle, 0f - num3, num3);  // 不能超过最大转向角
					IHardwareOwnerComponent ownerComponent = wheelNode.ownerComponent;
					if (strafeDirectionManager.strafingEnabled && ownerComponent.ownedByMe)
					{
						float num5 = Mathf.Abs(managerNode.strafingCustomAngleToStraightComponent.angleToStraight);
						steeringComponent.currentSteeringAngle = Mathf.Clamp(steeringComponent.currentSteeringAngle, 0f - num5, num5);
					}
					else if (ownerComponent.ownedByAi)
					{
						float num6 = num3 * Mathf.Abs(input);
						steeringComponent.currentSteeringAngle = Mathf.Clamp(steeringComponent.currentSteeringAngle, 0f - num6, num6);
					}
					steeringComponent.steeringStraight = false;
				}
				else if (!steeringComponent.steeringStraight)  // ！已经回正
				{
					steeringComponent.currentSteeringAngle -= Mathf.Sign(steeringComponent.currentSteeringAngle) * steeringComponent.steeringSpeed * fixedDeltaTime;
					if (steeringComponent.steeringSpeed * fixedDeltaTime > Mathf.Abs(steeringComponent.currentSteeringAngle))
					{
						steeringComponent.currentSteeringAngle = 0f;
						steeringComponent.steeringStraight = true;
					}
				}
				WheelCollider wheelCollider = wheelNode.wheelColliderDataComponent.wheelColliderData.wheelCollider;
				if (wheelCollider != null)
				{
					_steerValues.Add(new SteerValueStruct(wheelCollider, steeringComponent.currentSteeringAngle));
				}
			}
		}

		private static Vector3 UpdateInputValues(WheeledMachineNode node, PlayerStrafeDirectionManager strafeDirectionManager)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = default(Vector3);
			result._002Ector(node.strafingCustomInputComponent.forwardInput, node.strafingCustomInputComponent.turningInput, node.strafingCustomInputComponent.strafingInput);
			if (strafeDirectionManager.strafingEnabled && node.ownerComponent.ownedByMe)
			{
				result.y = CalculateCameraRelativeTurningInputVector(node.strafingCustomAngleToStraightComponent.angleToStraight, node.speedComponent.movingBackwards);
			}
			return result;
		}

		private static float CalculateCameraRelativeTurningInputVector(float angleToStraight, bool movingBackwards)
		{
			float num = 0f;
			float num2 = Mathf.Abs(angleToStraight);
			float num3 = 45f;
			if (Mathf.Abs(angleToStraight) > 5f)
			{
				if (angleToStraight < 0f)
				{
					num = 1f;
				}
				else if (angleToStraight > 0f)
				{
					num = -1f;
				}
				float num4 = Mathf.Clamp01(num2 / num3);
				num *= num4;
				if (movingBackwards)
				{
					num *= -1f;
				}
			}
			return num;
		}

		private static void ApplyFrictionThreadSafe(WheelNode node, float currentSpeed, FasterList<WheelValues> wheelValues)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 pendingForce = node.pendingForceComponent.pendingForce;
			float motorTorque = (!(pendingForce.get_sqrMagnitude() > 0.001f) && node.lateralAccelerationComponent.motorized) ? 0f : 1E-06f;
			float num = currentSpeed / (node.maxSpeedComponent.maxSpeed * 0.35f);
			num = Mathf.Clamp01(num);
			float stiffness = Mathf.Lerp(node.frictionStiffnessComponent.stoppedFrictionStiffness, node.frictionStiffnessComponent.movingFrictionStiffness, num);
			WheelColliderData wheelColliderData = node.wheelColliderDataComponent.wheelColliderData;
			WheelCollider wheelCollider = wheelColliderData.wheelCollider;
			wheelValues.Add(new WheelValues(stiffness, motorTorque, wheelCollider));
		}

		private static void UpdateForcePointThreadSafe(WheelNode node, RigidbodyThreadSafe rts)
		{
			Vector3 worldCenterOfMass = rts.worldCenterOfMass;
			Vector3 forcePointComponentTS = node.threadSafeWheelComponent.forcePointComponentTS;
			Vector3 val = worldCenterOfMass - forcePointComponentTS;
			Vector3 val2 = default(Vector3);
			val2._002Ector(0f, Mathf.Min(val.y, 1f), 0f);
			node.forcePointComponent.forcePoint = forcePointComponentTS + val2;  // 轮子的施力点在质心的y
		}

		private static void CalculateSlopeScalarThreadSafe(WheelNode node)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			float currentSlopeScalar = 1f;
			IFrictionAngleComponent frictionAngleComponent = node.frictionAngleComponent;
			float num = Vector3.Angle(node.pendingForceComponent.pendingForce, Vector3.get_up());
			if (num < node.frictionAngleComponent.angleWithMinFriction /*75°*/)  // 爬坡
			{
				currentSlopeScalar = frictionAngleComponent.minFrictionScalar;
			} 
			else if (num < node.frictionAngleComponent.angleWithMaxFriction/*40°*/)  // 40到75之间，坡度越大，动力（SlopeScalar）越大，  75以上恢复小动力
			{
				float angleWithMaxFriction = frictionAngleComponent.angleWithMaxFriction;
				float num2 = (num - angleWithMaxFriction) / (frictionAngleComponent.angleWithMinFriction - angleWithMaxFriction);
				currentSlopeScalar = num2 * frictionAngleComponent.minFrictionScalar + (1f - num2) * 1f; 
			}
			node.currentSlopeScalarComponent.currentSlopeScalar = currentSlopeScalar;
		}

		private static void ApplyForcesThreadSafe(WheelNode node, FasterList<ForceStruct> forcesAtPosition, FasterList<ForceStruct> forces)
		{
			if (node.groundedComponent.grounded)
			{
				CalculateSlopeScalarThreadSafe(node);
				Vector3 force = node.pendingForceComponent.pendingForce * node.currentSlopeScalarComponent.currentSlopeScalar;
				Rigidbody rb = node.rigidbodyComponent.rb;
				if (force.get_sqrMagnitude() > 0.001f)
				{
					forcesAtPosition.Add(new ForceStruct(rb, force, node.forcePointComponent.forcePoint, 0));
				}
				Vector3 pendingVelocityChangeForce = node.pendingForceComponent.pendingVelocityChangeForce;  // 刹车力
				if (pendingVelocityChangeForce.get_sqrMagnitude() > 0.001f)
				{
					forces.Add(new ForceStruct(rb, pendingVelocityChangeForce, 2));
				}
			}
		}
	}
}
