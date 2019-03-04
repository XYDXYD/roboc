using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("Play single animation and fire event when complete using the Animator component")]
	public class PlaySingleAnimWithAnimator : FsmStateAction
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Animation sequence to play")]
		public FsmGameObject animSequence;

		public FsmString animName;

		public FsmEvent triggerEventOnEnd;

		public FsmFloat animLength;

		public bool haltAnimationOnExitState;

		private bool _eventTriggered;

		private float _animTimeRemaining;

		private Animator _animation;

		public PlaySingleAnimWithAnimator()
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
			_animation = animSequence.get_Value().GetComponent<Animator>();
			PlayMainAnimation();
			_eventTriggered = false;
		}

		public override void OnExit()
		{
			if (haltAnimationOnExitState)
			{
				_animation.StopPlayback();
			}
		}

		public override void OnUpdate()
		{
			_animTimeRemaining -= Time.get_deltaTime();
			if (_animTimeRemaining <= 0f && !_eventTriggered)
			{
				this.get_Fsm().Event(triggerEventOnEnd);
				_eventTriggered = true;
			}
		}

		private void PlayMainAnimation()
		{
			string text = ((object)animName).ToString();
			_animation.Play(text);
			_animTimeRemaining = animLength.get_Value();
		}
	}
}
