using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal class WheelGraphicsEngine : SingleEntityViewEngine<WheelGraphicsNode>, IQueryingEntityViewEngine, IEngine
	{
		private ITaskRoutine _fixedTask;

		private ITaskRoutine _task;

		private int _wheelsCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public WheelGraphicsEngine()
		{
			_fixedTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetScheduler(StandardSchedulers.get_physicScheduler())
				.SetEnumeratorProvider((Func<IEnumerator>)FixedUpdate);
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(WheelGraphicsNode node)
		{
			if (_wheelsCount++ == 0)
			{
				_fixedTask.Start((Action<PausableTaskException>)null, (Action)null);
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(WheelGraphicsNode node)
		{
			if (--_wheelsCount == 0)
			{
				_fixedTask.Stop();
				_task.Stop();
			}
		}

		private IEnumerator FixedUpdate()
		{
			yield return null;
			while (true)
			{
				FasterReadOnlyList<WheelGraphicsNode> nodes = entityViewsDB.QueryEntityViews<WheelGraphicsNode>();
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					WheelGraphicsNode wheelGraphicsNode = nodes.get_Item(i);
					if (!wheelGraphicsNode.hardwareDisabledComponent.disabled && (!wheelGraphicsNode.visibilityComponent.offScreen || wheelGraphicsNode.visibilityComponent.inRange))
					{
						UpdateGrounded(wheelGraphicsNode);
						UpdateSpeed(wheelGraphicsNode, Time.get_fixedDeltaTime());
						UpdateRotation(wheelGraphicsNode);
					}
				}
				yield return null;
			}
		}

		private IEnumerator Update()
		{
			yield return null;
			while (true)
			{
				FasterReadOnlyList<WheelGraphicsNode> nodes = entityViewsDB.QueryEntityViews<WheelGraphicsNode>();
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					WheelGraphicsNode wheelGraphicsNode = nodes.get_Item(i);
					if (!wheelGraphicsNode.hardwareDisabledComponent.disabled && !wheelGraphicsNode.visibilityComponent.offScreen)
					{
						UpdateWheelGFX(wheelGraphicsNode, Time.get_deltaTime());
					}
				}
				yield return null;
			}
		}

		private void UpdateGrounded(WheelGraphicsNode node)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			float distanceToGround = node.wheelSuspensionComponent.fullSuspensionDistance;
			RaycastHit val = default(RaycastHit);
			bool flag = Physics.Raycast(t.get_position(), -node.rigidbodyComponent.rb.get_transform().get_up(), ref val, node.wheelSuspensionComponent.fullSuspensionDistance, GameLayers.ENVIRONMENT_LAYER_MASK);
			if (flag)
			{
				distanceToGround = val.get_distance();
			}
			if (!node.ownerComponent.ownedByMe)
			{
				node.groundedComponent.grounded = flag;
			}
			node.groundedComponent.distanceToGround = distanceToGround;
		}

		private void UpdateRotation(WheelGraphicsNode node)
		{
			if (!node.ownerComponent.ownedByAi && !node.ownerComponent.ownedByMe)
			{
				UpdateVelocityBasedRotation(node);
			}
		}

		private void UpdateVelocityBasedRotation(WheelGraphicsNode node)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			Quaternion rotation = node.rigidbodyComponent.rb.get_rotation();
			Quaternion val = rotation * Quaternion.Inverse(node.previousStateComponent.prevRotation);
			Vector3 eulerAngles = val.get_eulerAngles();
			float num = eulerAngles.y;
			if (num > 180f)
			{
				num -= 360f;
			}
			if (node.machineSideComponent.zSide == WheelZSide.Rear)
			{
				num = 0f - num;
			}
			if (node.wheelSpeedComponent.wheelSpeed < 0f)
			{
				num = 0f - num;
			}
			float maxSteeringAngle = node.steeringComponent.maxSteeringAngle;
			num = Mathf.Clamp(num * node.steeringComponent.steeringSpeed * 0.1f, 0f - maxSteeringAngle, maxSteeringAngle);
			node.steeringComponent.currentSteeringAngle = num;
			node.previousStateComponent.prevRotation = rotation;
		}

		private void UpdateSpeed(WheelGraphicsNode node, float deltaTime)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			float num = 0f;
			node.previousStateComponent.prevSpeed = node.wheelSpeedComponent.wheelSpeed;
			if (!node.groundedComponent.grounded)
			{
				float prevSpeed = node.previousStateComponent.prevSpeed;
				float num2 = 0f;
				MachineInputNode machineInputNode = entityViewsDB.QueryEntityView<MachineInputNode>(node.ownerComponent.machineId);
				Vector4 digitalInput = machineInputNode.machineInput.digitalInput;
				float z = digitalInput.z;
				if (z != 0f)
				{
					num2 = node.lateralAccelerationComponent.acceleration * deltaTime * z;
				}
				else
				{
					float num3 = (!(prevSpeed > 0f)) ? 1f : (-1f);
					num2 = Mathf.Min(node.brakeComponent.brakeForce * deltaTime, Mathf.Abs(prevSpeed)) * num3;
				}
				num = prevSpeed + num2;
				float maxSpeed = node.maxSpeedComponent.maxSpeed;
				num = Mathf.Clamp(num, 0f - maxSpeed, maxSpeed);
			}
			else
			{
				Vector3 val = (t.get_position() - node.previousStateComponent.prevPosition) / deltaTime;
				num = Vector3.Dot(val, node.rigidbodyComponent.rb.get_transform().get_forward());
			}
			node.previousStateComponent.prevPosition = t.get_position();
			node.wheelSpeedComponent.wheelSpeed = num;
		}

		private void UpdateWheelGFX(WheelGraphicsNode node, float deltaTime)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			float num = Mathf.Clamp(node.groundedComponent.distanceToGround, 0f, node.wheelSuspensionComponent.fullSuspensionDistance);
			Transform suspensionTransform = node.graphicsTransformComponent.suspensionTransform;
			if (suspensionTransform != null)
			{
				Vector3 position = t.get_position() + t.get_up() * node.wheelSuspensionComponent.wheelForwardOffset - (num - node.radiusComponent.wheelRadius) * node.rigidbodyComponent.rb.get_transform().get_up();
				suspensionTransform.set_position(position);
			}
			Transform steeringNodeTransform = node.graphicsTransformComponent.steeringNodeTransform;
			if (steeringNodeTransform != null)
			{
				Quaternion val = Quaternion.AngleAxis(node.steeringComponent.currentSteeringAngle, Vector3.get_up());
				steeringNodeTransform.set_localRotation(Quaternion.Slerp(steeringNodeTransform.get_localRotation(), val, 0.8f));
			}
			Transform wheelToRotateTransform = node.graphicsTransformComponent.wheelToRotateTransform;
			if (wheelToRotateTransform != null)
			{
				float num2 = Mathf.Sign(Vector3.Dot(t.get_up(), node.rigidbodyComponent.rb.get_transform().get_right()));
				float num3 = node.wheelSpeedComponent.wheelSpeed * node.radiusComponent.inverseCircumference * num2;
				wheelToRotateTransform.Rotate(node.rotationAxisComponent.rotationAxis, num3 * 360f * deltaTime, 1);
			}
		}
	}
}
