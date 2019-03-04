using System;

namespace Svelto.ECS.Legacy
{
	public class Dispatcher<S, T>
	{
		private readonly S _sender;

		public event Action<S, T> subscribers;

		public Dispatcher(S sender)
		{
			_sender = sender;
		}

		public void Dispatch(ref T value)
		{
			if (this.subscribers != null)
			{
				this.subscribers(_sender, value);
			}
		}
	}
	public class Dispatcher<T>
	{
		public event Action<T> subscribers;

		public void Dispatch(ref T value)
		{
			if (this.subscribers != null)
			{
				this.subscribers(value);
			}
		}
	}
	public class Dispatcher
	{
		private readonly int _senderID;

		public event Action<int> subscribers;

		public Dispatcher(int senderID)
		{
			_senderID = senderID;
		}

		public void Dispatch()
		{
			if (this.subscribers != null)
			{
				this.subscribers(_senderID);
			}
		}
	}
}
