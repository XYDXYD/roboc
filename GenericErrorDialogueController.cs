using UnityEngine;

internal sealed class GenericErrorDialogueController
{
	private Vector3 _centreButtonPosition;

	private Vector3 _originalOkButtonPosition;

	private Vector3 _buttonColliderOriginalSize;

	private float _originalOkButtonHighlightAbsoluteValue;

	private GenericErrorDialogue _dialogObject;

	private BoxCollider _buttonCollider;

	public bool isOpen
	{
		get;
		private set;
	}

	public GenericErrorDialogueController(GenericErrorDialogue view)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		_dialogObject = view;
		_originalOkButtonPosition = _dialogObject.okButton.get_transform().get_position();
		_originalOkButtonHighlightAbsoluteValue = _dialogObject.okButtonHighlight.rightAnchor.absolute;
		_buttonCollider = _dialogObject.okButton.GetComponent<BoxCollider>();
		_buttonColliderOriginalSize = _buttonCollider.get_size();
	}

	public void Open(GenericErrorData errorData)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		isOpen = true;
		_dialogObject.title.set_text(errorData.header);
		string text = StringUtil.Wrap(errorData.body, 50, 256);
		_dialogObject.bodyText.set_text(text);
		_dialogObject.okButtonText.set_text(errorData.okButtonText);
		_centreButtonPosition = _originalOkButtonPosition;
		_centreButtonPosition.x = 0f;
		bool flag = true;
		if (errorData.cancelButtonText == null || errorData.cancelButtonText.CompareTo(string.Empty) == 0)
		{
			_buttonCollider.set_size(_buttonColliderOriginalSize * 2f);
			_dialogObject.okButton.get_transform().set_position(_centreButtonPosition);
			_buttonCollider.set_center(_centreButtonPosition);
			_dialogObject.okButtonHighlight.rightAnchor.Set(1f, 0f);
			flag = false;
		}
		else
		{
			_dialogObject.okButton.get_transform().set_position(_originalOkButtonPosition);
			_dialogObject.cancelButtonText.set_text(errorData.cancelButtonText);
			_dialogObject.okButtonHighlight.rightAnchor.Set(1f, _originalOkButtonHighlightAbsoluteValue);
		}
		_dialogObject.cancelButton.set_enabled(flag);
		_dialogObject.cancelButton.get_gameObject().SetActive(flag);
		if (_dialogObject.OKbuttonFullSizeAnchors != null && _dialogObject.OKbuttonHalfSizeAnchors != null)
		{
			UIWidget component = _dialogObject.okButton.GetComponent<UIWidget>();
			if (!flag)
			{
				UIAnchorUtility.CopyAnchors(_dialogObject.OKbuttonFullSizeAnchors, component);
			}
			else
			{
				UIAnchorUtility.CopyAnchors(_dialogObject.OKbuttonHalfSizeAnchors, component);
			}
		}
		_dialogObject.okButton.onClicked = errorData.onOkClicked;
		_dialogObject.cancelButton.onClicked = errorData.onCancelClicked;
		_dialogObject.graphicHolder.SetActive(true);
	}

	public void Close()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_dialogObject.graphicHolder.SetActive(false);
		_buttonCollider.set_size(_buttonColliderOriginalSize);
		isOpen = false;
	}
}
