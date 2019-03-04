using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.RobotConfiguration
{
	internal sealed class RobotConfigurationDisplayEntityView : EntityView
	{
		public IRobotConfigurationShowHideComponent robotConfigurationDisplayComponent;

		public IDialogChoiceComponent dialogChoiceComponent;

		public IRobotControlCustomisationsComponent controlCustomisationComponent;

		public RobotConfigurationDisplayEntityView()
			: this()
		{
		}
	}
}
