using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Simulation.SinglePlayer.CapturePoints;
using System;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class MoveOnCapturePoint : Action
	{
		public SharedAIBehaviorTreeAgentNode Agent;

		public SharedTargetInfoComponent SelectedTargetInfo;

		public SharedAICaptureInfo CaptureInfo;

		public SharedAIInputWrapper AIInputWrapper;

		public SharedVector3 CurrentSteeringGoal;

		public SharedFloat MovementCommandValue;

		public SharedBool WillingToMove;

		private AIAgentDataComponentsNode _agent;

		private TargetInfo _target;

		private AICaptureInfo _captureInfo;

		private AIInputWrapper _inputWrapper;

		public MoveOnCapturePoint()
			: this()
		{
		}

		public override void OnStart()
		{
			_agent = Agent.get_Value();
			_target = SelectedTargetInfo.get_Value();
			_captureInfo = CaptureInfo.get_Value();
			_inputWrapper = AIInputWrapper.get_Value();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.ProjectOnPlane(_agent.aiMovementData.position, Vector3.get_up());
			Vector3 val2 = Vector3.ProjectOnPlane(_target.rigidBody.get_worldCenterOfMass(), Vector3.get_up());
			Vector3 val3 = Vector3.ProjectOnPlane(_captureInfo.Goal.Position, Vector3.get_up());
			float radius = _captureInfo.Goal.Radius;
			double num = Math.Pow(radius, 2.0);
			Vector3 val4 = val3;
			Vector3 val5 = val2 - val3;
			Vector3 val6 = val4 + val5.get_normalized() * radius;
			_inputWrapper.forwardAxis = 0f;
			CurrentSteeringGoal.set_Value(val2);
			SharedBool willingToMove = WillingToMove;
			Vector3 val7 = val - val6;
			willingToMove.set_Value((double)val7.get_sqrMagnitude() > num);
			UpdateSteering(val6);
			return 3;
		}

		private void UpdateSteering(Vector3 targetPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			AIUtility.UpdateSteering(targetPosition, _agent, _inputWrapper, MovementCommandValue, WillingToMove.get_Value(), 0.35f);
		}
	}
}
