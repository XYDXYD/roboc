using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.SinglePlayer
{
	internal static class AIMovementUtils
	{
		public enum MovementDirection
		{
			Any,
			ForceForward,
			ForceReverse
		}

		private struct AiWeaponRanges
		{
			public readonly float MinDesiredRange;

			public readonly float MaxDesiredRange;

			public readonly float MaxFiringRange;

			public AiWeaponRanges(float minDesiredRange, float maxDesiredRange, float maxFiringRange)
			{
				MinDesiredRange = minDesiredRange;
				MaxDesiredRange = maxDesiredRange;
				MaxFiringRange = maxFiringRange;
			}
		}

		public const float SIN_5_DEG = 0.0871557444f;

		public const float COS_5_DEG = 0.9961947f;

		public const float COS_45_DEG = 0.707106769f;

		public const float COS_30_DEG = 0.8660254f;

		private static readonly Dictionary<ItemCategory, AiWeaponRanges> Ranges = new Dictionary<ItemCategory, AiWeaponRanges>
		{
			{
				ItemCategory.Laser,
				new AiWeaponRanges(5f, 35f, 120f)
			},
			{
				ItemCategory.Rail,
				new AiWeaponRanges(40f, 60f, 150f)
			},
			{
				ItemCategory.Plasma,
				new AiWeaponRanges(15f, 25f, 90f)
			},
			{
				ItemCategory.Mortar,
				new AiWeaponRanges(90f, 90f, 120f)
			},
			{
				ItemCategory.Seeker,
				new AiWeaponRanges(10f, 20f, 100f)
			},
			{
				ItemCategory.Ion,
				new AiWeaponRanges(0f, 10f, 30f)
			},
			{
				ItemCategory.Tesla,
				new AiWeaponRanges(0f, 0f, 0f)
			},
			{
				ItemCategory.Aeroflak,
				new AiWeaponRanges(25f, 35f, 120f)
			},
			{
				ItemCategory.Chaingun,
				new AiWeaponRanges(10f, 25f, 90f)
			}
		};

		public static void MoveForwardTowardsTarget(Vector3 agentPosition, Vector3 desiredPosition, AIAgentDataComponentsNode agent)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Vector3 horDirection = desiredPosition - agentPosition;
			horDirection.y = 0f;
			horDirection.Normalize();
			Vector3 forward = agent.aiMovementData.rigidBody.get_transform().get_forward();
			float num = ComputeHorizontalrotationError(horDirection, forward);
			if (num > 0.0871557444f)
			{
				agent.aiInputWrapper.horizontalAxis = 1f;
			}
			else if (num < -0.0871557444f)
			{
				agent.aiInputWrapper.horizontalAxis = -1f;
			}
			else
			{
				agent.aiInputWrapper.horizontalAxis = 0f;
			}
			if (IsSliding(agent.aiMovementData.rigidBody))
			{
				agent.aiInputWrapper.forwardAxis = 0f;
			}
			else
			{
				agent.aiInputWrapper.forwardAxis = 1f;
			}
		}

		public static void MoveToCombatPosition(Vector3 agentPosition, Vector3 desiredPosition, AIAgentDataComponentsNode agent, SharedBool willingToMove, MovementDirection movementDirection = MovementDirection.Any)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = desiredPosition - agentPosition;
			willingToMove.set_Value(val.get_sqrMagnitude() > agent.aiMovementData.horizontalRadius * 2f);
			Vector3 val2 = val;
			val2.y = 0f;
			val2.Normalize();
			Vector3 forward = agent.aiMovementData.rigidBody.get_transform().get_forward();
			Vector3 val3 = Vector3.Cross(forward, val2);
			float y = val3.y;
			float num = Vector3.Dot(val2, forward);
			agent.aiInputWrapper.forwardAxis = 0f;
			agent.aiInputWrapper.horizontalAxis = 0f;
			switch (movementDirection)
			{
			case MovementDirection.ForceForward:
				if (num < 0f)
				{
					agent.aiInputWrapper.horizontalAxis = Mathf.Sign(y);
					agent.aiInputWrapper.forwardAxis = 0f;
				}
				else
				{
					RotateToAlign(agent, y);
					agent.aiInputWrapper.forwardAxis = 1f;
				}
				break;
			case MovementDirection.ForceReverse:
				if (num > 0f)
				{
					agent.aiInputWrapper.horizontalAxis = 0f - Mathf.Sign(y);
					agent.aiInputWrapper.forwardAxis = 0f;
				}
				else
				{
					RotateToAlign(agent, y);
					agent.aiInputWrapper.forwardAxis = -1f;
				}
				break;
			case MovementDirection.Any:
				RotateToAlign(agent, y);
				agent.aiInputWrapper.forwardAxis = Mathf.Sign(num);
				break;
			}
		}

		private static void RotateToAlign(AIAgentDataComponentsNode agent, float angleError)
		{
			if (angleError > 0.0871557444f)
			{
				agent.aiInputWrapper.horizontalAxis = 1f;
			}
			else if (angleError < -0.0871557444f)
			{
				agent.aiInputWrapper.horizontalAxis = -1f;
			}
			else
			{
				agent.aiInputWrapper.horizontalAxis = 0f;
			}
		}

		public static float ComputeHorizontalrotationError(Vector3 horDirection, Vector3 forward)
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

		public static bool IsSliding(Rigidbody rb)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = rb.get_velocity();
			val.y = 0f;
			float magnitude = val.get_magnitude();
			Vector3 forward = rb.get_transform().get_forward();
			forward.y = 0f;
			forward.Normalize();
			bool result = false;
			if (magnitude > 0.25f)
			{
				val /= magnitude;
				if (Mathf.Abs(Vector3.Dot(forward, val)) < 0.8660254f)
				{
					result = true;
				}
			}
			return result;
		}

		public static float CalculateCombatRange(float currentDistance, ItemCategory itemCategory)
		{
			AiWeaponRanges weaponRanges = GetWeaponRanges(itemCategory);
			return Mathf.Clamp(currentDistance, weaponRanges.MinDesiredRange, weaponRanges.MaxDesiredRange);
		}

		public static float GetMaxFiringRange(ItemCategory itemCategory)
		{
			AiWeaponRanges weaponRanges = GetWeaponRanges(itemCategory);
			return weaponRanges.MaxFiringRange;
		}

		private static AiWeaponRanges GetWeaponRanges(ItemCategory itemCategory)
		{
			if (!Ranges.TryGetValue(itemCategory, out AiWeaponRanges value))
			{
				throw new Exception("Unsupported item category: " + itemCategory);
			}
			return value;
		}
	}
}
