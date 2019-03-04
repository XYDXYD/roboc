using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.SinglePlayer.StunModule
{
	internal class AIStunEngine : SingleEntityViewEngine<MachineStunNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(MachineStunNode obj)
		{
			obj.stunComponent.machineStunned.subscribers += HandleStunStateChange;
		}

		protected override void Remove(MachineStunNode obj)
		{
			obj.stunComponent.machineStunned.subscribers -= HandleStunStateChange;
		}

		private void HandleStunStateChange(IMachineStunComponent machineStunComponent, int machineId)
		{
			AIAgentDataComponentsNode aIAgentDataComponentsNode = default(AIAgentDataComponentsNode);
			if (entityViewsDB.TryQueryEntityView<AIAgentDataComponentsNode>(machineId, ref aIAgentDataComponentsNode))
			{
				if (machineStunComponent.stunned)
				{
					aIAgentDataComponentsNode.agentBehaviorTreeComponent.aiAgentBehaviorTree.set_enabled(false);
					ResetInput(aIAgentDataComponentsNode.aiInputWrapper);
				}
				else
				{
					aIAgentDataComponentsNode.agentBehaviorTreeComponent.aiAgentBehaviorTree.set_enabled(true);
				}
			}
		}

		private void ResetInput(IAIInputWrapper inputWrapper)
		{
			inputWrapper.fire1 = 0f;
			inputWrapper.forwardAxis = 0f;
			inputWrapper.horizontalAxis = 0f;
			inputWrapper.pulseAR = false;
		}

		public void Ready()
		{
		}
	}
}
