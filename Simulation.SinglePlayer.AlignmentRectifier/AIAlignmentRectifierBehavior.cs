using System;

namespace Simulation.SinglePlayer.AlignmentRectifier
{
	internal class AIAlignmentRectifierBehavior : AlignmentRectifierBehaviour
	{
		public IAIAlignmentRectifierControlComponent aiAlignmentRectifierControlComponent
		{
			get;
			private set;
		}

		public int machineId
		{
			get;
			private set;
		}

		public event Action<AIAlignmentRectifierBehavior> OnAlignmentRectifierInterrupted = delegate
		{
		};

		public AIAlignmentRectifierBehavior(AIAgentDataComponentsNode agentDataNode, int machineId)
			: base(agentDataNode.aiMovementData.rigidBody, agentDataNode.aiMovementData.minmax)
		{
			aiAlignmentRectifierControlComponent = agentDataNode.aiAlignmentRectifierControlComponent;
			this.machineId = machineId;
		}

		public float GetElapsedTime()
		{
			return _elapsedTime;
		}

		protected override void HandleOnAlignmentRectifierCollisionDetected()
		{
			this.OnAlignmentRectifierInterrupted(this);
		}

		public override void GoIdle()
		{
			base.GoIdle();
			_rigidBody.set_useGravity(true);
		}
	}
}
