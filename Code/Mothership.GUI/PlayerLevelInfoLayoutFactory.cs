using Robocraft.GUI;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using UnityEngine;

namespace Mothership.GUI
{
	internal sealed class PlayerLevelInfoLayoutFactory
	{
		private IDataSource _playerLevelInfoDataSource;

		private ITopBarDisplay _topBarDisplay;

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IContainer container
		{
			private get;
			set;
		}

		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory requestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IComponentFactory genericComponentFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal HUDPlayerLevelPresenter hudPlayerLevelPresenter
		{
			private get;
			set;
		}

		public void Build(ITopBarDisplay topBarDisplay, IContainer container, PlayerLevelNeedRefreshObserver observer)
		{
			_topBarDisplay = topBarDisplay;
			OnLoadingSuccess("PlayerLevelInfo_Infinity", isInfinityVersion: true, observer);
		}

		private void BuildComponents(IPlayerLevelView playerLevelView, bool isInfinityVersion)
		{
			BuiltComponentElements builtComponentElements = genericComponentFactory.BuildComponent(GUIComponentType.TextLabel, _playerLevelInfoDataSource, playerLevelView.GetLabelGameObject(), makeInstance: false);
			builtComponentElements.componentController.SetName("Player Level Label");
			(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(0);
			builtComponentElements.componentController.Activate();
			if (isInfinityVersion)
			{
				builtComponentElements = genericComponentFactory.BuildComponent(GUIComponentType.TextLabel, _playerLevelInfoDataSource, playerLevelView.GetPlayerNameGameObject(), makeInstance: false);
				builtComponentElements.componentController.SetName("Player Name Label");
				(builtComponentElements.componentController as GenericTextLabelComponent).SetDataSourceIndex(3);
				builtComponentElements.componentController.Activate();
			}
			builtComponentElements = genericComponentFactory.BuildComponent(GUIComponentType.ProgressBar, _playerLevelInfoDataSource, playerLevelView.GetProgressGameObject(), makeInstance: false);
			builtComponentElements.componentController.SetName("Player Level Progress to next level");
			(builtComponentElements.componentController as GenericProgressBarComponent).SetDataSourceIndex(1);
			builtComponentElements.componentController.Activate();
		}

		private void OnLoadingSuccess(string playerLevelViewPrefabName, bool isInfinityVersion, PlayerLevelNeedRefreshObserver observer)
		{
			_playerLevelInfoDataSource = new PlayerLevelInfoDataSource(requestFactory);
			PlayerLevelController playerLevelController = container.Inject<PlayerLevelController>(new PlayerLevelController(_playerLevelInfoDataSource, observer));
			GameObject val = gameObjectFactory.Build(playerLevelViewPrefabName);
			IPlayerLevelView component = val.GetComponent<IPlayerLevelView>();
			playerLevelController.RegisterToContextNotifier(contextNotifier);
			playerLevelController.SetView(component);
			BuildComponents(component, isInfinityVersion);
			component.GetGameObject().get_transform().set_parent(_topBarDisplay.GetTopBar().PlayerLevelInfoTransform);
			val = gameObjectFactory.Build("BuildingXP");
			HUDPlayerLevelView component2 = val.GetComponent<HUDPlayerLevelView>();
			hudPlayerLevelPresenter.SetView(component2);
		}

		private void OnLoadingFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}
	}
}
