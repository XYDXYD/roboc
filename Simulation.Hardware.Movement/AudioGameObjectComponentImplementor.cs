using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal class AudioGameObjectComponentImplementor : IAudioGameObjectComponent
	{
		public GameObject audioGO
		{
			get;
			private set;
		}

		public AudioGameObjectComponentImplementor(GameObject machineCenter)
		{
			audioGO = machineCenter;
		}
	}
}
