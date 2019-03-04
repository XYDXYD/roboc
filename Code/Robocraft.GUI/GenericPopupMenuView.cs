using Mothership;
using Mothership.GUI.Social;
using Svelto.DataStructures;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;
using Utility;

namespace Robocraft.GUI
{
	internal class GenericPopupMenuView : MonoBehaviour, IChainListener, IGUIFactoryType, IChainRoot
	{
		public GameObject popupItemTemplate;

		[SerializeField]
		private Transform itemParent;

		[SerializeField]
		private UIGrid grid;

		[SerializeField]
		private UIWidget backgroundWidget;

		[SerializeField]
		private UIWidget windowTailLeft;

		[SerializeField]
		private UIWidget windowTailRight;

		[SerializeField]
		private UIWidget bgFadeLeft;

		[SerializeField]
		private UIWidget bgFadeRight;

		[SerializeField]
		public PopupMenuType popupMenu;

		[SerializeField]
		private int padding = 10;

		private int _popupItemHeight;

		private readonly FasterList<GenericPopupMenuViewItem> _popupItemsList = new FasterList<GenericPopupMenuViewItem>();

		private GenericPopupMenuController _controller;

		private SignalChain _signal;

		public PopupMenuType popupMenuType => popupMenu;

		public Type guiElementFactoryType => typeof(PopupContextMenuFactory);

		public GenericPopupMenuView()
			: this()
		{
		}

		private unsafe void Awake()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			_popupItemHeight = popupItemTemplate.GetComponent<UIWidget>().get_height();
			_signal = new SignalChain(this.get_transform());
			grid.hideInactive = false;
			UIGrid obj = grid;
			obj.onReposition = Delegate.Combine((Delegate)obj.onReposition, (Delegate)new OnReposition((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			Hide();
		}

		public void InjectController(GenericPopupMenuController controller)
		{
			_controller = controller;
		}

		public void SetViewName(string viewName_)
		{
			this.get_gameObject().set_name(viewName_);
			GameObject obj = popupItemTemplate;
			obj.set_name(obj.get_name() + " " + viewName_);
		}

		protected virtual void OnSelect(bool isSelected)
		{
			if (!isSelected && (UICamera.get_hoveredObject() == null || !UICamera.get_hoveredObject().CompareTag("ContextMenuButton")))
			{
				_controller.Hide();
			}
		}

		public void SetUnAnchoredPosition(UIWidget referenceWidget)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			Vector3 worldCenter = referenceWidget.get_worldCenter();
			int num = backgroundWidget.get_width() + windowTailLeft.get_width();
			bool flag = (float)Screen.get_width() * (0.5f + worldCenter.x) + (float)num > (float)Screen.get_width();
			float offset = 0.5f * (referenceWidget.get_worldCorners()[2].x - referenceWidget.get_worldCorners()[1].x);
			if (flag && windowTailRight == null)
			{
				Console.LogWarning("Cannot reposition menu to fit the screen because the prefab isn't properly setup");
				flag = false;
				offset = 0f;
			}
			SetUnAnchoredPosition(worldCenter, flag, offset);
		}

		public void SetUnAnchoredPosition(Vector3 position, bool mirror = false, float offset = 0f)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			if (mirror)
			{
				offset += backgroundWidget.get_worldCorners()[3].x - backgroundWidget.get_worldCorners()[2].x;
				Vector3 val = backgroundWidget.get_transform().get_position() - windowTailRight.get_transform().get_position();
				val.x -= offset;
				backgroundWidget.get_transform().set_position(position + val);
			}
			else
			{
				backgroundWidget.set_pivot(0);
				Vector3 val2 = backgroundWidget.get_transform().get_position() - windowTailLeft.get_transform().get_position();
				val2.x += offset;
				backgroundWidget.get_transform().set_position(position + val2);
			}
			SetMirrored(mirror);
		}

		private void SetMirrored(bool mirrored)
		{
			if (windowTailRight != null)
			{
				windowTailRight.get_gameObject().SetActive(mirrored);
			}
			if (windowTailLeft != null)
			{
				windowTailLeft.get_gameObject().SetActive(!mirrored);
			}
			if (bgFadeRight != null)
			{
				bgFadeRight.get_gameObject().SetActive(mirrored);
			}
			if (bgFadeLeft != null)
			{
				bgFadeLeft.get_gameObject().SetActive(!mirrored);
			}
		}

		public void Show()
		{
			UICamera.set_selectedObject(this.get_gameObject());
			this.get_gameObject().SetActive(true);
			grid.set_repositionNow(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
			for (int i = 0; i < _popupItemsList.get_Count(); i++)
			{
				_popupItemsList.get_Item(i).Hide();
			}
			_popupItemsList.FastClear();
		}

		public void AddItemToMenu(GameObject buttonObject, string optionName, string actionType)
		{
			GenericPopupMenuViewItem genericPopupMenuViewItem = buttonObject.GetComponent(typeof(GenericPopupMenuViewItem)) as GenericPopupMenuViewItem;
			genericPopupMenuViewItem.Initialise(optionName, actionType);
			genericPopupMenuViewItem.Reparent(itemParent);
			_popupItemsList.Add(genericPopupMenuViewItem);
			ResizeBackgroundSprite(_popupItemsList.get_Count());
		}

		private void OnGridReposition()
		{
			for (int i = 0; i < _popupItemsList.get_Count(); i++)
			{
				_popupItemsList.get_Item(i).Show();
			}
		}

		private void ResizeBackgroundSprite(int numItems)
		{
			backgroundWidget.set_height(numItems * (_popupItemHeight + padding));
		}

		public void DispatchGenericMessage(GenericComponentMessage message)
		{
			_signal.DeepBroadcast<GenericComponentMessage>(message);
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage message2 = (GenericComponentMessage)message;
				_controller.HandleMessage(message2);
			}
			else
			{
				_controller.Listen(message);
			}
		}

		private void Update()
		{
			_controller.TickWhileVisible();
		}
	}
}
