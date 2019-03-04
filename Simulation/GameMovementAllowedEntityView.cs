using Svelto.ECS;

namespace Simulation
{
	internal class GameMovementAllowedEntityView : EntityView
	{
		public IGameStartedComponent gameStartedComponent;

		public GameMovementAllowedEntityView()
			: this()
		{
		}
	}
}
