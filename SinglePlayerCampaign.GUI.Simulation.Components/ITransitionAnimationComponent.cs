namespace SinglePlayerCampaign.GUI.Simulation.Components
{
	public interface ITransitionAnimationComponent
	{
		bool IsPlaying
		{
			get;
		}

		void PlayWaveStart(bool isFinalWave);

		void PlayWaveComplete();

		void PlayStartBattle();

		void PlayEndBattle(bool won);
	}
}
