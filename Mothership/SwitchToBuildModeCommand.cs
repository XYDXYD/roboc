using Services.Web.Photon;
using Svelto.Command;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;

namespace Mothership
{
	internal sealed class SwitchToBuildModeCommand : IInjectableCommand<SwitchWorldDependency>, ICommand
	{
		private SwitchWorldDependency _dependency;

		[Inject]
		public WorldSwitching worldSwitching
		{
			get;
			private set;
		}

		[Inject]
		public RobotSanctionController robotSanctionController
		{
			get;
			private set;
		}

		[Inject]
		internal GaragePresenter garagePresenter
		{
			get;
			private set;
		}

		public ICommand Inject(SwitchWorldDependency dependency)
		{
			_dependency = dependency;
			return this;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run(robotSanctionController.CheckRobotSanction(string.Empty, delegate(RobotSanctionData sanction)
			{
				TaskRunner.get_Instance().Run(OnRobotSanctionTaskSucceeded(sanction));
			}));
		}

		private IEnumerator OnRobotSanctionTaskSucceeded(RobotSanctionData robotSanction)
		{
			if (robotSanction != null)
			{
				yield return garagePresenter.RefreshGarageData();
				garagePresenter.ShowGarageSlots();
				garagePresenter.LoadAndBuildRobot();
				garagePresenter.SelectCurrentGarageSlot();
			}
			else
			{
				worldSwitching.SwitchToBuildModeFromGarage(_dependency);
			}
		}
	}
}
