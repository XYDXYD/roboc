using RCNetwork.Events;
using Svelto.Command;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class SendTestDataClientCommand : IInjectableCommand<int>, ICommand
	{
		private int _messageCount;

		[Inject]
		internal INetworkEventManagerClient networkEventManagerClient
		{
			private get;
			set;
		}

		public void Execute()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			MachineDefinitionDependency dependency = new MachineDefinitionDependency(100, 1, Vector3.get_up(), Quaternion.get_identity(), 0, Vector3.get_down());
			for (int i = 0; i < _messageCount; i++)
			{
				networkEventManagerClient.SendEventToServer(NetworkEvent.TestConnection, dependency);
			}
		}

		public ICommand Inject(int dependency)
		{
			_messageCount = dependency;
			return this;
		}
	}
}
