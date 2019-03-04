using UnityEngine;

internal class GenericLoadingScreen : MonoBehaviour
{
	public UILabel label;

	public UISlider slider;

	public UIPanel queuePanel;

	public UILabel queueCount;

	public UISprite screenDarkener;

	public UIWidget additionalMessageContainer;

	public UILabel additionalMessageLabel;

	public Transform loadingIconTransform;

	public string text
	{
		get
		{
			return (!(label != null)) ? string.Empty : label.get_text();
		}
		set
		{
			if (label != null)
			{
				label.set_text(value);
			}
		}
	}

	public float backgroundOpacity
	{
		get
		{
			return screenDarkener.get_alpha();
		}
		set
		{
			screenDarkener.set_alpha(value);
		}
	}

	public GenericLoadingScreen()
		: this()
	{
	}
}
