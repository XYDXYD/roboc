using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class DamageVignetteEntityView : EntityView
	{
		public IDamageVignetteComponent damageVignetteComponent;

		public DamageVignetteEntityView()
			: this()
		{
		}
	}
}
