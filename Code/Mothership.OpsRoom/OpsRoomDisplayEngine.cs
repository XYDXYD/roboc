using Mothership.DailyQuest;
using Simulation;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.OpsRoom
{
	internal class OpsRoomDisplayEngine : SingleEntityViewEngine<OpsRoomDisplayEntityView>, IQueryingEntityViewEngine, IGUIDisplay, IEngine, IComponent
	{
		private readonly IGUIInputControllerMothership _guiInputController;

		private readonly IServiceRequestFactory _serviceRequestFactory;

		private readonly LoadingIconPresenter _loadingIconPresenter;

		private readonly DailyQuestController _dailyQuestController;

		public GuiScreens screenType => GuiScreens.OpsRoom;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyGUINoSwitching;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public OpsRoomDisplayEngine(IGUIInputControllerMothership guiInputController, IServiceRequestFactory serviceRequestFactory, LoadingIconPresenter loadingIconPresenter, DailyQuestController dailyQuestController)
		{
			_guiInputController = guiInputController;
			_serviceRequestFactory = serviceRequestFactory;
			_loadingIconPresenter = loadingIconPresenter;
			_dailyQuestController = dailyQuestController;
		}

		public void EnableBackground(bool enable)
		{
		}

		public void Ready()
		{
		}

		protected override void Add(OpsRoomDisplayEntityView entityView)
		{
			IOpsRoomDisplayComponent displayComponent = entityView.displayComponent;
			displayComponent.techTreeClicked.NotifyOnValueSet((Action<int, bool>)OpenTechTreeUI);
			displayComponent.missionClicked.NotifyOnValueSet((Action<int, bool>)OpenMissionUI);
			displayComponent.tierRanksClicked.NotifyOnValueSet((Action<int, bool>)OpenTierRanksUI);
		}

		protected override void Remove(OpsRoomDisplayEntityView entityView)
		{
			IOpsRoomDisplayComponent displayComponent = entityView.displayComponent;
			displayComponent.techTreeClicked.StopNotify((Action<int, bool>)OpenTechTreeUI);
			displayComponent.missionClicked.StopNotify((Action<int, bool>)OpenMissionUI);
			displayComponent.tierRanksClicked.StopNotify((Action<int, bool>)OpenTierRanksUI);
		}

		public GUIShowResult Show()
		{
			OpsRoomDisplayEntityView entityView = GetEntityView();
			entityView.displayComponent.Show();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			OpsRoomDisplayEntityView entityView = GetEntityView();
			entityView.displayComponent.Hide();
			return true;
		}

		public bool IsActive()
		{
			OpsRoomDisplayEntityView entityView = GetEntityView();
			return entityView.displayComponent.gameObject.get_activeSelf();
		}

		private OpsRoomDisplayEntityView GetEntityView()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityViewsDB.QueryEntityViews<OpsRoomDisplayEntityView>().get_Item(0);
		}

		private void OpenTierRanksUI(int entityID, bool clicked)
		{
			_guiInputController.ShowScreen(GuiScreens.LeagueScreen);
		}

		private void OpenMissionUI(int entityID, bool clicked)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			_guiInputController.ShowScreen(GuiScreens.DailyQuestScreen);
			if (entityViewsDB.QueryEntityViews<OpsRoomShowQuestsCTAEntityView>().get_Item(0).opsRoomCTAValuesComponent.newQuests.get_value() > 0)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)MarkQuestsAsSeen);
			}
		}

		private IEnumerator MarkQuestsAsSeen()
		{
			TaskService task = _serviceRequestFactory.Create<IMarkQuestsAsSeenRequest>().AsTask();
			yield return new HandleTaskServiceWithError(task, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("MarkQuestsAsSeen");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoading("MarkQuestsAsSeen");
			}).GetEnumerator();
			yield return _dailyQuestController.LoadData();
		}

		private void OpenTechTreeUI(int entityID, bool clicked)
		{
			_guiInputController.ShowScreen(GuiScreens.TechTree);
		}
	}
}
