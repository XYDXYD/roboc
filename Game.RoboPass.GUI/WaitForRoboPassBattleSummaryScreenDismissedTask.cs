using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Game.RoboPass.GUI
{
	public class WaitForRoboPassBattleSummaryScreenDismissedTask : ITask, IAbstractTask
	{
		private readonly Observer _roboPassBattleSummaryObserver;

		private bool _dismissed;

		public bool isDone
		{
			get;
			private set;
		}

		public WaitForRoboPassBattleSummaryScreenDismissedTask(Observer roboPassBattleSummaryObserver)
		{
			_roboPassBattleSummaryObserver = roboPassBattleSummaryObserver;
			_roboPassBattleSummaryObserver.AddAction((Action)OnScreenDismissed);
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)WaitForScreenDismiss);
		}

		private void OnScreenDismissed()
		{
			_dismissed = true;
			_roboPassBattleSummaryObserver.RemoveAction((Action)OnScreenDismissed);
		}

		private IEnumerator WaitForScreenDismiss()
		{
			while (!_dismissed)
			{
				yield return null;
			}
			isDone = true;
		}
	}
}
