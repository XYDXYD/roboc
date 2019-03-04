using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	internal sealed class MachineStunMonoBehaviour : MonoBehaviour, IMachineStunComponent
	{
		private Dispatcher<IMachineStunComponent, int> _machineStunned;

		private Dispatcher<IMachineStunComponent, int> _remoteMachineStunned;

		Dispatcher<IMachineStunComponent, int> IMachineStunComponent.machineStunned
		{
			get
			{
				return _machineStunned;
			}
		}

		Dispatcher<IMachineStunComponent, int> IMachineStunComponent.remoteMachineStunned
		{
			get
			{
				return _remoteMachineStunned;
			}
		}

		bool IMachineStunComponent.stunned
		{
			get;
			set;
		}

		float IMachineStunComponent.stunTimer
		{
			get;
			set;
		}

		public int stunningEmpLocator
		{
			get;
			set;
		}

		public MachineStunMonoBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_remoteMachineStunned = new Dispatcher<IMachineStunComponent, int>(this);
			_machineStunned = new Dispatcher<IMachineStunComponent, int>(this);
		}
	}
}
