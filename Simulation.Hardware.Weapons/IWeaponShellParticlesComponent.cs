using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	public interface IWeaponShellParticlesComponent
	{
		GameObject shellParticlesPrefab
		{
			get;
		}

		Transform[] shellParticlesLocations
		{
			get;
		}

		int nextShellLocation
		{
			get;
			set;
		}
	}
}
