using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

[RequireComponent(typeof(UIInputWithFocusEvents))]
internal sealed class ChatInputView : MonoBehaviour, IChainListener
{
	private ChatInputPresenter _presenter;

	private UIInputWithFocusEvents _input;

	private UIInputPlaceholder _placeholder;

	private BubbleSignal<IChainRoot> _messageBubble;

	public UIInput input => _input;

	public UIInputPlaceholder placeholder => _placeholder;

	public ChatInputView()
		: this()
	{
	}

	private unsafe void Awake()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		_input = this.GetComponent<UIInputWithFocusEvents>();
		_messageBubble = new BubbleSignal<IChainRoot>(this.get_transform());
		_input.OnInputGetFocus += OnInputGetFocus;
		_input.OnInputLoseFocus += OnInputLoseFocus;
		EventDelegate.Add(_input.onSubmit, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		_input.selectAllTextOnFocus = false;
		_input.disableTab = true;
		_placeholder = this.GetComponent<UIInputPlaceholder>();
	}

	internal void SetPresenter(ChatInputPresenter presenter)
	{
		_presenter = presenter;
	}

	public void OnClick()
	{
	}

	public void Listen(object message)
	{
		if (message is ChatGUIEvent)
		{
			_presenter.OnGUIEvent((ChatGUIEvent)message);
		}
	}

	private void Update()
	{
		_presenter.Tick();
	}

	internal void Bubble(ChatGUIEvent chatGUIEvent)
	{
		_messageBubble.Dispatch<ChatGUIEvent>(chatGUIEvent);
	}

	private void OnInputGetFocus()
	{
		_presenter.OnGetFocus();
	}

	private void OnInputLoseFocus()
	{
		_presenter.OnLoseFocus();
	}

	public void OnSubmit()
	{
		_presenter.OnSubmit();
	}
}
