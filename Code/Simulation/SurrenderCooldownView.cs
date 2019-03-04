using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class SurrenderCooldownView : MonoBehaviour
	{
		public UIButton surrenderButton;

		public UIButton surrenderCooldownTimer;

		public UILabel surrenderCooldownLabel;

		private bool _timerShowing;

		[Inject]
		internal SurrenderCooldownPresenter surrenderCooldownPresenter
		{
			private get;
			set;
		}

		public SurrenderCooldownView()
			: this()
		{
		}

		private void Start()
		{
			surrenderCooldownPresenter.RegisterView(this);
			surrenderCooldownTimer.get_gameObject().SetActive(true);
			surrenderButton.get_gameObject().SetActive(false);
			_timerShowing = true;
		}

		private void OnDestroy()
		{
			surrenderCooldownPresenter.UnregisterView(this);
		}

		public void ShowVoteInProgress()
		{
			surrenderCooldownLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strVotingInProgress"));
			surrenderCooldownTimer.get_gameObject().SetActive(true);
			surrenderButton.get_gameObject().SetActive(false);
			_timerShowing = true;
		}

		public void FlipCooldownTimerVisibility()
		{
			surrenderCooldownTimer.get_gameObject().SetActive(!_timerShowing);
			surrenderButton.get_gameObject().SetActive(_timerShowing);
			_timerShowing = !_timerShowing;
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void UpdateCooldownTimeLeft(int secondsRemaining)
		{
			surrenderCooldownLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCooldown") + FormatTime(secondsRemaining));
		}

		private string FormatTime(float seconds)
		{
			int num = Mathf.FloorToInt(seconds / 60f);
			int num2 = Mathf.FloorToInt(seconds - (float)(num * 60));
			return num.ToString() + ":" + num2.ToString("D2");
		}
	}
}
