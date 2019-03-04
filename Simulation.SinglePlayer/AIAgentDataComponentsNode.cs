using Simulation.Hardware;
using Simulation.SinglePlayer.PowerConsumption;
using Simulation.SinglePlayer.Rewards;
using Simulation.SinglePlayer.Shooting;
using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	public class AIAgentDataComponentsNode : EntityView
	{
		public IBehaviorTreeComponent agentBehaviorTreeComponent;

		public IAIBotIdDataComponent aiBotIdData;

		public IAIGameObjectMovementDataComponent aiMovementData;

		public IProtectTeamMateBonusComponent protectTeamMateBonusComponent;

		public IAIAlignmentRectifierControlComponent aiAlignmentRectifierControlComponent;

		public IAIWeaponShootingFeedbackComponent aiWeaponShootingFeedbackComponent;

		public IAIInputWrapper aiInputWrapper;

		public IAIPowerConsumptionComponent aiPowerConsumptionComponent;

		public IAIScoreComponent aiScoreComponent;

		public IAIEquippedWeaponComponent aiEquippedWeaponComponent;

		public IMachineStunComponent machineStunComponent;

		public IMachineVisibilityComponent machineVisibilityComponent;

		public IMachineFunctionalComponent rectifyingComponent;

		public IOwnerTeamComponent ownerTeamComponent;

		public AIAgentDataComponentsNode()
			: this()
		{
		}
	}
}
