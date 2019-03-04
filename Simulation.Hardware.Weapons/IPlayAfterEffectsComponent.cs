using Svelto.ECS.Legacy;

namespace Simulation.Hardware.Weapons
{
	internal interface IPlayAfterEffectsComponent
	{
		Dispatcher<IPlayAfterEffectsComponent, int> applyRecoil
		{
			get;
		}

		Dispatcher<IPlayAfterEffectsComponent, int> playMuzzleFlash
		{
			get;
		}

		Dispatcher<IPlayAfterEffectsComponent, int> playFiringSound
		{
			get;
		}
	}
}
