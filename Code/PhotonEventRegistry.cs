using ExitGames.Client.Photon;
using Services;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

internal abstract class PhotonEventRegistry<TEventCode> : PhotonEventRegistry where TEventCode : struct
{
	private readonly Dictionary<TEventCode, PhotonEventListenerBase> _eventListenersByCode = new Dictionary<TEventCode, PhotonEventListenerBase>();

	protected PhotonEventRegistry(IEventListenerFactory eventListenerFactory)
	{
		Dictionary<Type, IServiceEventListenerBase> dictionary = eventListenerFactory.CreateAllEventListeners();
		Dictionary<Type, IServiceEventListenerBase>.Enumerator enumerator = dictionary.GetEnumerator();
		while (true)
		{
			if (enumerator.MoveNext())
			{
				KeyValuePair<Type, IServiceEventListenerBase> current = enumerator.Current;
				PhotonEventListenerBase photonEventListenerBase = current.Value as PhotonEventListenerBase;
				if (photonEventListenerBase == null)
				{
					break;
				}
				TEventCode eventCode = (TEventCode)Enum.ToObject(typeof(TEventCode), photonEventListenerBase.EventCode);
				RegisterListener(current.Key, eventCode, photonEventListenerBase);
				continue;
			}
			return;
		}
		throw new Exception("Listener is not implemented by PhotonEventListenerBase");
	}

	internal override bool OnEvent(EventData eventData)
	{
		if (!Enum.IsDefined(typeof(TEventCode), eventData.Code))
		{
			return false;
		}
		TEventCode val = (TEventCode)Enum.ToObject(typeof(TEventCode), eventData.Code);
		if (_eventListenersByCode.ContainsKey(val))
		{
			_eventListenersByCode[val].OnEvent(eventData);
			return true;
		}
		throw new Exception("No event listener registered for " + val);
	}

	internal override void InternalEvent<TEventListener, TEventData>(TEventData eventData)
	{
		if (_eventListenersByInterface.ContainsKey(typeof(TEventListener)))
		{
			(_eventListenersByInterface[typeof(TEventListener)] as PhotonEventListener<TEventData>).Invoke(eventData);
			return;
		}
		throw new Exception("No event listener registered for event listener type " + typeof(TEventListener));
	}

	public PhotonEventListenerBase GetEventListener<TEventListener>()
	{
		Type typeFromHandle = typeof(TEventListener);
		return _eventListenersByInterface[typeFromHandle];
	}

	private void RegisterListener(Type interfaceType, TEventCode eventCode, PhotonEventListenerBase listener)
	{
		_eventListenersByCode[eventCode] = listener;
		_eventListenersByInterface.Add(interfaceType, listener);
	}
}
internal abstract class PhotonEventRegistry
{
	protected readonly Dictionary<Type, PhotonEventListenerBase> _eventListenersByInterface = new Dictionary<Type, PhotonEventListenerBase>();

	internal void Connected()
	{
		HashSet<IServiceEventContainer>.Enumerator enumerator = GetAllEventContainers().GetEnumerator();
		while (enumerator.MoveNext())
		{
			IServiceEventContainer current = enumerator.Current;
			current.Reconnected();
		}
	}

	internal void Disconnected()
	{
		HashSet<IServiceEventContainer>.Enumerator enumerator = GetAllEventContainers().GetEnumerator();
		while (enumerator.MoveNext())
		{
			IServiceEventContainer current = enumerator.Current;
			current.Disconnected();
		}
	}

	private HashSet<IServiceEventContainer> GetAllEventContainers()
	{
		HashSet<IServiceEventContainer> hashSet = new HashSet<IServiceEventContainer>();
		PhotonEventListenerBase[] array = _eventListenersByInterface.Values.ToArray();
		foreach (PhotonEventListenerBase photonEventListenerBase in array)
		{
			hashSet.UnionWith(photonEventListenerBase.GetAllContainers());
		}
		return hashSet;
	}

	internal abstract bool OnEvent(EventData eventData);

	internal abstract void InternalEvent<TEventListner, TEventData>(TEventData eventData);
}
