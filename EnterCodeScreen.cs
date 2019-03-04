using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

internal sealed class EnterCodeScreen : MonoBehaviour, IChainListener
{
	[SerializeField]
	private GameObject enterDialog;

	[SerializeField]
	private GameObject confirmDialog;

	[SerializeField]
	private UILabel codeLabel;

	[SerializeField]
	private UILabel confirmMessageLabel;

	private Action _onConfirmClicked;

	[Inject]
	internal EnterCodeDisplay enterCodeDisplay
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

	public EnterCodeScreen()
		: this()
	{
	}

	private void Start()
	{
		codeLabel.set_text(string.Empty);
		confirmDialog.get_gameObject().SetActive(false);
		enterCodeDisplay.SetView(this);
		Hide();
	}

	public void Show()
	{
		_onConfirmClicked = null;
		this.get_gameObject().SetActive(true);
		enterDialog.SetActive(true);
		confirmDialog.get_gameObject().SetActive(false);
		codeLabel.set_text(string.Empty);
	}

	public void Hide()
	{
		this.get_gameObject().SetActive(false);
	}

	public bool IsActive()
	{
		return this.get_gameObject().get_activeSelf();
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			switch ((ButtonType)message)
			{
			case ButtonType.Purchase:
				enterCodeDisplay.ProcessCode(codeLabel.get_text());
				break;
			case ButtonType.Confirm:
				_onConfirmClicked();
				break;
			case ButtonType.Cancel:
				guiInputController.CloseCurrentScreen();
				break;
			}
		}
	}

	public void Confirm(Action onConfirm, string messageStrKey = "")
	{
		_onConfirmClicked = onConfirm;
		if (messageStrKey != string.Empty)
		{
			confirmMessageLabel.set_text(StringTableBase<StringTable>.Instance.GetString(messageStrKey));
		}
		else
		{
			confirmMessageLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strEnterCodeSuccess"));
		}
		enterDialog.SetActive(false);
		confirmDialog.get_gameObject().SetActive(true);
	}
}
