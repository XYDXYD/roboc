using UnityEngine;
using Utility;

internal sealed class QuitListener : MonoBehaviour
{
	internal QuitListenerManager quitListenerManager
	{
		private get;
		set;
	}

	public QuitListener()
		: this()
	{
	}

	private void Start()
	{
		Console.Log("QuitListener.Start");
	}

	private void OnApplicationQuit()
	{
		Console.Log("QuitListener.OnApplicationQuit");
		if (quitListenerManager != null)
		{
			quitListenerManager.IsQuitting();
		}
	}
}
