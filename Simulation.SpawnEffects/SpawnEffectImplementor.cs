using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal class SpawnEffectImplementor : MonoBehaviour, ISpawnEffectComponent
	{
		[SerializeField]
		private Animation _animation;

		[SerializeField]
		private Transform _groundTransform;

		[SerializeField]
		private Transform _robotTransform;

		[SerializeField]
		private GameObject _robotVisibility;

		[SerializeField]
		private float _scaleFactor = 1f;

		[SerializeField]
		private string _audioEventForPlayer;

		[SerializeField]
		private string _audioEventForOthers;

		[SerializeField]
		private SpawnEffectsData _spawnEffectsData;

		public GameObject rootGameObject => this.get_gameObject();

		public Animation animation => _animation;

		public Transform groundTransform => _groundTransform;

		public Transform robotTransform => _robotTransform;

		public GameObject robotVisibility => _robotVisibility;

		public float scaleFactor => _scaleFactor;

		public string audioEventForPlayer => _audioEventForPlayer;

		public string audioEventForOthers => _audioEventForOthers;

		public SpawnEffectsData spawnEffectsData => _spawnEffectsData;

		public SpawnEffectImplementor()
			: this()
		{
		}
	}
}
