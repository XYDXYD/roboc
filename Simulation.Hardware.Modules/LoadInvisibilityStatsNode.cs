using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadInvisibilityStatsNode : EntityView
	{
		public ICloakStatsComponent cloakStatsComponent;

		public LoadInvisibilityStatsNode()
			: this()
		{
		}
	}
}
