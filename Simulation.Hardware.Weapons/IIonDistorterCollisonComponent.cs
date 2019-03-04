using Svelto.DataStructures;
using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IIonDistorterCollisonComponent
	{
		FasterList<Vector3> projectileDirections
		{
			get;
		}

		Dispatcher<IIonDistorterCollisonComponent, int> checkCollision
		{
			get;
		}
	}
}
