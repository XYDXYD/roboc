using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class PaletteSlotView : MonoBehaviour, IChainListener
	{
		public GameObject offStateObject;

		public GameObject onStateObject;

		public GameObject lockedStateObject;

		public UISprite offColorSprite;

		public UISprite onColorSprite;

		public UISprite lockedColorSprite;

		public UISprite lockSprite;

		public UIWidget uiWidget;

		public bool isPremium;

		private bool _locked;

		private byte _slotNum;

		public event Action<byte> ColorSelected = delegate
		{
		};

		public PaletteSlotView()
			: this()
		{
		}

		public void InitialiseSlots(Color color, Color lockColor, byte slotNum)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			offColorSprite.set_color(color);
			onColorSprite.set_color(color);
			if (isPremium)
			{
				lockedColorSprite.set_color(color);
				lockSprite.set_color(lockColor);
			}
			_slotNum = slotNum;
		}

		public void Resize(int cellSize)
		{
			UIWidget obj = uiWidget;
			uiWidget.set_height(cellSize);
			obj.set_width(cellSize);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				if (buttonType == ButtonType.SelectColor)
				{
					this.ColorSelected(_slotNum);
				}
			}
		}

		public void SetCurrentlySelected(bool selected)
		{
			onStateObject.SetActive(selected);
			offStateObject.SetActive(!selected);
		}

		public void ShowLockState(bool locked)
		{
			_locked = locked;
			if (isPremium)
			{
				lockedStateObject.SetActive(_locked);
			}
		}
	}
}
