using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LoadWeaponStatsEngine : SingleEntityViewEngine<LoadWeaponStatsNode>, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		private IDictionary<int, WeaponStatsData> _weaponStatsData;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			ILoadWeaponStatsRequest loadWeaponStatsRequest = serviceFactory.Create<ILoadWeaponStatsRequest>();
			loadWeaponStatsRequest.SetAnswer(new ServiceAnswer<IDictionary<int, WeaponStatsData>>(delegate(IDictionary<int, WeaponStatsData> statsData)
			{
				_weaponStatsData = statsData;
			})).Execute();
		}

		protected override void Add(LoadWeaponStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetWeaponStats(obj, weaponStats);
		}

		protected override void Remove(LoadWeaponStatsNode obj)
		{
		}

		private void SetWeaponStats(LoadWeaponStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			ItemDescriptor itemDescriptor = weaponStatsNode.itemDescriptorComponent.itemDescriptor;
			SetGenericWeaponStats(weaponStatsNode, weaponStats);
			IWeaponFireCostComponent weaponFireCostComponent = weaponStatsNode.weaponFireCostComponent;
			PopulateWeaponFireCost(weaponFireCostComponent, weaponStats);
			IFireTimingComponent fireTimingStats = weaponStatsNode.fireTimingStats;
			PopulateFireTimingStats(fireTimingStats, weaponStats, itemDescriptor);
			weaponStatsNode.weaponMisfireComponent.coolDownPenalty = weaponStats.smartRotationCooldown;
			weaponStatsNode.weaponMisfireComponent.misfireDebuffDuration = weaponStats.smartRotationExtraCooldownTime;
			weaponStatsNode.weaponMisfireComponent.misfireDebuffMaxStacks = weaponStats.smartRotationMaxStacks;
		}

		private void SetGenericWeaponStats(LoadWeaponStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			IWeaponAccuracyStatsComponent accuracyStats = weaponStatsNode.accuracyStats;
			IProjectileSpeedStatsComponent projectileSpeedStats = weaponStatsNode.projectileSpeedStats;
			IProjectileRangeComponent projectileRangeStats = weaponStatsNode.projectileRangeStats;
			IProjectileDamageStatsComponent projectileDamageStats = weaponStatsNode.projectileDamageStats;
			PopulateAccuracyStats(accuracyStats, weaponStats);
			PopulateProjectileSpeedStats(projectileSpeedStats, weaponStats);
			PopulateProjectileRangeStats(projectileRangeStats, weaponStats);
			PopulateDamageStats(projectileDamageStats, weaponStats);
		}

		private void PopulateAccuracyStats(IWeaponAccuracyStatsComponent accuracyStats, WeaponStatsData weaponStats)
		{
			accuracyStats.baseInAccuracyDegrees = weaponStats.baseInaccuracy;
			accuracyStats.baseAirInaccuracyDegrees = weaponStats.baseAirIncaauracy;
			accuracyStats.movementInAccuracyDegrees = weaponStats.movementInaccuracy;
			accuracyStats.movementMaxThresholdSpeed = weaponStats.movementMaxThresholdSpeed;
			accuracyStats.gunRotationThresholdSlow = weaponStats.gunRotationThresholdSlow;
			accuracyStats.movementInAccuracyDecayTime = weaponStats.movementInaccuracyDecayTime;
			accuracyStats.slowRotationInAccuracyDecayTime = weaponStats.slowRotationInaccuracyDecayTime;
			accuracyStats.quickRotationInAccuracyDecayTime = weaponStats.quickRotationInaccuracyDecayTime;
			accuracyStats.movementInAccuracyRecoveryTime = weaponStats.movementInaccuracyRecoveryTime;
			accuracyStats.repeatFireInAccuracyTotalDegrees = weaponStats.repeatFireInaccuracyTotalDegrees;
			accuracyStats.repeatFireInAccuracyDecayTime = weaponStats.repeatFireInaccuracyDecayTime;
			accuracyStats.repeatFireInAccuracyRecoveryTime = weaponStats.repeatFireInaccuracyRecoveryTime;
			accuracyStats.fireInstantAccuracyDecayDegrees = weaponStats.fireInstantAccuracyDecayDegrees;
			accuracyStats.accuracyNonRecoverTime = weaponStats.accuracyNonRecoverTime;
			accuracyStats.accuracyDecayTime = weaponStats.accuracyDecayTime;
		}

		private void PopulateProjectileSpeedStats(IProjectileSpeedStatsComponent projectileSpeedStats, WeaponStatsData weaponStats)
		{
			projectileSpeedStats.speed = weaponStats.projectileSpeed;
		}

		private void PopulateProjectileRangeStats(IProjectileRangeComponent projectileRangeStats, WeaponStatsData weaponStats)
		{
			projectileRangeStats.maxRange = weaponStats.projectileRange;
		}

		private void PopulateDamageStats(IProjectileDamageStatsComponent projectileDamageStats, WeaponStatsData weaponStats)
		{
			projectileDamageStats.damage = weaponStats.damageInflicted;
			projectileDamageStats.protoniumDamageScale = weaponStats.protoniumDamageScale;
			projectileDamageStats.damageBuff = 1f;
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
