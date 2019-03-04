using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class AFKBlockTimerScreen : MonoBehaviour, IInitialize, IChainListener
	{
		public GameObject blockedPanel;

		public UILabel timerCountUpText;

		public UIButton reconnectRegisterInGame;

		public string emptyValueText = "-";

		public UILabel noticeText;

		[Inject]
		internal BattleCountdownScreenController battleCountdownScreenController
		{
			private get;
			set;
		}

		public AFKBlockTimerScreen()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			battleCountdownScreenController.SetAFKBlockTimerScreenView(this);
		}

		public void Awake()
		{
			ClearAllValues();
			reconnectRegisterInGame.get_gameObject().SetActive(false);
			noticeText.set_text(StringTableBase<StringTable>.Instance.GetString("strBattleBlockedAFK"));
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void SetAFKBlockPanelVisible(bool visible)
		{
			blockedPanel.SetActive(visible);
		}

		public void SetAFKBlockedTimeCountdown(int time)
		{
			if (time < 0)
			{
				battleCountdownScreenController.SetBattleCountDownState(BattleCountdownScreenController.BattleCountdownState.WaitingForRejoinAction);
				reconnectRegisterInGame.get_gameObject().SetActive(true);
				reconnectRegisterInGame.SetState(0, true);
			}
			else
			{
				timerCountUpText.set_text(GuiUtilities.FormatTime(time));
			}
		}

		public void Listen(object message)
		{
			if (message is ButtonType && (ButtonType)message == ButtonType.Close)
			{
				this.get_gameObject().SetActive(false);
				battleCountdownScreenController.HandleRejoinBattleOnUnblocked();
			}
		}

		private void ClearAllValues()
		{
			timerCountUpText.set_text(emptyValueText);
		}
	}
}
