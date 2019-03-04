using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	internal sealed class MachineInvisibilityMonoBehaviour : MonoBehaviour, IMachineVisibilityComponent, IManaDrainComponent, ICloakStatsComponent
	{
		private Dispatcher<IMachineVisibilityComponent, int> _machineBecameVisible;

		private Dispatcher<IMachineVisibilityComponent, int> _machineBecameInvisible;

		private Dispatcher<IManaDrainComponent, ManaDrainingActivationData> _activateManaDraining;

		private Dispatcher<IManaDrainComponent, int> _manaDrained;

		bool IMachineVisibilityComponent.isVisible
		{
			get;
			set;
		}

		Dispatcher<IMachineVisibilityComponent, int> IMachineVisibilityComponent.machineBecameVisible
		{
			get
			{
				return _machineBecameVisible;
			}
		}

		Dispatcher<IMachineVisibilityComponent, int> IMachineVisibilityComponent.machineBecameInvisible
		{
			get
			{
				return _machineBecameInvisible;
			}
		}

		bool IManaDrainComponent.draining
		{
			get;
			set;
		}

		float IManaDrainComponent.drainRate
		{
			get;
			set;
		}

		Dispatcher<IManaDrainComponent, ManaDrainingActivationData> IManaDrainComponent.activateManaDraining
		{
			get
			{
				return _activateManaDraining;
			}
		}

		Dispatcher<IManaDrainComponent, int> IManaDrainComponent.manaDrained
		{
			get
			{
				return _manaDrained;
			}
		}

		float ICloakStatsComponent.toInvisibleDuration
		{
			get;
			set;
		}

		float ICloakStatsComponent.toVisibleDuration
		{
			get;
			set;
		}

		public MachineInvisibilityMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_machineBecameVisible = new Dispatcher<IMachineVisibilityComponent, int>(this);
			_machineBecameInvisible = new Dispatcher<IMachineVisibilityComponent, int>(this);
			_activateManaDraining = new Dispatcher<IManaDrainComponent, ManaDrainingActivationData>(this);
			_manaDrained = new Dispatcher<IManaDrainComponent, int>(this);
		}
	}
}
