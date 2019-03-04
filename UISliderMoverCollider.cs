using UnityEngine;

internal sealed class UISliderMoverCollider : MonoBehaviour
{
	public UISlider slider;

	public UISliderMoverCollider()
		: this()
	{
	}

	public float GetSliderValue()
	{
		if (slider != null)
		{
			return slider.get_value();
		}
		return 0f;
	}

	public void SetSliderValue(float fValue)
	{
		if (slider != null)
		{
			slider.set_value(fValue);
		}
	}
}
