using Robocraft.GUI.Iteration2;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Party
{
	internal class InvitablePlayerItemView : MonoBehaviour, IView, IChainListener
	{
		[SerializeField]
		private UILabel _playerName;

		[SerializeField]
		private UITexture _playerAvatar;

		[SerializeField]
		private GameObject _inviteButton;

		private InvitablePlayerItemPresenter _presenter;

		private BubbleSignal<IChainRoot> _bubble;

		internal UILabel playerName => _playerName;

		internal UITexture playerAvatar => _playerAvatar;

		internal GameObject inviteButton => _inviteButton;

		public InvitablePlayerItemView()
			: this()
		{
		}

		public void SetPresenter(InvitablePlayerItemPresenter presenter)
		{
			_presenter = presenter;
		}

		public void Listen(object message)
		{
			if (this.get_gameObject().get_activeSelf())
			{
				_presenter.Listen(message);
			}
		}

		public void Bubble(object message)
		{
			if (_bubble == null)
			{
				_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
			}
			_bubble.Dispatch<object>(message);
		}
	}
}
