using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal interface ISpinningBladesComponent
	{
		float spinForce
		{
			get;
		}

		float spinDeceleration
		{
			get;
		}

		Transform spinningTransform
		{
			get;
		}

		Transform blurTransform
		{
			get;
		}

		Renderer blurRenderer
		{
			get;
		}

		float currentSpinningSpeed
		{
			get;
			set;
		}

		float normalizedSpinningSpeed
		{
			get;
			set;
		}
	}
}
