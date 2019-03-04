using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal class TeslaRamCollisionObserver : MonoBehaviour
	{
		public TeslaRamMonoBehaviour owner;

		public TeslaRamCollisionObserver()
			: this()
		{
		}

		private void OnTriggerEnter(Collider c)
		{
			owner.OnTriggerEnterEvent(ref c);
		}

		private void OnTriggerExit(Collider c)
		{
			owner.OnTriggerExitEvent(ref c);
		}
	}
}
