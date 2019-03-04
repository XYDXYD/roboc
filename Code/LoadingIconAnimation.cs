using UnityEngine;

public class LoadingIconAnimation : MonoBehaviour
{
	private Animation _animation;

	private float _lastTime;

	private AnimationState _state;

	public LoadingIconAnimation()
		: this()
	{
	}

	private void Awake()
	{
		_animation = this.get_gameObject().GetComponent<Animation>();
		_animation.set_wrapMode(2);
	}

	private void OnEnable()
	{
		_animation.Play();
		_state = _animation.get_Item(_animation.get_clip().get_name());
		_state.set_time(_lastTime);
	}

	private void Update()
	{
		_lastTime = _state.get_time();
	}
}
