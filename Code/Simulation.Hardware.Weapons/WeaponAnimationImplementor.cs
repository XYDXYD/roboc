using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponAnimationImplementor : MonoBehaviour, IWeaponAnimationComponent
	{
		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private string _shootAnimationName;

		public Animator animator => _animator;

		public string shootAnimationName => _shootAnimationName;

		public WeaponAnimationImplementor()
			: this()
		{
		}
	}
}
