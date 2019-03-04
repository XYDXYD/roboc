using ExitGames.Client.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.WeakEvents;
using System;
using System.Collections.Generic;
using Utility;

internal abstract class PhotonEventListenerBase<TAction> : PhotonEventListenerBase where TAction : WeakActionBase
{
	private readonly Dictionary<WeakReference<IServiceEventContainer>, TAction> _callbacks = new Dictionary<WeakReference<IServiceEventContainer>, TAction>();

	protected readonly FasterList<KeyValuePair<WeakReference<IServiceEventContainer>, TAction>> _callbacksReusable = new FasterList<KeyValuePair<WeakReference<IServiceEventContainer>, KeyValuePair<WeakReference<IServiceEventContainer>, TAction>>>();

	public void AddCallback(IServiceEventContainer eventContainer, TAction callback, Action<Exception> errorCallback)
	{
		_callbacks.Add(new WeakReference<IServiceEventContainer>(eventContainer), callback);
		AddErrorCallback(eventContainer, errorCallback);
	}

	public override void RemoveCallback(IServiceEventContainer eventContainer)
	{
		Dictionary<WeakReference<IServiceEventContainer>, TAction>.KeyCollection.Enumerator enumerator = _callbacks.Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeakReference<IServiceEventContainer> current = enumerator.Current;
			if (current != null && current.get_IsValid() && current.get_Target() == eventContainer)
			{
				_callbacks.Remove(current);
				break;
			}
		}
		RemoveErrorCallback(eventContainer);
	}

	public override IEnumerable<IServiceEventContainer> GetAllContainers()
	{
		List<IServiceEventContainer> list = new List<IServiceEventContainer>(_callbacks.Count);
		Dictionary<WeakReference<IServiceEventContainer>, TAction>.KeyCollection.Enumerator enumerator = _callbacks.Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			WeakReference<IServiceEventContainer> current = enumerator.Current;
			if (current != null && current.get_IsValid())
			{
				list.Add(current.get_Target());
			}
		}
		return list;
	}

	protected unsafe IEnumerable<TAction> GetCallbacks()
	{
		_callbacksReusable.Clear();
		_callbacksReusable.AddRange((ICollection<KeyValuePair<WeakReference<IServiceEventContainer>, TAction>>)_callbacks);
		FasterListEnumerator<KeyValuePair<WeakReference<IServiceEventContainer>, TAction>> enumerator = _callbacksReusable.GetEnumerator();
		while (((FasterListEnumerator<KeyValuePair<WeakReference<IServiceEventContainer>, KeyValuePair<WeakReference<IServiceEventContainer>, TAction>>>*)(&enumerator))->MoveNext())
		{
			WeakReference<IServiceEventContainer> eventContainer = ((FasterListEnumerator<KeyValuePair<WeakReference<IServiceEventContainer>, KeyValuePair<WeakReference<IServiceEventContainer>, TAction>>>*)(&enumerator))->get_Current().Key;
			if (eventContainer == null || !eventContainer.get_IsValid())
			{
				Console.LogWarning("Garbage-collected event container found - this should have been disposed");
				_callbacks.Remove(eventContainer);
				continue;
			}
			TAction callback = ((FasterListEnumerator<KeyValuePair<WeakReference<IServiceEventContainer>, KeyValuePair<WeakReference<IServiceEventContainer>, TAction>>>*)(&enumerator))->get_Current().Value;
			if (callback == null || !callback.get_IsValid())
			{
				Console.LogWarning("Garbage-collected event callback found - the corresponding event continer should have been disposed");
				_callbacks.Remove(eventContainer);
			}
			else
			{
				yield return callback;
			}
		}
		_callbacksReusable.Clear();
	}
}
internal abstract class PhotonEventListenerBase
{
	private readonly Dictionary<WeakReference<IServiceEventContainer>, WeakAction<Exception>> _errorCallbacks = new Dictionary<WeakReference<IServiceEventContainer>, WeakAction<Exception>>();

	private readonly FasterList<KeyValuePair<WeakReference<IServiceEventContainer>, WeakAction<Exception>>> _errorCallbacksReusable = new FasterList<KeyValuePair<WeakReference<IServiceEventContainer>, WeakAction<Exception>>>();

	public abstract int EventCode
	{
		get;
	}

	public abstract void RemoveCallback(IServiceEventContainer eventContainer);

	public abstract IEnumerable<IServiceEventContainer> GetAllContainers();

	protected abstract void ParseData(EventData eventData);

	protected void RemoveErrorCallback(IServiceEventContainer eventContainer)
	{
		Dictionary<WeakReference<IServiceEventContainer>, WeakAction<Exception>>.KeyCollection.Enumerator enumerator = _errorCallbacks.Keys.GetEnumerator();
		WeakReference<IServiceEventContainer> current;
		do
		{
			if (enumerator.MoveNext())
			{
				current = enumerator.Current;
				continue;
			}
			return;
		}
		while (current == null || !current.get_IsValid() || current.get_Target() != eventContainer);
		_errorCallbacks.Remove(current);
	}

	public void OnEvent(EventData eventData)
	{
		try
		{
			ParseData(eventData);
		}
		catch (Exception exception)
		{
			ExceptionWhileParsing(exception);
		}
	}

	public void OnEvent(object message)
	{
		ParseData(message as EventData);
	}

	protected void ExceptionWhileParsing(Exception exception)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (_errorCallbacks.Count > 0)
		{
			_errorCallbacksReusable.FastClear();
			_errorCallbacksReusable.AddRange((ICollection<KeyValuePair<WeakReference<IServiceEventContainer>, WeakAction<Exception>>>)_errorCallbacks);
			FasterListEnumerator<KeyValuePair<WeakReference<IServiceEventContainer>, WeakAction<Exception>>> enumerator = _errorCallbacksReusable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WeakReference<IServiceEventContainer> key = enumerator.get_Current().Key;
				if (key == null || !key.get_IsValid())
				{
					Console.LogWarning("Garbage-collected event container found - this should have been disposed");
					_errorCallbacks.Remove(key);
					continue;
				}
				WeakAction<Exception> value = enumerator.get_Current().Value;
				if (value == null || !value.get_IsValid())
				{
					Console.LogWarning("Garbage-collected event callback found - the corresponding event continer should have been disposed");
					_errorCallbacks.Remove(key);
				}
				else
				{
					value.Invoke(exception);
				}
			}
			_errorCallbacksReusable.FastClear();
			return;
		}
		throw new Exception($"Unhandled exception parsing event data in {this}", exception);
	}

	protected void AddErrorCallback(IServiceEventContainer eventContainer, Action<Exception> errorCallback)
	{
		if (errorCallback != null)
		{
			_errorCallbacks.Add(new WeakReference<IServiceEventContainer>(eventContainer), new WeakAction<Exception>(errorCallback));
		}
	}
}
