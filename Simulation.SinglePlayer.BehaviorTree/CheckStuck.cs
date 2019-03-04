using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class CheckStuck : Conditional
	{
		public SharedAIBehaviorTreeAgentNode agent;

		public SharedGetUnstackState getUnstuckState;

		public SharedVector3 currentSteeringGoal;

		public SharedBool willingToMove;

		private const float POSITION_THRESHOLD = 1f;

		private const float SQ_POSITION_THRESHOLD = 1f;

		private const float ANGLE_THRESHOLD = 15f;

		private const float STUCK_TIME_THRESHOLD = 3f;

		private float _potentiallyStuckTime;

		private Vector3 _lastRecorderPosition;

		private Quaternion _lastRecordedRotation;

		private AIAgentDataComponentsNode _agent;

		public CheckStuck()
			: this()
		{
		}

		public override void OnStart()
		{
			_agent = agent.get_Value();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			TaskStatus result = 1;
			if (getUnstuckState.get_Value() == GetUnstuckState.Running)
			{
				result = 2;
			}
			else
			{
				Vector3 position = _agent.aiMovementData.position;
				if (willingToMove.get_Value())
				{
					_potentiallyStuckTime += Time.get_fixedDeltaTime();
					if (_potentiallyStuckTime > 3f)
					{
						if (Vector3.SqrMagnitude(position - _lastRecorderPosition) < 1f && Quaternion.Angle(_lastRecordedRotation, _agent.aiMovementData.rigidBody.get_rotation()) < 15f)
						{
							ResetPerfCounters();
							_potentiallyStuckTime = 0f;
							getUnstuckState.set_Value(GetUnstuckState.Running);
							result = 2;
						}
						else
						{
							_potentiallyStuckTime -= 3f;
							ResetPerfCounters();
						}
					}
				}
				else
				{
					ResetPerfCounters();
					_potentiallyStuckTime = 0f;
				}
			}
			return result;
		}

		private void ResetPerfCounters()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			_lastRecorderPosition = _agent.aiMovementData.position;
			_lastRecordedRotation = _agent.aiMovementData.rigidBody.get_rotation();
		}
	}
}
