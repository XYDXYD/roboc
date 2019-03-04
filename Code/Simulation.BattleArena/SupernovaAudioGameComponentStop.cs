using Fabric;
using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal class SupernovaAudioGameComponentStop : MonoBehaviour, ISupernovaAudioGameComponentStop, IComponent
	{
		public GroupComponent groupComponent;

		public SupernovaAudioGameComponentStop()
			: this()
		{
		}

		public void StopAudio()
		{
			groupComponent.Stop();
			groupComponent.SetVolume(0f);
		}
	}
}
