using UnityEngine;

internal sealed class NGWidgetColorAnimator : MonoBehaviour
{
	public Color AnimColor = Color.get_white();

	public UIWidget TargetWidget;

	public NGWidgetColorAnimator()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	private void Awake()
	{
		if (TargetWidget == null)
		{
			TargetWidget = this.GetComponent<UIWidget>();
		}
	}

	private void Update()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (TargetWidget != null)
		{
			TargetWidget.set_color(AnimColor);
		}
	}
}
