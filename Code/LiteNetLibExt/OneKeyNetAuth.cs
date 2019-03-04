using LiteNetLib;

namespace LiteNetLibExt
{
	public class OneKeyNetAuth : NetAuth
	{
		private NetEncryptionParams _parameters;

		public OneKeyNetAuth(NetEncryptionFactory factory, NetEncryptionParams parameters)
			: this(factory)
		{
			_parameters = parameters;
		}

		public override bool TryConnect(NetEndPoint endPoint, out NetEncryption crypt, out byte[] authBytes)
		{
			crypt = this.CreateCrypt(true, _parameters);
			authBytes = null;
			return true;
		}

		public override bool TryAcceptPhase1ValidateEndPoint(NetEndPoint endPoint, out NetEncryption crypt)
		{
			crypt = this.CreateCrypt(false, _parameters);
			return true;
		}
	}
}
