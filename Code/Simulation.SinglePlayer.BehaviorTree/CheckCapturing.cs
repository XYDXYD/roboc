using BehaviorDesigner.Runtime.Tasks;
using Simulation.SinglePlayer.CapturePoints;
using UnityEngine;

namespace Simulation.SinglePlayer.BehaviorTree
{
	public class CheckCapturing : Conditional
	{
		public SharedAIBehaviorTreeAgentNode Agent;

		public SharedAICapturePointDataArray CapturePointDataArray;

		public SharedAICaptureInfo CaptureInfo;

		private AIAgentDataComponentsNode _agent;

		private AICapturePointData[] _capturePointDataArray;

		private AICaptureInfo _captureInfo;

		public CheckCapturing()
			: this()
		{
		}

		public override void OnStart()
		{
			_agent = Agent.get_Value();
			_capturePointDataArray = CapturePointDataArray.get_Value();
			_captureInfo = CaptureInfo.get_Value();
			this.OnStart();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			AICapturePointData aICapturePointData = null;
			for (int i = 0; i < _capturePointDataArray.Length; i++)
			{
				AICapturePointData aICapturePointData2 = _capturePointDataArray[i];
				if (aICapturePointData2.Status != AICapturePointStatus.Captured || aICapturePointData2.OwnedByTeamId != _agent.ownerTeamComponent.ownerTeamId)
				{
					Vector3 val = Vector3.ProjectOnPlane(_agent.aiMovementData.position, Vector3.get_up());
					Vector3 val2 = Vector3.ProjectOnPlane(aICapturePointData2.Position, Vector3.get_up());
					Vector3 val3 = val - val2;
					float sqrMagnitude = val3.get_sqrMagnitude();
					if (sqrMagnitude < num || aICapturePointData == null)
					{
						num = sqrMagnitude;
						aICapturePointData = aICapturePointData2;
					}
				}
			}
			_captureInfo.Goal = aICapturePointData;
			_captureInfo.IsCapturing = (aICapturePointData != null && num < aICapturePointData.Radius * aICapturePointData.Radius);
			return (!_captureInfo.IsCapturing) ? 1 : 2;
		}
	}
}
