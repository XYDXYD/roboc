using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal sealed class SkiGraphicsEngine : SingleEntityViewEngine<SkiGraphicsNode>, IQueryingEntityViewEngine, IEngine
	{
		private ITaskRoutine _fixedTask;

		private ITaskRoutine _task;

		private int _skisCount;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public SkiGraphicsEngine()
		{
			_fixedTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetScheduler(StandardSchedulers.get_physicScheduler())
				.SetEnumeratorProvider((Func<IEnumerator>)FixedUpdate);
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(SkiGraphicsNode node)
		{
			if (_skisCount++ == 0)
			{
				_fixedTask.Start((Action<PausableTaskException>)null, (Action)null);
				_task.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		protected override void Remove(SkiGraphicsNode node)
		{
			if (--_skisCount == 0)
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
				FasterReadOnlyList<SkiGraphicsNode> nodes = entityViewsDB.QueryEntityViews<SkiGraphicsNode>();
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					SkiGraphicsNode skiGraphicsNode = nodes.get_Item(i);
					if (!skiGraphicsNode.hardwareDisabledComponent.disabled && (!skiGraphicsNode.visibilityComponent.offScreen || skiGraphicsNode.visibilityComponent.inRange))
					{
						UpdateGrounded(skiGraphicsNode);
						UpdateSpeed(skiGraphicsNode, Time.get_fixedDeltaTime());
						UpdateRotation(skiGraphicsNode);
					}
				}
				yield return null;
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				FasterReadOnlyList<SkiGraphicsNode> nodes = entityViewsDB.QueryEntityViews<SkiGraphicsNode>();
				for (int i = 0; i < nodes.get_Count(); i++)
				{
					SkiGraphicsNode skiGraphicsNode = nodes.get_Item(i);
					if (!skiGraphicsNode.hardwareDisabledComponent.disabled && !skiGraphicsNode.visibilityComponent.offScreen)
					{
						UpdateGFX(skiGraphicsNode, Time.get_deltaTime());
					}
				}
				yield return null;
			}
		}

		private void UpdateGrounded(SkiGraphicsNode node)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			ISkiRaycastComponent raycastComponent = node.raycastComponent;
			Transform t = node.transformComponent.T;
			Vector3 up = t.get_up();
			float raycastOffset = raycastComponent.raycastOffset;
			Vector3 val = t.get_forward() * raycastOffset;
			float raycastBackTrack = raycastComponent.raycastBackTrack;
			Vector3 val2 = raycastComponent.raycastPoint.get_position() - raycastBackTrack * up;
			float raycastDist = raycastComponent.raycastDist;
			RaycastHit val3 = default(RaycastHit);
			bool flag = Physics.Raycast(val2 + val, up, ref val3, raycastDist + raycastBackTrack, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
			RaycastHit val4 = default(RaycastHit);
			bool flag2 = Physics.Raycast(val2 - val, up, ref val4, raycastDist + raycastBackTrack, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
			float num = raycastDist;
			float num2 = raycastDist;
			if (flag)
			{
				num = Mathf.Clamp(val3.get_distance() - raycastBackTrack, 0f, raycastDist);
			}
			if (flag2)
			{
				num2 = Mathf.Clamp(val4.get_distance() - raycastBackTrack, 0f, raycastDist);
			}
			float num3 = (num + num2) * 0.5f;
			ISkiSuspensionComponent suspensionComponent = node.suspensionComponent;
			float num4 = Mathf.Clamp01(num3 / suspensionComponent.travel);
			node.suspensionComponent.localPosition = Vector3.Lerp(suspensionComponent.compressedOffset, suspensionComponent.extendedOffset, num4);
			float num5 = Mathf.Atan2(num - num2, 2f * raycastOffset);
			node.hingeComponent.currentRotation = Quaternion.AngleAxis(num5 * 57.29578f, Vector3.get_right());
			node.groundedComponent.grounded = (flag || flag2);
		}

		private void UpdateRotation(SkiGraphicsNode node)
		{
			if (!node.ownerComponent.ownedByAi && !node.ownerComponent.ownedByMe)
			{
				UpdateVelocityBasedRotation(node);
			}
		}

		private void UpdateSpeed(SkiGraphicsNode node, float deltaTime)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			Transform t = node.transformComponent.T;
			Vector3 val = (t.get_position() - node.previousStateComponent.prevPosition) / deltaTime;
			node.previousStateComponent.prevPosition = t.get_position();
			node.speedComponent.currentSpeed = Vector3.Dot(val, node.rigidbodyComponent.rb.get_transform().get_forward());
		}

		private void UpdateVelocityBasedRotation(SkiGraphicsNode node)
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
			if (node.speedComponent.currentSpeed < 0f)
			{
				num = 0f - num;
			}
			float maxSteeringAngle = node.steeringComponent.maxSteeringAngle;
			num = Mathf.Clamp(num * node.steeringComponent.steeringSpeed * 0.1f, 0f - maxSteeringAngle, maxSteeringAngle);
			node.steeringComponent.currentSteeringAngle = num;
			node.previousStateComponent.prevRotation = rotation;
		}

		private void UpdateGFX(SkiGraphicsNode node, float deltaTime)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			Transform suspensionTransform = node.graphicsTransformComponent.suspensionTransform;
			Vector3 localPosition = node.suspensionComponent.localPosition;
			suspensionTransform.set_localPosition(Vector3.Lerp(suspensionTransform.get_localPosition(), localPosition, 0.5f));
			Transform hingeTransform = node.graphicsTransformComponent.hingeTransform;
			Quaternion currentRotation = node.hingeComponent.currentRotation;
			hingeTransform.set_localRotation(Quaternion.Slerp(hingeTransform.get_localRotation(), currentRotation, 0.5f));
			Transform steeringNodeTransform = node.graphicsTransformComponent.steeringNodeTransform;
			if (steeringNodeTransform != null)
			{
				Quaternion val = Quaternion.AngleAxis(node.steeringComponent.currentSteeringAngle, Vector3.get_up());
				steeringNodeTransform.set_localRotation(Quaternion.Slerp(steeringNodeTransform.get_localRotation(), val, 0.5f));
			}
		}
	}
}
