using LiteNetLib;
using Network;
using System;
using System.IO;
using System.Text;

namespace RCNetwork.Utilities
{
	internal class RoboClientNetAuth : NetAuth
	{
		private NetEncryptionParams _cryptParams;

		private RoboUserInfo _user;

		private RoboClientNetAuth(NetEncryptionFactory factory, NetEncryptionParams cryptParams, RoboUserInfo user)
			: this(factory)
		{
			_cryptParams = ((cryptParams == null) ? null : cryptParams.Clone());
			_user = user.Clone();
		}

		public static RoboClientNetAuth Create(NetEncryptionFactory encrFactory, NetEncryptionParams encrParams, BinaryReader reader)
		{
			RoboUserInfo user = RoboUserInfo.Create(reader);
			return new RoboClientNetAuth(encrFactory, encrParams, user);
		}

		public static void Write(string xuid, byte[] peerKey, BinaryWriter writer)
		{
			RoboUserInfo roboUserInfo = new RoboUserInfo(xuid, peerKey);
			roboUserInfo.Write(writer);
		}

		public override bool TryConnect(NetEndPoint endPoint, out NetEncryption crypt, out byte[] authBytes)
		{
			crypt = this.CreateCrypt(true, _cryptParams);
			authBytes = _user.BuildAuthBytes();
			return true;
		}

		public override byte[] ClientGenerateChallengeResponse(NetEndPoint endPoint, byte[] challenge)
		{
			string customAuthString = this.get_CustomAuthString();
			byte[] bytes = Encoding.ASCII.GetBytes(customAuthString);
			NetworkLogger.Log($"[client_auth] {endPoint} ClientGenerateChallengeResponse challenge({BitConverter.ToString(challenge)}) response({BitConverter.ToString(bytes)})");
			return bytes;
		}

		public override bool TryAcceptPhase1ValidateEndPoint(NetEndPoint endPoint, out NetEncryption crypt)
		{
			crypt = null;
			return false;
		}
	}
}
