using Svelto.ECS;
using Svelto.IoC;

namespace Simulation.SinglePlayer
{
	internal class AILegController : LegController
	{
		[Inject]
		public IEntityFactory engineRoot
		{
			get;
			private set;
		}

		[Inject]
		public PlayerNamesContainer playerNamesContainer
		{
			get;
			private set;
		}
	}
}
