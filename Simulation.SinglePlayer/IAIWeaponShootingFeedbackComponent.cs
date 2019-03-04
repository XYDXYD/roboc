using Svelto.ECS.Legacy;

namespace Simulation.SinglePlayer
{
	public interface IAIWeaponShootingFeedbackComponent
	{
		Dispatcher<AIWeaponShootingFeedbackComponent, int> shotIsGoingToBeFired
		{
			get;
		}
	}
}
