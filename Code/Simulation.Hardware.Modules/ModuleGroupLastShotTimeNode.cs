using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class ModuleGroupLastShotTimeNode : EntityView
	{
		public IModuleGroupLastShotTimeComponent lastShotTimeComponent;

		public ModuleGroupLastShotTimeNode()
			: this()
		{
		}
	}
}
