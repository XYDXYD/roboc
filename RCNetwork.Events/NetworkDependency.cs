namespace RCNetwork.Events
{
	internal class NetworkDependency
	{
		public int senderId;

		public NetworkDependency()
		{
		}

		public NetworkDependency(byte[] data)
		{
			Deserialise(data);
		}

		public virtual byte[] Serialise()
		{
			return new byte[1];
		}

		public virtual void Deserialise(byte[] data)
		{
		}
	}
}
