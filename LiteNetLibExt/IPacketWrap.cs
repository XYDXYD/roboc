namespace LiteNetLibExt
{
	internal interface IPacketWrap
	{
		byte[] Encode(byte[] input, int offset, int inputLength, int sequenceNumber);

		byte[] Decode(byte[] input, int offset, int inputLength, out int sequenceNumber);
	}
}
