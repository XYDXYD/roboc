using Svelto.ES.Legacy;
using UnityEngine;

namespace Simulation
{
	internal class HealthBarView : MonoBehaviour, IHealthBarViewComponent, IComponent
	{
		public string autoRegenHealthAnimationNamePt2 = "Hud_Overclocker_regenHealthpt2";

		public string autoRegenHealthAnimationNamePt1 = "Hud_Overclocker_regenHealthpt1";

		public GameObject cray;

		public GameObject timer;

		public UILabel timerLabel;

		private Animation _animation;

		public HealthBarView()
			: this()
		{
		}

		private void Start()
		{
			_animation = this.GetComponent<Animation>();
		}

		public void PlaySecondAutoRegenAnimation()
		{
			AnimationState val = _animation.get_Item(autoRegenHealthAnimationNamePt2);
			val.set_wrapMode(2);
			val.set_time(0f);
			val.set_speed(1f);
			_animation.Play(autoRegenHealthAnimationNamePt2);
		}

		public void StopSecondAutoRegenAnimation()
		{
			AnimationState val = _animation.get_Item(autoRegenHealthAnimationNamePt2);
			val.set_wrapMode(0);
			val.set_time(0f);
			val.set_speed(-1f);
			_animation.Play(autoRegenHealthAnimationNamePt2);
		}

		public void PlayFirstAutoRegenAnimation()
		{
			_animation.Play(autoRegenHealthAnimationNamePt1);
		}

		public void SetTimerLabel(int timerValue, bool enabled)
		{
			timer.SetActive(enabled);
			cray.SetActive(!enabled);
			if (enabled)
			{
				timerLabel.set_text(timerValue.ToString());
			}
		}
	}
}
