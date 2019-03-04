using UnityEngine;

internal sealed class ServerTimeClientMock : IServerTimeClient
{
	public float time
	{
		get
		{
			return Time.get_time();
		}
		set
		{
		}
	}
}
