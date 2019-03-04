using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class FollowPathTask : Action
	{
		public SharedPathFollowingState PathFollowingStatusShared;

		public SharedPathCalculationState PathCalculationStateShared;

		public SharedPointArray Path;

		public SharedAIBehaviorTreeAgentNode agent;

		public SharedAIInputWrapper aiInputWrapperShared;

		public SharedFloat movementCommandValue;

		public SharedVector3 currentSteeringGoal;

		public SharedBool willingToMove;

		private int _currentPathIndex;

		private float SIN_5_DEG = 0.0871557444f;

		private AIAgentDataComponentsNode _agent;

		public FollowPathTask()
			: this()
		{
		}

		public override void OnAwake()
		{
			PathFollowingStatusShared.set_Value(PathFollowingState.Start);
			willingToMove.set_Value(true);
			this.OnAwake();
		}

		public override void OnStart()
		{
			_agent = agent.get_Value();
			_currentPathIndex = 0;
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			if (PathFollowingStatusShared.get_Value() == PathFollowingState.Start)
			{
				_currentPathIndex = 0;
				PathFollowingStatusShared.set_Value(PathFollowingState.Running);
			}
			willingToMove.set_Value(true);
			Vector3[] value = Path.get_Value();
			Vector3 position = _agent.aiMovementData.position;
			bool flag = false;
			int num = value.Length - 1;
			while (num >= _currentPathIndex && !flag)
			{
				NavMeshHit val = default(NavMeshHit);
				if (!NavMesh.Raycast(position, value[num], ref val, -1))
				{
					_currentPathIndex = num;
					flag = true;
				}
				num--;
			}
			Vector3 val2 = value[_currentPathIndex];
			float num2 = _agent.aiMovementData.horizontalRadius * 2f;
			if (Vector3.Distance(position, value[_currentPathIndex]) <= num2)
			{
				if (_currentPathIndex < value.Length - 1)
				{
					_currentPathIndex++;
					val2 = value[_currentPathIndex];
				}
				else
				{
					PathFollowingStatusShared.set_Value(PathFollowingState.Complete);
					_currentPathIndex = 0;
				}
			}
			if (PathFollowingStatusShared.get_Value() != PathFollowingState.Complete)
			{
				if (!flag)
				{
					PathFollowingStatusShared.set_Value(PathFollowingState.Failed);
					_currentPathIndex = 0;
				}
				else
				{
					currentSteeringGoal.set_Value(val2);
					UpdateSteering(val2);
					PathFollowingStatusShared.set_Value(PathFollowingState.Running);
				}
			}
			return 3;
		}

		public override void OnEnd()
		{
			aiInputWrapperShared.get_Value().forwardAxis = 0f;
			aiInputWrapperShared.get_Value().horizontalAxis = 0f;
			movementCommandValue.set_Value(0f);
			this.OnEnd();
		}

		private void UpdateSteering(Vector3 targetPosition)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _agent.aiMovementData.position;
			Vector3 val = targetPosition - position;
			Vector3 val2 = val;
			val2.y = 0f;
			val2.Normalize();
			Vector3 forward = _agent.aiMovementData.rigidBody.get_transform().get_forward();
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
			if (AIMovementUtils.IsSliding(_agent.aiMovementData.rigidBody))
			{
				willingToMove.set_Value(false);
				aiInputWrapperShared.get_Value().forwardAxis = 0f;
			}
			else
			{
				aiInputWrapperShared.get_Value().forwardAxis = 1f;
			}
			movementCommandValue.set_Value(1f);
		}
	}
}
