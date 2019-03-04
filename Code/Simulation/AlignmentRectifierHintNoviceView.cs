using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierHintNoviceView : MonoBehaviour
	{
		private enum HintState
		{
			Ready,
			Hint,
			HintCooldown
		}

		public AnimationClip hintNoviceAnimationClip;

		public float hintTimerLength = 3f;

		public float hintCooldownTimerLength = 5f;

		private Animation _animation;

		private string _audioEventName;

		private HintState _state;

		private float _timer;

		[Inject]
		internal AlignmentRectifierEngine alignmentRectifierManager
		{
			private get;
			set;
		}

		public AlignmentRectifierHintNoviceView()
			: this()
		{
		}

		private void Start()
		{
			_animation = this.GetComponentInChildren<Animation>();
			alignmentRectifierManager.RegisterView(this);
		}

		private void OnDestroy()
		{
			alignmentRectifierManager.UnregisterView(this);
		}

		private void Update()
		{
			switch (_state)
			{
			case HintState.Hint:
				_timer += Time.get_deltaTime();
				if (_timer >= hintTimerLength)
				{
					ChangeStateToCooldown();
				}
				break;
			case HintState.HintCooldown:
				_timer += Time.get_deltaTime();
				if (_timer >= hintCooldownTimerLength)
				{
					ChangestateToHint();
				}
				break;
			}
		}

		public void EnableHint(string audioEventName)
		{
			_audioEventName = audioEventName;
			if (_state == HintState.Ready)
			{
				ChangestateToHint();
			}
		}

		private void ChangeStateToCooldown()
		{
			EventManager.get_Instance().PostEvent(_audioEventName, 17, this.get_gameObject());
			EventManager.get_Instance().PostEvent(_audioEventName, 0, this.get_gameObject());
			_animation.Play(hintNoviceAnimationClip.get_name());
			_state = HintState.HintCooldown;
			_timer = 0f;
		}

		private void ChangestateToHint()
		{
			_state = HintState.Hint;
			_timer = 0f;
		}

		public void DisableHint()
		{
			_state = HintState.Ready;
		}
	}
}
