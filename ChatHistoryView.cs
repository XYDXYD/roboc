using Svelto.UI.Comms.SignalChain;
using UnityEngine;

[RequireComponent(typeof(BetterUITextList))]
internal sealed class ChatHistoryView : MonoBehaviour, IChainListener
{
	public float fadeTime = 10f;

	private ChatHistoryPresenter _presenter;

	private BetterUITextList _textList;

	public BetterUITextList textList => _textList;

	public ChatHistoryView()
		: this()
	{
	}

	private void Awake()
	{
		_textList = this.GetComponent<BetterUITextList>();
		_textList.Clear();
	}

	internal void SetPresenter(ChatHistoryPresenter presenter)
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

	private void Update()
	{
		_presenter.Tick();
	}
}
