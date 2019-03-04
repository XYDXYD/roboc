using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using Utility;

namespace Mothership
{
	internal class DummyTestModeScreenDisplayTutorial : DummyTestModeScreenDisplay
	{
		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			protected get;
			set;
		}

		public override GUIShowResult Show()
		{
			if (base.enterBattleChecker.IsMachineValidForBattle())
			{
				ExecuteTutorialSwitchCommand();
				return GUIShowResult.Showed;
			}
			return GUIShowResult.NotShowedSlim;
		}

		private void OnLoadError(ServiceBehaviour behaviour)
		{
			Console.Log("Load error occured in ILoadSignupDate while executing the tutorial switch command.");
			RemoteLogger.Error("Service request error", "warning... service requires for ILoadSignupDate in dummy test mode display failed", null);
		}

		protected void ExecuteTutorialSwitchCommand()
		{
			ILoadSignupDate loadSignupDate = serviceFactory.Create<ILoadSignupDate>();
			loadSignupDate.SetAnswer(new ServiceAnswer<DateTime>(delegate(DateTime signUpDate)
			{
				bool isNewPlayer_ = false;
				DateTime utcNow = DateTime.UtcNow;
				double totalHours = (utcNow - signUpDate).TotalHours;
				if (totalHours < 24.0)
				{
					isNewPlayer_ = true;
				}
				SwitchTutorialTestWorldDependency dependency = new SwitchTutorialTestWorldDependency("TestRobot", isRanked_: false, GameModeType.TestMode, isNewPlayer_);
				base.commandFactory.Build<SwitchToTutorialTestPlanetCommand>().Inject(dependency).Execute();
			}, OnLoadError));
			loadSignupDate.Execute();
		}
	}
}
