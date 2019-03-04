using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal interface ICameraShakeComponent
	{
		DispatchOnSet<int> applyShake
		{
			get;
		}

		float cameraShakeDuration
		{
			get;
		}

		float cameraShakeRotationMagnitude
		{
			get;
		}

		float cameraShakeTranslationMagnitude
		{
			get;
		}

		bool camShakeEnabled
		{
			get;
		}

		TranslationCurve shootCameraTranslationCurves
		{
			get;
		}

		RotationCurve shootCameraRotationCurves
		{
			get;
		}
	}
}
