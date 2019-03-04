using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Robocraft.GUI
{
	public abstract class GenericComponentViewBase : MonoBehaviour, IGenericComponentView, IChainListener, IAnchorUISource, IAnchorableUIElement
	{
		internal IGenericComponent _controller;

		private GenericMessagePropogator _bubbleUp;

		internal IGenericComponent Controller => _controller;

		protected GenericComponentViewBase()
			: this()
		{
		}

		public virtual void Setup()
		{
			_bubbleUp = new GenericMessagePropogator(this.get_transform());
		}

		public abstract void Hide();

		public abstract void Show();

		public void AnchorThisElementUnder(IAnchorUISource other)
		{
			UIRect anchorSource = other.GetAnchorSource();
			AnchorThisElementUnder(anchorSource);
		}

		public UIRect GetAnchorSource()
		{
			UIRect component = this.GetComponent<UIWidget>();
			if (component == null)
			{
				component = this.GetComponent<UIPanel>();
			}
			return component;
		}

		public void AnchorThisElementUnder(UIRect otherWidget)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			UIRect component = this.GetComponent<UIWidget>();
			if (component == null)
			{
				component = this.GetComponent<UIPanel>();
			}
			component.get_transform().set_parent(otherWidget.get_transform());
			component.get_transform().set_localScale(Vector3.get_one());
			component.SetAnchor(otherWidget.get_transform());
			component.leftAnchor.Set(0f, 0f);
			component.rightAnchor.Set(1f, 0f);
			component.topAnchor.Set(1f, 0f);
			component.bottomAnchor.Set(0f, 0f);
		}

		public void BubbleMessageUp(GenericComponentMessage message)
		{
			_bubbleUp.SendMessageUpTree(message);
		}

		public virtual void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if ((genericComponentMessage.Target == _controller.Name || genericComponentMessage.Target == string.Empty) && !genericComponentMessage.Consumed)
				{
					_controller.HandleMessage(genericComponentMessage);
				}
			}
		}

		void IGenericComponentView.SetController(IGenericComponent controller)
		{
			_controller = controller;
		}

		public void ReparentOnly(Transform other)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_parent(other);
			this.get_transform().set_localScale(Vector3.get_one());
		}
	}
}
