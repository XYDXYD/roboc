using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class StackDamageBonusNode : EntityView
	{
		public IStackDamageComponent stackDamageComponent;

		public IProjectileOwnerComponent ownerComponent;

		public IEntitySourceComponent entitySourceComponent;

		public StackDamageBonusNode()
			: this()
		{
		}
	}
}
