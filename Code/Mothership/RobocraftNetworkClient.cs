using LiteNetLib;
using Network;
using RCNetwork.Utilities;
using System.IO;

namespace Mothership
{
	internal class RobocraftNetworkClient : NetworkClient
	{
		protected override NetAuth CreateCustomNetAuth(NetEncryptionFactory encrFactory, NetEncryptionParams encrParams, BinaryReader reader)
		{
			return RoboClientNetAuth.Create(encrFactory, encrParams, reader);
		}
	}
}
