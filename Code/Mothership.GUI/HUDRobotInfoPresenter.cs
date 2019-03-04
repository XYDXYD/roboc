using CustomGames;
using Services.Web.Photon;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.GUI
{
	internal class HUDRobotInfoPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private HUDRobotInfoView _view;

		private HUDRobotInfoView.StyleVersion _garageStyle;

		private HUDRobotInfoView.StyleVersion _editStyle = HUDRobotInfoView.StyleVersion.EditMode;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal CustomGameStateObserver customGameStateObserver
		{
			private get;
			set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			guiInputController.OnScreenStateChange += ChangeHudViewAccordingToScreenState;
			customGameStateObserver.AddAction(new ObserverAction<CustomGameStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			guiInputController.OnScreenStateChange -= ChangeHudViewAccordingToScreenState;
			customGameStateObserver.RemoveAction(new ObserverAction<CustomGameStateDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		internal void RegisterView(HUDRobotInfoView view)
		{
			_view = view;
		}

		private void ChangeHudViewAccordingToScreenState()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)CheckIsInACustomGameSessionAndSetViewStyle);
		}

		private void OnCustomGameStateChanged(ref CustomGameStateDependency dependancy)
		{
			SetCustomGameState(dependancy.sessionId != null);
		}

		private IEnumerator CheckIsInACustomGameSessionAndSetViewStyle()
		{
			IRetrieveCustomGameSessionRequest request = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> task = new TaskService<RetrieveCustomGameSessionRequestData>(request);
			yield return task;
			SetCustomGameState(task.succeeded && task.result.Data != null);
		}

		private void SetCustomGameState(bool isInCustomGame)
		{
			if (isInCustomGame)
			{
				_garageStyle = HUDRobotInfoView.StyleVersion.GarageCustomGame;
				_editStyle = HUDRobotInfoView.StyleVersion.EditCustomGame;
			}
			else
			{
				_garageStyle = HUDRobotInfoView.StyleVersion.Garage;
				_editStyle = HUDRobotInfoView.StyleVersion.EditMode;
			}
			switch (guiInputController.GetActiveScreen())
			{
			case GuiScreens.Garage:
				_view.Show(_garageStyle);
				break;
			case GuiScreens.BuildMode:
				_view.Show(_editStyle);
				break;
			default:
				_view.Hide();
				break;
			}
		}
	}
}
