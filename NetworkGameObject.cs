using UnityEngine;
using Utility;

internal sealed class NetworkGameObject
{
	private GameObject _gameObject;

	private static NetworkGameObject _instance;

	public static NetworkGameObject Instance
	{
		get
		{
			if (_instance == null || _instance._gameObject == null)
			{
				InitInstance();
			}
			return _instance;
		}
	}

	public T GetNetworkComponent<T>() where T : Component
	{
		if (_gameObject != null)
		{
			T component = _gameObject.GetComponent<T>();
			if ((object)component != null)
			{
				return component;
			}
			return _gameObject.AddComponent<T>();
		}
		Console.LogWarning("NetworkGameobject == null (probably destroyed)");
		return (T)(object)null;
	}

	private static void InitInstance()
	{
		_instance = new NetworkGameObject();
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainNetworkObject");
		_instance._gameObject = gameObject;
	}
}
