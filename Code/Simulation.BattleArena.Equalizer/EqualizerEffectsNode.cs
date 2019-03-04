using Svelto.ECS;

namespace Simulation.BattleArena.Equalizer
{
	internal sealed class EqualizerEffectsNode : EntityView
	{
		public IMaterialComponent materialComponent;

		public IGodRayComponent godRaysComponent;

		public IAudioComponent audioComponent;

		public IRootComponent rootComponent;

		public EqualizerEffectsNode()
			: this()
		{
		}
	}
}
