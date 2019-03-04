using LiteNetLib;
using System;

namespace LiteNetLibExt
{
	internal class BasicNetAuthParams : NetAuthParams
	{
		private int _data;

		public BasicNetAuthParams(int data)
			: this()
		{
			_data = data;
		}

		public override byte[] CreateChallenge()
		{
			return BitConverter.GetBytes(_data);
		}

		public override bool TestChallenge(byte[] bytes)
		{
			if (bytes == null)
			{
				return false;
			}
			byte[] bytes2 = BitConverter.GetBytes(_data);
			if (bytes2.Length != bytes.Length)
			{
				return false;
			}
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes2[i] != bytes[i])
				{
					return false;
				}
			}
			return true;
		}

		public override void ChallengePassed(NetEncryption crypt)
		{
		}

		public override NetAuthParams Clone()
		{
			return new BasicNetAuthParams(_data);
		}
	}
}
