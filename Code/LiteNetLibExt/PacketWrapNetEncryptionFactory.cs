using LiteNetLib;

namespace LiteNetLibExt
{
	internal class PacketWrapNetEncryptionFactory : NetEncryptionFactory
	{
		private int _maxPacketSize;

		public override int WorstCasePadding => PacketWrapNetEncryption.StaticWorstCasePadding;

		public PacketWrapNetEncryptionFactory(int maxPacketSize)
			: this()
		{
			_maxPacketSize = maxPacketSize;
		}

		public override NetEncryption Create(NetEncryptionParams encryptionParams)
		{
			return new PacketWrapNetEncryption(encryptionParams, _maxPacketSize);
		}
	}
}
