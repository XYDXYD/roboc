using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class RespawnHealEffectData : MonoBehaviour
	{
		public GameObject respawnHealEffectPrefab;

		public GameObject respawnHealEffectEnemyPrefab;

		public float endAnimationDuration = 1.5f;

		[Inject]
		internal RespawnHealEffects respawnHealEffects
		{
			private get;
			set;
		}

		public RespawnHealEffectData()
			: this()
		{
		}

		private void Start()
		{
			respawnHealEffects.RegisterEffectData(this);
		}
	}
}
