using Simulation.Hardware.Movement.TankTracks;
using Simulation.Hardware.Movement.Wheeled;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal class CameraRelativeInputEngine : IPhysicallyTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float TRACKS_FAVOURING_FORWARDS_THRESHOLD = 90f;

		private const float WHEELS_FAVOURING_FORWARDS_THRESHOLD = 145f;

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

		public void PhysicsTick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CameraRelativeInputNode> val = entityViewsDB.QueryEntityViews<CameraRelativeInputNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				CameraRelativeInputNode cameraRelativeInputNode = val.get_Item(i);
				cameraRelativeInputNode.strafingCustomAngleToStraightComponent.customAngleUsed = false;
				cameraRelativeInputNode.strafingCustomAngleToStraightComponent.angleToStraight = 0f;
				if (cameraRelativeInputNode.ownerComponent.ownedByMe || cameraRelativeInputNode.ownerComponent.ownedByAi)
				{
					TankTrackMachineNode tankTrackMachineNode = null;
					entityViewsDB.TryQueryEntityView<TankTrackMachineNode>(cameraRelativeInputNode.get_ID(), ref tankTrackMachineNode);
					bool flag = tankTrackMachineNode != null && tankTrackMachineNode.numGroundedTracksComponent.groundedTracks > 0;
					WheeledMachineNode wheeledMachineNode = null;
					entityViewsDB.TryQueryEntityView<WheeledMachineNode>(cameraRelativeInputNode.get_ID(), ref wheeledMachineNode);
					bool flag2 = wheeledMachineNode != null && wheeledMachineNode.numGroundedWheelsComponent.groundedParts > 0;
					bool flag3 = strafeDirectionManager.strafingEnabled && cameraRelativeInputNode.ownerComponent.ownedByMe && strafeDirectionManager.sidewaysDrivingEnabled;
					bool allowCustomAngle = flag || (flag2 && flag3);
					float favouringForwardThreshold = 145f;
					if (flag)
					{
						favouringForwardThreshold = 90f;
					}
					IMachineControl machineInput = cameraRelativeInputNode.machineControlComponent.machineInput;
					Vector3 val2 = UpdateInputValues(cameraRelativeInputNode, machineInput, allowCustomAngle, favouringForwardThreshold);
					cameraRelativeInputNode.strafingCustomInputComponent.forwardInput = val2.x;
					cameraRelativeInputNode.strafingCustomInputComponent.turningInput = val2.y;
					cameraRelativeInputNode.strafingCustomInputComponent.strafingInput = val2.z;
				}
			}
		}

		private Vector3 UpdateInputValues(CameraRelativeInputNode node, IMachineControl input, bool allowCustomAngle, float favouringForwardThreshold)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			val.x = input.forwardAxis;
			val.y = input.horizontalAxis;
			if (input.strafeLeft)
			{
				val.z -= 1f;
			}
			else if (input.strafeRight)
			{
				val.z += 1f;
			}
			if (strafeDirectionManager.strafingEnabled && node.ownerComponent.ownedByMe)
			{
				val = UpdateStrafingInput(node, val, allowCustomAngle, favouringForwardThreshold);
			}
			else if (node.ownerComponent.ownedByMe)
			{
				val.y = CombineInputValues(val.y, val.z);
			}
			return val;
		}

		private float CombineInputValues(float inputA, float inputB)
		{
			bool flag = inputA > 0f || inputB > 0f;
			bool flag2 = inputA < 0f || inputB < 0f;
			float result = 0f;
			if (flag)
			{
				result = Mathf.Max(inputA, inputB);
			}
			if (flag2)
			{
				result = Mathf.Min(inputA, inputB);
			}
			return result;
		}

		private Vector3 UpdateStrafingInput(CameraRelativeInputNode node, Vector3 inputVector, bool allowCustomAngle, float favouringForwardThreshold)
		{
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			inputVector.z = CombineInputValues(inputVector.y, inputVector.z);
			bool customAngleUsed = false;
			float num = strafeDirectionManager.angleToStraight;
			if (allowCustomAngle && Mathf.Abs(inputVector.z) > 0f)
			{
				float num2 = 90f;
				if (Mathf.Abs(inputVector.x) > 0f)
				{
					num2 *= 0.5f;
				}
				float num3 = Mathf.Max(Mathf.Abs(inputVector.x), Mathf.Abs(inputVector.z));
				if (inputVector.z > 0f)
				{
					if (inputVector.x < 0f)
					{
						num += num2;
						inputVector.x = 0f - num3;
					}
					else
					{
						num -= num2;
						inputVector.x = num3;
					}
				}
				else if (inputVector.x < 0f)
				{
					num -= num2;
					inputVector.x = 0f - num3;
				}
				else
				{
					num += num2;
					inputVector.x = num3;
				}
				customAngleUsed = true;
			}
			node.strafingCustomAngleToStraightComponent.customAngleUsed = customAngleUsed;
			node.strafingCustomAngleToStraightComponent.angleToStraight = num;
			return inputVector;
		}
	}
}
