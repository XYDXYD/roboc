using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal class SpawnAnimationImplementor : MonoBehaviour, ISpawnAnimationComponent
	{
		[Range(0f, 90f)]
		[SerializeField]
		private float _pitch = 45f;

		[SerializeField]
		private float _ratioDistance = 3f;

		[SerializeField]
		private float _positionDuration = 0.5f;

		[SerializeField]
		private AnimationCurve _positionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private float _rotationDuration = 0.5f;

		[SerializeField]
		private AnimationCurve _rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private SimulationCamera _controlScript;

		public float pitch => _pitch;

		public float distance => _ratioDistance;

		public float positionDuration => _positionDuration;

		public AnimationCurve positionCurve => _positionCurve;

		public float rotationDuration => _rotationDuration;

		public AnimationCurve rotationCurve => _rotationCurve;

		public SimulationCamera controlScript => _controlScript;

		public SpawnAnimationImplementor()
			: this()
		{
		}
	}
}
