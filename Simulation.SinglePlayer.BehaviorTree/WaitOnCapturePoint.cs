using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Simulation.SinglePlayer.CapturePoints;
using System;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class WaitOnCapturePoint : Action
	{
		public SharedAIBehaviorTreeAgentNode Agent;

		public SharedAICaptureInfo CaptureInfo;

		public SharedAIInputWrapper AIInputWrapper;

		public SharedFloat MovementCommandValue;

		public SharedBool WillingToMove;

		private AIAgentDataComponentsNode _agent;

		private AICaptureInfo _captureInfo;

		private AIInputWrapper _inputWrapper;

		public WaitOnCapturePoint()
			: this()
		{
		}

		public override void OnStart()
		{
			_agent = Agent.get_Value();
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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.ProjectOnPlane(_agent.aiMovementData.position, Vector3.get_up());
			Vector3 val2 = Vector3.ProjectOnPlane(_captureInfo.Goal.Position, Vector3.get_up());
			Vector3 val3 = val - val2;
			float sqrMagnitude = val3.get_sqrMagnitude();
			double num = Math.Pow(_captureInfo.Goal.Radius, 2.0);
			double num2 = Math.Pow(0.5, 2.0);
			_inputWrapper.forwardAxis = 0f;
			WillingToMove.set_Value((double)sqrMagnitude > num * num2);
			UpdateSteering(_captureInfo.Goal.Position);
			return 3;
		}

		private void UpdateSteering(Vector3 targetPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			AIUtility.UpdateSteering(targetPosition, _agent, _inputWrapper, MovementCommandValue, WillingToMove.get_Value());
		}
	}
}
