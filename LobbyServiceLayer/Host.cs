namespace LobbyServiceLayer
{
	public class Host
	{
		public readonly string hostAddress;

		public readonly int hostPort;

		public Host(string hostAddress_, int hostPort_)
		{
			hostAddress = hostAddress_;
			hostPort = hostPort_;
		}
	}
}
