using UnityEngine;

[RequireComponent(typeof(UIInputWithFocusEvents))]
internal class UIInputPlaceholder : MonoBehaviour
{
	public UILabel placeholder;

	private UIInputWithFocusEvents _input;

	public UIInputPlaceholder()
		: this()
	{
	}

	private void Awake()
	{
		_input = this.GetComponent<UIInputWithFocusEvents>();
		_input.OnInputGetFocus += OnInputGetFocus;
		_input.OnInputLoseFocus += OnInputLoseFocus;
	}

	private void OnInputLoseFocus()
	{
		if (_input.get_value() == string.Empty && placeholder != null)
		{
			placeholder.get_gameObject().SetActive(true);
		}
	}

	private void OnInputGetFocus()
	{
		if (placeholder != null)
		{
			placeholder.get_gameObject().SetActive(false);
		}
	}
}
