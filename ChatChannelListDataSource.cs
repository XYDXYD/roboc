using Robocraft.GUI;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;

internal class ChatChannelListDataSource : IDataSource
{
	private ChatChannelContainer _container;

	private FasterList<IChatChannel> _channels = new FasterList<IChatChannel>();

	public ChatChannelListDataSource(ChatChannelContainer container)
	{
		_container = container;
	}

	public int NumberOfDataItemsAvailable(int dimension)
	{
		if (dimension == 0)
		{
			return _channels.get_Count();
		}
		return 0;
	}

	public T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
	{
		if (uniqueIdentifier1 < 0)
		{
			throw new InvalidDataIndexException(uniqueIdentifier1, uniqueIdentifier2, GetType().Name);
		}
		if (uniqueIdentifier1 >= NumberOfDataItemsAvailable(0))
		{
			throw new NoMoreDataException(uniqueIdentifier1, uniqueIdentifier2, GetType().Name);
		}
		return (T)_channels.get_Item(uniqueIdentifier1);
	}

	public IEnumerator RefreshData()
	{
		RefreshData(null, null);
		return null;
	}

	public void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
	{
		_channels.FastClear();
		IEnumerator<IChatChannel> enumerator = _container.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_channels.Add(enumerator.Current);
		}
	}
}
