using Svelto.ECS;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class AudioNode : EntityView
	{
		public IVisualPositionComponent visualPositionComponent;

		public IAudioComponent audioComponent;

		public AudioNode()
			: this()
		{
		}
	}
}
