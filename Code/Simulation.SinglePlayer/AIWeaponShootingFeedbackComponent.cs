using Svelto.ECS.Legacy;

namespace Simulation.SinglePlayer
{
	public class AIWeaponShootingFeedbackComponent : IAIWeaponShootingFeedbackComponent
	{
		private Dispatcher<AIWeaponShootingFeedbackComponent, int> _shotIsGoingToBeFired;

		public Dispatcher<AIWeaponShootingFeedbackComponent, int> shotIsGoingToBeFired => _shotIsGoingToBeFired;

		public AIWeaponShootingFeedbackComponent()
		{
			_shotIsGoingToBeFired = new Dispatcher<AIWeaponShootingFeedbackComponent, int>(this);
		}
	}
}
