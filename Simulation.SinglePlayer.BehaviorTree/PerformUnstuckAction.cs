using BehaviorDesigner.Runtime.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class PerformUnstuckAction : Action
	{
		public SharedGetUnstackState getUnstuckState;

		public SharedAIBehaviorTreeAgentNode agent;

		private const float BAKED_AGENT_HEIGHT = 3f;

		private Vector3 _goalPosition;

		private AIAgentDataComponentsNode _agent;

		public PerformUnstuckAction()
			: this()
		{
		}

		public override void OnAwake()
		{
			this.OnAwake();
		}

		public override void OnStart()
		{
			_agent = agent.get_Value();
			FindReachablePlace();
			_agent.aiAlignmentRectifierControlComponent.alignmentRectifierComplete.subscribers += HandleAlignmentRectifierComplete;
			_agent.aiAlignmentRectifierControlComponent.alignmentRectifierStarted.subscribers += HandleAlignmentRectifierStarted;
			_agent.aiInputWrapper.pulseAR = true;
			this.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _agent.aiMovementData.position;
			if (Vector3.Dot(_agent.aiMovementData.rigidBody.get_transform().get_up(), Vector3.get_up()) > 0.5f)
			{
				Vector3 forward = _agent.aiMovementData.rigidBody.get_transform().get_forward();
				forward.y = 0f;
				forward.Normalize();
				Vector3 val = _goalPosition - position;
				val.y = 0f;
				val.Normalize();
				float num = AIMovementUtils.ComputeHorizontalrotationError(val, forward);
				if (Mathf.Abs(num) > 0.0871557444f)
				{
					_agent.aiInputWrapper.horizontalAxis = Mathf.Sign(num);
				}
				else
				{
					_agent.aiInputWrapper.horizontalAxis = 0f;
				}
				float num2 = Vector3.Dot(val, forward);
				if (Math.Abs(num2) > 0.5f)
				{
					_agent.aiInputWrapper.forwardAxis = Mathf.Sign(num2);
				}
			}
			if (getUnstuckState.get_Value() != GetUnstuckState.Finished)
			{
				return 3;
			}
			return 2;
		}

		public override void OnEnd()
		{
			_agent.aiAlignmentRectifierControlComponent.alignmentRectifierComplete.subscribers -= HandleAlignmentRectifierComplete;
			_agent.aiAlignmentRectifierControlComponent.alignmentRectifierStarted.subscribers -= HandleAlignmentRectifierStarted;
			_agent.aiInputWrapper.pulseAR = false;
			_agent.aiInputWrapper.forwardAxis = 0f;
			_agent.aiInputWrapper.horizontalAxis = 0f;
			this.OnEnd();
		}

		private bool FindReachablePlace()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _agent.aiMovementData.position;
			float num = Math.Max(3f, _agent.aiMovementData.horizontalRadius);
			NavMeshHit val = default(NavMeshHit);
			if (NavMesh.SamplePosition(position, ref val, num, -1))
			{
				Vector3 val2 = val.get_position() - position;
				val2.y = 0f;
				val2.Normalize();
				val2 *= _agent.aiMovementData.horizontalRadius * 2f;
				_goalPosition = val.get_position() + val2;
				if (NavMesh.Raycast(position, _goalPosition, ref val, -1))
				{
					_goalPosition = val.get_position();
				}
				return true;
			}
			_goalPosition = position;
			return false;
		}

		private void HandleAlignmentRectifierStarted(IAIAlignmentRectifierControlComponent agentComponent, int agentNodeId)
		{
			_agent.aiInputWrapper.pulseAR = false;
			getUnstuckState.set_Value(GetUnstuckState.Running);
		}

		private void HandleAlignmentRectifierComplete(IAIAlignmentRectifierControlComponent agentComponent, float timeStamp)
		{
			getUnstuckState.set_Value(GetUnstuckState.Finished);
		}
	}
}
