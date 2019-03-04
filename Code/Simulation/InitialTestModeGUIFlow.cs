using RCNetwork.Client.UNet;
using Svelto.IoC;
using System.Collections;
using Utility;

namespace Simulation
{
	internal sealed class InitialTestModeGUIFlow : InitialSingleplayerGUIFlow
	{
		[Inject]
		internal ITutorialController tutorialController
		{
			private get;
			set;
		}

		public InitialTestModeGUIFlow(NetworkInitialisationMockClientUnity networkInitialisationMockClientUnity)
			: base(networkInitialisationMockClientUnity)
		{
		}

		protected override IEnumerator InitialDisplay()
		{
			yield return tutorialController.RequestTutorialStatusData();
		}

		public override void OnFrameworkInitialized()
		{
			base.OnGuiFlowComplete += OnGuiFlowCompleteStartTutorialStateMachine;
			base.OnFrameworkInitialized();
		}

		private void OnGuiFlowCompleteStartTutorialStateMachine()
		{
			if (tutorialController.TutorialInProgress())
			{
				tutorialController.ShowTutorialScreenAndActivateFSM();
				Console.Log("entering test mode as part of the tutorial flow");
			}
			else
			{
				Console.Log("entering test mode as part of normal flow");
			}
		}

		protected override void CustomiseUIStyle()
		{
			Console.Log("test mode has no battle stats presenter.");
		}
	}
}
