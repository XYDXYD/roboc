using Svelto.UI.Comms.SignalChain;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
internal class ChatChannelTagView : MonoBehaviour, IChainListener
{
	private UILabel _label;

	private ChatChannelTagPresenter _presenter;

	public UILabel label => _label;

	public ChatChannelTagView()
		: this()
	{
	}

	private void Awake()
	{
		_label = this.GetComponent<UILabel>();
		_label.set_supportEncoding(false);
	}

	public void Listen(object message)
	{
		if (message is ChatGUIEvent)
		{
			_presenter.OnGUIEvent((ChatGUIEvent)message);
		}
	}

	internal void SetPresenter(ChatChannelTagPresenter presenter)
	{
		_presenter = presenter;
	}
}
