using LiteNetLib;
using System;
using System.Diagnostics;
using Utility;

namespace LiteNetLibExt
{
	internal class PacketWrapNetEncryption : NetEncryption
	{
		private IPacketWrap _packetWrap;

		private int _encryptNextSequence;

		private int _maxPacketSize;

		private SequenceWindow _decryptSequenceWindow;

		public static int _seqWinPass;

		public static int _seqWinFail;

		private object _mutex = new object();

		public static int StaticWorstCasePadding = 68;

		public override int WorstCasePadding => StaticWorstCasePadding;

		public PacketWrapNetEncryption(NetEncryptionParams encryptionParams, int maxPacketSize)
			: this(encryptionParams)
		{
			_maxPacketSize = maxPacketSize;
			this.UpdateEncryptionParams(encryptionParams);
			PacketWrapNetEncryptionParams packetWrapNetEncryptionParams = encryptionParams as PacketWrapNetEncryptionParams;
			_encryptNextSequence = packetWrapNetEncryptionParams._initialSequence;
			_decryptSequenceWindow = new SequenceWindow(packetWrapNetEncryptionParams._initialSequence);
		}

		public override void UpdateEncryptionParams(NetEncryptionParams encryptionParams)
		{
			if (encryptionParams == null)
			{
				throw new ArgumentException("null encryptionParams");
			}
			if (!(encryptionParams is PacketWrapNetEncryptionParams))
			{
				throw new ArgumentException("bad encryptionParams type");
			}
			PacketWrapNetEncryptionParams packetWrapNetEncryptionParams = encryptionParams as PacketWrapNetEncryptionParams;
			_packetWrap = PacketWrapUtil.Create(packetWrapNetEncryptionParams._aesIV, packetWrapNetEncryptionParams._hmacKey, _maxPacketSize - this.get_WorstCasePadding());
		}

		public override byte[] Encrypt(byte[] bufferIn, int offset, int length)
		{
			lock (_mutex)
			{
				byte[] result = _packetWrap.Encode(bufferIn, offset, length, _encryptNextSequence);
				_encryptNextSequence++;
				return result;
			}
		}

		public override bool Decrypt(byte[] bufferIn, int offset, int packetLength, byte[] bufferOut, out int bufferOutLength)
		{
			lock (_mutex)
			{
				int sequenceNumber;
				byte[] array = _packetWrap.Decode(bufferIn, offset, packetLength, out sequenceNumber);
				bool result = false;
				bufferOutLength = 0;
				if (array != null)
				{
					if (ValidateSequenceNumber(sequenceNumber))
					{
						Array.Copy(array, 0, bufferOut, 0, array.Length);
						bufferOutLength = array.Length;
						result = true;
					}
					else
					{
						if (array.Length > 0 && array[0] == 9)
						{
							Console.LogError("FATAL: discarded a ConnectAccept packet!");
						}
						Warning("Decrypt failed sequence number validation");
					}
				}
				else
				{
					Warning("Decrypt failed Decode (possibly HMAC)");
				}
				return result;
			}
		}

		private bool ValidateSequenceNumber(int sequenceNumber)
		{
			if (!_decryptSequenceWindow.Validate(sequenceNumber))
			{
				_seqWinFail++;
				return false;
			}
			_seqWinPass++;
			return true;
		}

		private void Warning(string text)
		{
		}

		[Conditional("WARNING_LOG")]
		private void WarningLog(string text)
		{
			Console.LogWarning(text);
		}

		[Conditional("WARNING_ASSERT")]
		private void WarningAssert()
		{
			throw new Exception("warning");
		}

		[Conditional("ENC_DEC_LOG")]
		private void LogIOBuffer(string text, byte[] inBuffer, int inOffset, int inLength, byte[] outBuffer, int outLength)
		{
			string text2 = text + ": [" + Common.ByteArrayToString(inBuffer, inOffset, inLength) + "] -> [" + ((outBuffer == null) ? string.Empty : Common.ByteArrayToString(outBuffer, 0, outLength));
			Console.Log(text2);
		}
	}
}
