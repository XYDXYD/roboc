using Mothership.GUI.Social;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

internal class ChatView : MonoBehaviour, IChainRoot, IChainListener, IGUIFactoryType
{
	public GameObject[] enabledElementsOnFocus;

	public BuildModeChatStyle buildModeStyle;

	public EndBattleChatStyle endBattleStyle;

	public BattleQueueChatStyle battleQueueStyle;

	public CustomGameStyle customGameStyle;

	public GarageChatStyle garageGameStyle;

	private BubbleSignal<IChainRoot> _bubble;

	private SignalChain _signalChain;

	private UIWidget _container;

	private int _titleBarRightAnchorDefaultValue;

	private int _inputBarLeftAnchorDefaultValue;

	[Inject]
	internal ChatPresenter _presenter
	{
		private get;
		set;
	}

	public Type guiElementFactoryType => typeof(ChatFactory);

	public ChatView()
		: this()
	{
	}

	private void Awake()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		_container = this.GetComponent<UIWidget>();
		_bubble = new BubbleSignal<IChainRoot>(this.get_transform().get_parent());
		_signalChain = new SignalChain(this.get_transform());
		if (customGameStyle.titleBar != null)
		{
			_titleBarRightAnchorDefaultValue = customGameStyle.titleBar.rightAnchor.absolute;
		}
		if (customGameStyle.inputBar != null)
		{
			_inputBarLeftAnchorDefaultValue = customGameStyle.inputBar.leftAnchor.absolute;
		}
	}

	private void OnDestroy()
	{
		_presenter.ClearView(this);
	}

	public void DeepBroadcast(ChatGUIEvent.Type evType, object arg = null)
	{
		_signalChain.DeepBroadcast<ChatGUIEvent>(new ChatGUIEvent(evType, arg));
	}

	public void BubbleTargetedDispatch(ChatGUIEvent.Type evType, object arg = null)
	{
		_bubble.TargetedDispatch<ChatGUIEvent>(new ChatGUIEvent(evType, arg));
	}

	public void Listen(object obj)
	{
		_presenter.OnGUIEvent(obj);
	}

	public void SetVisible(bool visible)
	{
		this.get_gameObject().SetActive(visible);
	}

	public void ApplyGarageMinimised()
	{
		UndoCustomGameChanges();
		UIAnchorUtility.CopyAnchors(garageGameStyle._minimizedSize, _container);
	}

	public void ApplyGarageMaximised()
	{
		UndoCustomGameChanges();
		UIAnchorUtility.CopyAnchors(garageGameStyle._maximisedSize, _container);
	}

	public void ApplyBuildModeMinimised()
	{
		UndoCustomGameChanges();
		UIAnchorUtility.CopyAnchors(buildModeStyle._minimizedSize, _container);
	}

	public void ApplyBuildModeMaximised()
	{
		UndoCustomGameChanges();
		UIAnchorUtility.CopyAnchors(buildModeStyle._maximisedSize, _container);
	}

	public void ApplyBattleQueueModeMinimised()
	{
		UndoCustomGameChanges();
		UIAnchorUtility.CopyAnchors(battleQueueStyle._minimizedSize, _container);
	}

	public void ApplyBattleQueueModeMaximised()
	{
		UndoCustomGameChanges();
		UIAnchorUtility.CopyAnchors(battleQueueStyle._maximisedSize, _container);
	}

	private void UndoCustomGameChanges()
	{
		for (int i = 0; i < customGameStyle.disabledElements.Length; i++)
		{
			customGameStyle.disabledElements[i].SetActive(true);
		}
		if (customGameStyle.inputBar != null)
		{
			customGameStyle.inputBar.leftAnchor.absolute = _inputBarLeftAnchorDefaultValue;
		}
		if (customGameStyle.titleBar != null)
		{
			customGameStyle.titleBar.rightAnchor.absolute = _titleBarRightAnchorDefaultValue;
		}
	}

	public void ApplyEndBattleStyle()
	{
		for (int i = 0; i < endBattleStyle.enabledElements.Length; i++)
		{
			endBattleStyle.enabledElements[i].SetActive(true);
		}
		for (int j = 0; j < endBattleStyle.enabledElements.Length; j++)
		{
			endBattleStyle.enabledElementsOnFocus[j].SetActive(_presenter.IsChatFocused());
		}
		if (_container.panel != null)
		{
			if (_presenter.IsChatFocused())
			{
				_container.panel.set_depth(endBattleStyle.focussedDepth);
			}
			else
			{
				_container.panel.set_depth(endBattleStyle.unfocussedDepth);
			}
		}
	}

	public void ApplyCustomGameStyle()
	{
		for (int i = 0; i < customGameStyle.disabledElements.Length; i++)
		{
			customGameStyle.disabledElements[i].SetActive(false);
		}
		GameObject val = GameObject.FindGameObjectWithTag("CustomGameChatContainer");
		UIWidget component = val.GetComponent<UIWidget>();
		UIAnchorUtility.CopyAnchors(component, _container);
		if (customGameStyle.inputBar != null)
		{
			customGameStyle.inputBar.leftAnchor.absolute = 0;
		}
		if (customGameStyle.titleBar != null)
		{
			customGameStyle.titleBar.rightAnchor.absolute = 0;
		}
	}

	private void Update()
	{
		_presenter.Tick();
	}
}
