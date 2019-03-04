using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.GUI
{
	internal sealed class StatsHintPopupAreaEntityView : EntityView
	{
		public IMouseOverStateComponent mouseOverComponent;

		public ICubeTypeIDComponent cubeTypeIDComponent;

		public StatsHintPopupAreaEntityView()
			: this()
		{
		}
	}
}
