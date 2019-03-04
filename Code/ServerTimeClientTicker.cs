using Svelto.IoC;
using UnityEngine;

internal sealed class ServerTimeClientTicker : MonoBehaviour
{
	[Inject]
	public IServerTimeClient serverTime
	{
		private get;
		set;
	}

	public ServerTimeClientTicker()
		: this()
	{
	}

	private void Update()
	{
		serverTime.time += Time.get_deltaTime();
	}
}
