using UnityEngine;

internal class AnimationPlayer : MonoBehaviour
{
	private Animation _animation;

	public AnimationPlayer()
		: this()
	{
	}

	private void Start()
	{
		_animation = this.GetComponent<Animation>();
	}

	public void Play(string animationName)
	{
		_animation = this.GetComponent<Animation>();
		_animation.Play(animationName);
	}
}
