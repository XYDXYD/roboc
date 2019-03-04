using Robocraft.GUI;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	public abstract class ClanSectionViewBase : MonoBehaviour, IChainListener, IAnchorableUIElement, IChainRoot
	{
		private IClanSectionController _controller;

		private SignalChain _signal;

		private BubbleSignal<IClanRoot> _bubble;

		[Inject]
		internal ClanController clansController
		{
			private get;
			set;
		}

		internal abstract ClanSectionType SectionType
		{
			get;
		}

		protected ClanSectionViewBase()
			: this()
		{
		}

		public void Awake()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			_signal = new SignalChain(this.get_gameObject().get_transform());
		}

		public void Initialise()
		{
			_bubble = new BubbleSignal<IClanRoot>(this.get_gameObject().get_transform());
		}

		public void SetController(IClanSectionController controller)
		{
			_controller = controller;
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void BubbleSocialMessageUp(SocialMessage message)
		{
			_bubble.TargetedDispatch<SocialMessage>(message);
		}

		public void DispatchGenericMessage(GenericComponentMessage message)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			_signal = new SignalChain(this.get_gameObject().get_transform());
			_signal.DeepBroadcast(typeof(GenericComponentMessage), (object)message);
		}

		public void Listen(object message)
		{
			if (message is SocialMessage)
			{
				_controller.HandleClanMessage(message as SocialMessage);
			}
			else if (message is GenericComponentMessage)
			{
				_controller.HandleGenericMessage(message as GenericComponentMessage);
			}
			else
			{
				_controller.HandleMessage(message);
			}
		}

		public void AnchorThisElementUnder(UIRect otherWidget)
		{
			UIRect component = this.GetComponent<UIRect>();
			component.SetAnchor(otherWidget.get_transform());
			component.leftAnchor.Set(0f, 0f);
			component.rightAnchor.Set(1f, 0f);
			component.topAnchor.Set(1f, 0f);
			component.bottomAnchor.Set(0f, 0f);
		}

		public void AnchorThisElementUnder(IAnchorUISource other)
		{
			UIRect anchorSource = other.GetAnchorSource();
			AnchorThisElementUnder(anchorSource);
		}

		public void ReparentOnly(Transform other)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_parent(other);
			this.get_transform().set_localScale(Vector3.get_one());
			_bubble = new BubbleSignal<IClanRoot>(this.get_gameObject().get_transform());
		}
	}
}
