using Robocraft.GUI;
using Services;
using Services.Web;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utility;

namespace Mothership.GUI
{
	internal class BrawlDetailsDataSource : DataSourceBase
	{
		private Dictionary<string, Texture2D> _loadedTextures = new Dictionary<string, Texture2D>();

		private Texture2D _fallbackBrawlImage;

		private bool _firstVictoryResult;

		private BrawlClientParameters _parameters;

		private BrawlGameParameters _details;

		private IServiceRequestFactory _serviceFactory;

		private bool _invalidateData;

		public BrawlDetailsDataSource(IServiceRequestFactory serviceRequestFactory)
		{
			_serviceFactory = serviceRequestFactory;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return (_parameters != null) ? 1 : 0;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (typeof(T) == typeof(Texture2D))
			{
				return (T)Convert.ChangeType(GetTexture((BrawlDetailsFields)uniqueIdentifier1), typeof(Texture2D));
			}
			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(GetString((BrawlDetailsFields)uniqueIdentifier1), typeof(T));
			}
			if (typeof(T) == typeof(bool))
			{
				switch (uniqueIdentifier1)
				{
				case 0:
					return (T)Convert.ChangeType(_parameters.IsLocked, typeof(T));
				case 11:
					return (T)Convert.ChangeType(_firstVictoryResult, typeof(T));
				}
			}
			else
			{
				if (typeof(T) == typeof(int))
				{
					return (T)Convert.ChangeType(GetInt((BrawlDetailsFields)uniqueIdentifier1), typeof(T));
				}
				if (typeof(T) == typeof(float))
				{
					return (T)Convert.ChangeType(GetFloat((BrawlDetailsFields)uniqueIdentifier1), typeof(T));
				}
				if (typeof(T) == typeof(GameModeType) && uniqueIdentifier1 == 10 && _details != null)
				{
					return (T)Convert.ChangeType(_details.commonParameters.GameMode, typeof(T));
				}
			}
			Console.LogError("Cannot convert data field " + (BrawlDetailsFields)uniqueIdentifier1 + " to " + typeof(T));
			return default(T);
		}

		private string GetString(BrawlDetailsFields field)
		{
			if (_details == null)
			{
				return "---";
			}
			switch (field)
			{
			case BrawlDetailsFields.TITLE:
				return Localization.Get(_details.commonParameters.NameString, true);
			case BrawlDetailsFields.DESCRIPTION:
				return Localization.Get(_details.commonParameters.NameDesc, true);
			case BrawlDetailsFields.RULES:
				return GetDisplayedRules();
			case BrawlDetailsFields.BONUS_SEASON_XP_MULTIPLIER_STRING:
			{
				string text = Localization.Get("strBrawlSeasonXPString", true);
				if (text.Contains("{0}"))
				{
					return string.Format(text, _details.XpMultiplierForFirstPlay.ToString("n0"));
				}
				return text;
			}
			case BrawlDetailsFields.GAME_MODE:
				return _details.commonParameters.GameMode.ToString();
			default:
				Console.LogError("Cannot convert data field " + field + " to string");
				return null;
			}
		}

