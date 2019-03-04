namespace Simulation.Pit
{
	internal class HUDPlayerIDWidgetPit : HUDPlayerIdWidgetExtension
	{
		public UILabel PointsValueLabel;

		public UISprite LeaderIcon;

		private bool _isLeader;

		private int _killStreak;

		public override void UpdateAlpha(float alpha)
		{
			PointsValueLabel.set_alpha(alpha);
			LeaderIcon.set_alpha(alpha);
		}

		private void Start()
		{
			SetIsLeader(isLeader: false);
		}

		public void SetIsLeader(bool isLeader)
		{
			_isLeader = isLeader;
			LeaderIcon.set_enabled(isLeader);
			UpdateValue();
		}

		public void SetKillStreak(uint streak)
		{
			_killStreak = (int)streak;
			UpdateValue();
		}

		private void UpdateValue()
		{
			GetPlayerPointsValue();
		}

		private void GetPlayerPointsValue()
		{
			uint playerValue = PitUtils.GetPlayerValue(_isLeader, (uint)_killStreak);
			PointsValueLabel.set_text("+" + playerValue.ToString());
		}
	}
}
