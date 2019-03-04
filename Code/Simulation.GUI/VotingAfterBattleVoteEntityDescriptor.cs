using Svelto.ECS;

namespace Simulation.GUI
{
	internal class VotingAfterBattleVoteEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static VotingAfterBattleVoteEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<VotingAfterBattleRobotVoteNode>()
			};
		}

		public VotingAfterBattleVoteEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
