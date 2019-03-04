using Svelto.ECS;

namespace Simulation
{
	internal class DiscShieldManagingNode : EntityView
	{
		public IDiscShieldOwnerComponent ownerComponent;

		public IDiscShieldJustSpawnedComponent justSpawnedComponent;

		public IDiscShieldEffectsComponent effectsComponent;

		public IDiscShieldActivationTimeComponent activationTimeComponent;

		public IDiscShieldObjectComponent objectComponent;

		public IDiscShieldSettingsComponent settingsComponent;

		public IDiscShieldClosingTimeComponent closingTimeComponent;

		public DiscShieldManagingNode()
			: this()
		{
		}
	}
}
