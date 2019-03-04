using System;
using UnityEngine;

public class TutorialStartDialogueView : MonoBehaviour
{
	public UIButton Submit;

	public UIButton Cancel;

	private Action _onSubmitAction;

	private Action _onCancelAction;

	public TutorialStartDialogueView()
		: this()
	{
	}

	public void Show(Action onSubmit, Action onCancel)
	{
		Show();
		_onSubmitAction = onSubmit;
		_onCancelAction = onCancel;
	}

	public void OnSubmitClicked()
	{
		_onSubmitAction();
	}

	public void OnCancelClicked()
	{
		Hide();
		_onCancelAction();
	}

	private void Show()
	{
		this.set_enabled(true);
		this.get_gameObject().SetActive(true);
	}

	private void Hide()
	{
		this.set_enabled(false);
		this.get_gameObject().SetActive(false);
	}
}
