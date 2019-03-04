using UnityEngine;

namespace Simulation.SpawnEffects
{
	[CreateAssetMenu(fileName = "SpawnEffectsData", menuName = "ScriptableObjects/SpawnEffectsData")]
	internal class SpawnEffectsData : ScriptableObject
	{
		[SerializeField]
		private float _spawnDuration;

		public float spawnDuration => _spawnDuration;

		public SpawnEffectsData()
			: this()
		{
		}
	}
}
