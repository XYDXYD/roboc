using Svelto.ECS;
using UnityEngine;

namespace Simulation.Hardware
{
	internal class HardwareWorkingOrderImplementor : MonoBehaviour, IHardwareDisabledComponent, IImplementor
	{
		private DispatchOnChange<bool> _onIsPartEnabled;

		private DispatchOnChange<bool> _onIsPartDisabled;

		public bool disabled => _onIsPartDisabled.get_value();

		public DispatchOnChange<bool> isPartDisabled => _onIsPartDisabled;

		public bool enabled => _onIsPartEnabled.get_value();

		public DispatchOnChange<bool> isPartEnabled => _onIsPartEnabled;

		public HardwareWorkingOrderImplementor()
			: this()
		{
		}

		private void Awake()
		{
			_onIsPartDisabled = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
			_onIsPartEnabled = new DispatchOnChange<bool>(this.get_gameObject().GetInstanceID());
		}

		private void OnEnable()
		{
			_onIsPartDisabled.set_value(false);
			_onIsPartEnabled.set_value(true);
		}

		private void OnDisable()
		{
			_onIsPartDisabled.set_value(true);
			_onIsPartEnabled.set_value(false);
		}
	}
}
