using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal class TeslaWeaponFireEngine : SingleEntityViewEngine<TeslaRamNode>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		private void FireWeapon(IShootingComponent sender, int weaponId)
		{
			TeslaRamNode teslaRamNode = default(TeslaRamNode);
			if (entityViewsDB.TryQueryEntityView<TeslaRamNode>(weaponId, ref teslaRamNode))
			{
				teslaRamNode.shootingComponent.shotIsGoingToBeFired.Dispatch(ref weaponId);
			}
		}

		protected override void Add(TeslaRamNode node)
		{
			node.shootingComponent.shotIsReadyToFire.subscribers += FireWeapon;
		}

		protected override void Remove(TeslaRamNode node)
		{
			node.shootingComponent.shotIsReadyToFire.subscribers -= FireWeapon;
		}
	}
}
