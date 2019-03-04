using Game.ECS.GUI.Components;
using Svelto.ECS;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.ItemShop
{
	internal class ItemShopPopUpImplementor : MonoBehaviour, IItemShopPopUpComponent, IItemShopBundleComponent, IShowComponent, IDialogChoiceComponent, IChainListener, IChainRoot
	{
		[SerializeField]
		public UILabel titleLabel;

		[SerializeField]
		public UILabel infoLabel;

		[SerializeField]
		public UILabel rightButtLabel;

		[SerializeField]
		public UILabel leftButtLabel;

		[SerializeField]
		public UILabel centerButtLabel;

		[SerializeField]
		public GameObject doubleButtonsContainer;

		[SerializeField]
		public GameObject singleButtonContainer;

		UILabel IItemShopPopUpComponent.titleLabel
		{
			get
			{
				return titleLabel;
			}
		}

		UILabel IItemShopPopUpComponent.infoLabel
		{
			get
			{
				return infoLabel;
			}
		}

		UILabel IItemShopPopUpComponent.rightButtLabel
		{
			get
			{
				return rightButtLabel;
			}
		}

		UILabel IItemShopPopUpComponent.leftButtLabel
		{
			get
			{
				return leftButtLabel;
			}
		}

		UILabel IItemShopPopUpComponent.centerButtLabel
		{
			get
			{
				return centerButtLabel;
			}
		}

		ItemShopPopUpType IItemShopPopUpComponent.popupType
		{
			get;
			set;
		}

		GameObject IItemShopPopUpComponent.doubleButtonsContainer
		{
			get
			{
				return doubleButtonsContainer;
			}
		}

		GameObject IItemShopPopUpComponent.singleButtonContainer
		{
			get
			{
				return singleButtonContainer;
			}
		}

		public DispatchOnChange<bool> isShown
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> cancelPressed
		{
			get;
			private set;
		}

		public DispatchOnChange<bool> validatePressed
		{
			get;
			private set;
		}

		public ItemShopBundle bundle
		{
			get;
			set;
		}

		public ItemShopPopUpImplementor()
			: this()
		{
		}

		public void Initialize(int entityId)
		{
			this.get_gameObject().SetActive(false);
			isShown = new DispatchOnChange<bool>(entityId);
			isShown.NotifyOnValueSet((Action<int, bool>)Show);
			validatePressed = new DispatchOnChange<bool>(entityId);
			cancelPressed = new DispatchOnChange<bool>(entityId);
		}

		private void Show(int entityId, bool show)
		{
			this.get_gameObject().SetActive(show);
		}

		public void Listen(object message)
		{
			switch ((ButtonType)message)
			{
			case ButtonType.Confirm:
				validatePressed.set_value(true);
				validatePressed.set_value(false);
				break;
			case ButtonType.Cancel:
				cancelPressed.set_value(true);
				cancelPressed.set_value(false);
				break;
			}
		}
	}
}
