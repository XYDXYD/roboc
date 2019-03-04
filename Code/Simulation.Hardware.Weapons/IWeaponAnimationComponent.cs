using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponAnimationComponent
	{
		Animator animator
		{
			get;
		}

		string shootAnimationName
		{
			get;
		}
	}
}
