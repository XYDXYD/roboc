using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface ITeslaEffectComponent
	{
		bool isOpen
		{
			get;
			set;
		}

		Animation animation
		{
			get;
		}

		string audioLoop
		{
			get;
		}

		Dispatcher<ITeslaEffectComponent, int> triggerExit
		{
			get;
		}
	}
}
