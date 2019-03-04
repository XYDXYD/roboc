using Utility;

namespace LiteNetLibExt
{
	internal class PacketWrapUtil
	{
		public static IPacketWrap Create(byte[] iv, byte[] hmacKey, int maxPacketSize)
		{
			return new PacketWrapDLL(iv, hmacKey, maxPacketSize);
		}

		public static byte[] RandomGenerate(int numBytes)
		{
			return PacketWrapDLL.RandomGenerate(numBytes);
		}

		public static byte[] TestData()
		{
			int num = 3;
			byte[] array = new byte[num];
			array[0] = 97;
			array[1] = 98;
			array[2] = 99;
			return array;
		}

		public static void Test(byte[] hmacKey)
		{
			byte[] iv = new byte[16]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15
			};
			IPacketWrap packetWrap = Create(iv, hmacKey, 16);
			for (int i = 0; i < 2; i++)
			{
				byte[] array = TestData();
				Console.Log("Input: " + Common.ByteArrayToString(array, 0, array.Length));
				byte[] array2 = packetWrap.Encode(array, 0, array.Length, 242);
				if (array2 != null)
				{
					Console.Log("Encrypted: " + Common.ByteArrayToString(array2, 0, array2.Length));
					int sequenceNumber;
					byte[] array3 = packetWrap.Decode(array2, 0, array2.Length, out sequenceNumber);
					if (array3 != null)
					{
						Console.Log("Decrypted: " + Common.ByteArrayToString(array3, 0, array3.Length));
					}
					else
					{
						Console.Log("decryption failed");
					}
				}
				else
				{
					Console.Log("encryption failed");
				}
			}
			for (int j = 0; j < 4; j++)
			{
				byte[] array4 = RandomGenerate(32);
				if (array4 != null)
				{
					Console.Log("Random: " + Common.ByteArrayToString(array4, 0, array4.Length));
				}
				else
				{
					Console.Log("RandomGenerate() failed");
				}
			}
		}
	}
}
