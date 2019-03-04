using GameFramework;
using Svelto.ECS;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.TechTree
{
	internal class TechTreeDialogueImplementor : MonoBehaviour, ITechTreeDialogueButtonsComponent, ITechTreeDialogueLabelsComponent, ITechTreeDialogueTypeComponent, IGameObjectComponent, IInitializableImplementor, IChainListener, ITechTreeDialogueNodeComponent
	{
		[SerializeField]
		private TechTreeDialogueType dialogueType;

		[SerializeField]
		private UILabel amountLabel;

		[SerializeField]
		private UILabel nodeName;

		[SerializeField]
		private UISprite nodeSprite;

		private DispatchOnSet<bool> _confirm;

		private DispatchOnSet<bool> _cancel;

		private DispatchOnSet<bool> _dismissed;

		private BubbleSignal<IChainRoot> _bubbleSignal;

		DispatchOnSet<bool> ITechTreeDialogueButtonsComponent.ConfirmButton
		{
			get
			{
				return _confirm;
			}
		}

		DispatchOnSet<bool> ITechTreeDialogueButtonsComponent.CancelButton
		{
			get
			{
				return _cancel;
			}
		}

		DispatchOnSet<bool> ITechTreeDialogueButtonsComponent.Dismissed
		{
			get
			{
				return _dismissed;
			}
		}

		UILabel ITechTreeDialogueLabelsComponent.TPCostLabel
		{
			get
			{
				return amountLabel;
			}
		}

		TechTreeDialogueType ITechTreeDialogueTypeComponent.Type
		{
			get
			{
				return dialogueType;
			}
		}

		string ITechTreeDialogueNodeComponent.nodeName
		{
			set
			{
				nodeName.set_text(value);
			}
		}

		string ITechTreeDialogueNodeComponent.nodeSprite
		{
			set
			{
				nodeSprite.set_spriteName(value);
			}
		}

		public TechTreeDialogueImplementor()
			: this()
		{
		}

		public void Initialize()
		{
			_confirm = new DispatchOnSet<bool>(this.get_gameObject().GetInstanceID());
			_cancel = new DispatchOnSet<bool>(this.get_gameObject().GetInstanceID());
			_dismissed = new DispatchOnSet<bool>(this.get_gameObject().GetInstanceID());
			_bubbleSignal = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.Confirm:
					_confirm.set_value(true);
					break;
				case ButtonType.Cancel:
					_cancel.set_value(true);
					break;
				}
			}
			if (message is FloatingWidgetBehaviour)
			{
				_dismissed.set_value(true);
			}
		}

		private void OnEnable()
		{
			_bubbleSignal.TargetedDispatch<LockingDialogAppearing>();
		}

		private void OnDisable()
		{
			_bubbleSignal.TargetedDispatch<LockingDialogHiding>();
		}

		GameObject IGameObjectComponent.get_gameObject()
		{
			return this.get_gameObject();
		}

		Transform IGameObjectComponent.get_transform()
		{
			return this.get_transform();
		}
	}
}
