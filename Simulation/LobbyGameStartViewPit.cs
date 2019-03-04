using UnityEngine;

namespace Simulation
{
	internal sealed class LobbyGameStartViewPit : LobbyGameStartView
	{
		private bool playedInfo;

		private bool playedGo;

		public override void SetTimer(float seconds)
		{
			if (!this.get_gameObject().get_activeSelf())
			{
				return;
			}
			int num = Mathf.RoundToInt(seconds);
			if (num != _lastSeconds)
			{
				time.set_text(Mathf.RoundToInt(seconds).ToString());
				PlayWaitTimeAudio(num);
				_lastSeconds = num;
				if (_lastSeconds <= 15)
				{
					PlayInfoAnimation();
				}
				if (_lastSeconds <= 2)
				{
					PlayGOAnimation();
				}
			}
		}

		private void PlayInfoAnimation()
		{
			if (!playedInfo)
			{
				this.GetComponent<Animation>().Play("BattleStart_PIT");
				playedInfo = true;
			}
		}

		private void PlayGOAnimation()
		{
			if (!playedGo)
			{
				this.GetComponent<Animation>().Play("BattleStart_PIT_FadeOut");
				playedGo = true;
			}
		}
	}
}
