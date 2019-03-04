using Svelto.IoC;
using UnityEngine;

internal sealed class InputDialog : MonoBehaviour
{
	public UILabel BodyText;

	public InputDialogButton CancelButton;

	public UILabel CancelButtonText;

	public GameObject GraphicHolder;

	public UIInput Input;

	public UILabel Message;

	public InputDialogButton OkButton;

	public UILabel OkButtonText;

	public UILabel Title;

	public UIToggle Toggle;

	private InputDialogController _controller;

	private InputDialogData _data;

	[Inject]
	internal ICursorMode CursorMode
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputController GUIInputController
	{
		private get;
		set;
	}

	public bool IsOpen
	{
		get;
		private set;
	}

	public InputDialog()
		: this()
	{
		IsOpen = false;
		_controller = new InputDialogController(this);
	}

	public void Start()
	{
		_controller.Close();
	}

	public void Open(InputDialogData data)
	{
		_data = data;
		_controller.Open(_data);
	}

	public void Close()
	{
		if (IsOpen && CursorMode != null)
		{
			CursorMode.PopFreeMode();
		}
		IsOpen = false;
		_controller.Close();
	}

	public void Update()
	{
		if (_data != null)
		{
			if (CursorMode != null)
			{
				CursorMode.PushFreeMode();
			}
			_data = null;
			IsOpen = true;
		}
	}
}
