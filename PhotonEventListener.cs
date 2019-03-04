using Svelto.ServiceLayer;
using Svelto.WeakEvents;
using System;
using System.Collections.Generic;

internal abstract class PhotonEventListener<TData1, TData2> : PhotonEventListenerBase<WeakAction<TData1, TData2>>
{
	public void AddCallback(IServiceEventContainer eventContainer, Action<TData1, TData2> callBack, Action<Exception> errorCallback = null)
	{
		AddCallback(eventContainer, new WeakAction<TData1, TData2>(callBack), errorCallback);
	}

	public void Invoke(TData1 data1, TData2 data2)
	{
		IEnumerator<WeakAction<TData1, TData2>> enumerator = GetCallbacks().GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Invoke(data1, data2);
		}
	}
}
internal abstract class PhotonEventListener<TData> : PhotonEventListenerBase<WeakAction<TData>>
{
	public void AddCallback(IServiceEventContainer eventContainer, Action<TData> callBack, Action<Exception> errorCallback = null)
	{
		AddCallback(eventContainer, new WeakAction<TData>(callBack), errorCallback);
	}

	public void Invoke(TData data)
	{
		IEnumerator<WeakAction<TData>> enumerator = GetCallbacks().GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Invoke(data);
		}
	}
}
internal abstract class PhotonEventListener : PhotonEventListenerBase<WeakAction>
{
	public void AddCallback(IServiceEventContainer eventContainer, Action callBack, Action<Exception> errorCallback = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		AddCallback(eventContainer, new WeakAction(callBack), errorCallback);
	}

	public void Invoke()
	{
		IEnumerator<WeakAction> enumerator = GetCallbacks().GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Invoke();
		}
	}
}
