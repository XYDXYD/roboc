using System.IO;

namespace Network
{
	public class ErrorMessage : MessageBase
	{
		public int errorCode;

		public override void GhettoSerialize(BinaryWriter bw)
		{
			bw.Write((short)errorCode);
		}

		public override void GhettoDeserialize(BinaryReader br)
		{
			errorCode = br.ReadInt16();
		}
	}
}
