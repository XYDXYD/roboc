using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

internal abstract class PhotonEventContainer<TEventCode> : IServiceEventContainer, IDisposable where TEventCode : struct
{
	protected IPhotonClient Client;

	private readonly HashSet<WeakReference<PhotonEventListenerBase>> _eventListeners = new HashSet<WeakReference<PhotonEventListenerBase>>();

	private readonly PhotonEventRegistry<TEventCode> _eventRegistry;

	public bool IsConnected => Client.IsConnectedAndReady;

	public event Action ReconnectedEvent;

	public event Action DisconnectedEvent;

	protected PhotonEventContainer(PhotonEventRegistry<TEventCode> eventRegistry, IPhotonClient client)
	{
		_eventRegistry = eventRegistry;
		Client = client;
	}

	public void ListenTo<TListener>(Action callBack, Action<Exception> errorCallback = null) where TListener : IServiceEventListener
	{
		PhotonEventListener photonEventListener = _eventRegistry.GetEventListener<TListener>() as PhotonEventListener;
		if (photonEventListener == null)
		{
			throw new Exception("Failed to get listener for interface " + typeof(TListener));
		}
		photonEventListener.AddCallback(this, callBack, errorCallback);
		_eventListeners.Add(new WeakReference<PhotonEventListenerBase>((PhotonEventListenerBase)photonEventListener));
	}

	public void ListenTo<TListener, TData>(Action<TData> callBack, Action<Exception> errorCallback = null) where TListener : IServiceEventListener<TData>
	{
		PhotonEventListener<TData> photonEventListener = _eventRegistry.GetEventListener<TListener>() as PhotonEventListener<TData>;
		if (photonEventListener == null)
		{
			throw new Exception("Failed to get listener for interface " + typeof(TListener));
		}
		photonEventListener.AddCallback(this, callBack, errorCallback);
		_eventListeners.Add(new WeakReference<PhotonEventListenerBase>((PhotonEventListenerBase)photonEventListener));
	}

	public void ListenTo<TListener, TData1, TData2>(Action<TData1, TData2> callBack, Action<Exception> errorCallback = null) where TListener : IServiceEventListener<TData1, TData2>
	{
		PhotonEventListener<TData1, TData2> photonEventListener = _eventRegistry.GetEventListener<TListener>() as PhotonEventListener<TData1, TData2>;
		if (photonEventListener == null)
		{
			throw new Exception("Failed to get listener for interface " + typeof(TListener));
		}
		photonEventListener.AddCallback(this, callBack, errorCallback);
		_eventListeners.Add(new WeakReference<PhotonEventListenerBase>((PhotonEventListenerBase)photonEventListener));
	}

	public void Dispose()
	{
		foreach (WeakReference<PhotonEventListenerBase> eventListener in _eventListeners)
		{
			if (eventListener.get_IsValid())
			{
				eventListener.get_Target().RemoveCallback(this);
			}
		}
		_eventListeners.Clear();
		this.DisconnectedEvent = null;
		this.ReconnectedEvent = null;
	}

	public void RaiseInternalEvent<TEventListener>() where TEventListener : PhotonEventListener
	{
		HashSet<WeakReference<PhotonEventListenerBase>>.Enumerator enumerator = _eventListeners.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeakReference<PhotonEventListenerBase> current = enumerator.Current;
			if (current != null && current.get_IsValid())
			{
				TEventListener val = current.get_Target() as TEventListener;
				val?.Invoke();
			}
		}
	}

	public void RaiseInternalEvent<TEventListener, TData>(TData data) where TEventListener : PhotonEventListener<TData>
	{
		HashSet<WeakReference<PhotonEventListenerBase>>.Enumerator enumerator = _eventListeners.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeakReference<PhotonEventListenerBase> current = enumerator.Current;
			if (current != null && current.get_IsValid())
			{
				TEventListener val = current.get_Target() as TEventListener;
				val?.Invoke(data);
			}
		}
	}

	public void RaiseInternalEvent<TEventListener, TData1, TData2>(TData1 data1, TData2 data2) where TEventListener : PhotonEventListener<TData1, TData2>
	{
		HashSet<WeakReference<PhotonEventListenerBase>>.Enumerator enumerator = _eventListeners.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeakReference<PhotonEventListenerBase> current = enumerator.Current;
			if (current != null && current.get_IsValid())
			{
				TEventListener val = current.get_Target() as TEventListener;
				val?.Invoke(data1, data2);
			}
		}
	}

	public virtual void Reconnected()
	{
		if (this.ReconnectedEvent != null)
		{
			this.ReconnectedEvent();
		}
	}

	public virtual void Disconnected()
	{
		if (this.DisconnectedEvent != null)
		{
			this.DisconnectedEvent();
		}
	}
}
