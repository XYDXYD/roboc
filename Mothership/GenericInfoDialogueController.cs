namespace Mothership
{
	internal sealed class GenericInfoDialogueController
	{
		private GenericInfoDialogue _dialogObject;

		public bool isOpen
		{
			get;
			private set;
		}

		public GenericInfoDialogueController(GenericInfoDialogue view)
		{
			_dialogObject = view;
		}

		public void Open(GenericErrorData errorData)
		{
			_dialogObject.graphicHolder.SetActive(true);
			isOpen = true;
			_dialogObject.title.set_text(errorData.header);
			_dialogObject.bodyText.set_text(errorData.body);
			_dialogObject.okSingleButtonText.set_text(errorData.okButtonText);
			_dialogObject.okButtonText.set_text(errorData.okButtonText);
			_dialogObject.cancelButtonText.set_text(errorData.cancelButtonText);
			bool flag = true;
			if (errorData.cancelButtonText == null || errorData.cancelButtonText.CompareTo(string.Empty) == 0)
			{
				flag = false;
			}
			_dialogObject.cancelButton.set_enabled(flag);
			_dialogObject.cancelButton.get_gameObject().SetActive(flag);
			_dialogObject.okSingleButton.set_enabled(!flag);
			_dialogObject.okSingleButton.get_gameObject().SetActive(!flag);
			_dialogObject.okButton.set_enabled(flag);
			_dialogObject.okButtonText.get_gameObject().SetActive(flag);
			_dialogObject.okSingleButton.onClicked = errorData.onOkClicked;
			_dialogObject.okButton.onClicked = errorData.onOkClicked;
			_dialogObject.cancelButton.onClicked = errorData.onCancelClicked;
		}

		public void Close()
		{
			_dialogObject.graphicHolder.SetActive(false);
			isOpen = false;
		}
	}
}
