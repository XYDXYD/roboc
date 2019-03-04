using SinglePlayerCampaign.GUI.Simulation.Components;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Simulation.Implementors
{
	internal class WaveTransitionView : MonoBehaviour, ITransitionAnimationComponent
	{
		[SerializeField]
		private Animation anim;

		private const string ANIMATION_CAMPAIGN_BATTLE_START = "Campaign_BattleStart";

		private const string ANIMATION_CAMPAIGN_NORMAL_WAVE_START = "Campaign_NormalWaveStart";

		private const string ANIMATION_CAMPAIGN_BOSS_WAVE_START = "Campaign_BossWaveStart";

		private const string ANIMATION_CAMPAIGN_WAVE_COMPLETE = "Campaign_WaveComplete";

		bool ITransitionAnimationComponent.IsPlaying
		{
			get
			{
				return anim.get_isPlaying();
			}
		}

		public WaveTransitionView()
			: this()
		{
		}

		public void PlayWaveComplete()
		{
			PlayAnimation("Campaign_WaveComplete");
		}

		public void PlayWaveStart(bool isFinalWave)
		{
			string animationClipName = (!isFinalWave) ? "Campaign_NormalWaveStart" : "Campaign_BossWaveStart";
			PlayAnimation(animationClipName);
		}

		public void PlayStartBattle()
		{
			PlayAnimation("Campaign_BattleStart");
		}

		private void PlayAnimation(string animationClipName)
		{
			anim.Play(animationClipName);
		}

		public void PlayEndBattle(bool won)
		{
		}
	}
}
