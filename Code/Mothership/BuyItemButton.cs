using Services.Analytics;
using Svelto.Command;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class BuyItemButton : MonoBehaviour, IChainListener
	{
		[Inject]
		internal ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		internal IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public BuyItemButton()
			: this()
		{
		}

		void IChainListener.Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			ButtonType buttonType = (ButtonType)Convert.ToInt32(message);
			if (buttonType == ButtonType.PurchaseCosmeticCredits || buttonType == ButtonType.PurchasePremium)
			{
				if (worldSwitching.CurrentWorld == WorldSwitchMode.BuildMode)
				{
					commandFactory.Build<SwitchToGarageCommand>().Inject(new SwitchWorldDependency("RC_Mothership", _fastSwitch: false, OpenStoreScreen)).Execute();
				}
				else
				{
					OpenStoreScreen();
				}
			}
		}

		private void OpenStoreScreen()
		{
			PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "ColorPicker", startsNewChain: true);
			guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
		}
	}
}
