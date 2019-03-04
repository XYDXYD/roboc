using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal class RadarModuleImplementor : MonoBehaviour, IRadarComponent, IRadarStatsComponent, IRadarVFXComponent, IReadyEffectActivationComponent
	{
		[SerializeField]
		private GameObject _activationVfxPrefab;

		[SerializeField]
		private GameObject _enemyActivationVfxPrefab;

		[SerializeField]
		private Transform _activationVfxAnchor;

		[SerializeField]
		private Animator _animatorComponent;

		private DispatchOnChange<bool> _activateReadyEffect;

		public DispatchOnChange<bool> isRadarActive
		{
			get;
			private set;
		}

		public float radarRemainingTime
		{
			get;
			set;
		}

		public float radarDuration
		{
			get;
			set;
		}

		public bool effectActive
		{
			get;
			set;
		}

		public DispatchOnChange<bool> activateReadyEffect => _activateReadyEffect;

		public GameObject activationVfxPrefab => _activationVfxPrefab;

		public GameObject enemyActivationVfxPrefab => _enemyActivationVfxPrefab;

		public Transform vfxAnchor => _activationVfxAnchor;

		public Animator animatorComponent => _animatorComponent;

		public RadarModuleImplementor()
			: this()
		{
		}

		private void Awake()
		{
			isRadarActive = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_activateReadyEffect = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}
	}
}
