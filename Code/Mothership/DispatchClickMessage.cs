using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class DispatchClickMessage : MonoBehaviour
	{
		private BubbleSignal<IContextMenuRoot> _bubble;

		public DispatchClickMessage()
			: this()
		{
		}

		public void Awake()
		{
			_bubble = new BubbleSignal<IContextMenuRoot>(this.get_transform());
		}

		private void OnClick()
		{
			_bubble.TargetedDispatch<GenericComponentMessage>(new GenericComponentMessage(MessageType.ButtonClicked, "ListItemBackgroundClick", string.Empty));
		}
	}
}
