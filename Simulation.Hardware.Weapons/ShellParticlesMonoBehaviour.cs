using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	public class ShellParticlesMonoBehaviour : MonoBehaviour, IWeaponShellParticlesComponent, IImplementor
	{
		public GameObject _shellParticlesPrefab;

		public Transform[] _shellParticlesLocations;

		public GameObject shellParticlesPrefab => _shellParticlesPrefab;

		public Transform[] shellParticlesLocations => _shellParticlesLocations;

		public int nextShellLocation
		{
			get;
			set;
		}

		public ShellParticlesMonoBehaviour()
			: this()
		{
		}
	}
}
