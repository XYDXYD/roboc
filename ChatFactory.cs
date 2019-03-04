using Mothership.GUI.Social;
using Svelto.Factories;
using Svelto.IoC;
using UnityEngine;

internal class ChatFactory : IGUIElementFactory
{
	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	[Inject]
	internal ChatPresenter chatPresenter
	{
		private get;
		set;
	}

	public void Build(GameObject guiElementRoot, IContainer container)
	{
		container.BindSelf<ChatInputPresenter>();
		container.BindSelf<ChatHistoryPresenter>();
		container.BindSelf<ChatFilterPresenter>();
		container.BindSelf<ChatChannelTagPresenter>();
		ChatFilterPresenter chatFilterPresenter = container.Build<ChatFilterPresenter>();
		ChatInputPresenter chatInputPresenter = container.Build<ChatInputPresenter>();
		ChatHistoryPresenter chatHistoryPresenter = container.Build<ChatHistoryPresenter>();
		ChatChannelTagPresenter chatChannelTagPresenter = container.Build<ChatChannelTagPresenter>();
		ChatView chatView = container.Inject<ChatView>(guiElementRoot.GetComponentInChildren<ChatView>(true));
		chatView.SetVisible(visible: false);
		ChatFilterView chatFilterView = container.Inject<ChatFilterView>(chatView.GetComponentInChildren<ChatFilterView>(true));
		ChatInputView chatInputView = container.Inject<ChatInputView>(chatView.GetComponentInChildren<ChatInputView>(true));
		ChatHistoryView chatHistoryView = container.Inject<ChatHistoryView>(chatView.GetComponentInChildren<ChatHistoryView>(true));
		ChatChannelTagView chatChannelTagView = container.Inject<ChatChannelTagView>(chatView.GetComponentInChildren<ChatChannelTagView>(true));
		chatFilterView.SetPresenter(chatFilterPresenter);
		chatInputView.SetPresenter(chatInputPresenter);
		chatHistoryView.SetPresenter(chatHistoryPresenter);
		chatChannelTagView.SetPresenter(chatChannelTagPresenter);
		chatFilterPresenter.view = chatFilterView;
		chatInputPresenter.view = chatInputView;
		chatHistoryPresenter.view = chatHistoryView;
		chatChannelTagPresenter.view = chatChannelTagView;
		chatPresenter.SetView(chatView);
		ChatChannelListDataSource chatChannelListDataSource = new ChatChannelListDataSource(chatPresenter.chatChannelContainer);
		chatPresenter.SetChannelDataSource(chatChannelListDataSource);
		chatFilterPresenter.SetDataSource(chatChannelListDataSource);
		chatPresenter.InitializeView();
	}
}
