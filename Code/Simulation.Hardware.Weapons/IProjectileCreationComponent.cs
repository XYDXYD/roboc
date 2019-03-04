using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileCreationComponent
	{
		Vector3 launchDirection
		{
			get;
			set;
		}

		Dispatcher<IProjectileCreationComponent, ProjectileCreationParams> createProjectile
		{
			get;
		}
	}
}
