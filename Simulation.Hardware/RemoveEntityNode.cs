using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class RemoveEntityNode : EntityView
	{
		public IRemoveEntityComponent removeEntityComponent;

		public RemoveEntityNode()
			: this()
		{
		}
	}
}
