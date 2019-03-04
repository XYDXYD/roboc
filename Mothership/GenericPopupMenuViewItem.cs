using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	public class GenericPopupMenuViewItem : MonoBehaviour
	{
		[SerializeField]
		private UILabel optionText;

		private string _actionType = string.Empty;

		private BubbleSignal<GenericPopupMenuView> _bubble;

		public GenericPopupMenuViewItem()
			: this()
		{
		}

		public void Awake()
		{
			_bubble = new BubbleSignal<GenericPopupMenuView>(this.get_transform());
		}

		public void Initialise(string text, string actionType)
		{
			optionText.set_text(text);
			_actionType = actionType;
		}

		public void OnClick()
		{
			_bubble.TargetedDispatch<GenericComponentMessage>(new GenericComponentMessage(MessageType.ButtonClicked, _actionType, string.Empty));
		}

		public void Reparent(Transform itemParent)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().SetParent(itemParent);
			this.get_transform().set_localScale(Vector3.get_one());
			_bubble = new BubbleSignal<GenericPopupMenuView>(this.get_transform());
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_transform().SetParent(null);
			this.get_gameObject().SetActive(false);
		}
	}
}
