using Svelto.ECS;
using UnityEngine;

namespace Simulation
{
	internal sealed class ShieldModuleMonoBehaviour : MonoBehaviour, IReadyEffectActivationComponent
	{
		[SerializeField]
		private Animator readyEffectAnimator;

		private DispatchOnChange<bool> _activateReadyEffect;

		DispatchOnChange<bool> IReadyEffectActivationComponent.activateReadyEffect
		{
			get
			{
				return _activateReadyEffect;
			}
		}

		bool IReadyEffectActivationComponent.effectActive
		{
			get
			{
				return readyEffectAnimator.GetBool("Active");
			}
			set
			{
				readyEffectAnimator.SetBool("Active", value);
			}
		}

		public ShieldModuleMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_activateReadyEffect = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}
	}
}
