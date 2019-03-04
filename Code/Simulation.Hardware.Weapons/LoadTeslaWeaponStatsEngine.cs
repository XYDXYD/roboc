using Simulation.Hardware.Weapons.Tesla;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LoadTeslaWeaponStatsEngine : SingleEntityViewEngine<LoadTeslaWeaponStatsNode>, IInitialize
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

		protected override void Add(LoadTeslaWeaponStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetTeslaWeaponStats(obj, weaponStats);
		}

		protected override void Remove(LoadTeslaWeaponStatsNode obj)
		{
		}

		private void SetTeslaWeaponStats(LoadTeslaWeaponStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			PopulateTeslaDamageStats(weaponStatsNode.teslaDamageStats, weaponStats);
			PopulateDamageStats(weaponStatsNode.projectileDamageStats, weaponStats);
			PopulateWeaponFireCost(weaponStatsNode.weaponFireCostComponent, weaponStats);
			PopulateFireTimingStats(weaponStatsNode.fireTimingStats, weaponStats, weaponStatsNode.itemDescriptorComponent.itemDescriptor);
		}

		private void PopulateTeslaDamageStats(ITeslaDamageStats teslaDamageStats, WeaponStatsData weaponStats)
		{
			teslaDamageStats.teslaCharges = weaponStats.teslaCharges;
			teslaDamageStats.teslaDamage = weaponStats.teslaDamage;
		}

		private void PopulateDamageStats(IProjectileDamageStatsComponent projectileDamageStats, WeaponStatsData weaponStats)
		{
			projectileDamageStats.damage = weaponStats.damageInflicted;
			projectileDamageStats.protoniumDamageScale = weaponStats.protoniumDamageScale;
			projectileDamageStats.damageBuff = 1f;
			projectileDamageStats.damageMultiplier = 1f;
		}

		private void PopulateWeaponFireCost(IWeaponFireCostComponent weaponFireCostComponent, WeaponStatsData weaponStats)
		{
			weaponFireCostComponent.weaponFireCost = weaponStats.manaCost;
		}

		private void PopulateFireTimingStats(IFireTimingComponent fireTimingStats, WeaponStatsData weaponStats, ItemDescriptor itemDescriptor)
		{
			fireTimingStats.delayBetweenShots = weaponStats.cooldownBetweenShots;
			fireTimingStats.groupFirePeriod = weaponStats.groupFireScales;
			fireTimingStats.timingsLoaded.Dispatch(ref itemDescriptor);
		}
	}
}
