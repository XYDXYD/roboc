using ExitGames.Client.Photon;
using Svelto.DataStructures;
using Svelto.WeakEvents;
using System;
using System.Collections.Generic;
using Utility;

namespace ChatServiceLayer.Photon
{
	internal class ChatClient : PhotonClient, IChatClient
	{
		private readonly HashSet<WeakReference<IChatMessageReceiver>> _messageCallbacks = new HashSet<WeakReference<IChatMessageReceiver>>();

		private readonly Dictionary<string, Action<ChatReturnCode>> _sendMessageResponseListeners = new Dictionary<string, Action<ChatReturnCode>>();

		protected override string applicationID => "ChatServer";

		protected override string appVersion => "v1";

		protected override int connectAttempts => 2;

		protected override byte duplicateLoginCode => 0;

		protected override byte ccuExceededCode => 40;

		protected override byte ccuCheckPassedCode => 41;

		protected override bool CCUCheckRequired => false;

		protected override int emptyRoomTtl => 10000;

		protected override byte maxPlayerPerRoom => 50;

		protected override int pingInterval => 1000;

		protected override float connectAttemptDelay => 1f;

		protected override string serverAddressLocalOverrideKey => "ChatServerAddress";

		protected override string serverAddressS3Key => "PhotonChatServer";

		protected override int serviceConnectionInterval => 100;

		public ChatClient()
			: base(1)
		{
		}

		public bool IsConnecting()
		{
			return base.IsConnecting;
		}

		public void Connect(Action onFailedToConnect)
		{
			if (base.IsConnectedAndReady)
			{
				throw new Exception("Should not use Connect while already connected");
			}
			if (onFailedToConnect != null)
			{
				base.onUnexpectedDisconnection = (Action)Delegate.Combine(base.onUnexpectedDisconnection, onFailedToConnect);
			}
			ConnectToPhoton();
		}

		public void DeregisterMessageCallbacks(IChatMessageReceiver receiver)
		{
			Console.Log(receiver + "Deregistering from message events");
			_messageCallbacks.RemoveWhere((WeakReference<IChatMessageReceiver> callback) => callback.get_Target() == receiver);
		}

		public bool IsConnected()
		{
			return base.IsConnectedAndReady;
		}

		public override void OnEvent(EventData eventData)
		{
			try
			{
				switch (eventData.Code)
				{
				case 1:
				{
					string senderName2 = (string)eventData.Parameters[5];
					string senderDisplayName2 = (string)eventData.Parameters[30];
					string text2 = (string)eventData.Parameters[2];
					bool isDev2 = (bool)eventData.Parameters[6];
					bool isMod2 = (bool)eventData.Parameters[12];
					bool isAdmin2 = (bool)eventData.Parameters[13];
					string channelName = (string)eventData.Parameters[3];
					ChatChannelType channelType = (ChatChannelType)eventData.Parameters[1];
					MessageReceived(channelName, channelType, senderName2, senderDisplayName2, text2, isDev2, isMod2, isAdmin2);
					return;
				}
				case 2:
				{
					string senderName = (string)eventData.Parameters[5];
					string senderDisplayName = (string)eventData.Parameters[30];
					string text = (string)eventData.Parameters[2];
					bool isDev = (bool)eventData.Parameters[6];
					bool isMod = (bool)eventData.Parameters[12];
					bool isAdmin = (bool)eventData.Parameters[13];
					PrivateMessageReceived(senderName, senderDisplayName, text, isDev, isMod, isAdmin);
					return;
				}
				}
			}
			catch (Exception ex)
			{
				Console.LogException(ex);
			}
			base.OnEvent(eventData);
		}

		public override void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.OperationCode == 2)
			{
				string key = (string)operationResponse.Parameters[0];
				_sendMessageResponseListeners[key]((ChatReturnCode)operationResponse.ReturnCode);
				_sendMessageResponseListeners.Remove(key);
			}
			else
			{
				base.OnOperationResponse(operationResponse);
			}
		}

		public void RegisterForMessageCallbacks(IChatMessageReceiver receiver)
		{
			Console.Log(receiver + " Registering for message events");
			_messageCallbacks.Add(new WeakReference<IChatMessageReceiver>(receiver));
		}

		public void SendMessage(string channelName, ChatChannelType channelType, OutGoingChatMessage message, WeakAction<OutGoingChatMessage> onSuccess, WeakAction<ChatReasonCode> onError, Action<Exception> onException)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			Guid guid = Guid.NewGuid();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(3, channelName);
			dictionary.Add(1, channelType);
			dictionary.Add(2, message.RawText);
			dictionary.Add(0, guid.ToString());
			dictionary.Add(29, message.ChatLocation);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = 2;
			val.Parameters = parameters;
			if (!SendOp(val))
			{
				onException(new Exception("No connection to " + applicationID));
			}
			_sendMessageResponseListeners.Add(guid.ToString(), delegate(ChatReturnCode returnCode)
			{
				OnMessageSendResponse(returnCode, message, onSuccess, onError);
			});
		}

		public void SendPrivateMessage(string recipient, string message, string guid, Action<Exception> onFailed)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(0, guid);
			dictionary.Add(7, recipient);
			dictionary.Add(2, message);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = 3;
			val.Parameters = parameters;
			if (!SendOp(val))
			{
				onFailed(new Exception("No connection to " + applicationID));
			}
		}

		protected override void OnConnectedToMaster()
		{
			Console.Log("Photon chat client connected to master");
			base.OnConnectedToMaster();
		}

		private void MessageReceived(string channelName, ChatChannelType channelType, string senderName, string senderDisplayName, string text, bool isDev, bool isMod, bool isAdmin)
		{
			InboundChatMessage message = new InboundChatMessage(senderName, senderDisplayName, isDev, isMod, isAdmin, text, isPrivate: false, channelName, channelType);
			foreach (WeakReference<IChatMessageReceiver> messageCallback in _messageCallbacks)
			{
				IChatMessageReceiver target = messageCallback.get_Target();
				if (target == null)
				{
					Console.LogWarning("Dead weakreference found in ChatClient._messageCallbacks");
				}
				else
				{
					target.MessageReceived(message, this);
				}
			}
		}

		private void OnMessageSendResponse(ChatReturnCode returnCode, OutGoingChatMessage outGoingChatMessage, WeakAction<OutGoingChatMessage> onSuccess, WeakAction<ChatReasonCode> onError)
		{
			if (returnCode != 0)
			{
				onError.Invoke((ChatReasonCode)returnCode);
			}
			else
			{
				onSuccess.Invoke(outGoingChatMessage);
			}
		}

		private void PrivateMessageReceived(string senderName, string senderDisplayName, string text, bool isDev, bool isMod, bool isAdmin)
		{
			IncomingPrivateChatMessage message = new IncomingPrivateChatMessage(senderName, senderDisplayName, text, isDev, isMod, isAdmin);
			foreach (WeakReference<IChatMessageReceiver> messageCallback in _messageCallbacks)
			{
				IChatMessageReceiver target = messageCallback.get_Target();
				if (target == null)
				{
					Console.LogWarning("Dead weakreference found in ChatClient._messageCallbacks");
				}
				else
				{
					target.MessageReceived(message, this);
				}
			}
		}
	}
}
