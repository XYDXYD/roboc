using Svelto.ECS;

namespace Simulation
{
	internal class DiscShieldAudioNode : EntityView
	{
		public IDiscShieldAudioComponent audioComponent;

		public IDiscShieldObjectComponent objectComponent;

		public DiscShieldAudioNode()
			: this()
		{
		}
	}
}
