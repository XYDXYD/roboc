using Svelto.ECS;

namespace Simulation
{
	internal class DiscShieldSettingsNode : EntityView
	{
		public IDiscShieldSettingsComponent settingsComponent;

		public DiscShieldSettingsNode()
			: this()
		{
		}
	}
}
