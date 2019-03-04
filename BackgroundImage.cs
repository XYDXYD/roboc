using UnityEngine;

internal class BackgroundImage : MonoBehaviour
{
	public UIPanel panel;

	public BackgroundImage()
		: this()
	{
	}

	public void Show()
	{
		panel.get_gameObject().SetActive(true);
	}

	public void Hide()
	{
		panel.get_gameObject().SetActive(false);
	}
}
