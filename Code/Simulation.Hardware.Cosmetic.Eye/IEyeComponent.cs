using UnityEngine;

namespace Simulation.Hardware.Cosmetic.Eye
{
	public interface IEyeComponent
	{
		Transform[] lids
		{
			get;
		}

		Vector3[] axis
		{
			get;
		}

		float[] rotateAmounts
		{
			get;
		}

		Transform eyeBall
		{
			get;
		}
	}
}
