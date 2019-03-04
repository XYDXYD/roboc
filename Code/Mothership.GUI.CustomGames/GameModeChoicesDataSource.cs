using Robocraft.GUI;
using Services.Web;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.GUI.CustomGames
{
	internal class GameModeChoicesDataSource : DataSourceBase
	{
		private Dictionary<GameModeType, List<string>> _allowedMapsPerGameMode = new Dictionary<GameModeType, List<string>>();

		private IServiceRequestFactory _serviceFactory;

		public GameModeChoicesDataSource(IServiceRequestFactory serviceRequestFactory)
		{
			_serviceFactory = serviceRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return _allowedMapsPerGameMode.Count;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (typeof(T) == typeof(GameModeType))
			{
				if (uniqueIdentifier1 < 0 || uniqueIdentifier1 >= _allowedMapsPerGameMode.Count)
				{
					return default(T);
				}
				IEnumerator enumerator = _allowedMapsPerGameMode.GetEnumerator();
				enumerator.MoveNext();
				for (int i = 0; i < uniqueIdentifier1; i++)
				{
					enumerator.MoveNext();
				}
				return (T)Convert.ChangeType(((KeyValuePair<GameModeType, List<string>>)enumerator.Current).Key, typeof(T));
			}
			return default(T);
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			TaskRunner.get_Instance().Run(RefreshDataWithEmumerator(OnSuccess, OnFailed));
		}

		public IEnumerator RefreshDataWithEmumerator(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IEnumerator enumerator = RefreshData();
			yield return enumerator;
			object result = enumerator.Current;
			if (result == null)
			{
				OnSuccess();
				TriggerAllDataChanged();
			}
			else
			{
				OnFailed(result as ServiceBehaviour);
			}
		}

		public override IEnumerator RefreshData()
		{
			ILoadCustomGamesAllowedMapsRequest loadCustomGameRequest = _serviceFactory.Create<ILoadCustomGamesAllowedMapsRequest>();
			TaskService<CustomGamesAllowedMapsData> taskService = new TaskService<CustomGamesAllowedMapsData>(loadCustomGameRequest);
			yield return taskService;
			if (!taskService.succeeded)
			{
				yield return taskService.behaviour;
				yield break;
			}
			_allowedMapsPerGameMode = new Dictionary<GameModeType, List<string>>();
			foreach (KeyValuePair<GameModeType, List<string>> allowedMap in taskService.result.AllowedMaps)
			{
				if (allowedMap.Value.Count > 0)
				{
					_allowedMapsPerGameMode.Add(allowedMap.Key, allowedMap.Value);
				}
			}
			TriggerAllDataChanged();
		}
	}
}
