using ExitGames.Client.Photon;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class SocialClient : PhotonClient
	{
		private bool _reconnectedNeeded;

		[CompilerGenerated]
		private static SerializeMethod _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static DeserializeMethod _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static SerializeMethod _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static DeserializeMethod _003C_003Ef__mg_0024cache3;

		protected override string serverAddressS3Key => "PhotonSocialServer";

		protected override string serverAddressLocalOverrideKey => "SocialServerAddress";

		protected override string applicationID => "SocialServer";

		protected override string appVersion => "v1";

		protected override int serviceConnectionInterval => 100;

		protected override int pingInterval => 3000;

		protected override int connectAttempts => 1;

		protected override float connectAttemptDelay => 0f;

		protected override byte duplicateLoginCode => 8;

		protected override byte ccuExceededCode => 40;

		protected override byte ccuCheckPassedCode => 41;

		protected override bool CCUCheckRequired => false;

		protected override byte maxPlayerPerRoom => 50;

		protected override int emptyRoomTtl => 10000;

		public SocialClient()
			: base(1)
		{
		}

		private void OnReconnected()
		{
			Console.Log("Social connected");
			_reconnectedNeeded = false;
		}

		private void OnUnexpectedDisconnectionHandler()
		{
			Console.Log("Social unexpectedly lost connection, attempting reconnection..");
			if (base.onUnexpectedDisconnection == null)
			{
				base.onUnexpectedDisconnection = (Action)Delegate.Combine(base.onUnexpectedDisconnection, new Action(OnUnexpectedDisconnectionHandler));
			}
			_reconnectedNeeded = true;
			TaskRunner.get_Instance().Run(ReConnectAfterDelay());
		}

		private IEnumerator ReConnectAfterDelay()
		{
			while (_reconnectedNeeded)
			{
				yield return (object)new WaitForSecondsEnumerator(10f);
				if (this.get_IsConnected())
				{
					break;
				}
				if (_reconnectedNeeded)
				{
					Console.Log("Attempting reconnect process..");
					this.Connect();
					_reconnectedNeeded = false;
				}
			}
		}

		public override bool Disconnect()
		{
			bool result = base.Disconnect();
			base.onUnexpectedDisconnection = (Action)Delegate.Remove(base.onUnexpectedDisconnection, new Action(OnUnexpectedDisconnectionHandler));
			base.onConnected -= (Action)OnReconnected;
			_reconnectedNeeded = false;
			return result;
		}

		protected override void OnDisconnected()
		{
			base.OnDisconnected();
		}

		protected unsafe override void OnConnectedToMaster()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected O, but got Unknown
			Console.Log("Photon social peer connected to master server");
			base.onUnexpectedDisconnection = (Action)Delegate.Combine(base.onUnexpectedDisconnection, new Action(OnUnexpectedDisconnectionHandler));
			base.onConnected += (Action)OnReconnected;
			_reconnectedNeeded = false;
			Type typeFromHandle = typeof(Friend);
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = new SerializeMethod((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			}
			SerializeMethod obj = _003C_003Ef__mg_0024cache0;
			if (_003C_003Ef__mg_0024cache1 == null)
			{
				_003C_003Ef__mg_0024cache1 = new DeserializeMethod((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			}
			PhotonPeer.RegisterType(typeFromHandle, (byte)0, obj, _003C_003Ef__mg_0024cache1);
			Type typeFromHandle2 = typeof(PlatoonMember);
			if (_003C_003Ef__mg_0024cache2 == null)
			{
				_003C_003Ef__mg_0024cache2 = new SerializeMethod((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			}
			SerializeMethod obj2 = _003C_003Ef__mg_0024cache2;
			if (_003C_003Ef__mg_0024cache3 == null)
			{
				_003C_003Ef__mg_0024cache3 = new DeserializeMethod((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			}
			PhotonPeer.RegisterType(typeFromHandle2, (byte)1, obj2, _003C_003Ef__mg_0024cache3);
			base.OnConnectedToMaster();
		}
	}
}
