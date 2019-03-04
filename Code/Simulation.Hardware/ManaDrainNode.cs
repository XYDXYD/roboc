using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class ManaDrainNode : EntityView
	{
		public IManaDrainComponent manaDrainComponent;

		public IMachineOwnerComponent ownerComponent;

		public ManaDrainNode()
			: this()
		{
		}
	}
}
