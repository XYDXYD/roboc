using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware.Modules.PowerModule
{
	internal class PowerModuleMonoBehaviour : MonoBehaviour, IReadyEffectActivationComponent, IPowerModuleEffectsComponent
	{
		private DispatchOnChange<bool> _activateReadyEffect;

		[SerializeField]
		private GameObject allyActivationEffectPrefab;

		[SerializeField]
		private GameObject enemyActivationEffectPrefab;

		[SerializeField]
		private GameObject localPlayerActivationEffectPrefab;

		DispatchOnChange<bool> IReadyEffectActivationComponent.activateReadyEffect
		{
			get
			{
				return _activateReadyEffect;
			}
		}

		bool IReadyEffectActivationComponent.effectActive
		{
			get;
			set;
		}

		public GameObject AllyEffectPrefab => allyActivationEffectPrefab;

		public GameObject EnemyEffectPrefab => enemyActivationEffectPrefab;

		public GameObject LocalPlayerEffectPrefab => localPlayerActivationEffectPrefab;

		public PowerModuleMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_activateReadyEffect = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}
	}
}
