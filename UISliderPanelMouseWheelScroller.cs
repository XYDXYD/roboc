using UnityEngine;

internal class UISliderPanelMouseWheelScroller : MonoBehaviour
{
	public float scrollSpeed = 1f;

	public UIPanel panel;

	private Camera _uiCam;

	private UISlider _slider;

	private bool _enabled = true;

	public UISliderPanelMouseWheelScroller()
		: this()
	{
	}

	internal void SetEnabled(bool enabled)
	{
		_enabled = enabled;
	}

	private void Start()
	{
		_slider = this.GetComponent<UISlider>();
		_uiCam = UICamera.get_mainCamera();
	}

	private void Update()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!_enabled)
		{
			return;
		}
		Vector3 val = _uiCam.ScreenToWorldPoint(Input.get_mousePosition());
		if (panel.IsVisible(val))
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f)
			{
				UISlider slider = _slider;
				slider.set_value(slider.get_value() - axis * scrollSpeed);
			}
		}
	}
}
