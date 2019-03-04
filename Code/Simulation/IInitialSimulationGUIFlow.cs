using System;

namespace Simulation
{
	internal interface IInitialSimulationGUIFlow
	{
		event Action OnGuiFlowComplete;

		void OnReceiveEndOfSync();

		void OnStartCountdown();
	}
}
