using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal class EmpModuleComponentImplementor : MonoBehaviour, IReadyEffectActivationComponent, IEmpModuleCountdownComponent, IEmpModuleStunDurationComponent, IEmpModuleStunRadiusComponent
	{
		private DispatchOnChange<bool> _activateReadyEffect;

		private float _countdown;

		private float _effectRay;

		private float _stunDuration;

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

		float IEmpModuleCountdownComponent.countdown
		{
			get
			{
				return _countdown;
			}
			set
			{
				_countdown = value;
			}
		}

		float IEmpModuleStunDurationComponent.stunDuration
		{
			get
			{
				return _stunDuration;
			}
			set
			{
				_stunDuration = value;
			}
		}

		float IEmpModuleStunRadiusComponent.stunRadius
		{
			get;
			set;
		}

		public EmpModuleComponentImplementor()
			: this()
		{
		}

		private void Awake()
		{
			_activateReadyEffect = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}
	}
}
