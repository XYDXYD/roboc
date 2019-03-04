using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class GameOverPlayerDeadAnimation : MonoBehaviour, ITransitionAnimationComponent
	{
		[SerializeField]
		private Animation anim;

		private const string ANIMATION_CAMPAIGN_BATTLE_END_LOST = "HUD_DEFEAT";

		public bool IsPlaying => anim.get_isPlaying();

		public GameOverPlayerDeadAnimation()
			: this()
		{
		}

		public void PlayStartBattle()
		{
		}

		public void PlayWaveStart(bool isFinalWave)
		{
		}

		public void PlayWaveComplete()
		{
		}

		public void PlayEndBattle(bool won)
		{
			if (!won)
			{
				anim.Play("HUD_DEFEAT");
			}
		}
	}
}
