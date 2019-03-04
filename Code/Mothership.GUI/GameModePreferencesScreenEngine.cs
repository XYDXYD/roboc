using Services.Web.Photon;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.GUI
{
	internal class GameModePreferencesScreenEngine : SingleEntityViewEngine<GameModePreferencesScreenEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private IServiceRequestFactory _requestFactory;

		private LoadingIconPresenter _loadingIcon;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public GameModePreferencesScreenEngine(IServiceRequestFactory requestFactory, LoadingIconPresenter loadingIcon)
		{
			_requestFactory = requestFactory;
			_loadingIcon = loadingIcon;
		}

		public void Ready()
		{
		}

		protected override void Add(GameModePreferencesScreenEntityView entityView)
		{
			entityView.dialogChoiceComponent.validatePressed.NotifyOnValueSet((Action<int, bool>)OnValidateButtonPressed);
			entityView.preferencesComponent.preferences.NotifyOnValueSet((Action<int, GameModePreferences>)OnPreferencesChanged);
			entityView.showComponent.isShown.NotifyOnValueSet((Action<int, bool>)OnShow);
		}

		protected override void Remove(GameModePreferencesScreenEntityView entityView)
		{
			entityView.dialogChoiceComponent.validatePressed.StopNotify((Action<int, bool>)OnValidateButtonPressed);
			entityView.preferencesComponent.preferences.StopNotify((Action<int, GameModePreferences>)OnPreferencesChanged);
			entityView.showComponent.isShown.StopNotify((Action<int, bool>)OnShow);
		}

		private void OnShow(int entityId, bool show)
		{
			if (show)
			{
				TaskRunner.get_Instance().Run(RefreshGameModePreferences(entityId));
			}
			else
			{
				TaskRunner.get_Instance().Run(SavePreferences(entityId));
			}
		}

		private void OnPreferencesChanged(int sender, GameModePreferences prefs)
		{
			GameModePreferencesScreenEntityView gameModePreferencesScreenEntityView = default(GameModePreferencesScreenEntityView);
			if (entityViewsDB.TryQueryEntityView<GameModePreferencesScreenEntityView>(sender, ref gameModePreferencesScreenEntityView))
			{
				gameModePreferencesScreenEntityView.formValidationComponent.isFormValid = IsValid(gameModePreferencesScreenEntityView);
			}
		}

		private bool IsValid(GameModePreferencesScreenEntityView view)
		{
			return view.preferencesComponent.preferences.get_value().ContainsAny(view.preferencesComponent.availableGameModeTypes);
		}

		private void OnValidateButtonPressed(int sender, bool pressed)
		{
			if (pressed)
			{
				GameModePreferencesScreenEntityView gameModePreferencesScreenEntityView = entityViewsDB.QueryEntityView<GameModePreferencesScreenEntityView>(sender);
				gameModePreferencesScreenEntityView.showComponent.isShown.set_value(false);
			}
		}

		private IEnumerator RefreshGameModePreferences(int entityId)
		{
			LobbyType lobbyType = LobbyType.QuickPlay;
			IGetAvailableGameModesRequest request = _requestFactory.Create<IGetAvailableGameModesRequest>();
			request.Inject(lobbyType);
			TaskService<List<GameModeType>> task = new TaskService<List<GameModeType>>(request);
			_loadingIcon.NotifyLoading("LoadingGameModePreferences");
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIcon.NotifyLoading("LoadingGameModePreferences");
			}, delegate
			{
				_loadingIcon.NotifyLoadingDone("LoadingGameModePreferences");
			}).GetEnumerator();
			_loadingIcon.NotifyLoadingDone("LoadingGameModePreferences");
			if (task.succeeded)
			{
				List<GameModeType> availableGameModeTypes = task.result;
				IEnumerator en = FetchGameModePreferences();
				yield return en;
				if (en.Current is GameModePreferences)
				{
					GameModePreferences prefs = (GameModePreferences)en.Current;
					GameModePreferencesScreenEntityView view = entityViewsDB.QueryEntityView<GameModePreferencesScreenEntityView>(entityId);
					view.preferencesComponent.availableGameModeTypes = availableGameModeTypes;
					view.preferencesComponent.preferences.set_value(prefs);
				}
			}
		}

		private IEnumerator FetchGameModePreferences()
		{
			IGetGameModePreferencesRequest request = _requestFactory.Create<IGetGameModePreferencesRequest>();
			TaskService<GameModePreferences> task = new TaskService<GameModePreferences>(request);
			_loadingIcon.NotifyLoading("LoadingGameModePreferences");
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIcon.NotifyLoading("LoadingGameModePreferences");
			}, delegate
			{
				_loadingIcon.NotifyLoadingDone("LoadingGameModePreferences");
			}).GetEnumerator();
			_loadingIcon.NotifyLoadingDone("LoadingGameModePreferences");
			if (task.succeeded)
			{
				yield return task.result;
			}
		}

		private IEnumerator SavePreferences(int entityId)
		{
			GameModePreferencesScreenEntityView view = entityViewsDB.QueryEntityView<GameModePreferencesScreenEntityView>(entityId);
			if (!IsValid(view))
			{
				yield break;
			}
			GameModePreferences prefs = view.preferencesComponent.preferences.get_value();
			IEnumerator en = FetchGameModePreferences();
			yield return en;
			if (en.Current is GameModePreferences)
			{
				GameModePreferences a = (GameModePreferences)en.Current;
				if (a == prefs)
				{
					yield break;
				}
			}
			ISaveGameModePreferencesRequest request = _requestFactory.Create<ISaveGameModePreferencesRequest>();
			request.Inject(prefs);
			TaskService task = new TaskService(request);
			_loadingIcon.NotifyLoading("SavingGameModePreferences");
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIcon.NotifyLoading("LoadingGameModePreferences");
			}, delegate
			{
				_loadingIcon.NotifyLoadingDone("LoadingGameModePreferences");
			}).GetEnumerator();
			_loadingIcon.NotifyLoadingDone("SavingGameModePreferences");
			if (task.succeeded)
			{
				IGetGameModePreferencesRequest getGameModePreferencesRequest = _requestFactory.Create<IGetGameModePreferencesRequest>();
				getGameModePreferencesRequest.ClearCache();
			}
		}
	}
}
