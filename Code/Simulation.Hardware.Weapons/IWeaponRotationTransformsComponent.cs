using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponRotationTransformsComponent
	{
		Transform horizontalTransform
		{
			get;
		}

		Transform verticalTransform
		{
			get;
		}

		Transform secondVerticalTransform
		{
			get;
		}
	}
}
