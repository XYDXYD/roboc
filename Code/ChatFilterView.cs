using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

[RequireComponent(typeof(UIPopupList))]
internal class ChatFilterView : MonoBehaviour, IChainListener
{
	private ChatFilterPresenter _presenter;

	public UILabel popupListLabel;

	private UIPopupList _popupList;

	private BubbleSignal<IChainRoot> _bubble;

	public UIPopupList popupList
	{
		get
		{
			if (_popupList == null)
			{
				_popupList = this.GetComponent<UIPopupList>();
			}
			return _popupList;
		}
	}

	public ChatFilterView()
		: this()
	{
	}

	private unsafe void Awake()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		EventDelegate.Add(popupList.onChange, new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
	}

	internal void SetPresenter(ChatFilterPresenter presenter)
	{
		_presenter = presenter;
	}

	public void Listen(object message)
	{
		if (message is ChatGUIEvent)
		{
			_presenter.OnGUIEvent((ChatGUIEvent)message);
		}
	}

	public void Dispatch(ChatGUIEvent.Type eventType, object arg = null)
	{
		_bubble.Dispatch<ChatGUIEvent>(new ChatGUIEvent(eventType, arg));
	}

	private void OnItemSelected()
	{
		_presenter.OnItemSelected();
	}
}
