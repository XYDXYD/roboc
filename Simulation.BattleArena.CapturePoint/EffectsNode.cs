using Svelto.ECS;

namespace Simulation.BattleArena.CapturePoint
{
	internal sealed class EffectsNode : EntityView
	{
		public IPropComponent propComponent;

		public IAnimationComponent animationComponent;

		public ICaptureRingsComponent ringsComponent;

		public IColorComponent colorComponent;

		public ICaptureZoneComponent captureZoneComponent;

		public IProgressComponent progressComponent;

		public IVisualTeamComponent visualTeamComponent;

		public EffectsNode()
			: this()
		{
		}
	}
}
