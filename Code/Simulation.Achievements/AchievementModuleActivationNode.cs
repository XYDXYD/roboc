using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.Achievements
{
	internal sealed class AchievementModuleActivationNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IModuleActivationComponent activationComponent;

		public AchievementModuleActivationNode()
			: this()
		{
		}
	}
}
