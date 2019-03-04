using System.IO;

namespace Network
{
	public class NetworkMessage
	{
		public NetworkConnection conn;

		public short msgType;

		internal BinaryReader reader;

		private bool _isReliableMsg;

		public bool IsReliableMsg
		{
			get
			{
				return _isReliableMsg;
			}
			set
			{
				_isReliableMsg = value;
			}
		}

		public TMsg ReadMessage<TMsg>() where TMsg : MessageBase, new()
		{
			TMsg result = new TMsg();
			result.GhettoDeserialize(reader);
			return result;
		}

		public void ReadMessage<TMsg>(TMsg msg) where TMsg : MessageBase
		{
			msg.GhettoDeserialize(reader);
		}
	}
}
