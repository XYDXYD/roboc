using Svelto.IoC;
using System.Collections;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal sealed class InitialMothershipGUIFlowTutorial : InitialMothershipGUIFlow
	{
		[Inject]
		internal ITutorialController tutorialController
		{
			private get;
			set;
		}

		protected override IEnumerator InitialDisplay(bool editMode)
		{
			if (PlayerPrefs.GetInt("NewSession") == 1)
			{
				PlayerPrefs.SetInt("NewSession", 0);
			}
			base.loadingIconPresenter.forceOpaque = true;
			yield return null;
			base.guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			yield return tutorialController.RequestTutorialStatusData();
			yield return base.chatPresenter.InitializeInFlow();
			yield return base.tauntsMotherShipController.Initialise();
			EnableLoadingScreen();
			yield return base.cpuPower.LoadData();
			yield return base.premiumMembership.Initialize();
			yield return InitialiseRobotInfoPanel();
			base.hudPlayerLevelPresenter.DisablePlayerLevelHUDAndXPTracking();
			Console.Log("Waiting for garage loading");
			while (!base.garage.isReady)
			{
				yield return null;
			}
			while (!AllDependencyDataLoaded())
			{
				yield return null;
			}
			DisableLoadingScreen();
			if (tutorialController.TutorialInProgress())
			{
				tutorialController.ShowTutorialScreenAndActivateFSM();
			}
			Console.Log("all screens were dismissed");
			base.guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			base.guiInputController.MothershipFlowCompleted();
			base.mothershipReadyObservable.Dispatch();
		}

		private bool AllDependencyDataLoaded()
		{
			if (!base.cubesData.isReady)
			{
				return false;
			}
			return true;
		}
	}
}
