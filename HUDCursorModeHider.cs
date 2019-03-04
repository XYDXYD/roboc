using Svelto.IoC;
using UnityEngine;

internal sealed class HUDCursorModeHider : MonoBehaviour
{
	[Inject]
	internal IGUIInputController inputController
	{
		private get;
		set;
	}

	public HUDCursorModeHider()
		: this()
	{
	}

	public void Start()
	{
		inputController.OnScreenStateChange += OnScreenChange;
	}

	private void OnScreenChange()
	{
		if (inputController.IsActiveScreenFullHUDStyle())
		{
			this.get_gameObject().SetActive(true);
		}
		else
		{
			this.get_gameObject().SetActive(false);
		}
	}

	private void OnDestroy()
	{
		inputController.OnScreenStateChange -= OnScreenChange;
	}
}
