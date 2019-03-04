using UnityEngine;

internal sealed class UISliderPanelMover : MonoBehaviour
{
	public Vector2 panelSize = new Vector2(800f, 600f);

	public bool isVertcal = true;

	private UIPanel panel;

	private Vector3 startPos;

	private float horizOffset;

	private float vertOffset;

	public UISliderPanelMover()
		: this()
	{
	}//IL_000b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0010: Unknown result type (might be due to invalid IL or missing references)


	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		panel = this.GetComponent<UIPanel>();
		startPos = this.get_transform().get_localPosition();
	}

	public void OnValueChange()
	{
		UpdatePanel(UIProgressBar.current.get_value());
	}

	public void PanPanel()
	{
		isVertcal = !isVertcal;
		UpdatePanel(UIProgressBar.current.get_value());
		isVertcal = !isVertcal;
	}

	private void UpdatePanel(float value)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (isVertcal)
		{
			vertOffset = value * panelSize.y;
		}
		else
		{
			horizOffset = -1f * value * panelSize.x;
		}
		Vector3 localPosition = startPos;
		localPosition.y += vertOffset;
		localPosition.x += horizOffset;
		this.get_transform().set_localPosition(localPosition);
	}
}
