using Simulation.DeathEffects;
using Simulation.Hardware;
using Simulation.Hardware.Movement;
using Svelto.ECS;

namespace Simulation.SinglePlayer
{
	public class TutorialDummyMachineEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static TutorialDummyMachineEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[4]
			{
				new EntityViewBuilder<MachineInputNode>(),
				new EntityViewBuilder<MachineTopSpeedNode>(),
				new EntityViewBuilder<MachineWeaponOrderView>(),
				new EntityViewBuilder<MachinePlayDeathEffectEntityView>()
			};
		}

		public TutorialDummyMachineEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
