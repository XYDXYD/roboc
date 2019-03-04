using Robocraft.GUI;

internal class ChatFilterPresenter
{
	private const string ALL_CHANNELS = "strAllChannels";

	private IDataSource _dataSource;

	internal ChatFilterView view
	{
		private get;
		set;
	}

	public void SetDataSource(IDataSource dataSource)
	{
		_dataSource = dataSource;
	}

	public void OnGUIEvent(ChatGUIEvent ev)
	{
		switch (ev.type)
		{
		case ChatGUIEvent.Type.SetFilter:
			view.popupList.set_value(FormatChannelName(ev.channel));
			break;
		case ChatGUIEvent.Type.ClearFilter:
			view.popupList.set_value(Localization.Get("strAllChannels", true));
			break;
		case ChatGUIEvent.Type.UpdateChannelList:
			UpdateFromSource();
			break;
		case ChatGUIEvent.Type.Disconnected:
			view.popupList.set_enabled(false);
			break;
		case ChatGUIEvent.Type.Connected:
			view.popupList.set_enabled(true);
			break;
		}
	}

	internal void OnItemSelected()
	{
		object data = view.popupList.get_data();
		if (data == null)
		{
			view.Dispatch(ChatGUIEvent.Type.ClearFilter);
			return;
		}
		IChatChannel arg = (IChatChannel)data;
		view.Dispatch(ChatGUIEvent.Type.SetFilter, arg);
	}

	private void UpdateFromSource()
	{
		if (_dataSource != null)
		{
			object data = view.popupList.get_data();
			view.popupList.Clear();
			view.popupList.AddItem(StringTableBase<StringTable>.Instance.GetString("strAllChannels"), (object)null);
			for (int i = 0; i < _dataSource.NumberOfDataItemsAvailable(0); i++)
			{
				IChatChannel chatChannel = _dataSource.QueryData<IChatChannel>(i, 0);
				view.popupList.AddItem(FormatChannelName(chatChannel), (object)chatChannel);
			}
			view.popupListLabel.set_text((data != null) ? ((IChatChannel)data).VisibleName : view.popupList.items[0]);
		}
	}

	private static string FormatChannelName(IChatChannel channel)
	{
		return (channel.ChatChannelType != ChatChannelType.Private) ? channel.VisibleName : $"[{channel.VisibleName}]";
	}
}
