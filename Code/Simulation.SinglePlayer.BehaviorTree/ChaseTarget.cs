using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class ChaseTarget : Action
	{
		public SharedTargetInfoComponent selectedTargetInfo;

		public SharedAIBehaviorTreeAgentNode agent;

		public SharedFloat movementCommandValue;

		public SharedVector3 currentSteeringGoal;

		public SharedBool willingToMove;

		private TargetInfo _targetInfo;

		private AIAgentDataComponentsNode _agent;

		private Vector3 _desiredPosition;

		private float _lastUpdate;

		public ChaseTarget()
			: this()
		{
		}

		public override void OnStart()
		{
			_agent = agent.get_Value();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _agent.aiMovementData.position;
			if (selectedTargetInfo.get_Value() != _targetInfo)
			{
				_targetInfo = selectedTargetInfo.get_Value();
				GenerateDesiredPosition(position);
			}
			if (Time.get_time() - _lastUpdate > 1f)
			{
				GenerateDesiredPosition(position);
				_lastUpdate = Time.get_time();
			}
			Vector3 worldCenterOfMass = _targetInfo.rigidBody.get_worldCenterOfMass();
			Vector3 val = position - worldCenterOfMass;
			Vector3 val2 = _desiredPosition - worldCenterOfMass;
			AIMovementUtils.MovementDirection movementDirection = (val.get_sqrMagnitude() >= val2.get_sqrMagnitude()) ? AIMovementUtils.MovementDirection.ForceForward : AIMovementUtils.MovementDirection.ForceReverse;
			willingToMove.set_Value(true);
			AIMovementUtils.MoveToCombatPosition(position, _desiredPosition, _agent, willingToMove, movementDirection);
			movementCommandValue.set_Value(1f);
			return 3;
		}

		private void GenerateDesiredPosition(Vector3 agentPosition)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			Vector3 worldCenterOfMass = _targetInfo.rigidBody.get_worldCenterOfMass();
			Vector3 val = worldCenterOfMass - agentPosition;
			float num = AIMovementUtils.CalculateCombatRange(Vector3.Distance(worldCenterOfMass, agentPosition), _agent.aiEquippedWeaponComponent.itemCategory);
			_desiredPosition = worldCenterOfMass - val.get_normalized() * num;
			currentSteeringGoal.set_Value(worldCenterOfMass);
			if (_agent.aiEquippedWeaponComponent.itemCategory != ItemCategory.Rail)
			{
				_desiredPosition = AddNoise(_desiredPosition);
			}
		}

		private Vector3 AddNoise(Vector3 targetPosition)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			float num = Random.Range(-5f, 5f);
			float num2 = Random.Range(-5f, 5f);
			targetPosition.x += num;
			targetPosition.y += num2;
			return targetPosition;
		}

		public override void OnEnd()
		{
			_agent.aiInputWrapper.horizontalAxis = 0f;
			_agent.aiInputWrapper.forwardAxis = 0f;
			movementCommandValue.set_Value(0f);
		}
	}
}
