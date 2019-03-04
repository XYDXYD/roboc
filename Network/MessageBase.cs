using System.IO;

namespace Network
{
	public abstract class MessageBase
	{
		public virtual void GhettoSerialize(BinaryWriter bw)
		{
		}

		public virtual void GhettoDeserialize(BinaryReader br)
		{
		}
	}
}
