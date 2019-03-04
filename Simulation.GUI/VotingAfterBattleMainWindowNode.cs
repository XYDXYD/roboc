using EnginesGUI;
using Svelto.ECS;

namespace Simulation.GUI
{
	internal sealed class VotingAfterBattleMainWindowNode : EntityView
	{
		public IVotingAfterBattleMainWindowComponent votingAfterBattleMainWindowComponent;

		public IPanelSizeComponent panelSizeCompent;

		public VotingAfterBattleMainWindowNode()
			: this()
		{
		}
	}
}
