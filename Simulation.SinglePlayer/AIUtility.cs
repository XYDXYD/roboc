using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	internal static class AIUtility
	{
		private static readonly Quaternion _right90deg = Quaternion.AngleAxis(90f, Vector3.get_up());

		private static readonly float SIN_5_DEG = 0.0871557444f;

		public static float ComputeHorizontalRotationError(Vector3 horDirection, Vector3 forward)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Cross(forward, horDirection);
			float num = val.y;
			if (Vector3.Dot(forward, horDirection) < 0f)
			{
				num = Mathf.Sign(num);
			}
			return num;
		}

		public static void UpdateSteering(Vector3 targetPosition, AIAgentDataComponentsNode aiAgentDataComponentsNode, SharedAIInputWrapper aiInputWrapperShared, SharedFloat movementCommandValue, bool willingToMove, float forwardSpeed = 1f)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = aiAgentDataComponentsNode.aiMovementData.position;
			Vector3 val = targetPosition - position;
			Vector3 val2 = val;
			val2.y = 0f;
			val2.Normalize();
			Vector3 forward = aiAgentDataComponentsNode.aiMovementData.rigidBody.get_transform().get_forward();
			Vector3 val3 = Vector3.Cross(forward, val2);
			float num = val3.y;
			if (Vector3.Dot(forward, val2) < 0f)
			{
				num = Mathf.Sign(num);
			}
			if (num > SIN_5_DEG)
			{
				aiInputWrapperShared.get_Value().horizontalAxis = 1f;
			}
			else if (num < 0f - SIN_5_DEG)
			{
				aiInputWrapperShared.get_Value().horizontalAxis = -1f;
			}
			else
			{
				aiInputWrapperShared.get_Value().horizontalAxis = 0f;
			}
			if (willingToMove)
			{
				aiInputWrapperShared.get_Value().forwardAxis = forwardSpeed;
			}
			movementCommandValue.set_Value(1f);
		}
	}
}
