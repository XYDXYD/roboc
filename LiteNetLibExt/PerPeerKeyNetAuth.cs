using LiteNetLib;
using System.Collections.Generic;

namespace LiteNetLibExt
{
	public class PerPeerKeyNetAuth : NetAuth
	{
		private Dictionary<NetEndPoint, ConnectionInfo> _info = new Dictionary<NetEndPoint, ConnectionInfo>();

		public PerPeerKeyNetAuth(NetEncryptionFactory factory)
			: this(factory)
		{
		}

		public void SetConnectionInfo(NetEndPoint endPoint, NetEncryptionParams cryptParams, NetAuthParams authParams)
		{
			ConnectionInfo connectionInfo = new ConnectionInfo();
			connectionInfo._cryptParams = cryptParams.Clone();
			connectionInfo._authParams = ((authParams == null) ? null : authParams.Clone());
			connectionInfo._crypt = null;
			_info[endPoint] = connectionInfo;
		}

		public override bool TryConnect(NetEndPoint endPoint, out NetEncryption crypt, out byte[] authBytes)
		{
			if (_info.TryGetValue(endPoint, out ConnectionInfo value))
			{
				value._crypt = (crypt = this.CreateCrypt(true, value._cryptParams));
				authBytes = ((value._authParams == null) ? null : value._authParams.CreateChallenge());
				return true;
			}
			crypt = null;
			authBytes = null;
			return false;
		}

		public override bool TryAcceptPhase1ValidateEndPoint(NetEndPoint endPoint, out NetEncryption crypt)
		{
			if (_info.TryGetValue(endPoint, out ConnectionInfo value))
			{
				value._crypt = (crypt = this.CreateCrypt(true, value._cryptParams));
				return true;
			}
			crypt = null;
			return false;
		}

		public override bool TryAcceptPhase2ValidateAuthBytes(NetEndPoint endPoint, byte[] authBytes)
		{
			bool flag = false;
			if (_info.TryGetValue(endPoint, out ConnectionInfo value))
			{
				if (value._crypt != null && (value._authParams == null || value._authParams.TestChallenge(authBytes)))
				{
					flag = true;
				}
				if (!flag)
				{
					value._crypt = null;
				}
			}
			return flag;
		}

		public override void OnConnected(NetEndPoint endPoint)
		{
			if (_info.TryGetValue(endPoint, out ConnectionInfo value) && value._authParams != null)
			{
				value._authParams.ChallengePassed(value._crypt);
			}
		}

		public override void OnDisconnected(NetEndPoint endPoint)
		{
			if (_info.TryGetValue(endPoint, out ConnectionInfo value))
			{
				value._crypt = null;
			}
		}
	}
}
