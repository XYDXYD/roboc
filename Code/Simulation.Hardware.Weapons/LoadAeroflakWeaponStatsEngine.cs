using Simulation.Hardware.Weapons.AeroFlak;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LoadAeroflakWeaponStatsEngine : SingleEntityViewEngine<LoadAeroflakWeaponStatsNode>, IInitialize
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

		protected override void Add(LoadAeroflakWeaponStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetAeroflakWeaponStats(obj, weaponStats);
		}

		private void SetAeroflakWeaponStats(LoadAeroflakWeaponStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			IAeroflakProjectileStatsComponent aeroflakProjectileStats = weaponStatsNode.aeroflakProjectileStats;
			IStackDamageStatsComponent stackDamageStats = weaponStatsNode.stackDamageStats;
			PopulateAeroflakProjectileStats(aeroflakProjectileStats, stackDamageStats, weaponStats);
		}

		private void PopulateAeroflakProjectileStats(IAeroflakProjectileStatsComponent aeroflakProjectileStats, IStackDamageStatsComponent damageStrikeStats, WeaponStatsData weaponStats)
		{
			aeroflakProjectileStats.damageProximityHit = weaponStats.aeroflakProximityDamage;
			aeroflakProjectileStats.damageRadius = weaponStats.aeroflakDamageRadius;
			aeroflakProjectileStats.explosionRadius = weaponStats.aeroflakExplosionRadius;
			aeroflakProjectileStats.groundClearance = weaponStats.aeroflakGroundClearance;
			damageStrikeStats.buffMaxStacks = weaponStats.aeroflakBuffMaxStacks;
			damageStrikeStats.buffDamagePerStack = weaponStats.aeroflakBuffDamagePerStack;
			damageStrikeStats.buffStackExpireTime = weaponStats.aeroflakBuffTimeToExpire;
		}

		protected override void Remove(LoadAeroflakWeaponStatsNode obj)
		{
		}
	}
}
