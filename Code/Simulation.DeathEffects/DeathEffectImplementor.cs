using UnityEngine;

namespace Simulation.DeathEffects
{
	internal class DeathEffectImplementor : MonoBehaviour, IDeathEffectComponent
	{
		[SerializeField]
		private Animation _animation;

		[SerializeField]
		private float _scaleFactor = 1f;

		[SerializeField]
		private string _audioEventForPlayer;

		[SerializeField]
		private string _audioEventForOthers;

		[SerializeField]
		private bool _randomRotation;

		[SerializeField]
		private DeathEffectsData _deathEffectsData;

		public GameObject rootGameObject => this.get_gameObject();

		public Transform rootTransform => this.get_transform();

		public Animation animation => _animation;

		public float scaleFactor => _scaleFactor;

		public string audioEventForPlayer => _audioEventForPlayer;

		public string audioEventForOthers => _audioEventForOthers;

		public bool randomRotation => _randomRotation;

		public DeathEffectsData deathEffectsData => _deathEffectsData;

		public DeathEffectImplementor()
			: this()
		{
		}
	}
}
