using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal interface IRobotShakeComponent
	{
		DispatchOnSet<int> applyShake
		{
			get;
		}

		float robotShakeDuration
		{
			get;
		}

		float robotShakeMagnitude
		{
			get;
		}

		bool robotShakeEnabled
		{
			get;
		}

		TranslationCurve shootRobotCurves
		{
			get;
		}
	}
}
