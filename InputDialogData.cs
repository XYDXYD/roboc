using System;

internal sealed class InputDialogData
{
	public Action OnCancelClicked = delegate
	{
	};

	public Action<string> OnOkClicked = delegate
	{
	};

	public InputDialogData(Action<string> okClicked, Action cancelClicked)
	{
		OnOkClicked = okClicked;
		OnCancelClicked = cancelClicked;
	}

	public InputDialogData()
	{
	}
}
