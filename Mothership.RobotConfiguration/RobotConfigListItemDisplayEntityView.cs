using Svelto.ECS;

namespace Mothership.RobotConfiguration
{
	internal sealed class RobotConfigListItemDisplayEntityView : EntityView
	{
		public IRobotConfigListItemShowHideComponent robotListShowHideComponent;

		public IRobotConfigListItemSetupComponent setupComponent;

		public RobotConfigListItemDisplayEntityView()
			: this()
		{
		}
	}
}
