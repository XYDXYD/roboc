using Svelto.ECS;

namespace Simulation.GUI
{
	internal class VotingAfterBattleWidgetEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static VotingAfterBattleWidgetEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<VotingAfterBattleRobotWidgetNode>()
			};
		}

		public VotingAfterBattleWidgetEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
