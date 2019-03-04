using Svelto.ECS;

namespace Simulation
{
	internal class AutoHealGuiEntityView : EntityView
	{
		public IAutoHealGuiComponent autoHealGuiComponent;

		public AutoHealGuiEntityView()
			: this()
		{
		}
	}
}
