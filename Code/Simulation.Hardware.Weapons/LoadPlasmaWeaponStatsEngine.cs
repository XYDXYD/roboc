using Simulation.Hardware.Weapons.Plasma;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LoadPlasmaWeaponStatsEngine : SingleEntityViewEngine<LoadPlasmaWeaponStatsNode>, IInitialize
	{
		private IDictionary<int, WeaponStatsData> _weaponStatsData;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			ILoadWeaponStatsRequest loadWeaponStatsRequest = serviceFactory.Create<ILoadWeaponStatsRequest>();
			loadWeaponStatsRequest.SetAnswer(new ServiceAnswer<IDictionary<int, WeaponStatsData>>(delegate(IDictionary<int, WeaponStatsData> statsData)
			{
				_weaponStatsData = statsData;
			})).Execute();
		}

		protected override void Add(LoadPlasmaWeaponStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetPlasmaWeaponStats(obj, weaponStats);
		}

		protected override void Remove(LoadPlasmaWeaponStatsNode obj)
		{
		}

		private void SetPlasmaWeaponStats(LoadPlasmaWeaponStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			IPlasmaProjectileStatsComponent plasmaProjectileStats = weaponStatsNode.plasmaProjectileStats;
			PopulatePlasmaProjectileStats(plasmaProjectileStats, weaponStats);
		}

		private void PopulatePlasmaProjectileStats(IPlasmaProjectileStatsComponent plasmaProjectileStats, WeaponStatsData weaponStats)
		{
			plasmaProjectileStats.explosionRadius = weaponStats.damageRadius;
			plasmaProjectileStats.currentExplosionRadius = weaponStats.damageRadius;
			plasmaProjectileStats.startingRadiusScale = weaponStats.plasmaStartingRadiusScale;
			plasmaProjectileStats.timeToFullDamage = weaponStats.plasmaTimeToFullDamage;
		}
	}
}
