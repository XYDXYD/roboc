using Simulation.Hardware;
using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation
{
	internal class MachineInputImplementor : IMachineInputComponent
	{
		private Dispatcher<IMachineInputComponent, int> _firePressed;

		private Dispatcher<int> _zoomPressed;

		public IMachineControl machineInput
		{
			get;
			set;
		}

		public float fire1
		{
			get;
			set;
		}

		public float fire2
		{
			get;
			set;
		}

		public Vector4 digitalInput
		{
			get;
			set;
		}

		public Vector4 analogInput
		{
			get;
			set;
		}

		public Dispatcher<IMachineInputComponent, int> firePressed => _firePressed;

		public Dispatcher<int> zoomPressed => _zoomPressed;

		public bool fireHeldDown
		{
			get;
			set;
		}

		public MachineInputImplementor(IMachineControl _machineInput)
		{
			_firePressed = new Dispatcher<IMachineInputComponent, int>(this);
			_zoomPressed = new Dispatcher<int>();
			machineInput = _machineInput;
		}
	}
}
