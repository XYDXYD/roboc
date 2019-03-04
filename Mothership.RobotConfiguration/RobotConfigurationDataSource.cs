using Services.Web.Photon;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership.RobotConfiguration
{
	internal sealed class RobotConfigurationDataSource
	{
		private AllCustomisationsResponse _allCustomisationsInfo;

		private List<CustomisationsEntry> _ownedBaySkinCustomisations;

		private List<CustomisationsEntry> _ownedSpawnFXCustomisations;

		private List<CustomisationsEntry> _ownedDeathFXCustomisations;

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		public List<CustomisationsEntry> DeathEfffectCustomisations => _allCustomisationsInfo.AllDeathCustomisations;

		public List<CustomisationsEntry> SpawnEfffectCustomisations => _allCustomisationsInfo.AllSpawnCustomisations;

		public List<CustomisationsEntry> BaySkinCustomisations => _allCustomisationsInfo.AllSkinCustomisations;

		public List<CustomisationsEntry> OwnedDeathEfffectCustomisations => _ownedDeathFXCustomisations;

		public List<CustomisationsEntry> OwnedSpawnEfffectCustomisations => _ownedSpawnFXCustomisations;

		public List<CustomisationsEntry> OwnedBaySkinCustomisations => _ownedBaySkinCustomisations;

		public IEnumerator LoadData()
		{
			loadingIconPresenter.NotifyLoading("RobotConfiguration");
			ILoadAllCustomisationInfoRequest allCustomisationsLoadReq = serviceFactory.Create<ILoadAllCustomisationInfoRequest>();
			TaskService<AllCustomisationsResponse> allCustomisationsLoadTask = new TaskService<AllCustomisationsResponse>(allCustomisationsLoadReq);
			yield return allCustomisationsLoadTask;
			_allCustomisationsInfo = allCustomisationsLoadTask.result;
			IGetGarageBayUniqueIdRequest getRobotUniqueIdRequest = serviceFactory.Create<IGetGarageBayUniqueIdRequest>();
			getRobotUniqueIdRequest.ClearCache();
			TaskService<UniqueSlotIdentifier> getRobotUniqueIdTask = new TaskService<UniqueSlotIdentifier>(getRobotUniqueIdRequest);
			yield return new HandleTaskServiceWithError(getRobotUniqueIdTask, delegate
			{
				loadingIconPresenter.NotifyLoading("RobotConfiguration");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			}).GetEnumerator();
			UniqueSlotIdentifier currentSlotId = getRobotUniqueIdTask.result;
			GetRobotBayCustomisationsResponse result = null;
			IGetRobotBayCustomisationsRequest request = serviceFactory.Create<IGetRobotBayCustomisationsRequest>();
			request.ClearCache();
			request.Inject(currentSlotId.ToString());
			TaskService<GetRobotBayCustomisationsResponse> task = new TaskService<GetRobotBayCustomisationsResponse>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIconPresenter.NotifyLoading("RobotConfiguration");
			}, delegate
			{
				loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			}).GetEnumerator();
			if (task.succeeded)
			{
				result = task.result;
			}
			loadingIconPresenter.NotifyLoadingDone("RobotConfiguration");
			ExtractData();
			yield return result;
		}

		private void ExtractData()
		{
			_ownedBaySkinCustomisations = new List<CustomisationsEntry>();
			_ownedDeathFXCustomisations = new List<CustomisationsEntry>();
			_ownedSpawnFXCustomisations = new List<CustomisationsEntry>();
			foreach (CustomisationsEntry allSkinCustomisation in _allCustomisationsInfo.AllSkinCustomisations)
			{
				for (int i = 0; i < _allCustomisationsInfo.OwnedBaySkinCustomisations.Length; i++)
				{
					if (_allCustomisationsInfo.OwnedBaySkinCustomisations[i] == allSkinCustomisation.id)
					{
						_ownedBaySkinCustomisations.Add(allSkinCustomisation);
					}
				}
			}
			foreach (CustomisationsEntry allDeathCustomisation in _allCustomisationsInfo.AllDeathCustomisations)
			{
				for (int j = 0; j < _allCustomisationsInfo.OwnedDeathFXCustomisations.Length; j++)
				{
					if (_allCustomisationsInfo.OwnedDeathFXCustomisations[j] == allDeathCustomisation.id)
					{
						_ownedDeathFXCustomisations.Add(allDeathCustomisation);
					}
				}
			}
			foreach (CustomisationsEntry allSpawnCustomisation in _allCustomisationsInfo.AllSpawnCustomisations)
			{
				for (int k = 0; k < _allCustomisationsInfo.OwnedSpawnFXCustomisations.Length; k++)
				{
					if (_allCustomisationsInfo.OwnedSpawnFXCustomisations[k] == allSpawnCustomisation.id)
					{
						_ownedSpawnFXCustomisations.Add(allSpawnCustomisation);
					}
				}
			}
		}

		public string GetSpawnEffect(string id)
		{
			string prefab = GetPrefab(id, SpawnEfffectCustomisations);
			if (prefab == null)
			{
				Console.LogError("Spawn effect not found! id = " + id);
				return "Spawn";
			}
			return prefab;
		}

		public string GetDeathEffect(string id)
		{
			string prefab = GetPrefab(id, DeathEfffectCustomisations);
			if (prefab == null)
			{
				Console.LogError("Death effect not found! id = " + id);
				return "Explosion";
			}
			return prefab;
		}

		private static string GetPrefab(string id, List<CustomisationsEntry> list)
		{
			foreach (CustomisationsEntry item in list)
			{
				if (item.id == id)
				{
					return item.simulationPrefab;
				}
			}
			return null;
		}
	}
}
