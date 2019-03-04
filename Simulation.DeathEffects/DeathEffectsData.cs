using UnityEngine;

namespace Simulation.DeathEffects
{
	[CreateAssetMenu(fileName = "DeathEffectsData", menuName = "ScriptableObjects/DeathEffectsData")]
	internal class DeathEffectsData : ScriptableObject
	{
		[SerializeField]
		private float _deathDuration;

		public float deathDuration => _deathDuration;

		public DeathEffectsData()
			: this()
		{
		}
	}
}
