using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal interface IIonDistorterProjectileConeComponent
	{
		float coneAngle
		{
			get;
			set;
		}

		int numOfRaycasts
		{
			get;
			set;
		}

		int numOfCircles
		{
			get;
			set;
		}

		ParticleSystem[] bullets
		{
			get;
		}
	}
}
