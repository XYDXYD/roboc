using Simulation.Hardware.Weapons.RocketLauncher;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal sealed class LoadLockOnWeaponStatsEngine : SingleEntityViewEngine<LoadRocketLauncherStatsNode>, IInitialize
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

		protected override void Add(LoadRocketLauncherStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetRocketLauncherWeaponStats(obj, weaponStats);
		}

		protected override void Remove(LoadRocketLauncherStatsNode obj)
		{
		}

		private void SetRocketLauncherWeaponStats(LoadRocketLauncherStatsNode weaponStatsNode, WeaponStatsData weaponStats)
		{
			IHomingProjectileStatsComponent homingProjectileStats = weaponStatsNode.homingProjectileStats;
			ILockOnTargetingParametersComponent lockOnComponent = weaponStatsNode.lockOnComponent;
			PopulateHomingProjectileStats(homingProjectileStats, weaponStats);
			PopulateLockOnStats(lockOnComponent, weaponStats);
		}

		private void PopulateHomingProjectileStats(IHomingProjectileStatsComponent homingProjectileStats, WeaponStatsData weaponStats)
		{
			homingProjectileStats.maxRotationSpeedRad = weaponStats.maxRotationSpeed * ((float)Math.PI / 180f);
			homingProjectileStats.maxRotationAccelerationRad = weaponStats.rotationAcceleration * ((float)Math.PI / 180f);
			homingProjectileStats.initialRotationSpeedRad = weaponStats.initialRotationSpeed * ((float)Math.PI / 180f);
		}

		private void PopulateLockOnStats(ILockOnTargetingParametersComponent lockOn, WeaponStatsData weaponStats)
		{
			lockOn.lockTime = weaponStats.lockTime;
			lockOn.fullLockReleaseTime = weaponStats.fullLockReleaseTime;
			lockOn.changeLockTime = weaponStats.changeLockTime;
		}
	}
}
