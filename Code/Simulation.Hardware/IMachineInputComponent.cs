using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware
{
	internal interface IMachineInputComponent
	{
		IMachineControl machineInput
		{
			get;
			set;
		}

		float fire1
		{
			get;
			set;
		}

		float fire2
		{
			get;
			set;
		}

		Vector4 digitalInput
		{
			get;
			set;
		}

		Vector4 analogInput
		{
			get;
			set;
		}

		Dispatcher<IMachineInputComponent, int> firePressed
		{
			get;
		}

		Dispatcher<int> zoomPressed
		{
			get;
		}

		bool fireHeldDown
		{
			get;
			set;
		}
	}
}
