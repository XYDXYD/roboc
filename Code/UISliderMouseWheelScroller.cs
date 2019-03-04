using UnityEngine;

internal class UISliderMouseWheelScroller : MonoBehaviour
{
	public int leftBorder = 207;

	public int rightBorder;

	public int topBorder = 90;

	public int bottomBorder = 28;

	public float scrollSpeed = 1f;

	public bool anchorFromRightOfScreen;

	[Tooltip("Optional panel that you can use instead of hardcoded values - Values not used anymore if panel is defined")]
	public UIPanel optionalPanel;

	private UISlider _slider;

	public UIScrollScrollView scrollView;

	private int _reset = 2;

	private Camera _uiCam;

	public UISliderMouseWheelScroller()
		: this()
	{
	}

	private void Start()
	{
		_slider = this.GetComponent<UISlider>();
		_uiCam = UICamera.get_mainCamera();
		if (scrollView != null)
		{
			scrollView.scrollView.RestrictWithinBounds(true);
			_reset = 2;
		}
	}

	private bool MouseInRect()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 mousePosition = Input.get_mousePosition();
		if (optionalPanel != null)
		{
			Vector3 val = _uiCam.ScreenToWorldPoint(Input.get_mousePosition());
			return optionalPanel.IsVisible(val);
		}
		if (((!anchorFromRightOfScreen && mousePosition.x > (float)leftBorder) || (anchorFromRightOfScreen && mousePosition.x > (float)(Screen.get_width() - leftBorder))) && mousePosition.x < (float)(Screen.get_width() - rightBorder) && mousePosition.y > (float)bottomBorder && mousePosition.y < (float)(Screen.get_height() - topBorder))
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (scrollView != null && _reset > 0)
		{
			scrollView.scrollView.RestrictWithinBounds(true);
			scrollView.scrollView.ResetPosition();
			_reset--;
		}
		if (!(_slider != null) || !MouseInRect())
		{
			return;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			if (_slider != null)
			{
				UISlider slider = _slider;
				slider.set_value(slider.get_value() - axis * scrollSpeed);
			}
			if (scrollView != null)
			{
				scrollView.scrollView.Scroll(axis * scrollSpeed);
			}
		}
	}
}
