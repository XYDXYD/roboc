using Svelto.ECS;

namespace Simulation.Hardware
{
	internal interface IHardwareDisabledComponent
	{
		bool disabled
		{
			get;
		}

		DispatchOnChange<bool> isPartDisabled
		{
			get;
		}

		bool enabled
		{
			get;
		}

		DispatchOnChange<bool> isPartEnabled
		{
			get;
		}
	}
}
