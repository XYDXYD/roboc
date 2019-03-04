using Svelto.DataStructures;
using System.Collections.Generic;
using Utility;

internal class ChatBuffer : IChatMessageReceiver
{
	private static ChatBuffer _instance;

	public const uint BufferSize = 200u;

	private readonly Queue<ChatMessage> _buffer = new Queue<ChatMessage>();

	private readonly IChatClient _chatClient;

	private WeakReference<IChatMessageReceiver> _receiver;

	public static ChatBuffer Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChatBuffer(new ChatClientProvider().GetClient());
			}
			return _instance;
		}
	}

	public ChatBuffer(IChatClient chatClient)
	{
		_chatClient = chatClient;
		_chatClient.RegisterForMessageCallbacks(this);
	}

	public void AddMessage(ChatMessage message)
	{
		_buffer.Enqueue(message);
		if ((long)_buffer.Count >= 200L)
		{
			_buffer.Dequeue();
		}
		if (_receiver != null)
		{
			IChatMessageReceiver target = _receiver.get_Target();
			target.MessageReceived(message, this);
		}
	}

	public void Destroy()
	{
		if (_chatClient != null)
		{
			_chatClient.DeregisterMessageCallbacks(this);
		}
	}

	public List<ChatMessage> GetMessages()
	{
		return new List<ChatMessage>(_buffer);
	}

	public void MessageReceived(ChatMessage message, object sender)
	{
		AddMessage(message);
	}

	public void RemoveReceiver(IChatMessageReceiver receiver)
	{
		if (_receiver == null || _receiver.get_Target() != receiver)
		{
			Console.LogWarning("Cannot remove receiver that was not set");
		}
		_receiver = null;
	}

	public void SetReceiver(IChatMessageReceiver receiver)
	{
		_receiver = new WeakReference<IChatMessageReceiver>(receiver);
	}
}
