using Svelto.ECS;

namespace Simulation.GUI
{
	internal class VotingAfterBattleMainWindowEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static VotingAfterBattleMainWindowEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<VotingAfterBattleMainWindowNode>()
			};
		}

		public VotingAfterBattleMainWindowEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
