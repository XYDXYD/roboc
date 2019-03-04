using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadTeleportModuleStatsEngine : SingleEntityViewEngine<LoadTeleportModuleStatsNode>, IInitialize
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

		protected override void Add(LoadTeleportModuleStatsNode obj)
		{
			WeaponStatsData weaponStats = _weaponStatsData[obj.itemDescriptorComponent.itemDescriptor.GenerateKey()];
			SetModuleStats(obj, weaponStats);
		}

		protected override void Remove(LoadTeleportModuleStatsNode obj)
		{
		}

		private void SetModuleStats(LoadTeleportModuleStatsNode node, WeaponStatsData weaponStats)
		{
			node.settingsComponent.cameraDelay = weaponStats.cameraDelay;
			node.settingsComponent.cameraTime = weaponStats.cameraTime;
			node.settingsComponent.teleportTime = weaponStats.teleportTime;
		}
	}
}
