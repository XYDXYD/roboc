using Svelto.ECS;

namespace Simulation.DeathEffects
{
	internal class MachinePlayDeathEffectEntityView : EntityView
	{
		public IDeathEffectComponent deathEffectComponent;

		public IDeathEffectDependenciesComponent deathEffectDependenciesComponent;

		public MachinePlayDeathEffectEntityView()
			: this()
		{
		}
	}
}
