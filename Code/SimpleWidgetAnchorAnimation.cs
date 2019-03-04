using UnityEngine;

internal sealed class SimpleWidgetAnchorAnimation : MonoBehaviour
{
	public UIWidget Target;

	public float baseValue = 1f;

	public float range = 0.2f;

	public float speedModifier = 4f;

	private float _animTimer;

	public SimpleWidgetAnchorAnimation()
		: this()
	{
	}

	public void Update()
	{
		_animTimer += Time.get_deltaTime();
		float num = Mathf.Sin(_animTimer * speedModifier);
		float num2 = range * num;
		Target.rightAnchor.relative = baseValue + num2;
	}
}
