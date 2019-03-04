using SocialServiceLayer;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Simulation
{
	internal class BattleStatsFriendButtonView : MonoBehaviour, IChainListener
	{
		public BattleStatsPlayerWidget ParentObject;

		private bool _enabled = true;

		[Inject]
		internal BattleStatsPresenter battleStatsPresenter
		{
			get;
			private set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		public BattleStatsFriendButtonView()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (_enabled && message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				if (buttonType == ButtonType.Confirm)
				{
					_enabled = false;
					battleStatsPresenter.OnFriendButtonClicked(ParentObject.PlayerName);
				}
			}
		}
	}
}
