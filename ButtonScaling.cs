using UnityEngine;

internal class ButtonScaling : MonoBehaviour
{
	public Transform tweenTarget;

	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);

	public float duration = 0.2f;

	private Vector3 mScale;

	private bool mStarted;

	public ButtonScaling()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)


	private void Start()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!mStarted)
		{
			mStarted = true;
			if (tweenTarget == null)
			{
				tweenTarget = this.get_transform();
			}
			mScale = tweenTarget.get_localScale();
		}
	}

	public void OnHover(bool isOver)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (this.get_enabled())
		{
			if (!mStarted)
			{
				Start();
			}
			TweenScale.Begin(tweenTarget.get_gameObject(), duration, (!isOver) ? mScale : Vector3.Scale(mScale, hover)).method = 3;
		}
	}
}
