using System;

namespace Simulation
{
	internal interface ISpectatorModeActivator
	{
		void Register(Action<int, bool> onActivate);

		void Unregister(Action<int, bool> onActivate);
	}
}
