using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class RobotShakeDamageEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _entityViewsToBuild;

		static RobotShakeDamageEntityDescriptor()
		{
			_entityViewsToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[1]
			{
				new EntityViewBuilder<RobotShakeDamageEntityView>()
			};
		}

		public RobotShakeDamageEntityDescriptor()
			: this(_entityViewsToBuild)
		{
		}
	}
}
