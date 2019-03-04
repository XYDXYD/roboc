using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Game.RoboPass.GUI.EntityViews
{
	internal class RoboPassXpGradeUIEntityView : EntityView
	{
		public IProgressBarUIComponent progressBarUIComponent;

		public ILabelUIComponent labelUIComponent;

		public RoboPassXpGradeUIEntityView()
			: this()
		{
		}
	}
}
