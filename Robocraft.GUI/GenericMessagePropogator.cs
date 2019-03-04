using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Robocraft.GUI
{
	public class GenericMessagePropogator
	{
		private Transform _startNode;

		public GenericMessagePropogator(Transform startNode)
		{
			_startNode = startNode;
		}

		public void SendMessageUpTree(GenericComponentMessage message)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Expected O, but got Unknown
			Transform parent = _startNode.get_transform().get_parent();
			IChainRoot val = null;
			while (parent != null && val == null)
			{
				GameObject gameObject = parent.get_gameObject();
				val = gameObject.GetComponent(typeof(IChainRoot));
				IChainListener[] components = gameObject.GetComponents<IChainListener>();
				for (int i = 0; i < components.GetLength(0); i++)
				{
					components[i].Listen((object)message);
					if (message.Consumed)
					{
						return;
					}
				}
				parent = parent.get_parent();
			}
		}
	}
}
