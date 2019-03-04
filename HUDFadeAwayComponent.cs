using Svelto.ES.Legacy;
using UnityEngine;

internal class HUDFadeAwayComponent : MonoBehaviour, IFadeGuiElementsComponent, IComponent
{
	[SerializeField]
	private float fadeAwaySpeed = 2f;

	private UIPanel _panel;

	public HUDFadeAwayComponent()
		: this()
	{
	}

	private void Start()
	{
		_panel = this.GetComponent<UIPanel>();
	}

	public float GetFadeAwaySpeed()
	{
		return fadeAwaySpeed;
	}

	public float GetCurrentAlpha()
	{
		if (_panel != null)
		{
			return _panel.get_alpha();
		}
		return 1f;
	}

	public void SetCurrentAlpha(float alpha)
	{
		_panel.set_alpha(alpha);
	}
}
