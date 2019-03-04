using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Modules.Sight;
using Simulation.Hardware.Movement;
using Simulation.Sight;
using Simulation.SpawnEffects;
using Svelto.ECS;

namespace Simulation
{
	internal class RemoteMachineEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static RemoteMachineEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[18]
			{
				new EntityViewBuilder<RemoveEntityNode>(),
				new EntityViewBuilder<MachineInvisibilityNode>(),
				new EntityViewBuilder<ManaDrainNode>(),
				new EntityViewBuilder<MachineRigidbodyNode>(),
				new EntityViewBuilder<MachineStunNode>(),
				new EntityViewBuilder<SpottableMachineEntityView>(),
				new EntityViewBuilder<MachineAliveStateNode>(),
				new EntityViewBuilder<RadarTagNode>(),
				new EntityViewBuilder<HealingPriorityNode>(),
				new EntityViewBuilder<MachineTopSpeedNode>(),
				new EntityViewBuilder<TaunterMachineNode>(),
				new EntityViewBuilder<MachineRaycastView>(),
				new EntityViewBuilder<MachineWeaponOrderView>(),
				new EntityViewBuilder<MachineInputNode>(),
				new EntityViewBuilder<MachineTargetsEntityView>(),
				new EntityViewBuilder<MachineMotorDetectionEntityView>(),
				new EntityViewBuilder<MachinePlaySpawnEffectEntityView>(),
				new EntityViewBuilder<MachinePlayDeathEffectEntityView>()
			};
		}

		public RemoteMachineEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
