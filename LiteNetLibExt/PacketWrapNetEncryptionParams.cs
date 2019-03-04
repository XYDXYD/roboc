using LiteNetLib;
using System;
using System.IO;

namespace LiteNetLibExt
{
	internal class PacketWrapNetEncryptionParams : NetEncryptionParams
	{
		public byte[] _aesIV;

		public byte[] _hmacKey;

		public int _initialSequence;

		public PacketWrapNetEncryptionParams()
			: this()
		{
		}

		public override void Serialize(BinaryWriter bw)
		{
			bw.Write((byte)_aesIV.Length);
			bw.Write(_aesIV);
			bw.Write((byte)_hmacKey.Length);
			bw.Write(_hmacKey);
			bw.Write(_initialSequence);
		}

		public override void Deserialize(BinaryReader br)
		{
			int count = br.ReadByte();
			_aesIV = br.ReadBytes(count);
			count = br.ReadByte();
			_hmacKey = br.ReadBytes(count);
			_initialSequence = br.ReadInt32();
		}

		public override string ToString()
		{
			return "(IV:" + BitConverter.ToString(_aesIV) + ", HMAC:" + BitConverter.ToString(_hmacKey) + ", InitSeq:" + _initialSequence.ToString() + ")";
		}

		public override NetEncryptionParams Clone()
		{
			PacketWrapNetEncryptionParams packetWrapNetEncryptionParams = new PacketWrapNetEncryptionParams();
			packetWrapNetEncryptionParams._aesIV = ((_aesIV == null) ? null : (_aesIV.Clone() as byte[]));
			packetWrapNetEncryptionParams._hmacKey = (_hmacKey.Clone() as byte[]);
			packetWrapNetEncryptionParams._initialSequence = _initialSequence;
			return packetWrapNetEncryptionParams;
		}
	}
}
