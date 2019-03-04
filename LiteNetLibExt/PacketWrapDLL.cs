using System;
using System.Runtime.InteropServices;

namespace LiteNetLibExt
{
	internal class PacketWrapDLL : IPacketWrap
	{
		private int _maxPacketSize;

		private int _dllSlack;

		private const int _wrapSlack = 4;

		private static bool _dllInited;

		private IntPtr _encryptTranslate;

		private IntPtr _decryptTranslate;

		private IntPtr _encryptInput;

		private IntPtr _encryptOutput;

		private IntPtr _decryptInput;

		private IntPtr _decryptOutput;

		private int _plainTextBufferSize;

		private int _cipherTextBufferSize;

		public PacketWrapDLL(byte[] iv, byte[] hmacKey, int maxPacketSize)
		{
			if (!_dllInited)
			{
				DllInit();
				_dllInited = true;
			}
			_encryptTranslate = AllocTranslate(iv, hmacKey);
			_decryptTranslate = AllocTranslate(iv, hmacKey);
			_dllSlack = Translate2GetSlackWithHash(_encryptTranslate);
			_maxPacketSize = maxPacketSize;
			_plainTextBufferSize = maxPacketSize + 4;
			_cipherTextBufferSize = maxPacketSize + 4 + _dllSlack;
			_encryptInput = Marshal.AllocHGlobal(_plainTextBufferSize);
			_encryptOutput = Marshal.AllocHGlobal(_cipherTextBufferSize);
			_decryptInput = Marshal.AllocHGlobal(_cipherTextBufferSize);
			_decryptOutput = Marshal.AllocHGlobal(_plainTextBufferSize);
		}

		[DllImport("BCrypt_C_WRT")]
		private static extern void DllInit();

		[DllImport("BCrypt_C_WRT")]
		private static extern IntPtr TranslateCreate(IntPtr ivBytes, int numIVBytes, IntPtr hmacBytes, int numHMACBytes);

		[DllImport("BCrypt_C_WRT")]
		private static extern void TranslateDestroy(IntPtr algorithm);

		[DllImport("BCrypt_C_WRT")]
		private static extern int Translate2GetSlackWithHash(IntPtr translate);

		[DllImport("BCrypt_C_WRT")]
		private static extern bool Translate2EncryptAndHashData(IntPtr translate, IntPtr inputBytes, int numInputBytes, IntPtr outputBytes, int maxOutputBytes, out int numOutputBytes);

		[DllImport("BCrypt_C_WRT")]
		private static extern bool Translate2DecryptAndHashData(IntPtr translate, IntPtr inputBytes, int numInputBytes, IntPtr outputBytes, int maxOutputBytes, out int numOutputBytes);

		[DllImport("BCrypt_C_WRT")]
		private static extern bool RandomGenerate(IntPtr outputBytes, int numOutputBytes);

		~PacketWrapDLL()
		{
			Marshal.FreeHGlobal(_encryptInput);
			Marshal.FreeHGlobal(_encryptOutput);
			Marshal.FreeHGlobal(_decryptInput);
			Marshal.FreeHGlobal(_decryptOutput);
			TranslateDestroy(_encryptTranslate);
			TranslateDestroy(_decryptTranslate);
		}

		public int GetEncryptSlackSize()
		{
			return 4 + _dllSlack;
		}

		public byte[] Encode(byte[] input, int offset, int inputLength, int sequenceNumber)
		{
			byte[] array = null;
			if (inputLength > _maxPacketSize)
			{
				throw new ArgumentException("input too large");
			}
			int numInputBytes = inputLength + 4;
			Marshal.Copy(input, offset, _encryptInput, inputLength);
			byte[] bytes = BitConverter.GetBytes(sequenceNumber);
			Marshal.Copy(bytes, 0, new IntPtr(_encryptInput.ToInt64() + inputLength), bytes.Length);
			if (Translate2EncryptAndHashData(_encryptTranslate, _encryptInput, numInputBytes, _encryptOutput, _cipherTextBufferSize, out int numOutputBytes))
			{
				array = new byte[numOutputBytes];
				Marshal.Copy(_encryptOutput, array, 0, numOutputBytes);
			}
			return array;
		}

		public byte[] Decode(byte[] input, int offset, int inputLength, out int sequenceNumber)
		{
			byte[] array = null;
			sequenceNumber = -1;
			Marshal.Copy(input, offset, _decryptInput, inputLength);
			if (inputLength > _maxPacketSize + 4 + _dllSlack)
			{
				throw new ArgumentException("input too large");
			}
			if (Translate2DecryptAndHashData(_decryptTranslate, _decryptInput, inputLength, _decryptOutput, _plainTextBufferSize, out int numOutputBytes))
			{
				int num = numOutputBytes - 4;
				if ((uint)num > _maxPacketSize)
				{
					throw new ArgumentException("output larger than expected");
				}
				array = new byte[num];
				Marshal.Copy(_decryptOutput, array, 0, num);
				IntPtr source = new IntPtr(_decryptOutput.ToInt64() + num);
				byte[] array2 = new byte[4];
				Marshal.Copy(source, array2, 0, 4);
				sequenceNumber = BitConverter.ToInt32(array2, 0);
			}
			return array;
		}

		private static IntPtr AllocTranslate(byte[] iv, byte[] hmacKey)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			int num = 0;
			int num2 = 0;
			if (iv != null)
			{
				num = iv.Length;
				intPtr = Marshal.AllocHGlobal(num);
				Marshal.Copy(iv, 0, intPtr, num);
			}
			if (hmacKey != null)
			{
				num2 = hmacKey.Length;
				intPtr2 = Marshal.AllocHGlobal(num2);
				Marshal.Copy(hmacKey, 0, intPtr2, num2);
			}
			IntPtr result = TranslateCreate(intPtr, num, intPtr2, num2);
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr2);
			}
			return result;
		}

		public static byte[] RandomGenerate(int numBytes)
		{
			byte[] array = null;
			IntPtr intPtr = Marshal.AllocHGlobal(numBytes);
			if (intPtr != IntPtr.Zero)
			{
				if (RandomGenerate(intPtr, numBytes))
				{
					array = new byte[numBytes];
					Marshal.Copy(intPtr, array, 0, numBytes);
				}
				Marshal.FreeHGlobal(intPtr);
			}
			return array;
		}
	}
}
