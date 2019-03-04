using Svelto.IoC;
using UnityEngine;

public class BigLoadingScreen : MonoBehaviour, IInitialize
{
	[Inject]
	internal WorldSwitchLoadingDisplay display
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	public BigLoadingScreen()
		: this()
	{
	}

	private void Awake()
	{
		this.get_gameObject().SetActive(false);
	}

	public void OnDependenciesInjected()
	{
		if (display == null)
		{
			this.get_gameObject().SetActive(true);
			return;
		}
		display.view = this.get_gameObject();
		guiInputController.ShowScreen(GuiScreens.WorldSwitchLoading);
	}
}
