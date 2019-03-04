using System;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal sealed class SupernovaAudioObserver
	{
		public event Action<GameObject> OnSupernovaPlay = delegate
		{
		};

		public void PlaySupernovaAudio(GameObject supernovaGameObject)
		{
			this.OnSupernovaPlay(supernovaGameObject);
		}
	}
}
