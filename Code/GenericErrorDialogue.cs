using Svelto.DataStructures;
using Svelto.IoC;
using UnityEngine;

internal sealed class GenericErrorDialogue : MonoBehaviour
{
	public UILabel bodyText;

	public GenericErrorDialogueButton cancelButton;

	public UILabel cancelButtonText;

	public GameObject graphicHolder;

	public GenericErrorDialogueButton okButton;

	public UISprite okButtonHighlight;

	public UILabel okButtonText;

	public UILabel title;

	public UIWidget OKbuttonFullSizeAnchors;

	public UIWidget OKbuttonHalfSizeAnchors;

	private GenericErrorDialogueController _controller;

	private GenericErrorData _errorData;

	private bool _hasBeenOpened;

	public bool isOpen => this.get_gameObject().get_activeInHierarchy() && this.get_enabled();

	[Inject]
	internal WeakReference<ICursorMode> cursorMode
	{
		private get;
		set;
	}

	public GenericErrorDialogue()
		: this()
	{
	}

	public void Close()
	{
		if (_hasBeenOpened && cursorMode != null && cursorMode.get_Target() != null)
		{
			cursorMode.get_Target().ReleaseForceFree();
		}
		_hasBeenOpened = false;
		_errorData = null;
		this.set_enabled(false);
		this.get_gameObject().SetActive(false);
		_controller.Close();
	}

	public void Open(GenericErrorData errorData)
	{
		_errorData = errorData;
		this.set_enabled(true);
		this.get_gameObject().SetActive(true);
		if (_hasBeenOpened)
		{
			Close();
		}
	}

	private void OnDisable()
	{
		Close();
	}

	private void Start()
	{
		_controller = new GenericErrorDialogueController(this);
		if (_errorData == null)
		{
			Close();
		}
	}

	private void Update()
	{
		if (_errorData != null)
		{
			if (cursorMode != null && cursorMode.get_Target() != null)
			{
				cursorMode.get_Target().ForceFree();
			}
			_controller.Open(_errorData);
			_errorData = null;
			_hasBeenOpened = true;
		}
	}
}
