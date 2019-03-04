using LobbyServiceLayer;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;

namespace Simulation
{
	internal class TLOG_PlayerKillTrackerDataCacheFactory_Tencent
	{
		private readonly IContainer _container;

		private readonly IEntityFactory _entityFactory;

		private readonly EnginesRoot _engineRoot;

		internal TLOG_PlayerKillTrackerDataCacheFactory_Tencent(IEntityFactory entityFactory, IContainer container, EnginesRoot root)
		{
			_engineRoot = root;
			_container = container;
			_entityFactory = entityFactory;
		}

		internal void BuildEntities()
		{
			int num = 0;
			TLOG_PlayerKillTrackerDataCacheImplementor_Tencent tLOG_PlayerKillTrackerDataCacheImplementor_Tencent = new TLOG_PlayerKillTrackerDataCacheImplementor_Tencent();
			object[] array = new object[1]
			{
				tLOG_PlayerKillTrackerDataCacheImplementor_Tencent
			};
			_entityFactory.BuildEntity<TLOG_PlayerKillTrackerDataCacheEntityDescriptor_Tencent>(num, array);
		}

		internal void BuildEngine()
		{
			TLOG_PlayerKillTrackerDataCacheEngine_Tencent tLOG_PlayerKillTrackerDataCacheEngine_Tencent = new TLOG_PlayerKillTrackerDataCacheEngine_Tencent(_container.Build<ILobbyRequestFactory>(), _container.Build<IServiceRequestFactory>(), _container.Build<MachinePreloader>());
			_engineRoot.AddEngine(tLOG_PlayerKillTrackerDataCacheEngine_Tencent);
		}
	}
}
