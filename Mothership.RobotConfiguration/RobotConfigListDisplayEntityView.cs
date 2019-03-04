using Svelto.ECS;

namespace Mothership.RobotConfiguration
{
	internal sealed class RobotConfigListDisplayEntityView : EntityView
	{
		public IRobotConfigListDisplayComponent listComponent;

		public RobotConfigListDisplayEntityView()
			: this()
		{
		}
	}
}
