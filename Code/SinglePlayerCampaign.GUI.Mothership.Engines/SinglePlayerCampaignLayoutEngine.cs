using Mothership;
using Mothership.SinglePlayerCampaign;
using ServerStateServiceLayer;
using Services;
using Services.Web;
using Services.Web.Photon;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.GUI.Mothership.DataTypes;
using SinglePlayerCampaign.GUI.Mothership.EntityViews;
using SinglePlayerServiceLayer;
using Svelto.Command;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace SinglePlayerCampaign.GUI.Mothership.Engines
{
	internal class SinglePlayerCampaignLayoutEngine : MultiEntityViewsEngine<SinglePlayerCampaignLayoutEntityView, CampaignDataEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ISinglePlayerRequestFactory _spRequestFactory;

		private readonly LoadingIconPresenter _loadingIconPresenter;

		private readonly IServiceEventContainer _serviceEventContainer;

		private readonly IServiceRequestFactory _serviceRequestFactory;

		private readonly ICommandFactory _commandFactory;

		private readonly IGUIInputControllerMothership _guiInputController;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public SinglePlayerCampaignLayoutEngine(ISinglePlayerRequestFactory spRequestFactory, LoadingIconPresenter loadingIconPresenter, IServiceRequestFactory serviceRequestFactory, ICommandFactory commandFactory, IGUIInputControllerMothership guiInputController, IEventContainerFactory serverStateEventContainerFactory)
		{
			_spRequestFactory = spRequestFactory;
			_loadingIconPresenter = loadingIconPresenter;
			_serviceRequestFactory = serviceRequestFactory;
			_commandFactory = commandFactory;
			_guiInputController = guiInputController;
			_guiInputController.OnScreenStateChange += ToggleScreens;
			_serviceEventContainer = serverStateEventContainerFactory.Create();
			_serviceEventContainer.ListenTo<ICampaignsChangedEventListener>(LoadAndShowCampaigns);
		}

		public void Ready()
		{
		}

		protected override void Add(SinglePlayerCampaignLayoutEntityView entityView)
		{
			SinglePlayerCampaignInfoScreen infoScreen = entityView.layoutComponent.infoScreen;
			infoScreen.Initialise();
			infoScreen.backClicked.NotifyOnValueSet((Action<int, bool>)ShowCampaignList);
			infoScreen.startClicked.NotifyOnValueSet((Action<int, SelectedCampaignToStart>)CampaignSelected);
		}

		protected override void Remove(SinglePlayerCampaignLayoutEntityView entityView)
		{
			SinglePlayerCampaignInfoScreen infoScreen = entityView.layoutComponent.infoScreen;
			infoScreen.backClicked.StopNotify((Action<int, bool>)ShowCampaignList);
			infoScreen.startClicked.StopNotify((Action<int, SelectedCampaignToStart>)CampaignSelected);
		}

		protected override void Add(CampaignDataEntityView entityView)
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAndShowCampaignsEnumerator);
		}

		protected override void Remove(CampaignDataEntityView entityView)
		{
		}

		private IEnumerator LoadAndShowCampaignsEnumerator()
		{
			_loadingIconPresenter.NotifyLoading("LoadingSinglePlayerCampaigns");
			ISinglePlayerCampaignListComponent layoutComponent = entityViewsDB.QueryEntityViews<SinglePlayerCampaignLayoutEntityView>().get_Item(0).layoutComponent;
			UIGrid uiGrid = layoutComponent.uiGrid;
			GameObject uiGridGO = uiGrid.get_gameObject();
			IEnumerator enumerator = uiGridGO.get_transform().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = enumerator.Current;
					if (val.get_gameObject().get_activeSelf())
					{
						SinglePlayerCampaignImplementor component = val.get_gameObject().GetComponent<SinglePlayerCampaignImplementor>();
						component.campaignClicked.StopNotify((Action<int, Campaign?>)ShowCampaignSelected);
						val.get_gameObject().SetActive(false);
						Object.Destroy(val.get_gameObject());
					}
				}
			}
			finally
			{
				IDisposable disposable;
				IDisposable disposable2 = disposable = (enumerator as IDisposable);
				if (disposable != null)
				{
					disposable2.Dispose();
				}
			}
			ILoadSinglePlayerCampaignsRequest loadSinglePlayerCampaignsReq = _spRequestFactory.Create<ILoadSinglePlayerCampaignsRequest>();
			TaskService<GetCampaignsRequestResult> loadSinglePlayerCampaignsTS = loadSinglePlayerCampaignsReq.AsTask();
			HandleTaskServiceWithError handleTSWithError2 = new HandleTaskServiceWithError(loadSinglePlayerCampaignsTS, delegate
			{
				_loadingIconPresenter.NotifyLoading("LoadingSinglePlayerCampaigns");
			}, delegate
			{
				_loadingIconPresenter.NotifyLoadingDone("LoadingSinglePlayerCampaigns");
			});
			yield return handleTSWithError2.GetEnumerator();
			Campaign[] campaigns = loadSinglePlayerCampaignsTS.result.CampaignsGameParameters;
			Campaign[] array = campaigns;
			for (int i = 0; i < array.Length; i++)
			{
				Campaign campaign = array[i];
				GameObject go = NGUITools.AddChild(uiGridGO, layoutComponent.campaignTemplateGO);
				go.SetActive(true);
				SinglePlayerCampaignImplementor singlePlayerCampaignImplementor = go.GetComponent<SinglePlayerCampaignImplementor>();
				singlePlayerCampaignImplementor.Initialise();
				singlePlayerCampaignImplementor.campaignSet.set_value((Campaign?)campaign);
				singlePlayerCampaignImplementor.campaignClicked.NotifyOnValueSet((Action<int, Campaign?>)ShowCampaignSelected);
				bool[] difficultiesCompletedPerCampaign = campaign.difficultiesCompletedPerCampaign;
				int length = singlePlayerCampaignImplementor.WidgetCounterMaxValue = (int)difficultiesCompletedPerCampaign.LongLength;
				for (int j = 0; j < length; j++)
				{
					if (difficultiesCompletedPerCampaign[j])
					{
						singlePlayerCampaignImplementor.WidgetCounterValue = j;
					}
				}
				singlePlayerCampaignImplementor.campaignType = campaign.campaignType;
				ILoadImageTextureRequest loadImageTextureReq = _serviceRequestFactory.Create<ILoadImageTextureRequest>();
				loadImageTextureReq.Inject(new LoadImageDependency(campaign.CampaignImage, "CampaignDataUrl"));
				TaskService<Texture2D> loadImageTextureTS = loadImageTextureReq.AsTask();
				handleTSWithError2 = new HandleTaskServiceWithError(loadImageTextureTS, delegate
				{
					_loadingIconPresenter.NotifyLoading("LoadingSinglePlayerCampaigns");
				}, delegate
				{
					_loadingIconPresenter.NotifyLoadingDone("LoadingSinglePlayerCampaigns");
				});
				yield return handleTSWithError2.GetEnumerator();
				singlePlayerCampaignImplementor.imageTexture = loadImageTextureTS.result;
			}
			uiGrid.Reposition();
			_loadingIconPresenter.NotifyLoadingDone("LoadingSinglePlayerCampaigns");
		}

		private void ShowCampaignSelected(int entityID, Campaign? campaign)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (campaign.HasValue)
			{
				ISinglePlayerCampaignListComponent layoutComponent = entityViewsDB.QueryEntityViews<SinglePlayerCampaignLayoutEntityView>().get_Item(0).layoutComponent;
				layoutComponent.infoScreen.campaignSet.set_value(campaign);
				layoutComponent.infoScreen.show.set_value(true);
				layoutComponent.show.set_value(false);
			}
		}

		private void CampaignSelected(int entityID, SelectedCampaignToStart campaign)
		{
			if (campaign != null)
			{
				Console.Log("CampaignSelected: " + campaign.CampaignID + " Difficulty: " + campaign.Difficulty);
				_commandFactory.Build<TryQueueForSinglePlayerCampaign>().Inject(campaign).Execute();
			}
		}

		private void ShowCampaignList(int entityID, bool clicked)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (clicked)
			{
				ISinglePlayerCampaignListComponent layoutComponent = entityViewsDB.QueryEntityViews<SinglePlayerCampaignLayoutEntityView>().get_Item(0).layoutComponent;
				layoutComponent.show.set_value(true);
				layoutComponent.infoScreen.show.set_value(false);
			}
		}

		private void ToggleScreens()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (_guiInputController.GetActiveScreen() != GuiScreens.PlayScreen)
			{
				ISinglePlayerCampaignListComponent layoutComponent = entityViewsDB.QueryEntityViews<SinglePlayerCampaignLayoutEntityView>().get_Item(0).layoutComponent;
				layoutComponent.show.set_value(false);
				layoutComponent.infoScreen.show.set_value(false);
			}
		}

		private void LoadAndShowCampaigns()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAndShowCampaignsEnumerator);
		}
	}
}
