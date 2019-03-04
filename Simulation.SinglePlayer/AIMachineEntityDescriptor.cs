using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Modules.Sight;
using Simulation.Hardware.Movement;
using Simulation.Sight;
using Simulation.SpawnEffects;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	public class AIMachineEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static AIMachineEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[26]
			{
				new EntityViewBuilder<MachineInputNode>(),
				new EntityViewBuilder<AIAgentDataComponentsNode>(),
				new EntityViewBuilder<MachineStunNode>(),
				new EntityViewBuilder<AISpotterMachineNode>(),
				new EntityViewBuilder<SpottableMachineEntityView>(),
				new EntityViewBuilder<MachineInvisibilityNode>(),
				new EntityViewBuilder<RadarTagNode>(),
				new EntityViewBuilder<MachineAliveStateNode>(),
				new EntityViewBuilder<HealingPriorityNode>(),
				new EntityViewBuilder<MachineTopSpeedNode>(),
				new EntityViewBuilder<MachineRaycastView>(),
				new EntityViewBuilder<MachineWeaponOrderView>(),
				new EntityViewBuilder<MachineRectifierNode>(),
				new EntityViewBuilder<MachineMotionSenderEntityView>(),
				new EntityViewBuilder<MachineRigidbodyNode>(),
				new EntityViewBuilder<AIMachineEntityView>(),
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<KillAssistAwarderEntityView>(),
				new EntityViewBuilder<HealAssistAwarderEntityView>(),
				new EntityViewBuilder<AutoHealEntityView>(),
				new EntityViewBuilder<MachineWeaponsBlockedNode>(),
				new EntityViewBuilder<MachineTargetsEntityView>(),
				new EntityViewBuilder<MachinePhysicsActivationEntityView>(),
				new EntityViewBuilder<MachineMotorDetectionEntityView>(),
				new EntityViewBuilder<MachinePlaySpawnEffectEntityView>(),
				new EntityViewBuilder<MachinePlayDeathEffectEntityView>()
			};
		}

		public AIMachineEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
