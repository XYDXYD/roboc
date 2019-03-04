using UnityEngine;
using UnityEngine.Serialization;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ProjectileTrailMonoBehaviour : MonoBehaviour, IProjectileTrailEffectsComponent
	{
		[FormerlySerializedAs("trailRendererObject")]
		public GameObject rendererObject;

		[SerializeField]
		private GameObject _disableOnHit;

		private TrailRenderer _trailRenderer;

		public GameObject trailRendererObject => rendererObject;

		public float disableCountdown
		{
			get;
			set;
		}

		public TrailRenderer projectileTrail => _trailRenderer;

		public GameObject disableOnHit => _disableOnHit;

		public ProjectileTrailMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_trailRenderer = trailRendererObject.GetComponentInChildren<TrailRenderer>();
			_trailRenderer.set_time(Mathf.Max(0.15f, _trailRenderer.get_time()));
		}
	}
}
