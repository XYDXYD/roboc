using Svelto.IoC;
using UnityEngine;
using Utility;

public sealed class BuildModeShortcutHintsView : MonoBehaviour, IInitialize
{
	[SerializeField]
	private UIWidget container;

	[SerializeField]
	private UIWidget containerWhenOffset;

	[SerializeField]
	private UIWidget AdvancedInfoPanel;

	private UIWidget _uiWidget;

	[Inject]
	public BuildModeShortcutHintsPresenter Presenter
	{
		private get;
		set;
	}

	public BuildModeShortcutHintsView()
		: this()
	{
	}

	public void OnDependenciesInjected()
	{
		Presenter.RegisterView(this);
	}

	public void Show()
	{
		this.get_gameObject().SetActive(true);
	}

	public void Hide()
	{
		this.get_gameObject().SetActive(false);
	}

	public void SetStyleForWhenCustomGameShown(bool setting)
	{
		if (setting)
		{
			Console.Log("adapting advance info panel for when custom game is shown");
			AdvancedInfoPanel.SetAnchor(containerWhenOffset.get_gameObject());
		}
		else
		{
			Console.Log("adapting advance info panel for when custom game is hidden");
			AdvancedInfoPanel.SetAnchor(container.get_gameObject());
		}
		AdvancedInfoPanel.UpdateAnchors();
	}
}
