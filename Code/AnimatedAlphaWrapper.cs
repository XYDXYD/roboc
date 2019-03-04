public class AnimatedAlphaWrapper : AnimatedAlpha
{
	private float _initialAlpha;

	public AnimatedAlphaWrapper()
		: this()
	{
	}

	private void Awake()
	{
		_initialAlpha = base.alpha;
	}

	private void OnDisable()
	{
		base.alpha = _initialAlpha;
	}
}
