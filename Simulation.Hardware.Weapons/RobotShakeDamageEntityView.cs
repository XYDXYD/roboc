using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class RobotShakeDamageEntityView : EntityView
	{
		public IRobotShakeDamageComponent robotShakeDamageComponent;

		public RobotShakeDamageEntityView()
			: this()
		{
		}
	}
}
