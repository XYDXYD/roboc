using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Simulation
{
	internal class BattleStatsCloseButtonListener : MonoBehaviour, IChainListener
	{
		[Inject]
		internal BattleStatsPresenter battleStatsPresenter
		{
			private get;
			set;
		}

		public BattleStatsCloseButtonListener()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				if (buttonType == ButtonType.Confirm)
				{
					battleStatsPresenter.GoBackToMothership();
				}
			}
		}
	}
}
