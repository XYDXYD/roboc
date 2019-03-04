using Robocraft.GUI;
using Services.Web;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.GUI.CustomGames
{
	internal class MapChoicesDataSource : DataSourceBase
	{
		private List<MapChoiceDataEntry> _allowedMapsData = new List<MapChoiceDataEntry>();

		private IServiceRequestFactory _serviceFactory;

		public MapChoicesDataSource(IServiceRequestFactory serviceRequestFactory)
		{
			_serviceFactory = serviceRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return _allowedMapsData.Count;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (typeof(T) == typeof(MapChoiceDataEntry))
			{
				if (uniqueIdentifier1 < 0 || uniqueIdentifier1 >= _allowedMapsData.Count)
				{
					return default(T);
				}
				MapChoiceDataEntry mapChoiceDataEntry = _allowedMapsData[uniqueIdentifier1];
				return (T)Convert.ChangeType(mapChoiceDataEntry, typeof(T));
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
			TaskService<CustomGamesAllowedMapsData> loadAllowedMapsTask = new TaskService<CustomGamesAllowedMapsData>(loadCustomGameRequest);
			yield return loadAllowedMapsTask;
			if (!loadAllowedMapsTask.succeeded)
			{
				yield return loadAllowedMapsTask.behaviour;
				yield break;
			}
			IRetrieveCustomGameSessionRequest retrieveRequest = _serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> refreshSessionTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveRequest);
			yield return refreshSessionTask;
			if (!refreshSessionTask.succeeded)
			{
				yield return refreshSessionTask.behaviour;
			}
			else
			{
				if (!refreshSessionTask.succeeded || !loadAllowedMapsTask.succeeded)
				{
					yield break;
				}
				Dictionary<GameModeType, List<string>> dictionary = new Dictionary<GameModeType, List<string>>();
				foreach (KeyValuePair<GameModeType, List<string>> allowedMap in loadAllowedMapsTask.result.AllowedMaps)
				{
					if (allowedMap.Value.Count > 0)
					{
						dictionary.Add(allowedMap.Key, allowedMap.Value);
					}
				}
				_allowedMapsData = new List<MapChoiceDataEntry>();
				if (refreshSessionTask.result.Data != null)
				{
					string value = refreshSessionTask.result.Data.Config["GameMode"];
					GameModeType key = (GameModeType)Enum.Parse(typeof(GameModeType), value, ignoreCase: false);
					if (dictionary.ContainsKey(key))
					{
						foreach (string item2 in dictionary[key])
						{
							if (loadAllowedMapsTask.result.MapNameStrings.ContainsKey(item2))
							{
								string key2 = loadAllowedMapsTask.result.MapNameStrings[item2];
								MapChoiceDataEntry item = new MapChoiceDataEntry(item2, StringTableBase<StringTable>.Instance.GetString(key2));
								_allowedMapsData.Add(item);
							}
						}
					}
					else
					{
						_allowedMapsData = new List<MapChoiceDataEntry>();
					}
					TriggerAllDataChanged();
				}
			}
		}
	}
}
