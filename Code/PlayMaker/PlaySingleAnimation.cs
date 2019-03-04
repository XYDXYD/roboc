using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("Play single animation and fire event when complete.")]
	public class PlaySingleAnimation : FsmStateAction
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Animation sequence to play")]
		public FsmGameObject animSequence;

		public FsmString animName;

		public FsmEvent triggerEventOnEnd;

		public bool loopForever;

		public bool haltAnimationOnExitState;

		private bool _eventTriggered;

		private float _animTimeRemaining;

		private Animation _animation;

		public PlaySingleAnimation()
			: this()
		{
		}

		public override void Reset()
		{
			_animation = null;
			animSequence = null;
			animName = FsmString.op_Implicit(string.Empty);
			_animTimeRemaining = 0f;
			triggerEventOnEnd = null;
			_eventTriggered = false;
		}

		public override void OnEnter()
		{
			_animation = animSequence.get_Value().GetComponent<Animation>();
			string text = ((object)animName).ToString();
			if (loopForever)
			{
				_animation.set_wrapMode(2);
			}
			else
			{
				_animation.set_wrapMode(1);
			}
			PlayMainAnimation();
			_eventTriggered = false;
		}

		public override void OnExit()
		{
			if (haltAnimationOnExitState)
			{
				_animation.Stop();
			}
		}

		public override void OnUpdate()
		{
			_animTimeRemaining -= Time.get_deltaTime();
			if (_animTimeRemaining <= 0f && !_eventTriggered && !loopForever)
			{
				this.get_Fsm().Event(triggerEventOnEnd);
				_eventTriggered = true;
			}
		}

		private void PlayMainAnimation()
		{
			string text = ((object)animName).ToString();
			_animation.Play(text);
			_animTimeRemaining = _animation.get_Item(text).get_length();
		}
	}
}
