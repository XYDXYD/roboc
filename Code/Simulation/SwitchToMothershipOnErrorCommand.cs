using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class SwitchToMothershipOnErrorCommand : ICommand
	{
		[Inject]
		public BonusManager bonusManager
		{
			private get;
			set;
		}

		[Inject]
		public WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		public void Execute()
		{
			bonusManager.ConnectionError();
			worldSwitching.SwitchToLastMothershipGameMode(fastSwitch: true);
		}
	}
}
