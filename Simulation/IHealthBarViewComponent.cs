using Svelto.ES.Legacy;

namespace Simulation
{
	internal interface IHealthBarViewComponent : IComponent
	{
		void SetTimerLabel(int timerValue, bool enabled);

		void PlayFirstAutoRegenAnimation();

		void PlaySecondAutoRegenAnimation();

		void StopSecondAutoRegenAnimation();
	}
}
