using Simulation.Achievements;
using Simulation.DeathEffects;
using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Modules;
using Simulation.Hardware.Movement;
using Simulation.Sight;
using Simulation.SpawnEffects;
using Svelto.ECS;

namespace Simulation.Hardware
{
	internal class LocalMachineEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static LocalMachineEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[32]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<MachineInvisibilityNode>(),
				new EntityViewBuilder<MachineRigidbodyNode>(),
				new EntityViewBuilder<ManaDrainNode>(),
				new EntityViewBuilder<MachineInputNode>(),
				new EntityViewBuilder<MachineGroundedNode>(),
				new EntityViewBuilder<LoadInvisibilityStatsNode>(),
				new EntityViewBuilder<MachineStunNode>(),
				new EntityViewBuilder<PlayerTargetNode>(),
				new EntityViewBuilder<HighSpeedColliderNode>(),
				new EntityViewBuilder<MachineWeaponsBlockedNode>(),
				new EntityViewBuilder<MachineAliveStateNode>(),
				new EntityViewBuilder<SpotterMachineEntityView>(),
				new EntityViewBuilder<SpottableMachineEntityView>(),
				new EntityViewBuilder<HealingPriorityNode>(),
				new EntityViewBuilder<MachineTopSpeedNode>(),
				new EntityViewBuilder<TaunterMachineNode>(),
				new EntityViewBuilder<MachineRectifierNode>(),
				new EntityViewBuilder<MachineRaycastView>(),
				new EntityViewBuilder<MachineWeaponOrderView>(),
				new EntityViewBuilder<AchievementMachineVisibilityNode>(),
				new EntityViewBuilder<KillAssistAwarderEntityView>(),
				new EntityViewBuilder<HealAssistAwarderEntityView>(),
				new EntityViewBuilder<AutoHealEntityView>(),
				new EntityViewBuilder<AutoHealGuiEntityView>(),
				new EntityViewBuilder<MachineMotionSenderEntityView>(),
				new EntityViewBuilder<MachineTargetsEntityView>(),
				new EntityViewBuilder<MachinePhysicsActivationEntityView>(),
				new EntityViewBuilder<MachineMotorDetectionEntityView>(),
				new EntityViewBuilder<MachinePlaySpawnEffectEntityView>(),
				new EntityViewBuilder<MachinePlayDeathEffectEntityView>(),
				new EntityViewBuilder<PowerBarConsumptionEntityView>()
			};
		}

		public LocalMachineEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
