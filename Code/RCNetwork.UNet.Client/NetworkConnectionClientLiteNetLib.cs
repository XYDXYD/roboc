using LiteNetLib;
using Network;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Utility;

namespace RCNetwork.UNet.Client
{
	internal class NetworkConnectionClientLiteNetLib : INetworkConnectionClient
	{
		private NetworkClient _networkClient;

		private ITaskRoutine _task;

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		public NetworkConnectionClientLiteNetLib(NetworkClient networkClient)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			_networkClient = networkClient;
			NetworkLogger.Attach();
			_task = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			_task.SetEnumerator((IEnumerator)new LoopActionEnumerator((Action)NetworkClient.UpdateClients));
			_task.Start((Action<PausableTaskException>)null, (Action)null);
		}

		public void Connect(string hostIp, int hostPort, NetworkConfig networkConfig, byte[] encryptionParams)
		{
			if (_networkClient.isConnected)
			{
				Console.LogWarning("Connect shouldn't be called if you expect to be connected");
				return;
			}
			Console.Log("Connecting to " + hostIp + ":" + hostPort);
			ConnectionConfigHelper.SetupConnection(networkConfig, encryptionParams, out ConnectionConfig config, out ConfigData configData);
			_networkClient.Configure(config, configData, 1);
			_networkClient.Connect(hostIp, hostPort);
		}

		public void Disconnect()
		{
			Console.Log("Disconnecting...network client status: " + _networkClient.isConnected);
			_networkClient.Disconnect();
			_networkClient.Shutdown();
			_task.Stop();
			if (NetworkClient.active)
			{
				Console.LogWarning("Network connections left open after disconnect");
				NetworkClient.ShutdownAll();
			}
			NetworkLogger.Detach();
			_networkClient = null;
		}
	}
}