		private string GetDisplayedRules()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _details.commonParameters.RuleStrings.Length; i++)
			{
				stringBuilder.Append("- ");
				string value = LocalizeRule(_details.commonParameters.RuleStrings[i], _details.commonParameters.RuleParameters[i]);
				stringBuilder.Append(value);
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}

		private static string LocalizeRule(string str, string[] vars)
		{
			string text = Localization.Get(str, true);
			if (vars.Length != 0)
			{
				object[] array = new object[vars.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = vars[i];
					double result = 0.0;
					if (!double.TryParse(text2, out result))
					{
						text2 = Localization.Get(text2, true);
					}
					array[i] = text2;
				}
				text = string.Format(text, array);
			}
			return text;
		}

		private Texture2D GetTexture(BrawlDetailsFields field)
		{
			if (_details == null)
			{
				return _fallbackBrawlImage;
			}
			switch (field)
			{
			case BrawlDetailsFields.DETAILS_IMAGE:
				if (_loadedTextures.ContainsKey(_details.BrawlDetailsImageName) && _loadedTextures[_details.BrawlDetailsImageName] != null)
				{
					return _loadedTextures[_details.BrawlDetailsImageName];
				}
				return _fallbackBrawlImage;
			case BrawlDetailsFields.DETAILS_IMAGE_OFF:
				if (_loadedTextures.ContainsKey(_details.BrawlDetailsImageOffName) && _loadedTextures[_details.BrawlDetailsImageOffName] != null)
				{
					return _loadedTextures[_details.BrawlDetailsImageOffName];
				}
				return _fallbackBrawlImage;
			case BrawlDetailsFields.DETAILS_TOP_IMAGE:
				if (_loadedTextures.ContainsKey(_details.BrawlDetailsTopImageName) && _loadedTextures[_details.BrawlDetailsTopImageName] != null)
				{
					return _loadedTextures[_details.BrawlDetailsTopImageName];
				}
				return _fallbackBrawlImage;
			default:
				return null;
			}
		}

		private int GetInt(BrawlDetailsFields field)
		{
			if (_details == null)
			{
				return 0;
			}
			Console.LogError("Cannot convert data field " + field + " to int");
			return 0;
		}

		private float GetFloat(BrawlDetailsFields field)
		{
			if (_details == null)
			{
				return 0f;
			}
			if (field == BrawlDetailsFields.BONUS_SEASON_XP_MULTIPLIER)
			{
				return _details.XpMultiplierForFirstPlay;
			}
			Console.LogError("Cannot convert data field " + field + " to float");
			return 0f;
		}

		public void InvalidateCache()
		{
			_invalidateData = true;
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
			}
			else
			{
				OnFailed(result as ServiceBehaviour);
			}
		}

		public override IEnumerator RefreshData()
		{
			IGetBrawlParametersRequest request = _serviceFactory.Create<IGetBrawlParametersRequest>();
			if (_invalidateData)
			{
				request.ClearCache();
				_invalidateData = false;
			}
			TaskService<GetBrawlRequestResult> brawlTask = new TaskService<GetBrawlRequestResult>(request);
			yield return brawlTask;
			if (!brawlTask.succeeded)
			{
				yield return brawlTask.behaviour;
				yield break;
			}
			ILoadFirstBrawlVictoryPendingRequest firstVictoryRequest = _serviceFactory.Create<ILoadFirstBrawlVictoryPendingRequest>();
			firstVictoryRequest.ClearCache();
			firstVictoryRequest.Inject(brawlTask.result.BrawlParameters.BrawlNumber);
			TaskService<bool> firstVictoryTask = new TaskService<bool>(firstVictoryRequest);
			yield return firstVictoryTask;
			if (!firstVictoryTask.succeeded)
			{
				yield return firstVictoryTask.behaviour;
				yield break;
			}
			OnGotBrawlData(firstVictoryResult: firstVictoryTask.result, result: brawlTask.result);
			if (_details != null)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)GetAllImagesParallel);
			}
			yield return null;
		}

		private IEnumerator GetAllImagesParallel()
		{
			ParallelTaskCollection collection = new ParallelTaskCollection();
			collection.Add(ExecuteLoadBrawlImageRequest(_details.BrawlDetailsImageName));
			collection.Add(ExecuteLoadBrawlImageRequest(_details.BrawlDetailsImageOffName));
			collection.Add(ExecuteLoadBrawlImageRequest(_details.BrawlDetailsTopImageName));
			yield return collection;
			TriggerAllDataChanged();
			yield return null;
		}

		private IEnumerator ExecuteLoadBrawlImageRequest(string imageToLoad)
		{
			ILoadImageTextureRequest loadImageRequest = _serviceFactory.Create<ILoadImageTextureRequest>();
			loadImageRequest.Inject(new LoadImageDependency(imageToLoad));
			TaskService<Texture2D> task = loadImageRequest.AsTask();
			yield return task;
			if (task.succeeded)
			{
				_loadedTextures[imageToLoad] = task.result;
				yield break;
			}
			_loadedTextures[imageToLoad] = null;
			RemoteLogger.Error("Error fetching brawl image from CDN", "failed to fetch brawl image from the CDN: ", imageToLoad);
		}

		public void SetFallbackBrawlImage(Texture2D image)
		{
			_fallbackBrawlImage = image;
		}

		private void OnGotBrawlData(GetBrawlRequestResult result, bool firstVictoryResult)
		{
			_firstVictoryResult = firstVictoryResult;
			_parameters = result.BrawlParameters;
			_details = result.Details;
			if (_parameters.IsLocked)
			{
			}
		}
	}
}
