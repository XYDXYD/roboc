internal sealed class InputDialogController
{
	private readonly InputDialog _inputDialog;

	public bool IsOpen
	{
		get;
		private set;
	}

	public InputDialogController(InputDialog view)
	{
		_inputDialog = view;
	}

	public void Open(InputDialogData errorData)
	{
		_inputDialog.OkButton.OnClicked = delegate
		{
			if (_inputDialog.Input != null)
			{
				errorData.OnOkClicked(_inputDialog.Input.get_value());
			}
			else
			{
				errorData.OnOkClicked(null);
			}
		};
		_inputDialog.CancelButton.OnClicked = errorData.OnCancelClicked;
		_inputDialog.GraphicHolder.SetActive(true);
		IsOpen = true;
	}

	public void Close()
	{
		_inputDialog.GraphicHolder.SetActive(false);
		IsOpen = false;
	}
}
